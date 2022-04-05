namespace Baltic.DataModel.Execution
{
    public class ResourceReservationRange
    {
        public ResourceReservation MaxReservation { get; set; }
        public ResourceReservation MinReservation { get; set; }

        public ResourceReservationRange(){
            MaxReservation = new ResourceReservation();
            MinReservation = new ResourceReservation();
        }
    }
}