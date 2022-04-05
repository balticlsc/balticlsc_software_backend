using Baltic.DataModel.CAL;

namespace Baltic.UnitManager.Models {
	public class XComputationApplication : XComputationUnit {

		public string DiagramUid { get; set; }
		
		public XComputationApplication(ComputationApplication app = null) : base(app)
		{
			if (null == app) return;
			DiagramUid = app.DiagramUid;
		}
		
		public new ComputationUnit ToModelObject()
		{
			ComputationApplication unit = (ComputationApplication) base.ToModelObject();
			unit.DiagramUid = DiagramUid;
			return unit;
		}

	}
}