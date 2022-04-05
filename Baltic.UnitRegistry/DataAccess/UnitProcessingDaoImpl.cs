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
using Serilog;

namespace Baltic.UnitRegistry.DataAccess
{
    public class UnitProcessingDaoImpl : UnitGeneralDaoImpl, IUnitProcessing
    {
        private TaskDataSetTable _dataSetTable = null;
        private TaskDataSetTable DataSetTable =>
            _dataSetTable ??= new TaskDataSetTable();
        
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
                        multiplicity = dataSet.Multiplicity,
                        useruid = dataSet.OwnerUid,
                        data = dataSet.Data.Values,
                        accessdata = dataSet.AccessData.Values,
                        // TODO - typeid, structureid, accessid - check for nulls
                        typeid =  DTypeTable.Single(new {uid = dataSet.Type.Uid}).id,
                        structureid = DStructTable.Single(new {uid = dataSet.Structure.Uid}).id,
                        accessid = ATypeTable.Single(new {uid = dataSet.Access.Uid}).id
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
            CUnitTable.Delete(dSet);
            return 0;
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
                    dSet.multiplicity = dataSet.Multiplicity;
                    dSet.useruid = dataSet.OwnerUid;
                    dSet.data = dataSet.Data.Values;
                    dSet.accessdata = dataSet.AccessData.Values;
                    // TODO - typeid, structureid, accessid - check for nulls
                    dSet.typeid = DTypeTable.Single(new {uid = dataSet.Type.Uid}).id;
                    dSet.structureid = DStructTable.Single(new {uid = dataSet.Structure.Uid}).id;
                    dSet.accessid = ATypeTable.Single(new {uid = dataSet.Access.Uid}).id;
                    dSet = trans.Update(DataSetTable,dSet);
                    if (null == dSet) throw new Exception();

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
                Multiplicity = dSet.multiplicity,
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
                    Multiplicity = dSet.multiplicity,
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
            return GetUnit(unit.uid);
        }

        private void ResetDataSets()
        {
            var dataSets = DataSetTable.All();
            foreach (var dataSet in dataSets)
                DataSetTable.Delete(dataSet);
            
            List<TaskDataSet> dSets = UnitRegistryInit.GetInitDataSets();
            foreach (TaskDataSet dSet in dSets)
                AddDataSetToShelf(dSet);
        }
        
    }
}