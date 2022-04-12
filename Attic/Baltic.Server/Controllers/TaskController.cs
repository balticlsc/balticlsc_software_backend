using System;
using System.Collections.Generic;
using Baltic.Server.Controllers.Models;
using Microsoft.AspNetCore.Mvc;
using ComputationJob = Baltic.Server.Models.Jobs.ComputationJob;
using ResourceUsageMetric = Baltic.Server.Models.Resources.ResourceUsageMetric;
using ComputationTask = Baltic.Server.Models.Computation.ComputationTask;

namespace Baltic.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("task")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        /// <summary>
        /// [GetTasks] Return list of all computation tasks.
        /// </summary>
        /// <returns>Computation tasks as an array.</returns>
        [HttpGet]
        [Route("list")]
        public IEnumerable<ComputationTask> GetTasks()
        {
            var tasks = new List<ComputationTask>()
            {
                new ComputationTask() 
                {
                    Id=4,
                    UserId=15,
                    Status="running",
                    ComputationApplicationReleaseId=13,
                    Progress=20.5f,
                    ReservedCredits=76.3f,
                    EstimatedCredits=55.3f,
                    ConsumedCredits=10.7f,
                    StartTime=DateTime.Now - new TimeSpan(1,1,1),
                    EndTime= new DateTime(),
                    Priority= 4,
                    IsPrivate= false,
                    SafeMode= false,
                    JobList = new List<ComputationJob>() 
                    {
                        new ComputationJob()
                    {
                        Name = "Volume counter",
                        ClusterId = "BalticCluster01",
                        ComputationModuleName = "volume counter",
                        ComputationModuleReleaseId = "v1.0",
                        ComputationTaskId = "1",
                        Progress = 0.0F,
                        Status = "pending",
                        StartTime = new DateTime(),
                        EndTime = new DateTime(),
                        EstimatedCredits = 180,
                        Id = "0",
                        ReservedCredits = 200,
                        UsedCredits = 0,
                        ResourceUsageMetrics = new List<ResourceUsageMetric>()
                        {
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "20"},
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "CPU", Value = "31"},
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "RAM", Value = "66"}
                        }
                    },
                        new ComputationJob()
                    {
                        Name = "Median Counter",
                        ClusterId = "BalticCluster01",
                        ComputationModuleName = "median counter",
                        ComputationModuleReleaseId = "v1.0",
                        ComputationTaskId = "1",
                        Progress = 64.25F,
                        Status = "running",
                        StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                        EndTime = new DateTime(),
                        EstimatedCredits = 128,
                        Id = "1",
                        ReservedCredits = 150,
                        UsedCredits = 90,
                        ResourceUsageMetrics = new List<ResourceUsageMetric>()
                        {
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "10"},
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "12"},
                            new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "CPU", Value = "8"}
                        }
                    }, 
                        new ComputationJob()
                    {
                        Name = "neural network",
                        ClusterId = "BalticCluster01",
                        ComputationModuleName = "convolutional neural network",
                        ComputationModuleReleaseId = "v2.0",
                        ComputationTaskId = "1",
                        Progress = 100.0F,
                        Status = "completed",
                        StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                        EndTime = new DateTime(2019, 12, 3, 6, 24, 48),
                        EstimatedCredits = 195,
                        Id = "2",
                        ReservedCredits = 210,
                        UsedCredits = 200,
                        ResourceUsageMetrics = new List<ResourceUsageMetric>()
                        {
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "18"},
                            new ResourceUsageMetric() {MachineId = "baltic5", ResourceType = "GPU", Value = "16"},
                            new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "GPU", Value = "10"}
                        }
                    }
                }

                },
                new ComputationTask()
                {
                    Id=5,
                    UserId=15,
                    Status="pending",
                    ComputationApplicationReleaseId=22,
                    Progress=85.3f,
                    ReservedCredits=92.3f,
                    EstimatedCredits=70.3f,
                    ConsumedCredits=54.3f,
                    StartTime=DateTime.Now - new TimeSpan(3,1,4,1),
                    EndTime= new DateTime(),
                    Priority= 3,
                    IsPrivate= true,
                    SafeMode= true,
                    JobList = new List<ComputationJob>()
                    {
                        new ComputationJob()
                        {
                            Id = "0",
                            Name = "PI counter",
                            ComputationTaskId = "2",
                            ComputationModuleReleaseId = "v1.0",
                            ComputationModuleName = "PI counter",
                            ClusterId = "BalticCluster03",
                            Progress = 100.0f,
                            Status = "completed",
                            StartTime = DateTime.Today,
                            EndTime = DateTime.Today,
                            ReservedCredits = 180,
                            EstimatedCredits = 175,
                            UsedCredits = 150,
                            ResourceUsageMetrics = new List<ResourceUsageMetric>()
                            {
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "48",
                                    MachineId = "baltic12",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "32",
                                    MachineId = "baltic13",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "60",
                                    MachineId = "baltic14",
                                },
                            },
                        },
                        new ComputationJob()
                        {
                            Id = "2",
                            Name = "Neural network",
                            ComputationTaskId = "2",
                            ComputationModuleReleaseId = "v2.2",
                            ComputationModuleName = "Deep learning neural network",
                            ClusterId = "BalticCluster04",
                            Progress = 0.0f,
                            Status = "created",
                            StartTime = new DateTime(),
                            EndTime = new DateTime(),
                            ReservedCredits = 0,
                            EstimatedCredits = 180,
                            UsedCredits = 0,
                            ResourceUsageMetrics = new List<ResourceUsageMetric>()
                            {
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "CPU",
                                    Value = "85",
                                    MachineId = "baltic1",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "CPU",
                                    Value = "90",
                                    MachineId = "baltic2",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "40",
                                    MachineId = "baltic2",
                                },
                            },
                        },
                        new ComputationJob()
                        {
                            Id = "3",
                            Name = "Genetic algorithm",
                            ComputationTaskId = "2",
                            ComputationModuleReleaseId = "v3.2",
                            ComputationModuleName = "Genetic Traveling Salesman Problem solving module",
                            ClusterId = "BalticCluster05",
                            Progress = 64.10f,
                            Status = "running",
                            StartTime = new DateTime(2019, 12, 1, 10, 35, 46),
                            EndTime = new DateTime(),
                            ReservedCredits = 320,
                            EstimatedCredits = 250,
                            UsedCredits = 240,
                            ResourceUsageMetrics = new List<ResourceUsageMetric>()
                            {
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "65",
                                    MachineId = "baltic1",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "46",
                                    MachineId = "baltic2",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "CPU",
                                    Value = "90",
                                    MachineId = "baltic2",
                                },
                            },
                        }
                    }
                },
                new ComputationTask()
                {
                    Id=6,
                    UserId=15,
                    Status="finished",
                    ComputationApplicationReleaseId=21,
                    Progress=100.0f,
                    ReservedCredits=52.1f,
                    EstimatedCredits=43.6f,
                    ConsumedCredits=50.1f,
                    StartTime = DateTime.Now - new TimeSpan(2,13,10,5),
                    EndTime= DateTime.Now - new TimeSpan(1,1,1),
                    Priority= 0,
                    IsPrivate= true,
                    SafeMode= true,
                    JobList = new List<ComputationJob>()
                    {
                        new ComputationJob()
                        {
                            Id = "3",
                            Name = "Material thermal conduction",
                            ComputationTaskId = "3",
                            ComputationModuleReleaseId = "v.2.3",
                            ComputationModuleName = "Thermal mapping",
                            ClusterId = "4",
                            Progress = 44.4f,
                            Status = "working",
                            StartTime = DateTime.Today,
                            EndTime = new DateTime(),
                            ReservedCredits = 102,
                            EstimatedCredits = 42,
                            UsedCredits = 12,
                            ResourceUsageMetrics = new List<ResourceUsageMetric>()
                            {
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "34",
                                    MachineId = "4",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "CPU",
                                    Value = "22",
                                    MachineId = "4",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "RAM",
                                    Value = "21",
                                    MachineId = "4",
                                },
                            },
                        },
                        new ComputationJob()
                        {
                            Id = "3",
                            Name = "Particle swarm optimization",
                            ComputationTaskId = "3",
                            ComputationModuleReleaseId = "44",
                            ComputationModuleName = "Particle logic creating",
                            ClusterId = "3",
                            Progress = 34.2f,
                            Status = "corrupted",
                            StartTime = DateTime.Today,
                            EndTime = new DateTime(),
                            ReservedCredits = 220,
                            EstimatedCredits = 200,
                            UsedCredits = 65,
                            ResourceUsageMetrics = new List<ResourceUsageMetric>()
                            {
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "CPU",
                                    Value = "34",
                                    MachineId = "1",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "22",
                                    MachineId = "1",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "RAM",
                                    Value = "35",
                                    MachineId = "1",
                                },
                            },
                        },
                        new ComputationJob()
                        {
                            Id = "4",
                            Name = "Path finding",
                            ComputationTaskId = "99",
                            ComputationModuleReleaseId = "v.2.2",
                            ComputationModuleName = "Genetics evaluating",
                            ClusterId = "4",
                            Progress = 54.2f,
                            Status = "stopped",
                            StartTime = DateTime.Today,
                            EndTime = new DateTime(),
                            ReservedCredits = 150,
                            EstimatedCredits = 100,
                            UsedCredits = 33,
                            ResourceUsageMetrics = new List<ResourceUsageMetric>()
                            {
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "GPU",
                                    Value = "44",
                                    MachineId = "2",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "CPU",
                                    Value = "98",
                                    MachineId = "2",
                                },
                                new ResourceUsageMetric()
                                {
                                    ResourceType = "RAM",
                                    Value = "32",
                                    MachineId = "2",
                                },
                            },
                        }
                    }
                }
            };

            return tasks;
        }

        /// <summary>
        /// [ActivateTask] Begins to execute next stage of computation task with given id
        /// </summary>
        /// <param name="computationTaskId">Id of computation task.</param>
        /// <returns>Single task with updated status.</returns>
        [HttpPost]
        [Route("activate")]
        public ComputationTask ActivateTask([FromBody] string computationTaskId)
        {
            int.TryParse(computationTaskId, out var customId);

            var computationTask = new ComputationTask()
            {
                Id = customId,
                UserId = 15,
                Status = "created",
                ComputationApplicationReleaseId = 11,
                Progress = 0.0f,
                ReservedCredits = 150.0f,
                EstimatedCredits = 145.0f,
                ConsumedCredits = 0.0f,
                StartTime = DateTime.Now,
                EndTime = new DateTime(),
                Priority = 1,
                IsPrivate = true,
                SafeMode = true,
                JobList = new List<ComputationJob>()
            };

            return computationTask;
        }

        /// <summary>
        /// [PauseTask] Pause computation task with given id.
        /// </summary>
        /// <param name="computationTaskId">Id of computation task.</param>
        /// <returns>Single task with updated status.</returns>
        [HttpPost]
        [Route("pause")]
        public ComputationTask PauseTask([FromBody] string computationTaskId)
        {
            return new ComputationTask();
        }

        /// <summary>
        /// [AbortTask] Abort computation task with given id.
        /// </summary>
        /// <param name="computationTaskId">Id of computation task.</param>
        /// <returns>Single task with updated status.</returns>
        [HttpGet]
        [Route("abort")]
        public ComputationTask AbortTask([FromQuery] string computationTaskId)
        {
            return new ComputationTask();
        }

        /// <summary>
        /// [GetTaskJobs] Returns list of computation jobs for a task with given id.
        /// </summary>
        /// <param name="computationTaskId">Id of computation task.</param>
        /// <returns>Computations Jobs for computation task with given id as a array.</returns>
        [HttpGet]
        [Route("joblist")]
        public IEnumerable<ComputationJob> GetTaskJobs([FromQuery] string computationTaskId)
        {
            var taskJobs = new List<ComputationJob>()
            {
                new ComputationJob()
                {
                    Name = "Job1",
                    ClusterId = "BalticCluster01",
                    ComputationModuleName = "volume counter",
                    ComputationModuleReleaseId = "v1.0",
                    ComputationTaskId = computationTaskId,
                    Progress = 0.0F,
                    Status = "pending",
                    StartTime = new DateTime(),
                    EndTime = new DateTime(),
                    EstimatedCredits = 180,
                    Id = "0",
                    ReservedCredits = 200,
                    UsedCredits = 0,
                    ResourceUsageMetrics = new List<ResourceUsageMetric>()
                    {
                        new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "0"},
                        new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "CPU", Value = "0"},
                        new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "RAM", Value = "0"}
                    }
                },
                new ComputationJob()
                {
                    Name = "Median counter",
                    ClusterId = "BalticCluster01",
                    ComputationModuleName = "median counter",
                    ComputationModuleReleaseId = "v1.0",
                    ComputationTaskId = computationTaskId,
                    Progress = 64.25F,
                    Status = "running",
                    StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                    EndTime = new DateTime(),
                    EstimatedCredits = 128,
                    Id = "1",
                    ReservedCredits = 150,
                    UsedCredits = 90,
                    ResourceUsageMetrics = new List<ResourceUsageMetric>()
                    {
                        new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "50"},
                        new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "42"},
                        new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "CPU", Value = "36"}
                    }
                },
                new ComputationJob()
                {
                    Name = "neural network",
                    ClusterId = "BalticCluster01",
                    ComputationModuleName = "convolutional neural network",
                    ComputationModuleReleaseId = "v2.0",
                    ComputationTaskId = computationTaskId,
                    Progress = 100.0F,
                    Status = "completed",
                    StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                    EndTime = new DateTime(2019, 12, 3, 6, 24, 48),
                    EstimatedCredits = 195,
                    Id = "2",
                    ReservedCredits = 210,
                    UsedCredits = 200,
                    ResourceUsageMetrics = new List<ResourceUsageMetric>()
                    {
                        new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "28"},
                        new ResourceUsageMetric() {MachineId = "baltic5", ResourceType = "GPU", Value = "18"},
                        new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "GPU", Value = "50"}
                    }
                }
            };

            return taskJobs;
        }

        /// <summary>
        /// [GetTask] Returns computation task with given id.
        /// </summary>
        /// <param name="computationTaskId">Id of computation task.</param>
        /// <returns>Single computation task with given id.</returns>
        [HttpGet]
        public ComputationTask GetTask([FromQuery] string computationTaskId)
        {
            int.TryParse(computationTaskId, out var customId);

            var task = new ComputationTask()
            {
                Id = customId,
                UserId = 15,
                Status = "pending",
                ComputationApplicationReleaseId = 12,
                Progress = 31.2f,
                ReservedCredits = 100.0f,
                EstimatedCredits = 85.4f,
                ConsumedCredits = 20.2f,
                StartTime = DateTime.Now - new TimeSpan( 2, 3, 0),
                EndTime = DateTime.Now + new TimeSpan(1,2,10,0),
                Priority = 3,
                IsPrivate = true,
                SafeMode = true,
                JobList = new List<ComputationJob>
                {
                    new ComputationJob()
                    {
                        ClusterId = "BalticCluster01",
                        ComputationModuleName = "volume counter",
                        ComputationModuleReleaseId = "v1.0",
                        ComputationTaskId = computationTaskId,
                        Progress = 0.0F,
                        Status = "pending for start",
                        StartTime = new DateTime(),
                        EndTime = new DateTime(),
                        EstimatedCredits = 180,
                        Id = "0",
                        ReservedCredits = 200,
                        UsedCredits = 0,
                        ResourceUsageMetrics = new List<ResourceUsageMetric>()
                        {
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "0"},
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "CPU", Value = "0"},
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "RAM", Value = "0"}
                        }
                    },
                    new ComputationJob()
                    {
                        ClusterId = "BalticCluster01",
                        ComputationModuleName = "median counter",
                        ComputationModuleReleaseId = "v1.0",
                        ComputationTaskId = computationTaskId,
                        Progress = 64.25F,
                        Status = "running",
                        StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                        EndTime = new DateTime(),
                        EstimatedCredits = 128,
                        Id = "1",
                        ReservedCredits = 150,
                        UsedCredits = 90,
                        ResourceUsageMetrics = new List<ResourceUsageMetric>()
                        {
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "50"},
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "42"},
                            new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "CPU", Value = "36"}
                        }
                    },
                    new ComputationJob()
                    {
                        ClusterId = "BalticCluster01",
                        ComputationModuleName = "convolutional neural network",
                        ComputationModuleReleaseId = "v2.0",
                        ComputationTaskId = computationTaskId,
                        Progress = 100.0F,
                        Status = "completed",
                        StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                        EndTime = new DateTime(2019, 12, 3, 6, 24, 48),
                        EstimatedCredits = 195,
                        Id = "2",
                        ReservedCredits = 210,
                        UsedCredits = 200,
                        ResourceUsageMetrics = new List<ResourceUsageMetric>()
                        {
                            new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "28"},
                            new ResourceUsageMetric() {MachineId = "baltic5", ResourceType = "GPU", Value = "18"},
                            new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "GPU", Value = "50"}
                        }
                    }
                }
            };

            return task;
        }

        /// <summary>
        /// [GetTaskContextDefinitionUpdates] Returns definitions of complementary parameters needed for further progress of computation.
        /// </summary>
        /// <param name="computationTaskId">Id of computation task.</param>
        /// <returns>User interaction requests for a computation task with given id as a array.</returns>
        [HttpGet]
        [Route("context")]
        public IEnumerable<UserInteractionRequest>
            GetTaskContextDefinitionUpdates(
                [FromQuery] string computationTaskId) // do obsługi tylko na froncie JSON wytyczne do budowy 
            // do interakcji z taskiem tak samo jak przy PrepareComputationApplicationContextDefinition
            // dla wielu inteakcji Dictionary<userIneractionIdString,userInteractionString> 
        {
            return new List<UserInteractionRequest>();
        }

        /// <summary>
        ///  [UpdateTaskContext] Allows to enter values of complementary parameters needed for further progress of computation.
        /// </summary>
        /// <param name="UserInteractionId">Id of user interaction.</param>
        /// <param name="contextUpdateJSON">Values of parameters as JSON.</param>
        [HttpPost]
        [Route("context")]
        public void UpdateTaskContext([FromBody] string UserInteractionId,
                string contextUpdateJSON) // do obsługi tylko na froncie JSON wytyczne do budowy 
            // do interakcji z taskiem tak samo jak przy PrepareComputationApplicationContextDefinition
        {
        }

        /// <summary>
        /// [GetJob] Returns computation jobs with given id.
        /// </summary>
        /// <param name="computationJobId">Id of computation job.</param>
        /// <returns>Single computation job with given id.</returns>
        [HttpGet]
        [Route("job")]
        public ComputationJob GetJob([FromQuery] string computationJobId)
        {
            var computationJob = new ComputationJob()
            {
                Name = "Job name",
                ClusterId = "BalticCluster01",
                ComputationModuleName = "module name",
                ComputationModuleReleaseId = "v1.0",
                ComputationTaskId = "12",
                Progress = 64.25F,
                Status = "running",
                StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                EndTime = new DateTime(),
                EstimatedCredits = 128,
                Id = computationJobId ?? "0",
                ReservedCredits = 150,
                UsedCredits = 90,
                ResourceUsageMetrics = new List<ResourceUsageMetric>()
                {
                    new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "10"},
                    new ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "12"},
                    new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "CPU", Value = "8"}
                }
            };
            return computationJob;
        }
    }
}