using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Engine.JobBroker;
using Baltic.Node.Engine.DataAccess;
using Baltic.Node.Engine.DataModel;
using Baltic.Node.Engine.ServerAccess;
using Microsoft.Extensions.Configuration;
using Serilog;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Node.Engine.BatchManager
{
	public class BatchManager : IBalticNode, IMessageConsumer, ITokens {
		private IBalticServer _server;
		private ICluster _cluster;
		/// <summary>
		/// string - BatchMsgUid
		/// </summary>
		private IDictionary<string,BatchInstanceInfo> _batchInfos;
		
		/// <summary>
		/// string - BatchMsgUid
		/// </summary>
		private static readonly ConcurrentDictionary<string,SemaphoreSlim> BatchLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
		
		/// <summary>
		/// string #1 - JobInstanceUid (JobInstanceInfo.MsgUid), string #2 - Resource name
		/// </summary>
		private static readonly ConcurrentDictionary<string,ConcurrentDictionary<string,SemaphoreSlim>> JobInstanceLocks = 
			new ConcurrentDictionary<string, ConcurrentDictionary<string,SemaphoreSlim>>();

		private short LogLevel => short.Parse(_configuration["LogLevel"]);

		private IConfiguration _configuration;

		public BatchManager(IBalticServer q, ICluster cp, BatchManagerDbMock db, IConfiguration configuration)
		{
			_batchInfos = db.BatchInfos;
			_server = q; _cluster = cp;
			_configuration = configuration;
		}
		
		// ========== From IMessageConsumer ============================================================================

		/// 
		/// <param name="bim"></param>
		public short BatchInstanceMessageReceived(BatchInstanceMessage bim)
		{
			Log.Debug(ConsoleString() + "BatchInstanceMessage received from queue " + (new QueueId(bim)));
			
			BatchInstanceInfo bi = new BatchInstanceInfo()
			{
				BatchInstanceMessage = bim
			};
			
			// TODO: remove -> quota set for testing k8s
			bim.Quota = new ResourceReservation()
			{
				Cpus = 2000, //msec
				Memory = 2048, // MB
				Gpus = 0, // no
				Storage = 1 // GB
			};

			for (int k = 0; k < bim.ServiceBuilds.Count; k++)
			{
				bim.ServiceBuilds[k].Resources.Cpus = 250;
				bim.ServiceBuilds[k].Resources.Memory = 256;
				bim.ServiceBuilds[k].Resources.Gpus = new GpuRequest()
				{
					Quantity = 0,
					Type = "none"
				};
			}

			Log.Debug($"{bim.ServiceBuilds.Count} builds in batch:\n " +
			          $"{JsonSerializer.Serialize(bim.ServiceBuilds, new JsonSerializerOptions{WriteIndented = true})}");
			
			// Remove END
			
			short ret = _cluster.StartBatch(bim.MsgUid,bim.Quota,bim.ServiceBuilds); // namespace for the batch instance (Kubernetes, Swarm etc.)

			if (0 == ret)
			{
				_batchInfos.Add(bim.MsgUid, bi);
				_server.ConfirmBatchStart(bim.MsgUid, bim.JobQueueIds);
			}

			return ret;
		}
		
		/// 
		/// <param name="bem"></param>
		public short BatchExecutionMessageReceived(BatchExecutionMessage bem)
		{
			//*test*
			Log.Debug(ConsoleString() + "BatchExecutionMessage received from queue " + (new QueueId(bem)).ToString());
			//*test*
			
			// TODO - update with more logic

			return 0;
		}
		
		public short TokenMessageReceived(TokenMessage tm)
		{
			QueueId id = new QueueId(tm);

			//*test* 
			Log.Debug(ConsoleString() + "TokenMessage received from Queue " + id.ToString());
			//*test*

			// TODO - correct this for many simple job instances (with the same RequiredPinQueues) running in a specific batch instance
			BatchInstanceInfo bi = null;
			JobInstanceInfo ji = null;
			foreach (BatchInstanceInfo bii in _batchInfos.Values)
				if (null != (ji = bii.JobInfos.Values.ToList()
					.Find(i => i.JobInstanceMessage.RequiredPinQueues.Values.Contains(id))))
				{
					bi = bii;
					break;
				}

			if (null == bi)
				return -1;
			if (bi.TokenMessages.ContainsKey(tm.MsgUid))
				return -2; // should not happen
			try
			{
				bi.TokenMessages.Add(tm.MsgUid, tm);

				if (ji.JobInstanceMessage.IsSimple)
				{
					LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "AcceptsJobs", 1, "TokenMessageReceived");
					if (!ji.AcceptsJobs)
					{
						bi.TokenMessages.Remove(tm.MsgUid);
						Log.Debug($"{ConsoleString()} TokenMessage from Queue {id} was not forwarded to Job Instance {ji.JobInstanceMessage.MsgUid}" +
						          $" (current status is NOT ACCEPTING jobs)");
						return -1;
					}

					// make "tm" its own copy without the TokenSeqStack
					tm = DBMapper.Map<TokenMessage>(tm, new TokenMessage());
					tm.TokenSeqStack = new SeqTokenStack();
					// mark it as busy if not multitasking
					ji.AcceptsJobs = ji.JobInstanceMessage.IsMultitasking;
					
					if (ji.JobInstanceMessage.IsMultitasking)
						Log.Debug(ConsoleString() + "Job Instance " + ji.JobInstanceMessage.MsgUid +
						          " (Image: " + ji.JobInstanceMessage.Build.Image +
						          ") current status is: ACCEPTING jobs (multitasking).");
					else
						Log.Debug(ConsoleString() + "Job Instance " + ji.JobInstanceMessage.MsgUid +
						          " (Image: " + ji.JobInstanceMessage.Build.Image + ") current status is: NOT ACCEPTING jobs.");
				}

				//*test*
				Log.Debug(ConsoleString() + "Token SENT TO Job: " + bi.BatchInstanceMessage.MsgUid +
				          "\n==> " + tm);
				//*test*
				
				short result = (short) (10 * ji.Handle.ProcessTokenMessage(tm));
				if (0 == result)
				{
					ji.TokensReceived++; // TODO persist in a DB
					if (ji.JobInstanceMessage.IsSimple)
						_server.ConfirmJobStart(ji.JobInstanceMessage.MsgUid, new List<QueueId>(), false);
				}
				else
				{
					bi.TokenMessages.Remove(tm.MsgUid);
					if (ji.JobInstanceMessage.IsSimple && !ji.JobInstanceMessage.IsMultitasking)
					{
						ji.AcceptsJobs = true;
						Log.Debug(ConsoleString() + "Job Instance " + ji.JobInstanceMessage.MsgUid +
						          " (Image: " + ji.JobInstanceMessage.Build.Image + ") current status is: ACCEPTING jobs (message rejected by the job instance).");
					}
				}

				return result;
			}
			catch(Exception e)
			{
				bi.TokenMessages.Remove(tm.MsgUid);
				if (ji.JobInstanceMessage.IsSimple && !ji.JobInstanceMessage.IsMultitasking)
				{
					ji.AcceptsJobs = true;
					Log.Debug(ConsoleString() + "Job Instance " + ji.JobInstanceMessage.MsgUid +
					          " (Image: " + ji.JobInstanceMessage.Build.Image + ") current status is: ACCEPTING jobs (message delivery failure).");
				}

				if (e is AggregateException exception &&
				    exception.InnerExceptions.ToList().Exists(ie => ie is HttpRequestException))
				{
					Log.Warning("Warning: Http message delivery failure (ProcessTokenMessage)");
					return -3;
				}

				throw;
			}
			finally
			{
				if (ji.JobInstanceMessage.IsSimple)
					LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "AcceptsJobs", -1, "TokenMessageReceived");
			}
		}
		
		public short JobInstanceMessageReceived(JobInstanceMessage jim)
		{
			QueueId id = new QueueId(jim);
			//*test* 
			Log.Debug(ConsoleString() + "JobInstanceMessage received from queue " + id);
			//*test*

			BatchInstanceInfo bi = _batchInfos.Values.ToList().Find(i => i.BatchInstanceMessage.JobQueueIds.Contains(id));
			
			if (null == bi)
				return -1;
			
			LockBatchAccess(bi.BatchInstanceMessage.MsgUid, 1, "JobInstanceMessageReceived");
			try
			{
				// a) JobMessage indicates the need to create a new instance or b) no existing running job instance can be reused

				// determine which AccessTypes are needed for the current job instance
				Dictionary<string, string> selectedAccess = new Dictionary<string, string>(); // string - AccessType, string - AccessCredentials
				AccessUtility.SetMongoDbConnectionstring(_configuration["tmpMongoDbConnectionString"]);
				foreach (KeyValuePair<string, string> acc in AccessUtility.GetStorageAccess(bi.BatchInstanceMessage.ServiceBuilds))
					if (jim.RequiredAccessTypes.Contains(acc.Key))
						selectedAccess.Add(acc.Key, acc.Value);

				// create a new JobInstanceInfo
				JobInstanceInfo jii = new JobInstanceInfo()
				{
					JobInstanceMessage = jim,
					AcceptsJobs =
						jim.IsSimple || jim.IsMultitasking // job instances can accept jobs if it is multitasking
				};
				// initialise message counters for the output pins to "0"
				foreach (KeyValuePair<string, long> counter in jim.ProvidedPinTokens)
				{
					jii.ProducedTokenCounters.Add(counter.Key, 0);
				}

				LockJobInstanceAccess(jii.JobInstanceMessage.MsgUid, "Status",1, "JobInstanceMessageReceived");
				try
				{
					try
					{
						bi.JobInfos.Add(jim.MsgUid, jii);

						// start the job instance on the cluster
						BalticModuleBuild build = AccessUtility.AddEnvironmentToJobBuild(jim.Build, jim.MsgUid,
							bi.BatchInstanceMessage.MsgUid, selectedAccess,_configuration["clusterProjectName"]);
						// TODO: remove -> resource set for k8s test
						// this do noting ??
						build.Resources.Cpus = 250;
						build.Resources.Memory = 256;
						build.Resources.Gpus = new GpuRequest()
						{
							Quantity = 0,
							Type = "none"
						};
						jii.Handle = _cluster.StartJob(build, bi.BatchInstanceMessage.MsgUid, jim.MsgUid);
						if (null == jii.Handle)
							throw new Exception("Job Instance could not be started properly");
					}
					finally
					{
						LockJobInstanceAccess(jii.JobInstanceMessage.MsgUid, "Status",-1, "JobInstanceMessageReceived");
					}
				}
				catch
				{
					if (bi.JobInfos.ContainsKey(jim.MsgUid))
						bi.JobInfos.Remove(jim.MsgUid);
					ConcurrentDictionary<string, SemaphoreSlim> removed;
					JobInstanceLocks.Remove(jii.JobInstanceMessage.MsgUid, out removed);
					return -1;
				}

				//*test*
				Log.Debug(ConsoleString() + " Job Instance START: " + jim.Build + "\nCN## " + jim);
				//*test*

				// inform the server that the job instance (with the job execution) has indeed started  and register with the RequiredPinQueues
				_server.ConfirmJobStart(jim.MsgUid, jim.RequiredPinQueues.Values.ToList(), true);

				return 0;
			}
			finally
			{
				LockBatchAccess(bi.BatchInstanceMessage.MsgUid, -1, "JobInstanceMessageReceived");
			}
		}
		
		public short JobExecutionMessageReceived(JobExecutionMessage jem)
		{
			QueueId id = new QueueId(jem);
			//*test* 
			Log.Debug(ConsoleString() + "JobExecutionMessage received from queue " + id.ToString());
			//*test*

			// find BatchInstanceInfo associated with the appropriate queue
			BatchInstanceInfo bi = _batchInfos.Values.ToList().Find(i => i.BatchInstanceMessage.JobQueueIds.Contains(id));
			
			if (null == bi)
				return -1;
			
			LockBatchAccess(bi.BatchInstanceMessage.MsgUid, 1, "JobExecutionMessageReceived");
			try
			{
				// find any job instances that are of the same job type and are not busy
				JobInstanceInfo matchingInstance = bi.JobInfos.Values.ToList()
					.Find(jii => jii.JobInstanceMessage.JobUid == jem.JobUid && jii.AcceptsJobs);
				if (null == matchingInstance)
					return -1;
				LockJobInstanceAccess(matchingInstance.JobInstanceMessage.MsgUid, "Status", 1, "JobExecutionMessageReceived");
				try
				{
					// reset the token counters
					matchingInstance.TokensReceived = 0;
					matchingInstance.TokensProcessed = 0;
					// assign a new job execution to the current job instance
					matchingInstance.JobInstanceMessage.RequiredPinQueues = jem.RequiredPinQueues;
					// mark the job instance as busy (if not multitasking)
					matchingInstance.AcceptsJobs = matchingInstance.JobInstanceMessage.IsMultitasking;
					
					Log.Debug(ConsoleString() + " Job Execution START: " + matchingInstance.JobInstanceMessage.Build +
					          "\nCN## " + jem);
					if (matchingInstance.JobInstanceMessage.IsMultitasking)
						Log.Debug(ConsoleString() + "Job Instance " + matchingInstance.JobInstanceMessage.MsgUid +
						          " (Image: " + matchingInstance.JobInstanceMessage.Build.Image +
						          ") current status is: ACCEPTING jobs (multitasking).");
					else
						Log.Debug(ConsoleString() + "Job Instance " + matchingInstance.JobInstanceMessage.MsgUid +
						          " (Image: " + matchingInstance.JobInstanceMessage.Build.Image +
						          ") current status is: NOT ACCEPTING jobs.");

					// inform the server that the job execution has indeed started and register with the RequiredPinQueues
					_server.ConfirmJobStart(matchingInstance.JobInstanceMessage.MsgUid,
						jem.RequiredPinQueues.Values.ToList(), false);
					return 0;
				}
				finally
				{
					LockJobInstanceAccess(matchingInstance.JobInstanceMessage.MsgUid, "Status", -1, "JobExecutionMessageReceived");
				}
			}
			finally
			{
				LockBatchAccess(bi.BatchInstanceMessage.MsgUid, -1, "JobExecutionMessageReceived");
			}
		}

		// ========== From IBalticNode =================================================================================

		public List<FullJobStatus> GetBatchJobStatuses(string batchMsgUid)
		{
			List<FullJobStatus> result = new List<FullJobStatus>();
			if (!_batchInfos.ContainsKey(batchMsgUid))
				return null;
			BatchInstanceInfo bi = _batchInfos[batchMsgUid];
			FullJobStatus js;
			foreach (JobInstanceInfo info in bi.JobInfos.Values)
			{
				LockJobInstanceAccess(info.JobInstanceMessage.MsgUid, "Status",1, "GetBatchJobStatuses");
				try
				{
					js = new FullJobStatus()
					{
						JobInstanceUid = info.JobInstanceMessage.MsgUid,
						TokensProcessed = info.TokensProcessed,
						TokensReceived = info.TokensReceived
					};
					JobStatus status = info.Handle.GetStatus();
					if (null != status)
					{
						js.JobProgress = status.JobProgress;
						js.Status = status.Status;
					}

					result.Add(js);
				}
				finally
				{
					LockJobInstanceAccess(info.JobInstanceMessage.MsgUid, "Status",-1, "GetBatchJobStatuses");
				}
			}
			return result;
		}

		public short FinishJobInstance(string jobInstanceUid)
		{
			BatchInstanceInfo bi = null;
			JobInstanceInfo ji = null;
			// find the appropriate batch instance info and job instance info
			foreach (BatchInstanceInfo instance in _batchInfos.Values)
				if (null != (ji = instance.JobInfos.Values.ToList()
					.Find(i => jobInstanceUid == i.JobInstanceMessage.MsgUid)))
				{
					bi = instance;
					break;
				}

			if (null == bi)
				return -1; // should not happen

			LockBatchAccess(bi.BatchInstanceMessage.MsgUid, 1, "FinishJobInstance");

			try
			{
				// stop the physical job instance
				_cluster.StopJob(ji.JobInstanceMessage.MsgUid,bi.BatchInstanceMessage.MsgUid);
				bi.JobInfos.Remove(ji.JobInstanceMessage.MsgUid);
				Log.Debug(ConsoleString() + "Instance " + jobInstanceUid + 
				          " (Image: " + ji.JobInstanceMessage.Build.Image + ") status: FINISHED.");
				return 0;
			}
			finally
			{
				LockBatchAccess(bi.BatchInstanceMessage.MsgUid, -1, "FinishJobInstance");
			}
		}

		public short FinishJobExecution(string jobInstanceUid)
		{
			BatchInstanceInfo bi = null;
			JobInstanceInfo ji = null;
			// find the appropriate batch instance info and job instance info
			foreach(BatchInstanceInfo instance in _batchInfos.Values)
				if (null != (ji = instance.JobInfos.Values.ToList().Find(i => jobInstanceUid == i.JobInstanceMessage.MsgUid)))
				{
					bi = instance;
					break;
				}
			if (null == bi)
				return -1; // should not happen

			LockBatchAccess(bi.BatchInstanceMessage.MsgUid, 1, "FinishJobExecution");
			LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "AcceptsJobs", 1, "FinishJobExecution");
			try
			{
				// mark the instance as not busy anymore
				ji.AcceptsJobs = true;
				Log.Debug(ConsoleString() + "Job Instance " + jobInstanceUid + 
				          " (Image: " + ji.JobInstanceMessage.Build.Image + ") current status is: ACCEPTING jobs (finished previous Job Execution).");
			}
			finally
			{
				LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "AcceptsJobs", -1, "FinishJobExecution");
				LockBatchAccess(bi.BatchInstanceMessage.MsgUid, -1, "FinishJobExecution");
			}
			// TODO inform the queue about job instance availability
			return 0;
		}

		public short FinishJobBatch(string batchMsgUid)
		{
			_cluster.StopBatch(_batchInfos[batchMsgUid].BatchInstanceMessage.MsgUid);
			_batchInfos.Remove(batchMsgUid);
			SemaphoreSlim removed;
			BatchLocks.Remove(batchMsgUid, out removed);
			return 0;
		}
		
		// ========= From ITokens ======================================================================================

		/// <summary>
		/// Receive and process token produced by Job
		/// </summary>
		/// <param name="tm"></param>
		/// <param name="requiredMsgUid"></param>
		/// <param name="finalMsg"></param>
		/// <returns></returns>
		public short PutTokenMessage(TokenMessage tm, string requiredMsgUid, bool finalMsg)
		{
			if (string.IsNullOrEmpty(tm?.SenderUid)) return -1;
			BatchInstanceInfo bi = _batchInfos.Values.ToList().Find(info => info.JobInfos.ContainsKey(tm.SenderUid));
			
			if (null == bi)
				return -1;

			JobInstanceMessage jm = bi.JobInfos[tm.SenderUid].JobInstanceMessage;
			LockJobInstanceAccess(jm.MsgUid,"Tokens",1, "PutTokenMessage");

			try
			{
				if (null == requiredMsgUid || !bi.TokenMessages.ContainsKey(requiredMsgUid)) return -2;
				if (!jm.ProvidedPinTokens.ContainsKey(tm.PinName)) return -3;
				if (null == tm.QueueSeqStack || !tm.QueueSeqStack.IsEmpty()) return -4;

				TokenMessage oldTm = bi.TokenMessages[requiredMsgUid];
				tm.QueueSeqStack = oldTm.QueueSeqStack.Copy();
				if (jm.IsSimple)
					tm.QueueSeqStack.AddRange(oldTm.TokenSeqStack);

				if (jm.IsSplitter)
				{
					IDictionary<string, long> jobCounters = bi.JobInfos[tm.SenderUid].ProducedTokenCounters;
					tm.QueueSeqStack.Push(new SeqToken()
						{SeqUid = tm.SenderUid, No = jobCounters[tm.PinName], IsFinal = finalMsg});
					if (!finalMsg)
						jobCounters[tm.PinName] = jobCounters[tm.PinName] + 1;
					else
						jobCounters[tm.PinName] = 0;
				}

				tm.TaskUid = bi.TokenMessages[requiredMsgUid].TaskUid;
				tm.TokenNo = jm.ProvidedPinTokens[tm.PinName];

				//*test*s
				Log.Debug(ConsoleString() + "Token RECEIVED FROM Batch: " + bi.BatchInstanceMessage.MsgUid +
				          "\n==> " + tm);
				//*test*

				if (tm.SenderUid != jm.MsgUid)
					tm.SenderUid = jm.MsgUid;

				short result = _server.PutTokenMessage(tm);
				return (short) (0 == result ? 0 : result - 4);
			}
			finally
			{
				LockJobInstanceAccess(jm.MsgUid,"Tokens",-1, "PutTokenMessage");
			}
		}

		/// <summary>
		/// Acknowledge tokens after the Job has finished their processing
		/// </summary>
		/// <param name="msgUids"></param>
		/// <param name="senderUid"></param>
		/// <param name="isFinal"></param>
		/// <param name="isFailed"></param>
		/// <param name="note"></param>
		/// <returns></returns>
		public short AckTokenMessages(List<string> msgUids, string senderUid, bool isFinal, bool isFailed, string note)
		{
			if (null == senderUid) return -1;
			if (null == msgUids || 0 == msgUids.Count) return -2;

			BatchInstanceInfo bi = _batchInfos.Values.ToList().Find(info => info.JobInfos.ContainsKey(senderUid));
			if (null == bi)
				return -1;
			JobInstanceInfo ji = bi.JobInfos[senderUid];
			LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "Tokens", 1, "AckTokenMessages");
			LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "Status", 1, "AckTokenMessages");
			
			try
			{
				QueueId id;
				Dictionary<string, QueueId> qMsgUids = new Dictionary<string, QueueId>();
				foreach (string msg in msgUids)
				{
					if (bi.TokenMessages.ContainsKey(msg))
					{
						id = new QueueId(bi.TokenMessages[msg]);
						qMsgUids.Add(bi.TokenMessages[msg].MsgUid, id);
					}
					else
						return -3;
				}

				FullJobStatus fStatus = new FullJobStatus()
				{
					JobInstanceUid = ji.JobInstanceMessage.MsgUid,
					TokensProcessed = ji.TokensProcessed + msgUids.Count,
					TokensReceived = ji.TokensReceived
				};
				JobStatus status;
				if (null != (status = ji.Handle.GetStatus()))
				{
					fStatus.JobProgress = status.JobProgress;
					fStatus.Status = status.Status;
				}

				short ret = _server.AckMessages(qMsgUids, fStatus, isFinal, isFailed, note);

				if (0 == ret)
				{
					ji.TokensProcessed += msgUids.Count;
					foreach (string msg in msgUids)
						bi.TokenMessages.Remove(msg);
					
					Log.Debug($"{ConsoleString()} Ack RECEIVED FROM Batch: {bi.BatchInstanceMessage.MsgUid}, " +
					          $"Job Instance: {ji.JobInstanceMessage.MsgUid} ({ji.JobInstanceMessage.Build.Image})" +
					          $"\n==> {string.Join(", ", msgUids)}");
					//*test*
					
					return 0;
				}

				return (short) (ret - 2);
			}
			finally
			{
				LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "Status", -1, "AckTokenMessages");
				LockJobInstanceAccess(ji.JobInstanceMessage.MsgUid, "Tokens", -1, "AckTokenMessages");
			}
		}
		
		// ========== Utility methods ==================================================================================

		/// 
		/// <param name="batchMsgUid"></param>
		/// <param name="lockUnlock"></param>
		/// <param name="label"></param>
		private short LockBatchAccess(string batchMsgUid, short lockUnlock, string label)
		{
			// TODO remove the semaphore from the dictionary when batch instance is finished
			BatchLocks.TryAdd(batchMsgUid, new SemaphoreSlim(1,1));
			if (2 < LogLevel) 
				Log.Debug(ConsoleString() + "LOCK batch (" + batchMsgUid + ") " + 
			          (1 == lockUnlock ? "WAITING" : "RELEASED") + " in: " + label);
			if (0 < lockUnlock)
			{
				BatchLocks[batchMsgUid].Wait();
				if (2 < LogLevel)
					Log.Debug(ConsoleString() + "LOCK batch (" + batchMsgUid + ") CLOSED in: " + label);
				return 1;
			}
			if (0 > lockUnlock) {
				BatchLocks[batchMsgUid].Release();
				return 0;
			}
			return -1;
		}

		/// 
		/// <param name="jobInstanceUid"></param>
		/// <param name="resourceName"></param>
		/// <param name="lockUnlock"></param>
		/// <param name="label"></param>
		private short LockJobInstanceAccess(string jobInstanceUid, string resourceName, short lockUnlock, string label)
		{
			// TODO remove the semaphore from the dictionary when batch instance is finished
			JobInstanceLocks.TryAdd(jobInstanceUid, new ConcurrentDictionary<string, SemaphoreSlim>());
			JobInstanceLocks[jobInstanceUid].TryAdd(resourceName, new SemaphoreSlim(1,1));

			if (2 < LogLevel)
				Log.Debug(ConsoleString() + "LOCK " + resourceName + " (" + jobInstanceUid + ") " + 
				          (1 == lockUnlock ? "WAITING" : "RELEASED") + " in: " + label);
			if (0 < lockUnlock)
			{
				JobInstanceLocks[jobInstanceUid][resourceName].Wait();
				if (2 < LogLevel)
					Log.Debug(ConsoleString() + "LOCK " + resourceName + " (" + jobInstanceUid + ") CLOSED in: " + label);
				return 1;
			}
			if (0 > lockUnlock) {
				JobInstanceLocks[jobInstanceUid][resourceName].Release();
				return 0;
			}
			return -1;
		}
		
		private string ConsoleString(){
			return "## NODE.BATCHMGR ## " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}
	}
}