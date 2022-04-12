using System;
using System.Collections.Generic;
using Baltic.Server.Controllers.Models;
using Microsoft.AspNetCore.Mvc;
using ResourceUsageMetric = Baltic.Server.Models.Resources.ResourceUsageMetric;
using ComputationJob = Baltic.Server.Models.Jobs.ComputationJob;

namespace Baltic.Server.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("resource")]
    [ApiController]
    public class NetworkController : ControllerBase
    {

        /// <summary>
        /// Returns list of cluster nodes from user's resource shelf.
        /// </summary>
        /// <returns>Cluster nodes from user's shelf as an array.</returns>
        [HttpGet]
        [Route("shelf")]
        public IEnumerable<Models.Node> GetShelfResources()
        {
            return new List<Models.Node>()
            {
                new Models.Node()
                {
                    Country = "Poland",
                    Id = "6432",
                    Status = "Started",
                    IsPrivate = true,
                    DataThroughput = new List<string>(){""},
                    Endpoint = "54.65.65.21",
                    HighAvailability = 10,
                    Performance = new List<Benchmark>()
                    {
                        new Benchmark(), new Benchmark()
                    },
                    TimeAvailability = new List<TimeSlot>()
                    {
                        new TimeSlot(), new TimeSlot()
                    }
                },

                new Models.Node()
                {
                    Country = "Latvia",
                    Id = "1442",
                    Status = "Stopped",
                    IsPrivate = true,
                    DataThroughput = new List<string>(){""},
                    Endpoint = "57.77.65.11",
                    HighAvailability = 10,
                    Performance = new List<Benchmark>()
                    {
                        new Benchmark(), new Benchmark()
                    },
                    TimeAvailability = new List<TimeSlot>()
                    {
                        new TimeSlot(), new TimeSlot()
                    }
                }

            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("creditstatistics")]
        public IEnumerable<EarnedCredit> GetCreditStatistics([FromQuery] EarnedCreditRequest request)
        {
            return new List<EarnedCredit>()
            {

                new EarnedCredit()
                {
                    Credits = new List<float>() { 12.0f,22.5f,33.6f },
                    Intervals = new List<string>() { "10.10.2019 - 12.10.2019", "13.10.2019 - 15.10.2019", "15.11.2019 - 17.11.2019" }
                },
                new EarnedCredit()
                {
                    Credits = new List<float>() { 13.3f,43.5f,24.5f },
                    Intervals = new List<string>() {"8.8.2019 - 8.8.2019", "22.10.2019 - 25.10.2019", "10.12.2019 - 11.12.2019" }
                },
                new EarnedCredit()
                {
                    Credits = new List<float>() { 11.0f,232.53f,23.46f },
                    Intervals = new List<string>() {"8.4.2019 - 10.4.2019", "30.6.2019 - 3.7.2019", "13.9.2019 - 17.9.2019" }
                },
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        public IEnumerable<Models.Node> GetResources()
        {
            List<Models.Node> list = new List<Models.Node>()
            {
                new Models.Node()
                {
                    Country = "Germany",
                    DataThroughput = new List<string>() {"1GB/s", "2GB/s", "3GB/s", "4GB/s"},
                    Endpoint = "11.43.32.933",
                    HighAvailability = 20,
                    Id = "123",
                    IsPrivate = false,
                    Performance = new List<Benchmark>() {new Benchmark(), new Benchmark(), new Benchmark()},
                    Status = "Available",
                    TimeAvailability = new List<TimeSlot>() {new TimeSlot(), new TimeSlot(), new TimeSlot()}
                },
                new Models.Node()
                {
                    Country = "Spain",
                    DataThroughput = new List<string>() {"1GB/s", "2GB/s", "3GB/s", "4GB/s"},
                    Endpoint = "12.87.9.976",
                    HighAvailability = 12,
                    Id = "124",
                    IsPrivate = true,
                    Performance = new List<Benchmark>() {new Benchmark(), new Benchmark(), new Benchmark()},
                    Status = "Available",
                    TimeAvailability = new List<TimeSlot>() {new TimeSlot(), new TimeSlot(), new TimeSlot()}
                },
                new Models.Node()
                {
                    Country = "Switzerland",
                    DataThroughput = new List<string>() {"1GB/s", "2GB/s", "3GB/s", "4GB/s"},
                    Endpoint = "13.67.5.666",
                    HighAvailability = 12,
                    Id = "125",
                    IsPrivate = true,
                    Performance = new List<Benchmark>() {new Benchmark(), new Benchmark(), new Benchmark()},
                    Status = "Available",
                    TimeAvailability = new List<TimeSlot>() {new TimeSlot(), new TimeSlot(), new TimeSlot()}
                },
                new Models.Node()
                {
                    Country = "Norway",
                    DataThroughput = new List<string>() {"1GB/s", "2GB/s", "3GB/s", "4GB/s"},
                    Endpoint = "1`4.98.5.431",
                    HighAvailability = 12,
                    Id = "126",
                    IsPrivate = true,
                    Performance = new List<Benchmark>() {new Benchmark(), new Benchmark(), new Benchmark()},
                    Status = "Available",
                    TimeAvailability = new List<TimeSlot>() {new TimeSlot(), new TimeSlot(), new TimeSlot()}
                }
            };
            return list;
        }


        /// <summary>
        /// Returns cluster node with given id.
        /// </summary>
        /// <returns>Single cluster node with given id.</returns>
        [HttpGet]
        public Models.Node GetResource([FromQuery]string nodeID)
        {
            Models.Node n = new Models.Node()
            {
                Country = "Poland",
                DataThroughput = new List<string>() { "1GB/s", "2GB/s", "3GB/s", "4GB/s" },
                Endpoint = "10.41.2.222",
                HighAvailability = 12,
                Id = nodeID,
                IsPrivate = false,
                Performance = new List<Benchmark>() { new Benchmark(), new Benchmark(), new Benchmark() },
                Status = "Available",
                TimeAvailability = new List<TimeSlot>() { new TimeSlot(), new TimeSlot(), new TimeSlot() }
            };
            return n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string CreateResource(Models.Node node)
        {
        
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("configuration")]
        public string UpdateResourceConfiguration(Models.Node node)
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("machinelist")]
        public IEnumerable<Machine> GetMachinesForResource([FromQuery]string nodeID)
        {
            var machines = new List<Machine>()
            {
                new Machine() { Id = nodeID + 2,
                    AvailabilityManifest = new List<TimeSlot>() { new TimeSlot(), new TimeSlot(), new TimeSlot()},
                    BenchmarkManifest = new List<Benchmark>() {  new Benchmark(), new Benchmark(), new Benchmark()},
                    Endpoint = "12.168.11.10",
                    ResourceDescription = "Powerful multi-GPU Server."
                },
                new Machine() { Id = nodeID + 3,
                    AvailabilityManifest = new List<TimeSlot>() { new TimeSlot(), new TimeSlot(), new TimeSlot()},
                    BenchmarkManifest = new List<Benchmark>() {  new Benchmark(), new Benchmark(), new Benchmark()},
                    Endpoint = "10.111.0.1",
                    ResourceDescription = "4x RTX 2070 Super, Intel i9 9920X, 64GB RAM."
                },
                new Machine() { Id = nodeID + 4,
                    AvailabilityManifest = new List<TimeSlot>() { new TimeSlot(), new TimeSlot(), new TimeSlot()},
                    BenchmarkManifest = new List<Benchmark>() {  new Benchmark(), new Benchmark(), new Benchmark()},
                    Endpoint = "76.122.10.2",
                    ResourceDescription = "Server designed for deep learning simulations."
                }
            };

            return machines;
        }

         /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("joblist")]
        public IEnumerable<ComputationJob> GetJobsForResource([FromQuery] string nodeID)
        {
            return new List<ComputationJob>()
            {
                new Server.Models.Jobs.ComputationJob()
                {
                    Name = "Random chance calculator",
                    ClusterId = "BalticCluster01",
                    ComputationModuleName = "volume counter",
                    ComputationModuleReleaseId = "v1.0",
                    ComputationTaskId = "1",
                    Progress = 0.0F,
                    Status = "pending",
                    StartTime = new DateTime(),
                    EndTime = new DateTime(),
                    EstimatedCredits = 180,
                    Id = "4",
                    ReservedCredits = 200,
                    UsedCredits = 0,
                    ResourceUsageMetrics = new List<Server.Models.Resources.ResourceUsageMetric>()
                    {
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "0"},
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "CPU", Value = "0"},
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "RAM", Value = "0"}
                    }
                },
                new Server.Models.Jobs.ComputationJob()
                {
                    Name = "Median counter",
                    ClusterId = "BalticCluster01",
                    ComputationModuleName = "median counter",
                    ComputationModuleReleaseId = "v1.0",
                    ComputationTaskId = "32",
                    Progress = 64.25F,
                    Status = "running",
                    StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                    EndTime = new DateTime(),
                    EstimatedCredits = 128,
                    Id = "21",
                    ReservedCredits = 150,
                    UsedCredits = 30,
                    ResourceUsageMetrics = new List<Server.Models.Resources.ResourceUsageMetric>()
                    {
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "50"},
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "42"},
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "CPU", Value = "36"}
                    }
                },
                new Server.Models.Jobs.ComputationJob()
                {
                    Name = "neural network",
                    ClusterId = "BalticCluster01",
                    ComputationModuleName = "convolutional neural network",
                    ComputationModuleReleaseId = "v2.0",
                    ComputationTaskId = "3",
                    Progress = 100.0F,
                    Status = "completed",
                    StartTime = new DateTime(2019, 12, 1, 12, 34, 45),
                    EndTime = new DateTime(2019, 12, 3, 6, 24, 48),
                    EstimatedCredits = 195,
                    Id = "12",
                    ReservedCredits = 210,
                    UsedCredits = 200,
                    ResourceUsageMetrics = new List<Server.Models.Resources.ResourceUsageMetric>()
                    {
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic4", ResourceType = "GPU", Value = "28"},
                        new Server.Models.Resources.ResourceUsageMetric() {MachineId = "baltic5", ResourceType = "GPU", Value = "18"},
                        new ResourceUsageMetric() {MachineId = "baltic6", ResourceType = "GPU", Value = "50"}
                    }
                }
            };
        }




         /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("machine")]
        public Machine GetMachine([FromQuery]string machineID)
        {
            var machine = new Machine() { Id = machineID, 
                AvailabilityManifest = new List<TimeSlot>() { new TimeSlot(), new TimeSlot(), new TimeSlot()},
                BenchmarkManifest = new List<Benchmark>() {  new Benchmark(), new Benchmark(), new Benchmark()},
                Endpoint = "10.127.11.10",
                ResourceDescription = "16x RTX 2080Ti, 2x Intel i9 10980X, 256GB RAM."
            };

            return machine;
        }
        
    }
}