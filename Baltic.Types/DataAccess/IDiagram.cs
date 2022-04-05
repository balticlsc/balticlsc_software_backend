using Baltic.DataModel.Diagram;

namespace Baltic.Types.DataAccess {
	public interface IDiagram  {

		string CreateDiagram();

		/// 
		/// <param name="diagramUid"></param>
		CALDiagram GetDiagram(string diagramUid);

		/// 
		/// <param name="diagramUid"></param>
		string CopyDiagram(string diagramUid);
	}
}