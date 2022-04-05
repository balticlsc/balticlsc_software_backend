using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Types.Protos;
using Microsoft.Extensions.Configuration;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Types.Entities
{
    public class DataModelImplFactory : IDataModelImplFactory
    {

        private IConfiguration _configuration;

        public DataModelImplFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public CJobBatch CreateCJobBatch()
        {
            return (CJobBatch) new CJobBatchImpl
            {
                Uid = Guid.NewGuid().ToString(),
                Jobs = new List<CJob>(),
                Services = new List<CService>(),
                Tokens = new List<CDataToken>(),
                BatchExecutions = new List<BatchExecution>()
            };
        }

        public CTask CreateCTask()
        {
            return (CTask) new CTaskImpl
            {
                Uid = Guid.NewGuid().ToString(),
                Batches = new List<CJobBatch>(),
                Tokens = new List<CDataToken>(),
                Execution = new TaskExecutionImpl()
            };
        }

        public CJob CreateCJob()
        {
            return new CJobImpl
            {
                Uid = Guid.NewGuid().ToString(),
                Tokens = new List<CDataToken>(),
                JobExecutions = new List<JobExecution>(),
                JobInstances = new List<JobInstance>()
            };
        }
        
        public CService CreateCService()
        {
            return new CServiceImpl
            {
                Uid = "b-" + Guid.NewGuid().ToString(),
                Tokens = new List<CDataToken>(),
            };
        }

        public CDataToken CreateCDataToken()
        {
            return (CDataToken) new CDataTokenImpl
            {
                Uid = Guid.NewGuid().ToString(),
                Depths = new List<int>()
            };
        }

        public BatchExecution CreateBatchExecution()
        {
            return (BatchExecution) new BatchExecutionImpl()
            {
                JobExecutions = new List<JobExecution>(),
                JobInstances = new List<JobInstance>()
            };
        }

        public JobExecution CreateJobExecution()
        {
            return (JobExecution) new JobExecutionImpl();
        }
        
        public JobInstance CreateJobInstance()
        {
            return (JobInstance) new JobInstanceImpl()
            {
                Usage = new List<ResourceUsage>(),
                Executions = new List<JobExecution>()
            };
        }

        public BalticModuleBuild CreateBalticModuleBuild()
        {
            return new BalticModuleBuild();
        }
        
        public BalticModuleBuild CreateBalticModuleBuild(XBalticModuleBuild xBuild)
        {
            BalticModuleBuild build = DBMapper.Map<BalticModuleBuild>(xBuild, CreateBalticModuleBuild());
            build.Scope = (NetworkScope) xBuild.Scope;
            build.Resources = DBMapper.Map<ResourceRequest>(xBuild.Resources,new ResourceRequest()
            {
                Gpus = DBMapper.Map<GpuRequest>(build.Resources.Gpus,new GpuRequest())
            });
            build.Volumes = xBuild.Volumes.Select(v => DBMapper.Map<VolumeDescription>(v, new VolumeDescription())).ToList();
            build.CommandArguments = xBuild.CommandArguments.ToList();
            build.ConfigFiles =  xBuild.ConfigFiles.Select(c => DBMapper.Map<ConfigFileDescription>(c,new ConfigFileDescription())).ToList();
            build.EnvironmentVariables = new Dictionary<string, string>();
            foreach (XEnvironmentVariable ev in xBuild.EnvironmentVariables) 
                build.EnvironmentVariables.Add(ev.Key,ev.Value);
            build.PortMappings = xBuild.PortMappings.Select(pm => DBMapper.Map<PortMapping>(pm, new PortMapping()))
                .ToList();
            return build;
        }

        public BalticModuleBuild CreateModuleManagerBuild()
        {
            return new BalticModuleBuild()
            {
                BatchId = "",
                ModuleId = "",
                Image = "balticlsc/balticmodulemanager:latest",
                EnvironmentVariables = new Dictionary<string, string>()
                {
                    {"ModuleManagerPort", "7301"},
                    {"BatchManagerUrl", $"{_configuration["batchManagerUrl"]}:{_configuration["nodePort"]}"}
                },
                Command = "",
                CommandArguments = new List<string>(),
                Scope = "true" == _configuration["localExecution"] ? NetworkScope.Cluster : NetworkScope.Workspace,
                PortMappings = new List<PortMapping>()
                    {
                        new PortMapping() {ContainerPort = 7301, PublishedPort = 7301, Protocol = CommProtocol.Tcp},
                        new PortMapping() {ContainerPort = 7300, PublishedPort = 7300, Protocol = CommProtocol.Tcp}
                    },
                
                Volumes = new List<VolumeDescription>(),
                ConfigFiles = new List<ConfigFileDescription>(),
                Resources = new ResourceRequest()
                {
                    Cpus = 250, //msec
                    Memory = 256, // MB
                    Gpus = new GpuRequest()
                    {
                        Quantity = 0,
                        Type = "none"
                    }
                }
            };
        }
    }
}