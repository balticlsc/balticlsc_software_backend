using Baltic.DataModel.CAL;
using Baltic.Types.DataAccess;

namespace Baltic.UnitManager.Models
{
    public class XApplicationRelease : XUnitRelease
    {
		public string DiagramUid { get; set; }
        
        public XApplicationRelease(ComputationApplicationRelease release = null) : base(release)
        {
            if (null == release) return;
            DiagramUid = release.DiagramUid;
        }
        
        public override ComputationUnitRelease ToModelObject(IUnitManagement unitRegistry, ComputationUnit unit)
        {
            ComputationApplicationRelease release = (ComputationApplicationRelease) ToModelObject(new ComputationApplicationRelease(), unitRegistry);
            release.DiagramUid = DiagramUid;
            return release;
        }
    }
}
