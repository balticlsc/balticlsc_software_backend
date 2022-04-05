using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;
using Baltic.Engine.TaskManager;
using Baltic.Types.DataAccess;

namespace Baltic.Engine.UnitManager {
	public class UnitDevManager : IUnitDevManager {

		private IUnitManagement _unitRegistry;
		private ICALTranslation _translation;
		private IDiagram _diagramRegistry;

		public UnitDevManager(IUnitManagement ur, ICALTranslation ct, IDiagram dr)
		{
			_unitRegistry = ur;
			_translation = ct;
			_diagramRegistry = dr;
		}

		/// 
		/// <param name="name"></param>
		/// <param name="userUid"></param>
		public string CreateApp(string name, string userUid)
		{
			string diagramUid = _diagramRegistry.CreateDiagram();
			return _unitRegistry.CreateApp(name,diagramUid,userUid);
		}

		/// 
		/// <param name="appUid"></param>
		/// <param name="version"></param>
		public string CreateAppRelease(string appUid, string version)
		{
			ComputationApplication app = (ComputationApplication) _unitRegistry.GetUnit(appUid);
			string diagramUid = _diagramRegistry.CopyDiagram(app.DiagramUid);
			ComputationApplicationRelease rel = _translation.CreateAppRelease(version, diagramUid);
			return _unitRegistry.AddReleaseToUnit(appUid,rel);
		}	

		/// 
		/// <param name="unitUid"></param>
		public short DeleteUnit(string unitUid)
		{
			ComputationUnit unit = _unitRegistry.GetUnit(unitUid);
			if (null == unit) return -1; 
			if (null != unit.Releases && 0 != unit.Releases.Count) return -2;
			return _unitRegistry.DeleteUnit(unitUid);
		}

		/// 
		/// <param name="releaseUid"></param>
		public short DeleteUnitRelease(string releaseUid)
		{
			ComputationUnitRelease release = _unitRegistry.GetUnitRelease(releaseUid);
			if (null == release || release.Status == UnitReleaseStatus.Approved) return -1;
			return _unitRegistry.DeleteUnitRelease(releaseUid);
		}

	}
}