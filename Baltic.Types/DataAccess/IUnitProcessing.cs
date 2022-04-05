using System.Collections.Generic;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;

namespace Baltic.Types.DataAccess
{
    public interface IUnitProcessing : IUnitGeneral 
    {
        /// 
		/// <param name="dataSet"></param>
		string AddDataSetToShelf(TaskDataSet dataSet);

		/// 
		/// <param name="dataSetUid"></param>
		short DeleteDataSet(string dataSetUid);

		/// 
		/// <param name="dataSet"></param>
		short UpdateDataSet(TaskDataSet dataSet);

		/// 
		/// <param name="dataSetUid"></param>
		TaskDataSet GetDataSet(string dataSetUid);

		/// 
		/// <param name="usedUid"></param>
		IEnumerable<TaskDataSet> GetDataShelf(string usedUid);

		public ComputationModuleRelease FindSystemUnit(string unitName, DataPin inputPin, DataPin outputPin);
		
    }
}