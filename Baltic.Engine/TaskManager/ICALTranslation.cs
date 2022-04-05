using Baltic.DataModel.CAL;

namespace Baltic.Engine.TaskManager
{
	public interface ICALTranslation {

		/// 
		/// <param name="version"></param>
		/// <param name="diagramUid"></param>
		ComputationApplicationRelease CreateAppRelease(string version, string diagramUid);
	}
}