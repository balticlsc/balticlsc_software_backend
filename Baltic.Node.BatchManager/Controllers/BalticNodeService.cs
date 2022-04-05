using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.Engine.JobBroker;
using Baltic.Node.Engine.BatchManager;
using Baltic.Types.Entities;
using Baltic.Types.Protos;
using Grpc.Core;

namespace Baltic.Node.BatchManager.Controllers
{
    public class BalticNodeService : BalticNodeServiceApi.BalticNodeServiceApiBase
    {
        private IBalticNode _node;
        private IMessageConsumer _consumer;
        private IDataModelImplFactory _factory;
        
        public BalticNodeService(IBalticNode node, IMessageConsumer consumer, IDataModelImplFactory factory)
        {
            _node = node;
            _consumer = consumer;
            _factory = factory;
        }

        public override Task<ResponseStatus> BatchInstanceMessageReceived(XBatchInstanceMessage request, ServerCallContext context)
        {
            BatchInstanceMessage msg = DBMapper.Map<BatchInstanceMessage>(request, new BatchInstanceMessage()
            {
                QueueSeqStack = new SeqTokenStack(request.QueueSeqStack.Select(s => new SeqToken()
                {
                    No = s.No,
                    IsFinal = s.IsFinal,
                    SeqUid = s.SeqUid
                }).ToList()),
                JobQueueIds = request.JobQueueIds.Select(QueueId.Parse).ToList(),
                ServiceBuilds = request.ServiceBuilds.Select(sb => _factory.CreateBalticModuleBuild(sb)).ToList(),
                Quota = DBMapper.Map<ResourceReservation>(request.Quota,new ResourceReservation())
            });

            short result = _consumer.BatchInstanceMessageReceived(msg);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }
        
        public override Task<ResponseStatus> BatchExecutionMessageReceived(XBatchExecutionMessage request, ServerCallContext context)
        {
            BatchExecutionMessage msg = DBMapper.Map<BatchExecutionMessage>(request, new BatchExecutionMessage()
            {
                QueueSeqStack = new SeqTokenStack(request.QueueSeqStack.Select(s => new SeqToken()
                {
                    No = s.No,
                    IsFinal = s.IsFinal,
                    SeqUid = s.SeqUid
                }).ToList()),
                JobQueueIds = request.JobQueueIds.Select(QueueId.Parse).ToList(),
            });

            short result = _consumer.BatchExecutionMessageReceived(msg);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }

        public override Task<ResponseStatus> FinishJobInstance(XJobInstanceRequest request, ServerCallContext context)
        {
            short result = _node.FinishJobInstance(request.JobMsgUid);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }
        
        public override Task<ResponseStatus> FinishJobExecution(XJobInstanceRequest request, ServerCallContext context)
        {
            short result = _node.FinishJobExecution(request.JobMsgUid);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }

        public override Task<ResponseStatus> FinishJobBatch(XBatchInstanceRequest request, ServerCallContext context)
        {
            short result = _node.FinishJobBatch(request.JobsQueueUid);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }

        public override Task<ResponseStatus> JobExecutionMessageReceived(XJobExecutionMessage request, ServerCallContext context)
        {
            JobExecutionMessage jem = DBMapper.Map<JobExecutionMessage>(request, new JobExecutionMessage()
            {
                QueueSeqStack = new SeqTokenStack(request.QueueSeqStack.Select(s => new SeqToken()
                {
                    No = s.No,
                    IsFinal = s.IsFinal,
                    SeqUid = s.SeqUid
                }).ToList()),
                RequiredPinQueues = new ConcurrentDictionary<string, QueueId>()
            });
            foreach(StringStringPair elem in request.RequiredPinQueues)
                jem.RequiredPinQueues.Add(elem.Key,QueueId.Parse(elem.Value));
            
            short result = _consumer.JobExecutionMessageReceived(jem);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }

        public override Task<ResponseStatus> JobInstanceMessageReceived(XJobInstanceMessage request, ServerCallContext context)
        {
            JobInstanceMessage jim = DBMapper.Map<JobInstanceMessage>(request, new JobInstanceMessage()
            {
                QueueSeqStack = new SeqTokenStack(request.QueueSeqStack.Select(s => new SeqToken()
                {
                    No = s.No,
                    IsFinal = s.IsFinal,
                    SeqUid = s.SeqUid
                }).ToList()),
                RequiredAccessTypes = request.RequiredAccessTypes.ToList(),
                ProvidedPinTokens = new ConcurrentDictionary<string, long>(),
                RequiredPinQueues = new ConcurrentDictionary<string, QueueId>(),
                Build = _factory.CreateBalticModuleBuild(request.Build)
            });
            foreach(StringLongPair elem in request.ProvidedPinTokens)
                jim.ProvidedPinTokens.Add(elem.Key,elem.Value);
            foreach(StringStringPair elem in request.RequiredPinQueues)
                jim.RequiredPinQueues.Add(elem.Key,QueueId.Parse(elem.Value));
            
            short result = _consumer.JobInstanceMessageReceived(jim);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }

        public override Task<ResponseStatus> TokenMessageReceived(XTokenMessage request, ServerCallContext context)
        {
            TokenMessage tm = DBMapper.Map<TokenMessage>(request, new TokenMessage()
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
            
            short result = _consumer.TokenMessageReceived(tm);
            return Task.FromResult(new ResponseStatus()
            {
                Code = 0 == result ? ResponseStatus.Types.Codes.Ok : ResponseStatus.Types.Codes.Unknown
            });
        }

        public override Task<XJobInstanceStatusList> GetBatchJobStatuses(XBatchInstanceRequest request, ServerCallContext context)
        {
            List<FullJobStatus> statuses = _node.GetBatchJobStatuses(request.JobsQueueUid);
            List<XFullJobStatus> xStatuses = null != statuses
                ? statuses.Select(s => DBMapper.Map<XFullJobStatus>(s, new XFullJobStatus()
                {
                    Status = (int) s.Status
                })).ToList()
                : new List<XFullJobStatus>();
            return Task.FromResult(new XJobInstanceStatusList()
            {
                Statuses = {xStatuses}
            });
        }
    }
}