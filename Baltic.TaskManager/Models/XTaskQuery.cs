using System;
using Baltic.DataModel.Types;

namespace Baltic.TaskManager.Models
{
    public class XTaskQuery
    {
        //from: CTask
        public string AppUid { get; set; }

        //from: TaskExecution
        public int Priority { get; set; }
        public bool IsPrivate { get; set; }
        public bool IncludeArchived { get; set; }

        //from: ExecutionRecord
        public ComputationStatus Status { get; set; }
        public DateTime StartLowerLimit { get; set; }
        public DateTime StartUpperLimit { get; set; }
        public DateTime FinishLowerLimit { get; set; }
        public DateTime FinishUpperLimit { get; set; }

        //additional from: Baltic-grudzien
        public int UserId { get; set; }
    }
}