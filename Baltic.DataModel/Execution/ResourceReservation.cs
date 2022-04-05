 namespace Baltic.DataModel.Execution
{
    public class ResourceReservation
    {
        public int Cpus { get; set; } // in milli CPUs
        public int Gpus { get; set; } // in GPUs
        public int Memory { get; set; } // in MB
        public int Storage { get; set; } // in GB

        public ResourceReservation()
        {
            Cpus = -1;
            Gpus = -1;
            Memory = -1;
            Storage = -1;
        }
    }
}