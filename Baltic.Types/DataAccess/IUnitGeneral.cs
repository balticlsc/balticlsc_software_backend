using System.Collections.Generic;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;

namespace Baltic.Types.DataAccess {
	public interface IUnitGeneral  {

		/// 
		/// <param name="releaseUid"></param>
		ComputationUnitRelease GetUnitRelease(string releaseUid);

		/// 
		/// <param name="unitUid"></param>
		ComputationUnit GetUnit(string unitUid);

		/// 
		/// <param name="dtUid"></param>
		DataType GetDataType(string dtUid);

		List<DataType> GetDataTypes();

		/// 
		/// <param name="dsUid"></param>
		DataStructure GetDataStructure(string dsUid);

		List<DataStructure> GetDataStructures();

		/// 
		/// <param name="atUid"></param>
		AccessType GetAccessType(string atUid);

		List<AccessType> GetAccessTypes();
	}
}