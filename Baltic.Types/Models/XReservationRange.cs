using Baltic.DataModel.Execution;

namespace Baltic.Types.Models
{
    public class XReservationRange
    {
        public int MinCPUs { get; set; }
        public int MinGPUs { get; set; }
        public int MinMemory { get; set; }
        public int MinStorage { get; set; }
        public int MaxCPUs { get; set; }
        public int MaxGPUs { get; set; }
        public int MaxMemory { get; set; }
        public int MaxStorage { get; set; }
        
        public XReservationRange(ResourceReservationRange range = null)
        {
            if (null == range) return;
            MinMemory = range.MinReservation.Memory;
            MaxMemory = range.MaxReservation.Memory;
            MinStorage = range.MinReservation.Storage;
            MaxStorage = range.MaxReservation.Storage;
            MinCPUs = range.MinReservation.Cpus;
            MaxCPUs = range.MaxReservation.Cpus;
            MinGPUs = range.MinReservation.Gpus;
            MaxGPUs = range.MaxReservation.Gpus;
        }

        public ResourceReservationRange ToModelObject()
        {
            ResourceReservationRange range = new ResourceReservationRange();
            range.MinReservation.Memory = MinMemory;
            range.MaxReservation.Memory = MaxMemory;
            range.MinReservation.Storage = MinStorage;
            range.MaxReservation.Storage = MaxStorage;
            range.MinReservation.Cpus = MinCPUs;
            range.MaxReservation.Cpus = MaxCPUs;
            range.MinReservation.Gpus = MinGPUs;
            range.MaxReservation.Gpus = MaxGPUs;
            return range;
        }
    }
}