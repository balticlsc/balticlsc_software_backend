using System;
using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Execution
{
    public class TaskQuery
    {
        //from: CTask
        public string AppUid { get; set; }
        public string UserUid { get; set; }

        //from: TaskExecution
        public int Priority { get; set; }
        public bool IsPrivate { get; set; }

        //from: ExecutionRecord
        public List<ComputationStatus> Statuses { get; set; }
        public DateTime StartLowerLimit { get; set; }
        public DateTime StartUpperLimit { get; set; }
        public DateTime FinishLowerLimit { get; set; }
        public DateTime FinishUpperLimit { get; set; }
    }
}
