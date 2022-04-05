using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.CommonServices;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Engine.JobBroker;
using Baltic.Types.Protos;
using Baltic.Types.QueueAccess;
using Serilog;

namespace Baltic.TaskManager.Proxies {
	public class BalticNodeProxy : IBalticNode, IQueueConsumer {
		
		private readonly BalticNodeServiceApi.BalticNodeServiceApiClient _balticNode;

		public BalticNodeProxy(NodeManager nodeManager, string nodeUid)
		{
			_balticNode = nodeManager.GetClient<BalticNodeServiceApi.BalticNodeServiceApiClient>(nodeUid);
		}
		
		public List<FullJobStatus> GetBatchJobStatuses(string batchMsgUid)
		{
			try
			{
				var statuses =
					_balticNode.GetBatchJobStatuses(new XBatchInstanceRequest() {JobsQueueUid = batchMsgUid});
				// TODO - handle an empty 'statuses' object (how?) @R.Roszczyk
				return statuses.Statuses.Select(s => DBMapper.Map<FullJobStatus>(s, new FullJobStatus()
				{
					Status = (ComputationStatus) s.Status
				})).ToList();
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
				return null;
			}
		}

		public short FinishJobInstance(string jobInstanceUid)
		{
			try
			{
				int result = -1 * (int) _balticNode.FinishJobInstance(new XJobInstanceRequest() {JobMsgUid = jobInstanceUid}).Code;
				return (short) (0 == result ? 0 : result - 1);
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
				return -1;
			}
		}
		
		public short FinishJobExecution(string jobInstanceUid)
		{
			try
			{
				int result = -1 * (int) _balticNode.FinishJobExecution(new XJobInstanceRequest() {JobMsgUid = jobInstanceUid}).Code;
				return (short) (0 == result ? 0 : result - 1);
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
				return -1;
			}
		}

		public short FinishJobBatch(string batchMsgUid)
		{
			try
			{
				int result = -1 * (int) _balticNode.FinishJobBatch(new XBatchInstanceRequest() {JobsQueueUid = batchMsgUid}).Code;
				return (short) (0 == result ? 0 : result - 1);
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
				return -1;
			}
		}

		public short MessageReceived(Message msg)
		{
			try
			{
				int result;
				if (msg is BatchInstanceMessage bim)
				{
					XBatchInstanceMessage xmsg = DBMapper.Map<XBatchInstanceMessage>(bim, new XBatchInstanceMessage()
					{
						QueueSeqStack =
						{
							bim.QueueSeqStack.Select(s => new XSeqToken()
							{
								No = s.No,
								IsFinal = s.IsFinal,
								SeqUid = s.SeqUid
							})
						},
						JobQueueIds = {bim.JobQueueIds.Select(m => m.ToString())},
						ServiceBuilds = {bim.ServiceBuilds.Select(sb => new XBalticModuleBuild(sb))},
						Quota = DBMapper.Map<XWorkspaceQuota>(bim.Quota,new XWorkspaceQuota())
					});
					result = -1 * (int) _balticNode.BatchInstanceMessageReceived(xmsg).Code;
				}
				else if (msg is BatchExecutionMessage bem)
				{
					XBatchExecutionMessage xmsg = DBMapper.Map<XBatchExecutionMessage>(bem, new XBatchExecutionMessage()
					{
						QueueSeqStack =
						{
							bem.QueueSeqStack.Select(s => new XSeqToken()
							{
								No = s.No,
								IsFinal = s.IsFinal,
								SeqUid = s.SeqUid
							})
						},
						JobQueueIds = {bem.JobQueueIds.Select(m => m.ToString())}
					});
					result = -1 * (int) _balticNode.BatchExecutionMessageReceived(xmsg).Code;
				} else if (msg is TokenMessage tm)
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
					result = -1 * (int) _balticNode.TokenMessageReceived(tokenMsg).Code;
				}
				else if (msg is JobInstanceMessage jim)
				{
					XJobInstanceMessage jobMsg = DBMapper.Map<XJobInstanceMessage>(msg, new XJobInstanceMessage()
					{
						QueueSeqStack = {jim.QueueSeqStack.Select(s => new XSeqToken()
						{
							No = s.No,
							IsFinal = s.IsFinal,
							SeqUid = s.SeqUid
						})},
						RequiredAccessTypes = {jim.RequiredAccessTypes},
						ProvidedPinTokens =
						{
							jim.ProvidedPinTokens.Select(t => new StringLongPair()
							{
								Key = t.Key, Value = t.Value
							})
						},
						RequiredPinQueues =
						{
							jim.RequiredPinQueues.Select(t => new StringStringPair()
							{
								Key = t.Key, Value = t.Value.ToString()
							})
						},
						Build = new XBalticModuleBuild(jim.Build)
					});

					result = -1 * (int) _balticNode.JobInstanceMessageReceived(jobMsg).Code;
				}
				else if (msg is JobExecutionMessage jem)
				{
					XJobExecutionMessage jobMsg = DBMapper.Map<XJobExecutionMessage>(jem, new XJobExecutionMessage()
					{
						QueueSeqStack = {jem.QueueSeqStack.Select(s => new XSeqToken()
						{
							No = s.No,
							IsFinal = s.IsFinal,
							SeqUid = s.SeqUid
						})},
						RequiredPinQueues =
						{
							jem.RequiredPinQueues.Select(t => new StringStringPair()
							{
								Key = t.Key, Value = t.Value.ToString()
							})
						}
					});

					result = -1 * (int) _balticNode.JobExecutionMessageReceived(jobMsg).Code;
				}
				else return -2;

				return (short) (0 == result ? 0 : result - 2);
			}
			catch (Exception e)
			{
				Log.Debug(e.ToString());
				return -2;
			}
		}
	}
}