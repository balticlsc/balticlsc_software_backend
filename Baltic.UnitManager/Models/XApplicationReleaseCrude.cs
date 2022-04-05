using Baltic.DataModel.CAL;
using Baltic.Types.DataAccess;

namespace Baltic.UnitManager.Models {
	public class XApplicationReleaseCrude : XUnitReleaseCrude {

		public XApplicationReleaseCrude(ComputationApplicationRelease release = null) : base(release) {}

		public override ComputationUnitRelease ToModelObject(IUnitManagement unitRegistry, ComputationUnit unit)
		{
			return ToModelObject(new ComputationApplicationRelease(){Unit = unit}, unitRegistry);
		}

	}
}