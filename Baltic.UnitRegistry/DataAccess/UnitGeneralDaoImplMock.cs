using System.Collections.Generic;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.UnitRegistry.DataAccess {
	public class UnitGeneralDaoImplMock : IUnitGeneral {
		
		protected List<ComputationApplicationRelease> _cars;
		protected List<ComputationApplication> _cas;
		protected List<ComputationModuleRelease> _cmrs;
		protected List<ComputationModule> _cms;
		
		private List<DataType> _dts;
		private List<DataStructure> _dss;
		private List<AccessType> _ats;

		public UnitGeneralDaoImplMock(UnitRegistryMock registry)
		{
			_cars = registry.Cars;
			_cas = registry.Cas;
			_cmrs = registry.Cmrs;
			_cms = registry.Cms;
			_dts = registry.Dts;
			_dss = registry.Dss;
			_ats = registry.Ats;
		}

		/// 
		/// <param name="releaseUid"></param>
		public ComputationUnitRelease GetUnitRelease(string releaseUid)
		{
			ComputationUnitRelease rel = _cars.Find(r => r.Uid == releaseUid);
			if (null == rel)
				rel = _cmrs.Find(r => r.Uid == releaseUid);
			return rel;
		}

		/// 
		/// <param name="unitUid"></param>
		public ComputationUnit GetUnit(string unitUid)
		{
			ComputationUnit result = _cas.Find(a => a.Uid == unitUid);
			if (null == result)
				result = _cms.Find(m => m.Uid == unitUid);
			return result;
		}

		/// 
		/// <param name="dtUid"></param>
		public DataType GetDataType(string dtUid)
		{
			DataType type = _dts.Find(dt => dt.Uid == dtUid);
			return type;
		}

		public List<DataType> GetDataTypes()
		{
			return _dts;
		}

		public DataStructure GetDataStructure(string dsUid)
		{
			if (string.IsNullOrEmpty(dsUid))
				return null;
			DataStructure type = _dss.Find(dt => dt.Uid == dsUid);
			return type;
		}

		public List<DataStructure> GetDataStructures()
		{
			return _dss;
		}

		/// 
		/// <param name="atUid"></param>
		public AccessType GetAccessType(string atUid){
			AccessType type = _ats.Find(dt => dt.Uid == atUid);
			return type;
		}

		public List<AccessType> GetAccessTypes()
		{
			return _ats;
		}

	}
}