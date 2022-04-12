using System;
using System.Collections.Generic;
using Baltic.Server.Models.Jobs;

namespace Baltic.Server.Models.Computation
{
    public class ComputationTask
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public int ComputationApplicationReleaseId { get; set; }
        public IEnumerable<ComputationJob> JobList { get; set; }
        public float Progress { get; set; }
        public float ReservedCredits { get; set; }
        public float EstimatedCredits { get; set; }
        public float ConsumedCredits { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Priority { get; set; }
        public bool IsPrivate { get; set; }
        public bool SafeMode { get; set; }  // czy ma byc uruchomiony z konkretną wersją modułu
    }
}
