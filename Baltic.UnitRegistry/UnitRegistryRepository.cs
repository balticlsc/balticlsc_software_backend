using System.Linq;
using Baltic.DataModel.CAL;
using Baltic.UnitRegistry.Tables;

namespace Baltic.UnitRegistry
{
    public class UnitRegistryRepository
    {
        private ComputationApplicationTable _computationApplicationTable = null;

        private ComputationApplicationTable ComputationApplicationTable =>
            _computationApplicationTable ??= new ComputationApplicationTable();

        public ComputationApplication GetComputationApplication(string uid)
        {
            var ca = ComputationApplicationTable.Single(new {uid = uid});
            if (null != ca)
                return new ComputationApplication
                {
                    Name = ca.name,
                    Uid = ca.uid,
                    AuthorUid = ca.authoruid
                };
            return null;
            // ComputationApplicationTable.All(new{authoruid = uid}).ToList;
        }

        public bool InsertComputationApplication(ComputationApplication ca)
        {
            var inserted = ComputationApplicationTable.Insert(new
            {
                name = ca.Name,
                uid = ca.Uid
            });
            return null != inserted;
        }

        public bool UpdateComputationApplication(ComputationApplication ca)
        {
            var current = ComputationApplicationTable.Single(new {uid = ca.Uid});
            if (null != current)
            {
                current.Name = ca.Name;
                return null != ComputationApplicationTable.Update(current);
            }
            return false;
        }
    }
}