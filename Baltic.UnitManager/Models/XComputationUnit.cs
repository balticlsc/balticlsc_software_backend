
using System.Collections.Generic;
using System.Linq;
using Baltic.Core.Utils;
using Baltic.DataModel.CAL;
using Baltic.Types.DataAccess;

namespace Baltic.UnitManager.Models {
	public class XComputationUnit : XComputationUnitCrude {

		public List<XUnitReleaseCrude> Releases { get; set; }

		public XComputationUnit(ComputationUnit unit = null) : base(unit)
		{
			if (null == unit) return;
			Releases = unit.Releases.Select(r => IsApp ?
				(XUnitReleaseCrude) new XApplicationReleaseCrude((ComputationApplicationRelease) r) :
				(XUnitReleaseCrude) new XModuleReleaseCrude((ComputationModuleRelease) r)
				).ToList();
		}
		
		public new ComputationUnit ToModelObject(IUnitManagement unitRegistry)
		{
			ComputationUnit unit = base.ToModelObject();
			unit.Releases = Releases.Select(r => r.ToModelObject(unitRegistry, unit)).ToList();
			return unit;
		}
	}
}