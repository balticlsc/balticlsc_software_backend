using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.Node.Engine.DataAccess;
using Serilog;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Node.Engine.Job
{
	public class XOutputTokenMessage
	{
		public string PinName { get; set; }

		public string SenderUid { get; set; }

		public string Values { get; set; }

		public string BaseMsgUid { get; set; }

		public bool IsFinal { get; set; }
	}
	
	public class XTokensAck {
		public List<string> MsgUids { get; set; }
		public string SenderUid { get; set; }
	}
	
	public class Job : IJob {
		
		private static readonly HttpClient _client = new HttpClient();

		private string _image;
		private string _jobInstanceUid;
		private IDictionary<string,List<TokenMessage>> _messages;

		private SemaphoreSlim _semaphore;

		public Job(){
			_messages = new ConcurrentDictionary<string, List<TokenMessage>>();
			_semaphore = new SemaphoreSlim(1,1);
		}
	
		/// 
		/// <param name="build"></param>
		public void Init(BalticModuleBuild build){
			_image = build.Image; _jobInstanceUid = build.EnvironmentVariables["SYS_MODULE_INSTANCE_UID"];
		}

		/// 
		/// <param name="tm"></param>
		public short ProcessTokenMessage(TokenMessage tm, out string responseMessage)
		{
			_semaphore.Wait();
			try
			{
				_messages.TryAdd(tm.PinName, new List<TokenMessage>());
				_messages[tm.PinName].Add(tm);
				responseMessage = "";
				return 0;
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public JobStatus GetStatus()
		{
			return null;
		}

		public void RunJob(){
			bool finish = false;
		
			while (!finish)
			{
				switch (_image)
				{
					case "fs001":
						finish = ProcessFrameSplitter();
						break;
					case "is001":
						finish = ProcessTokensSplitter();
						break;
					case "ip002":
						finish = ProcessTokensProcessor();
						break;
					case "im003":
						finish = ProcessTokensMerger();
						break;
					case "copy-in":
						finish = ProcessCopyIn();
						break;
					case "copy-out":
						finish = ProcessCopyOut();
						break;
				}
				Thread.Sleep(500);
			}
			//*test*
			Log.Debug(consoleString() + "Finished: " + _image + " i=" + _jobInstanceUid + "\n");
			//*test*
		}
	
		private bool ProcessFrameSplitter()
		{
			// Build == "fs001"
		
			if (!_messages.ContainsKey("film") || 0 == _messages["film"].Count) return false;
			TokenMessage tm = _messages["film"][0];
		
			//*test*
			Log.Debug(consoleString() + "JobFrameSplitter: received message");
			//*test*
		
			Thread.Sleep(2000);
		
			CDataSet ds = new CDataSet() {
				Values = tm.DataSet.Values + "<split>filmf1"
			};
			TokenMessage tm2 = new TokenMessage() {
				PinName = "filmf1", DataSet = ds, SenderUid = _jobInstanceUid
			};
//		job_sdk.putTokenMessage(tm2,tm.msg_uid,false);
//		
//		ds = new DataSet() {
//				name = tm.data_set.name + "<split>filmf1"
//		};
//		tm2 = new TokenMessage() {
//			pin_uid = "filmf01", data_set = ds
//		};
			PutTokenMessage(tm2,tm.MsgUid,true);
			FinalizeTokenMessageProcessing(new List<string>(){tm.MsgUid},_jobInstanceUid);
			_messages["film"].Remove(tm);
			return true;
		}

		private bool ProcessTokensSplitter()
		{
			// Build == "is001"
		
			if (!_messages.ContainsKey("image") || 0 == _messages["image"].Count) return false;
			TokenMessage tm = _messages["image"][0];
		
			//*test*
			Log.Debug(consoleString() + "JobImageSplitter: received message");
			//*test*
		
			Thread.Sleep(3000);
		
			CDataSet ds = new CDataSet() {
				Values = tm.DataSet.Values + "<split>imagep1"
			};
			TokenMessage tm2 = new TokenMessage() {
				PinName = "imagep1", DataSet = ds, SenderUid = _jobInstanceUid
			};
			PutTokenMessage(tm2,tm.MsgUid,false);
		
			ds = new CDataSet() {
				Values = tm.DataSet.Values + "<split>imagep1"
			};
			tm2 = new TokenMessage() {
				PinName = "imagep1", DataSet = ds, SenderUid = _jobInstanceUid
			};
			PutTokenMessage(tm2,tm.MsgUid,true);
		
			ds = new CDataSet() {
				Values = tm.DataSet.Values + "<split>imagep2"
			};
			tm2 = new TokenMessage() {
				PinName = "imagep2", DataSet = ds, SenderUid = _jobInstanceUid
			};
			PutTokenMessage(tm2,tm.MsgUid,true);
		
			ds = new CDataSet() {
				Values = tm.DataSet.Values + "<split>imagep3"
			};
			tm2 = new TokenMessage() {
				PinName = "imagep3", DataSet = ds, SenderUid = _jobInstanceUid
			};
			PutTokenMessage(tm2,tm.MsgUid,true);
			FinalizeTokenMessageProcessing(new List<string>(){tm.MsgUid},_jobInstanceUid);
			_messages["image"].Remove(tm);
			return true;
		}
	
		private bool ProcessTokensProcessor()
		{
			// Build == "ip002"
			TokenMessage tm, tm2;
			_semaphore.Wait();
			try
			{
				if (!_messages.ContainsKey("imagep") || 0 == _messages["imagep"].Count) return false;
				tm = _messages["imagep"][0];

				//*test*
				Log.Debug(consoleString() + "JobImageProcessor: received message");
				//*test*

				CDataSet ds = new CDataSet()
				{
					Values = tm.DataSet.Values + "<proc>imagepp"
				};
				tm2 = new TokenMessage()
				{
					PinName = "imagepp", DataSet = ds, SenderUid = _jobInstanceUid
				};
			}
			finally
			{
				_semaphore.Release();
			}
			
			Thread.Sleep(2000);

			PutTokenMessage(tm2,tm.MsgUid,true);
			FinalizeTokenMessageProcessing(new List<string>(){tm.MsgUid},_jobInstanceUid);
			_messages["imagep"].Remove(tm);
			return false; // 0 == _messages["imagep"].Count;
		}
	
		private bool ProcessTokensMerger()
		{
			// Build == "im003"
		
			string new_name = "";
			int proc = 0;
			int cnt1 = 0, cnt2 = 0, cnt3 = 0;
			TokenMessage tm;
			List<TokenMessage> tokensToFinalise = new List<TokenMessage>();
			
			while (proc<3)
			{
				_semaphore.Wait();
				try
				{
					if (_messages.ContainsKey("imagerp1") && cnt1 < _messages["imagerp1"].Count)
					{
						tm = _messages["imagerp1"][cnt1];
						new_name = new_name + "<imagerp1>" + tm.DataSet.Values + tm.TokenSeqStack[0] +
						           tm.TokenSeqStack[1] + "|";
						cnt1++;
						tokensToFinalise.Add(tm);
						if (tm.TokenSeqStack[0].IsFinal) proc++;
					}

					if (_messages.ContainsKey("imagerp2") && cnt2 < _messages["imagerp2"].Count)
					{
						tm = _messages["imagerp2"][cnt2];
						new_name = new_name + "<imagerp2>" + tm.DataSet.Values + tm.TokenSeqStack[0] +
						           tm.TokenSeqStack[1] + "|";
						cnt2++;
						tokensToFinalise.Add(tm);
						if (tm.TokenSeqStack[0].IsFinal) proc++;
					}

					if (_messages.ContainsKey("imagerp3") && cnt3 < _messages["imagerp3"].Count)
					{
						tm = _messages["imagerp3"][cnt3];
						new_name = new_name + "<imagerp3>" + tm.DataSet.Values + tm.TokenSeqStack[0] +
						           tm.TokenSeqStack[1] + "|";
						cnt3++;
						tokensToFinalise.Add(tm);
						if (tm.TokenSeqStack[0].IsFinal) proc++;
					}
				}
				finally
				{
					_semaphore.Release();
				}

				Thread.Sleep(200);
			}
		
			CDataSet ds = new CDataSet() {Values = new_name};
		
			//*test*
			Log.Debug(consoleString() + "JobImageMerger: received messages");
			//*test*
		
			Thread.Sleep(1000);

			tm = new TokenMessage() {
				PinName = "fimage", DataSet = ds, SenderUid = _jobInstanceUid
			};
		
			PutTokenMessage(tm,_messages["imagerp1"][0].MsgUid,true);
			
			FinalizeTokenMessageProcessing(tokensToFinalise.Select(t => t.MsgUid).ToList(),_jobInstanceUid);
			foreach (TokenMessage t in tokensToFinalise)
			{
				if (_messages["imagerp1"].Contains(t))
					_messages["imagerp1"].Remove(t);
				else if (_messages["imagerp2"].Contains(t))
					_messages["imagerp2"].Remove(t);
				else
					_messages["imagerp3"].Remove(t);
			}
			return true;
		}
	
		private bool ProcessCopyIn()
		{
			// Build == "copy-in"
		
			if (!_messages.ContainsKey("input") || 0 == _messages["input"].Count)
				return false;
			TokenMessage tm = _messages["input"][0];
		
			Thread.Sleep(2000);
		
			//*test*
			Log.Debug(consoleString() + "JobCopyIn INPUT: " + tm.DataSet.Values + "\n");
			//*test*
		
			TokenMessage tm2 = new TokenMessage {
				DataSet = tm.DataSet,
				PinName = "output",
				SenderUid = _jobInstanceUid
			};
			PutTokenMessage(tm2,tm.MsgUid,true);
			FinalizeTokenMessageProcessing(new List<string>(){tm.MsgUid},_jobInstanceUid);
			_messages["input"].Remove(tm);
			return 0 == _messages["input"].Count;
		}
	
		private bool ProcessCopyOut()
		{
			// Build == "copy-out"
		
			if (!_messages.ContainsKey("input") || 0 == _messages["input"].Count) return false;
			if (!_messages.ContainsKey("output") || 0 == _messages["output"].Count) return false;
			TokenMessage tm = _messages["input"][0], tm2 = _messages["output"][0];
		
			Thread.Sleep(1000);
		
			//*test*
			Log.Debug(consoleString() + "JobCopyOut FINAL OUTPUT: " + tm.DataSet.Values);
			//*test*
		
			FinalizeTokenMessageProcessing(new List<string>(){tm.MsgUid,tm2.MsgUid},_jobInstanceUid);
			_messages["input"].Remove(tm);
			_messages["output"].Remove(tm2);
			return true;
		}

		private short PutTokenMessage(TokenMessage tm, string requiredMsgUid, bool finalMsg)
		{
			XOutputTokenMessage message = new XOutputTokenMessage()
			{
				SenderUid = tm.SenderUid,
				PinName = tm.PinName,
				Values = tm.DataSet.Values,
				BaseMsgUid = requiredMsgUid,
				IsFinal = finalMsg
			};
			string body = JsonSerializer.Serialize(message);
            
			HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost:7000/token")
			{
				Content = new StringContent(body, Encoding.UTF8, "application/json")
			};
			var taskResult = _client.SendAsync(request).Result;
			return (short) (taskResult.IsSuccessStatusCode ? 0 : -1);
		}

		private short FinalizeTokenMessageProcessing(List<string> msgUids, string senderUid)
		{
			XTokensAck message = new XTokensAck()
			{
				MsgUids = msgUids,
				SenderUid = senderUid
			};
			string body = JsonSerializer.Serialize(message);
            
			HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "http://localhost:7000/ack")
			{
				Content = new StringContent(body, Encoding.UTF8, "application/json")
			};
			var taskResult = _client.SendAsync(request).Result;
			return (short) (taskResult.IsSuccessStatusCode ? 0 : -1);
		}
	
		private string consoleString(){
			return "## NODE.JOB ## " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}

	}
}

