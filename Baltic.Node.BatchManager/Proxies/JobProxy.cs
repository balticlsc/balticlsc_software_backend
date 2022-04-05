using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.Node.BatchManager.Models;
using Baltic.Node.Engine.DataAccess;
using Serilog;

namespace Baltic.Node.BatchManager.Proxies
{
	public class JobProxy : IJob
	{
		private string _endpoint;
		private string _moduleId;
		private static readonly HttpClient Client = Init();

		private static HttpClient Init()
		{
			HttpClient client = new HttpClient(); 
			// TODO set default headers here (if necessary)
			return client;
		}
		
		public JobProxy(string endpoint, string moduleId)
		{
			_endpoint = endpoint;
			_moduleId = moduleId;
		}
		
		/// 
		/// <param name="tm"></param>
		public short ProcessTokenMessage(TokenMessage tm)
		{
			XInputTokenMessage msg = new XInputTokenMessage()
			{
				MsgUid = tm.MsgUid,
				PinName = tm.PinName,
				AccessType = tm.AccessType,
				Values = tm.DataSet.Values,
				TokenSeqStack = tm.TokenSeqStack.Select(t => DBMapper.Map<XSeqToken>(t, new XSeqToken()))
			};
			string body = JsonSerializer.Serialize(msg);
			string url = $"{_endpoint}/token/{_moduleId}";
			Log.Debug($"Attempt to send POST message to: {url}");
			// HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), _endpoint+"/token" + (""==_moduleId?"":"/") + _moduleId)
			// HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), url)
			// {
			// 	Version = HttpVersion.Version20,
			// 	Content = new StringContent(body, Encoding.UTF8, "application/json")
			// };
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(url),
				Method = HttpMethod.Post,
				Content = new StringContent(body, Encoding.UTF8, "application/json")
			};
			var taskResult = Client.SendAsync(request).Result;
			Log.Debug("JobProxy "+_endpoint+" return code "+taskResult.StatusCode);
			return (short) (taskResult.IsSuccessStatusCode ? 0 : -1);
		}

		public JobStatus GetStatus()
		{
			XJobStatus status;
			try
			{
				HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"),
					_endpoint + "/status" + ("" == _moduleId ? "" : "/") + _moduleId);
				var taskResult = Client.SendAsync(request).Result;
				if (!taskResult.IsSuccessStatusCode)
					return null;
				
				var result = taskResult.Content.ReadAsStringAsync().Result;
				status = JsonSerializer.Deserialize<XJobStatus>(result);
			}
			catch (Exception e)
			{
				if (e is AggregateException exception &&
				    exception.InnerExceptions.ToList().Exists(ie => ie is HttpRequestException))
					Log.Warning("Warning: Http message delivery failure (GetStatus)");
				else
					Log.Debug(e.StackTrace);
				return null;
			}

			return DBMapper.Map<JobStatus>(status, new JobStatus());
		}
		
		// TODO remove MOCK below
		public void InjectUid(string uid)
		{
			HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), _endpoint + "/uid" + (""==_moduleId?"":"/") + _moduleId + "?val="+uid);
			var taskResult = Client.SendAsync(request).Result;
		}
		
		public string CheckUid()
		{
			HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), _endpoint + "/uid" + (""==_moduleId?"":"/") + _moduleId);
			var taskResult = Client.SendAsync(request).Result;
			var result = taskResult.Content.ReadAsStringAsync().Result;
			return result;
		}
	}
}