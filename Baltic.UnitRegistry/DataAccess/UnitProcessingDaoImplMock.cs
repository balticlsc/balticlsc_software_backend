using System;
using System.Collections.Generic;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.UnitRegistry.DataAccess
{
	public class UnitProcessingDaoImplMock : UnitGeneralDaoImplMock, IUnitProcessing
	{
		private IDictionary<string, List<TaskDataSet>> _dataShelf;

		public UnitProcessingDaoImplMock(UnitRegistryMock registry):base(registry)
		{
			_dataShelf = registry.DataShelf;
		}

		/// 
		/// <param name="unitUid"></param>
		/// <param name="release"></param>
		public string AddReleaseToUnit(string unitUid, ComputationUnitRelease release){
			ComputationApplication apl = _cas.Find(app => app.Uid == unitUid);
			if (null != apl) apl.Releases.Add(release);
			_cars.Add((ComputationApplicationRelease) release);
			return release.Uid;
		}

		/// 
		/// <param name="dataSet"></param>
		public string AddDataSetToShelf(TaskDataSet dataSet)
		{
			dataSet.Uid = Guid.NewGuid().ToString();
			if (!_dataShelf.ContainsKey(dataSet.OwnerUid))
				_dataShelf.Add(dataSet.OwnerUid,new List<TaskDataSet>());
			_dataShelf[dataSet.OwnerUid].Add(dataSet);
			return dataSet.Uid;
		}

		/// 
		/// <param name="dataSetUid"></param>
		public short DeleteDataSet(string dataSetUid)
		{
			foreach (KeyValuePair<string, List<TaskDataSet>> shelf in _dataShelf)
			{
				TaskDataSet toDelete = shelf.Value.Find(ds => ds.Uid == dataSetUid);
				if (null != toDelete)
				{
					shelf.Value.Remove(toDelete);
					return 0;
				}
			}
			return -1;
		}

		/// 
		/// <param name="dataSet"></param>
		public short UpdateDataSet(TaskDataSet dataSet)
		{
			foreach (KeyValuePair<string, List<TaskDataSet>> shelf in _dataShelf)
			{
				TaskDataSet toUpdate = shelf.Value.Find(ds => ds.Uid == dataSet.Uid);
				if (null != toUpdate)
				{
					DBMapper.Map<TaskDataSet>(dataSet, toUpdate);
					DBMapper.Map<CDataSet>(dataSet.Data, toUpdate.Data);
					return 0;
				}
			}
			return -1;
		}

		/// 
		/// <param name="dataSetUid"></param>
		public TaskDataSet GetDataSet(string dataSetUid)
		{
			foreach (KeyValuePair<string, List<TaskDataSet>> shelf in _dataShelf)
			{
				TaskDataSet toFind = shelf.Value.Find(ds => ds.Uid == dataSetUid);
				if (null != toFind)
				{
					return toFind;
				}
			}
			return null;
		}

		/// 
		/// <param name="userUid"></param>
		public IEnumerable<TaskDataSet> GetDataShelf(string userUid)
		{
			if (!_dataShelf.ContainsKey(userUid))
				return new List<TaskDataSet>();
			return _dataShelf[userUid];
		}

		public ComputationModuleRelease FindSystemUnit(string unitName, DataPin inputPin, DataPin outputPin)
		{
			ComputationModuleRelease ret = _cmrs.Find(r =>
				"system" == r.Unit.AuthorUid && unitName == r.Unit.Name);
			return ret;
		}
		
	}
}