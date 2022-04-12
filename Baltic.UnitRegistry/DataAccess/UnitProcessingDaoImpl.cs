using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.Database;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;
using Baltic.UnitRegistry.Tables;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Baltic.UnitRegistry.DataAccess
{
    public class UnitProcessingDaoImpl : UnitGeneralDaoImpl, IUnitProcessing
    {
        private IConfiguration _configuration;

        public UnitProcessingDaoImpl(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        /// 
        /// <param name="dataSet"></param>
        public string AddDataSetToShelf(TaskDataSet dataSet)
        {
            if ("!Reset!" == dataSet.Name)
            {
                ResetDataSets();
                return null;
            };
            
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    var dSet = trans.Insert(DataSetTable, new
                    {
                        uid = Guid.NewGuid().ToString(),
                        name = dataSet.Name,
                        multiplicity = (int)dataSet.Multiplicity,
                        useruid = dataSet.OwnerUid,
                        data = dataSet.Data.Values,
                        accessdata = dataSet.AccessData.Values,
                        // TODO - typeid, structureid, accessid - check for nulls
                        typeid =  DTypeTable.Single(new {uid = dataSet.Type.Uid}).id,
                        structureid = null != dataSet.Structure ? DStructTable.Single(new {uid = dataSet.Structure.Uid}).id : null,
                        accessid = null != dataSet.Access ? ATypeTable.Single(new {uid = dataSet.Access.Uid}).id : null
                    }).Single();
                    if (null == dSet) throw new Exception();

                    trans.CommitTransaction();
                    return dSet.uid;
                }
                catch (Exception e)
                {
                    trans.RollbackTransaction();
                    Log.Debug(e.ToString());
                    return null;
                }
            }
        }
        /// 
        /// <param name="dataSetUid"></param>
        public short DeleteDataSet(string dataSetUid)
        {
            var dSet = DataSetTable.Single(new {uid = dataSetUid});
            if (null == dSet)
                return -1;
            return 1 == DataSetTable.Delete(dSet) ? (short)0 : (short)-2;
        }
        
        /// 
        /// <param name="dataSet"></param>
        public short UpdateDataSet(TaskDataSet dataSet)
        {
            var dSet = DataSetTable.Single(new {uid = dataSet.Uid});
            if (null == dSet)
                return -1;
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    dSet.name = dataSet.Name;
                    dSet.multiplicity = (int)dataSet.Multiplicity;
                    dSet.useruid = dataSet.OwnerUid;
                    dSet.data = dataSet.Data.Values;
                    dSet.accessdata = dataSet.AccessData.Values;
                    // TODO - typeid, structureid, accessid - check for nulls
                    dSet.typeid = DTypeTable.Single(new {uid = dataSet.Type.Uid}).id;
                    dSet.structureid = null != dataSet.Structure ? DStructTable.Single(new {uid = dataSet.Structure.Uid}).id : null;
                    dSet.accessid = null != dataSet.Access ? ATypeTable.Single(new {uid = dataSet.Access.Uid}).id : null;
                    int uRes = trans.Update(DataSetTable,dSet);
                    if (1 != uRes) throw new Exception();

                    trans.CommitTransaction();
                    return 0;
                }
                catch (Exception e)
                {
                    trans.RollbackTransaction();
                    Log.Debug(e.ToString());
                    return -3;
                }
            }
        }

        /// 
        /// <param name="dataSetUid"></param>
        public TaskDataSet GetDataSet(string dataSetUid)
        {
            var dSet = DataSetTable.Single(new {uid = dataSetUid});
            if (null == dSet)
                return null;
            var type = DTypeTable.Single(new {id = dSet.typeid});
            var structure = DStructTable.Single(new {id = dSet.structureid});
            var access = ATypeTable.Single(new {id = dSet.accessid});
            
            return new TaskDataSet()
            {
                Uid = dSet.uid,
                Name = dSet.name,
                Multiplicity = (CMultiplicity)dSet.multiplicity,
                OwnerUid = dSet.useruid,
                Data = new CDataSet(){Values = dSet.data},
                AccessData = new CDataSet(){Values = dSet.accessdata},
                Type = null != type ? GetDataType(type.uid) : null,
                Structure = null != structure ? GetDataStructure(structure.uid) : null,
                Access = null != access ? GetAccessType(access.uid) : null
            };
        }

        /// 
        /// <param name="userUid"></param>
        public IEnumerable<TaskDataSet> GetDataShelf(string userUid)
        {
            var dSets = DataSetTable.All(new {useruid = userUid});
            List<TaskDataSet> ret = new List<TaskDataSet>();
            
            foreach (var dSet in dSets)
            {
                var type = DTypeTable.Single(new {id = dSet.typeid});
                var structure = DStructTable.Single(new {id = dSet.structureid});
                var access = ATypeTable.Single(new {id = dSet.accessid});
                TaskDataSet dataSet = new TaskDataSet()
                {
                    Uid = dSet.uid,
                    Name = dSet.name,
                    Multiplicity = (CMultiplicity)dSet.multiplicity,
                    OwnerUid = dSet.useruid,
                    Data = new CDataSet() {Values = dSet.data},
                    AccessData = new CDataSet() {Values = dSet.accessdata},
                    Type = null != type ? GetDataType(type.uid) : null,
                    Structure = null != structure ? GetDataStructure(structure.uid) : null,
                    Access = null != access ? GetAccessType(access.uid) : null
                };
                // TODO - maybe optimize the number of Type, Structure and Access objects
                ret.Add(dataSet);
            }

            return ret;
        }

        public ComputationModuleRelease FindSystemUnit(string unitName, DataPin inputPin, DataPin outputPin)
        {
            var unit = CUnitTable.Single(new {name = unitName, authoruid = "system"});
            var release = CUnitRelTable.Single(new {unitid = unit.id});
            return MapModuleRelease(release);
        }

        private void ResetDataSets()
        {
            var dataSets = DataSetTable.All();
            foreach (var dataSet in dataSets)
                DataSetTable.Delete(dataSet);
            
            // TODO - adapt to use proper GUIDs in UnitRegistryInit for data/access types
            List<TaskDataSet> dSets = (new UnitRegistryInit(_configuration)).GetInitDataSets();
            foreach (TaskDataSet dSet in dSets)
                AddDataSetToShelf(dSet);
        }
        
    }
}