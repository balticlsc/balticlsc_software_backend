using Baltic.TaskRegistry.Entities;
using Mighty;

namespace Baltic.TaskRegistry.Tables
{
    public class ResourceReservationsTable : MightyOrm<ResourceReservationEntity>
    {
        public ResourceReservationsTable() : base(GlobalConnectionString, table: "resourcereservations", primaryKeys: "id", sequence: "resourcereservations_id_seq")
        {
        }
    }
}
