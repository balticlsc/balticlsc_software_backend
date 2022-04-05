using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.Node.Engine.ServerAccess;
using Baltic.Types.Protos;
using Grpc.Net.Client;
using Serilog;

namespace Baltic.Node.BatchManager.Proxies
{
	public class BalticServerProxy : IBalticServer {

		private BalticServerServiceApi.BalticServerServiceApiClient _balticServer;

		/// 
		/// <param name="masterChannel"></param>
		public BalticServerProxy(GrpcChannel masterChannel){
			_balticServer = new BalticServerServiceApi.BalticServerServiceApiClient(masterChannel);
		}

		/// 
		/// <param name="msgUids"></param>
		/// <param name="status"></param>
		/// <param name="isFinal"></param>
		/// <param name="isFailed"></param>
		/// <param name="note"></param>
		public short AckMessages(Dictionary<string,QueueId> msgUids, FullJobStatus status, bool isFinal, bool isFailed, string note)
		{
			List<StringStringPair> xMsgUids = msgUids.Select(m => new StringStringPair()
			{
				Key = m.Key, Value = m.Value.ToString()
			}).ToList();
			try
			{
				int result = -1 * (int) _balticServer.AckMessages(new XAckRequest()
				{
					MsgUids = {xMsgUids},
					Status = DBMapper.Map<XFullJobStatus>(status, new XFullJobStatus()
					{
						Status = (int) status.Status
					}),
					IsFinal = isFinal,
					IsFailed = isFailed,
					Note = note ?? ""
				}).Code;
				return (short) (0 == result ? 0 : result - 1);
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
				return -1;
			}
		}

		/// 
		/// <param name="tm"></param>
		public short PutTokenMessage(TokenMessage tm)
		{
			// TODO verify (two lines below)
			if (null == tm.AccessType) tm.AccessType = "";
			if (null == tm.TargetAccessType) tm.TargetAccessType = "";
			XTokenMessage tokenMsg = DBMapper.Map<XTokenMessage>(tm, new XTokenMessage()
			{
				QueueSeqStack = {tm.QueueSeqStack.Select(s => new XSeqToken()
				{
					No = s.No,
					IsFinal = s.IsFinal,
					SeqUid = s.SeqUid
				})},
				TokenSeqStack = {tm.TokenSeqStack.Select(s => new XSeqToken()
				{
					No = s.No,
					IsFinal = s.IsFinal,
					SeqUid = s.SeqUid
				})},
				Values = tm.DataSet.Values ?? ""
			});
			try
			{
				int result = -1 * (int) _balticServer.PutTokenMessage(tokenMsg).Code;
				return (short) (0 == result ? 0 : result - 1);
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
				return -1;
			}
		}

		/// 
		/// <param name="batchMsgUid"></param>
		/// <param name="jobQueueIds"></param>
		public void ConfirmBatchStart(string batchMsgUid, List<QueueId> jobQueueIds)
		{
			try
			{
				_balticServer.ConfirmBatchStart(new XConfirmBatchRequest()
				{
					BatchMsgUid = batchMsgUid,
					RequiredJobQueues = {jobQueueIds.Select(m => m.ToString())}
				});
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
			}
		}

		/// 
		/// <param name="instanceUid"></param>
		/// <param name="requiredPinQueues"></param>
		/// <param name="isNewInstance"></param>
		public void ConfirmJobStart(string instanceUid, List<QueueId> requiredPinQueues, bool isNewInstance)
		{
			try{
				XConfirmJobRequest cjr = new XConfirmJobRequest()
				{
					InstanceUid = instanceUid,
					RequiredPinQueues = {requiredPinQueues.Select((m => m.ToString()))},
					IsNewInstance = isNewInstance
				};
				_balticServer.ConfirmJobStart(cjr);
			} catch  (Exception e)
			{
				Log.Debug(e.ToString());
			}
		}
	}
}