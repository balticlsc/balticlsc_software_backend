using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Engine.JobBroker;
using Baltic.Engine.TaskProcessor;
using Baltic.Types.Protos;
using Grpc.Core;

namespace Baltic.TaskManager.Controllers
{
    public class BalticServerService : BalticServerServiceApi.BalticServerServiceApiBase
    {
        private ITaskProcessor _taskProcessor;
        private IJobBroker _jobBroker;
        
        public BalticServerService(ITaskProcessor tp, IJobBroker jb){
            _taskProcessor = tp;
            _jobBroker = jb;
        }

        public override Task<ResponseStatus> AckMessages(XAckRequest request, ServerCallContext context)
        {
            FullJobStatus status = DBMapper.Map<FullJobStatus>(request.Status, new FullJobStatus()
            {
                Status = (ComputationStatus) request.Status.Status
            });
            _jobBroker.UpdateJobStatus(status,request.IsFailed,request.Note);
            
            Dictionary<string,QueueId> msgUids = new Dictionary<string,QueueId>();
            foreach (StringStringPair msgUid in request.MsgUids)
                msgUids.Add(msgUid.Key,QueueId.Parse(msgUid.Value));
            short result = _taskProcessor.AckMessages(msgUids,request.Status.JobInstanceUid,request.IsFinal,request.IsFailed);
            
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }

        public override Task<ResponseStatus> ConfirmBatchStart(XConfirmBatchRequest request, ServerCallContext context)
        {
            _jobBroker.ConfirmBatchStart(request.BatchMsgUid,request.RequiredJobQueues.Select(QueueId.Parse).ToList());
            return Task.FromResult(new ResponseStatus()
            {
                Code = ResponseStatus.Types.Codes.Ok
            });
        }

        public override Task<ResponseStatus> ConfirmJobStart(XConfirmJobRequest request, ServerCallContext context)
        {
            _jobBroker.ConfirmJobStart(request.InstanceUid, request.RequiredPinQueues.Select(QueueId.Parse).ToList(), request.IsNewInstance);
            return Task.FromResult(new ResponseStatus()
            {
                Code = ResponseStatus.Types.Codes.Ok
            });
        }

        public override Task<ResponseStatus> PutTokenMessage(XTokenMessage request, ServerCallContext context)
        {
            TokenMessage msg = DBMapper.Map<TokenMessage>(request, new TokenMessage()
            {
                QueueSeqStack = new SeqTokenStack(request.QueueSeqStack.Select(s => new SeqToken()
                {
                    No = s.No,
                    IsFinal = s.IsFinal,
                    SeqUid = s.SeqUid
                }).ToList()),
                TokenSeqStack = new SeqTokenStack(request.TokenSeqStack.Select(s => new SeqToken()
                {
                    No = s.No,
                    IsFinal = s.IsFinal,
                    SeqUid = s.SeqUid
                }).ToList()),
                DataSet = new CDataSet()
                {
                    Values = request.Values
                }
            });
            short result = _taskProcessor.PutTokenMessage(msg);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }
    }
}