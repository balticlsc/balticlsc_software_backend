using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Execution
{
    public class TaskParameters
    {
        public string TaskName { get; set; }
        public int Priority { get; set; }
        public float ReservedCredits { get; set; }
        public float AuxStorageCredits { get; set; }
        public bool IsPrivate { get; set; }
        public UnitStrength ClusterAllocation { get; set; }
        public string ClusterUid { get; set; }
        public ResourceReservationRange ReservationRange { get; set; }
        public List<UnitCallParameter> CustomParameters { get; set; }

        public TaskParameters()
        {
            ReservationRange = new ResourceReservationRange();
            CustomParameters = new List<UnitCallParameter>();
        }
    }
}