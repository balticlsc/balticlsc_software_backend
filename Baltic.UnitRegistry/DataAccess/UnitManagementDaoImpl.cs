using System;
using System.Collections.Generic;
using System.Linq;
using Baltic.Database;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;
using Baltic.UnitRegistry.Tables;
using Mighty;
using Serilog;

namespace Baltic.UnitRegistry.DataAccess
{
    public class UnitManagementDaoImpl : UnitGeneralDaoImpl, IUnitManagement
    {
        private AppShelfTable _aShelfTable = null;
        private AppShelfTable AShelfTable =>
            _aShelfTable ??= new AppShelfTable();
        
        private ToolboxTable _toolTable = null;
        private ToolboxTable ToolTable =>
            _toolTable ??= new ToolboxTable();
        
        /// 
        /// <param name="appName"></param>
        /// <param name="diagramUid"></param>
        /// <param name="userUid"></param>
        public string CreateApp(string appName, string diagramUid, string userUid)
        {
            if ("!Reset!" == appName)
            {
                ResetUnitRegistry();
                return null;
            } else if ("!Restore!" == appName)
            {
                RestoreUnitRegistry();
                return null;
            }

            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    var unit = trans.Insert(CUnitTable, new
                    {
                        name = appName,
                        uid = Guid.NewGuid().ToString(),
                        authoruid = userUid,
                        isapplication = true,
                        classid = 0
                    }).Single();
                    if (null == unit) throw new Exception();

                    var app = trans.Insert(CAppTable, new
                    {
                        computationunitid = unit.id,
                        diagramuid = diagramUid
                    }).Single();
                    if (null == app) throw new Exception();

                    var descr = trans.Insert(UDescTable, new
                    {
                        computationunitid = unit.id,
                        icon = "https://www.balticlsc.eu/model/_icons/default.png"
                    }).Single();
                    if (null == descr) throw new Exception();

                    trans.CommitTransaction();
                    return unit.uid;
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
        /// <param name="unitUid"></param>
        /// <param name="release"></param>
        public string AddReleaseToUnit(string unitUid, ComputationUnitRelease release)
        {
            var unit = CUnitTable.Single(new {uid = unitUid});
            if (null == unit)
                return null;

            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    var uRel = trans.Insert(CUnitRelTable, new
                    {
                        uid = Guid.NewGuid().ToString(),
                        unitid = unit.id,
                        isapplication = unit.isapplication,
                        version = release.Version,
                        status = release.Status,
                    }).Single();
                    if (null == uRel) throw new Exception();

                    var rDesc = trans.Insert(RDescTable, new
                    {
                        computationunitreleaseid = uRel.id,
                        date = DateTime.Now,
                        description = release.Descriptor.Description,
                        isopensource = release.Descriptor.IsOpenSource,
                        usagecounter = release.Descriptor.UsageCounter
                    }).Single();
                    if (null == rDesc) throw new Exception();

                    if (release is ComputationModuleRelease moduleRelease)
                    {
                        var modRel = trans.Insert(CModuleRelTable, new
                        {
                            computationunitreleaseid = uRel.id,
                            image = moduleRelease.Image,
                            command = moduleRelease.Command,
                            ismultitasking = moduleRelease.IsMultitasking,
                        }).Single();
                        if (null == modRel) throw new Exception();

                        var rServices = trans.Insert(ReqServiceTable, 
                            moduleRelease.RequiredServiceUids.Select(u => new
                        {
                            moduleid = modRel.id,
                            serviceid = GetServiceId(u)
                        }));
                        if (null == rServices || rServices.Count() != moduleRelease.RequiredServiceUids.Count())
                            throw new Exception();

                        var cParams = trans.Insert(CParTable, 
                            moduleRelease.CredentialParameters.Select(cp => new
                            {
                                computationmodulereleaseid = modRel.id,
                                environmentvariablename = cp.EnvironmentVariableName,
                                accesscredentialname = cp.AccessCredentialName,
                                defaultcredentialvalue = cp.DefaultCredentialValue
                            }));
                        if (null == cParams || cParams.Count() != moduleRelease.CredentialParameters.Count())
                            throw new Exception();

                        var cArgs = trans.Insert(CArgTable,
                            moduleRelease.CommandArguments.Select(ca => new
                            {
                                computationmodulereleaseid = modRel.id,
                                value = ca
                            }));
                        if (null == cArgs || cArgs.Count() != moduleRelease.CommandArguments.Count())
                            throw new Exception();
                    }
                    else
                    {
                        var appRel = trans.Insert(CAppRelTable, new
                        {
                            computationunitreleaseid = uRel.id,
                            diagramuid = ((ComputationApplicationRelease) release).DiagramUid
                        }).Single();
                        if (null == appRel) throw new Exception();
                    }

                    var dPins = trans.Insert(PinTable, release.DeclaredPins.Select(p => new
                    {
                        computationunitreleaseid = uRel.id,
                        name = p.Name,
                        uid = p.Uid,
                        binding = p.Binding,
                        datamultiplicity = p.DataMultiplicity,
                        tokenmultiplicity = p.TokenMultiplicity,
                        // TODO - typeid, structureid, accessid - check for nulls
                        typeid =  DTypeTable.Single(new {uid = p.Type.Uid}),
                        structureid = DStructTable.Single(new {uid = p.Structure.Uid}),
                        accessid = ATypeTable.Single(new {uid = p.Access.Uid})
                    }));
                    if (null == dPins || dPins.Count() != release.DeclaredPins.Count) throw new Exception();
                    
                    return uRel.uid;
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
        /// <param name="moduleName"></param>
        /// <param name="userUid"></param>
        public string CreateModule(string moduleName, string userUid)
        {
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    var unit = trans.Insert(CUnitTable, new
                    {
                        name = moduleName,
                        uid = Guid.NewGuid().ToString(),
                        authoruid = userUid,
                        isapplication = false,
                        classid = 0
                    }).Single();
                    if (null == unit) throw new Exception();

                    var module = trans.Insert(CModuleTable, new
                    {
                        computationunitid = unit.id,
                        isservice = false
                    }).Single();
                    if (null == module) throw new Exception();

                    var descr = trans.Insert(UDescTable, new
                    {
                        computationunitid = unit.id,
                        icon = "https://www.balticlsc.eu/model/_icons/default.png"
                    }).Single();
                    if (null == descr) throw new Exception();

                    trans.CommitTransaction();
                    return unit.uid;
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
        /// <param name="query"></param>
        public List<ComputationUnit> FindUnits(UnitQuery query)
        {
            IEnumerable<dynamic> units;
            // TODO - OnlyLastRelease
            if (query.AllUnits)
            {
                if (string.IsNullOrEmpty(query.AuthorUid))
                    units = CUnitTable.All();
                else
                    units = CUnitTable.All(new
                    {
                        authoruid = query.AuthorUid
                    });
            }
            else
            {
                if (string.IsNullOrEmpty(query.AuthorUid))
                    units = CUnitTable.All(new
                    {
                        isapplication = query.IsApp
                    });
                else
                    units = CUnitTable.All(new
                    {
                        authoruid = query.AuthorUid,
                        isapplication = query.IsApp
                    });
            }
            return units.Select(u => u.isapplication ? 
                (ComputationUnit) MapApplication(u) : (ComputationUnit) MapModule(u)).ToList();
        }
        
        /// 
        /// <param name="query"></param>
        public List<ComputationUnitRelease> FindUnitReleases(UnitQuery query)
        {
            IEnumerable<dynamic> units;
            // TODO - OnlyLastRelease
            if (query.AllUnits)
                units = CUnitRelTable.All();
            else
                units = CUnitRelTable.All(new
                {
                    isapplication = query.IsApp
                });
            List<ComputationUnitRelease> ret = units.Select(u => u.isapplication ? 
                (ComputationUnitRelease) MapApplicationRelease(u) : (ComputationUnitRelease) MapModuleRelease(u)).ToList();
            return ret.FindAll(r => r.Unit.AuthorUid == query.AuthorUid);
        }


        /// 
        /// <param name="unit"></param>
        public short UpdateUnit(ComputationUnit unit)
        {
            var currUnit = CUnitTable.Single(new {uid = unit.Uid});
            if (null == currUnit)
                return -1;
            if (unit is ComputationApplication != currUnit.isapplication)
                return -2;
            var descr = UDescTable.Single(new {computationunitid = currUnit.id});
            if (null == descr)
                return -1;
            var unitExt = currUnit.isapplication ? CAppTable.Single(new {computationunitid = currUnit.id}) :
                CModuleTable.Single(new {computationunitid = currUnit.id});
            if (null == unitExt)
                return -1;
            var keywords = KeywTable.All(new {unitdescriptorid = descr.id});
            if (null == keywords)
                return -1;
            
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    currUnit.name = unit.Name;
                    currUnit.authoruid = unit.AuthorUid;
                    currUnit.classid = PClassTable.Single(new {name = unit.Class.Name}).id;
                    currUnit = trans.Update(CUnitTable,currUnit);
                    if (null == currUnit) throw new Exception();

                    descr.shortdescription = unit.Descriptor.ShortDescription;
                    descr.longdescription = unit.Descriptor.LongDescription;
                    descr.icon = unit.Descriptor.Icon;
                    descr = trans.Update(UDescTable, descr);
                    if (null == descr) throw new Exception();

                    trans.Delete(KeywTable, keywords.ToArray());
                    keywords = trans.Insert(KeywTable, unit.Descriptor.Keywords.Select(k => new
                        {
                            unitdescriptorid = descr.id,
                            value = k
                        }));
                    if (null == keywords || keywords.Count() != unit.Descriptor.Keywords.Count())
                        throw new Exception();
                    
                    // TODO - ratings

                    if (unit is ComputationModule module)
                        unitExt.isservice = module.IsService;
                    else
                        unitExt.diagramuid = ((ComputationApplication) unit).DiagramUid;
                    unitExt = trans.Update(CAppTable,unitExt);
                    if (null == unitExt) throw new Exception();

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
        /// <param name="release"></param>
        public short UpdateUnitRelease(ComputationUnitRelease release)
        {
            var currRelease = CUnitRelTable.Single(new {uid = release.Uid});
            if (null == currRelease)
                return -1;
            if (release is ComputationApplicationRelease != currRelease.isapplication)
                return -2;
            var descr = RDescTable.Single(new {computationunitreleaseid = currRelease.id});
            if (null == descr)
                return -1;
            var releaseExt = currRelease.isapplication ?
                CAppRelTable.Single(new {computationunitreleaseid = currRelease.id}) :
                CModuleRelTable.Single(new {computationunitreleaseid = currRelease.id});
            if (null == releaseExt)
                return -1;
            var parameters = UParamTable.All(new {computationunitreleaseid = currRelease.id});
            if (null == parameters)
                return -1;
            
            
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    currRelease.version = release.Version;
                    currRelease.status = release.Status;
                    currRelease = trans.Update(CUnitRelTable, currRelease);
                    if (null == currRelease) throw new Exception();

                    descr.description = release.Descriptor.Description;
                    descr.isopensource = release.Descriptor.IsOpenSource;
                    descr.usagecounter = release.Descriptor.UsageCounter;
                    descr = trans.Update(RDescTable, descr);
                    if (null == descr) throw new Exception();
                    
                    var unitParams = UParamTable.All(new {computationunitreleaseid = currRelease.id});
                    trans.Delete(UParamTable, unitParams.ToList().FindAll(up => 
                        !release.Parameters.Exists(p => p.Uid == up.uid)).ToArray());

                    foreach (UnitParameter up in release.Parameters)
                    {
                        var unitParam = UParamTable.Single(new {uid = up.Uid});
                        if (null == unitParam)
                        {
                            unitParam = trans.Insert(UParamTable, new
                            {
                                computatiounitreleaseid = currRelease.id,
                                nameorpath = up.NameOrPath,
                                defaultvalue = up.DefaultValue,
                                type = up.Type,
                                ismandatory = up.IsMandatory,
                                targetparameterid = UParamTable.Single(new{uid = up.TargetParameterUid}).id
                            });
                        }
                        else
                        {
                            unitParam.nameorpath = up.NameOrPath;
                            unitParam.defaultvalue = up.DefaultValue;
                            unitParam.type = up.Type;
                            unitParam.ismandatory = up.IsMandatory;
                            unitParam.targetparameterid = UParamTable.Single(new {uid = up.TargetParameterUid}).id;
                            unitParam = trans.Update(UParamTable, unitParam);
                        }
                        if (null == unitParam)
                            throw new Exception();
                    }

                    if (release is ComputationModuleRelease moduleRelease)
                    {
                        releaseExt.image = moduleRelease.Image;
                        releaseExt.command = moduleRelease.Command;
                        releaseExt.ismultitasking = moduleRelease.IsMultitasking;
                        releaseExt = trans.Update(CModuleRelTable, releaseExt);
                        if (null == releaseExt) throw new Exception();

                        var commandArgs = CArgTable.All(new {computationmodulereleaseid = releaseExt.id});
                        trans.Delete(CArgTable, commandArgs.ToArray());
                        commandArgs = trans.Insert(CArgTable, moduleRelease.CommandArguments.Select(ca => new
                        {
                            computationmodulereleaseid = releaseExt.id,
                            value = ca
                        }));
                        if (null == commandArgs || commandArgs.Count() != moduleRelease.CommandArguments.Count())
                            throw new Exception();
                        
                        var credParams = CParTable.All(new {computationmodulereleaseid = releaseExt.id});
                        trans.Delete(CParTable, credParams.ToArray());
                        credParams = trans.Insert(CParTable, moduleRelease.CredentialParameters.Select(cp => new
                        {
                            computationmodulereleaseid = releaseExt.id,
                            environmentvariablename = cp.EnvironmentVariableName,
                            accesscredentialname = cp.AccessCredentialName,
                            defaultcredentialvalue = cp.DefaultCredentialValue
                        }));
                        if (null == credParams || credParams.Count() != moduleRelease.CredentialParameters.Count())
                            throw new Exception();

                        var reqServices = ReqServiceTable.All(new {moduleid = releaseExt.id});
                        trans.Delete(ReqServiceTable, reqServices.ToArray());
                        reqServices = trans.Insert(ReqServiceTable, moduleRelease.RequiredServiceUids.Select(rs => new
                        {
                            moduleid = releaseExt.id,
                            serviceid = GetServiceId(rs)
                        }));
                        if (null == reqServices || reqServices.Count() != moduleRelease.RequiredServiceUids.Count())
                            throw new Exception();
                    }
                    else
                    {
                        releaseExt.diagramuid = ((ComputationApplicationRelease) release).DiagramUid;
                        releaseExt = trans.Update(CAppRelTable, releaseExt);
                        if (null == releaseExt) throw new Exception();
                    }

                    var dataPins = PinTable.All(new {computationunitleaseid = currRelease.id});
                    trans.Delete(PinTable, dataPins.ToArray());
                    dataPins = trans.Insert(PinTable, release.DeclaredPins.Select(dp => new
                    {
                        computationunitreleaseid = currRelease.id,
                        name = dp.Name,
                        uid = dp.Uid,
                        binding = dp.Binding,
                        datamultiplicity = dp.DataMultiplicity,
                        tokenmultiplicity = dp.TokenMultiplicity,
                        // TODO - typeid, structureid, accessid - check for nulls
                        typeid =  DTypeTable.Single(new {uid = dp.Type.Uid}),
                        structureid = DStructTable.Single(new {uid = dp.Structure.Uid}),
                        accessid = ATypeTable.Single(new {uid = dp.Access.Uid})
                    }));
                    if (null == dataPins || dataPins.Count() != release.DeclaredPins.Count())
                        throw new Exception();
                    
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
        /// <param name="unitUid"></param>
        public short DeleteUnit(string unitUid)
        {
            var cUnit = CUnitTable.Single(new {uid = unitUid});
            if (null == cUnit)
                return -1;
            CUnitTable.Delete(cUnit);
            return 0;
        }

        /// 
        /// <param name="unitRelUid"></param>
        public short DeleteUnitRelease(string unitRelUid)
        {
            var cUnitRel = CUnitRelTable.Single(new {uid = unitRelUid});
            if (null == cUnitRel)
                return -1;
            CUnitTable.Delete(cUnitRel);
            return 0;
        }

        /// 
        /// <param name="releaseUid"></param>
        /// <param name="userUid"></param>
        /// <param name="toToolbox"></param>
        public short AddUnitToShelf(string releaseUid, string userUid, bool toToolbox)
        {
            var unitRel = CUnitRelTable.Single(new {uid = releaseUid});
            if (null == unitRel)
                return -1;

            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    MightyOrm table;
                    if (toToolbox)
                        table = ToolTable;
                    else
                        table = AShelfTable;

                    var tableElem = trans.Insert(table, new
                    {
                        useruid = userUid,
                        computationunitreleaseid = unitRel.id
                    }).Single();
                    if (null == tableElem) throw new Exception();

                    trans.CommitTransaction();
                    return 0;
                }
                catch (Exception e)
                {
                    trans.RollbackTransaction();
                    Log.Debug(e.ToString());
                    return -2;
                }
            }
        }

        public short RemoveUnitFromShelf(string releaseUid, string userUid, bool fromToolbox)
        {
            var unitRel = CUnitRelTable.Single(new {uid = releaseUid});
            if (null == unitRel)
                return -1;
            
            MightyOrm table;
            if (fromToolbox)
                table = ToolTable;
            else
                table = AShelfTable;

            var shelfElem = table.Single(new
            {
                useruid = userUid,
                computationunitreleaseId = unitRel.id
            });
            if (null == shelfElem)
                return -1;

            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    trans.Delete(table, shelfElem);
                    trans.CommitTransaction();
                    return 0;
                }
                catch (Exception e)
                {
                    trans.RollbackTransaction();
                    Log.Debug(e.ToString());
                    return -2;
                }
            }
        }
        
        private int GetServiceId(string sUid)
        {
            var cUnitRel = CUnitRelTable.Single(new {uid = sUid});
            var cModuleRel = CModuleRelTable.Single((new {computationunitreleaseid = cUnitRel.id}));
            return cModuleRel.id;
        }
        
        private void ResetUnitRegistry()
        {
            var units = CUnitTable.All();
            foreach (var unit in units)
                CUnitTable.Delete(unit);
            var dTypes = DTypeTable.All();
            foreach (var dType in dTypes)
                DTypeTable.Delete(dType);
            var aTypes = ATypeTable.All();
            foreach (var aType in aTypes)
                ATypeTable.Delete(aType);
            var dStructs = DStructTable.All();
            foreach (var dStruct in dStructs)
                DStructTable.Delete(dStruct);
            
            CreateService(UnitRegistryInit.GetMongoDbService());
            CreateService(UnitRegistryInit.GetFtpService());
            
            CreateDataTypes(UnitRegistryInit.GetInitDataTypes());
            CreateDataStructures(UnitRegistryInit.GetInitDataStructures());
            CreateAccessTypes(UnitRegistryInit.GetInitAccessTypes());
        }

        private void RestoreUnitRegistry()
        {
            ResetUnitRegistry();
            
            List<ComputationModule> modules = UnitRegistryInit.GetInitModules();
            foreach (ComputationModule module in modules)
            {
                string mUid = CreateModule(module.Name, "user1");
                foreach (ComputationUnitRelease release in module.Releases)
                {
                    string rUid = AddReleaseToUnit(mUid, release);
                    AddUnitToShelf(rUid, "user1", true);
                }
            }
            
            List<ComputationApplication> apps = UnitRegistryInit.GetInitApplications();
            foreach (ComputationApplication app in apps)
            {
                string mUid = CreateApp(app.Name, "d0","user1");
                foreach (ComputationUnitRelease release in app.Releases) 
                    AddReleaseToUnit(mUid, release);
            }
        }
        
        private void CreateService(ComputationModule service)
        {
            string uid = CreateModule(service.Name, service.AuthorUid);
            foreach (ComputationUnitRelease release in service.Releases)
                AddReleaseToUnit(uid,release);
        }

        private void CreateDataTypes(List<DataType> dTypes)
        {
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    foreach (DataType dType in dTypes){
                        var dataType = trans.Insert(CUnitTable, new
                        {
                            uid = dType.Uid,
                            name = dType.Name,
                            version = dType.Version,
                            isbuiltin = dType.IsBuiltIn,
                            isstuctured = dType.IsStructured
                        }).Single();
                        if (null == dataType) throw new Exception();
                    }

                    trans.CommitTransaction();
                }
                catch (Exception e)
                {
                    trans.RollbackTransaction();
                    Log.Debug(e.ToString());
                }
            }
        }
        
        private void CreateDataStructures(List<DataStructure> dStructs)
        {
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    foreach (DataStructure dStruct in dStructs){
                        var dataType = trans.Insert(CUnitTable, new
                        {
                            uid = dStruct.Uid,
                            name = dStruct.Name,
                            version = dStruct.Version,
                            isbuiltin = dStruct.IsBuiltIn,
                            dataschema = dStruct.DataSchema
                        }).Single();
                        if (null == dataType) throw new Exception();
                    }

                    trans.CommitTransaction();
                }
                catch (Exception e)
                {
                    trans.RollbackTransaction();
                    Log.Debug(e.ToString());
                }
            }
        }
        
        private void CreateAccessTypes(List<AccessType> aTypes)
        {
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    foreach (AccessType aType in aTypes){
                        var storage = CModuleRelTable.Single(new {name = aType.Name});
                        var dataType = trans.Insert(CUnitTable, new
                        {
                            uid = aType.Uid,
                            name = aType.Name,
                            version = aType.Version,
                            isbuiltin = aType.IsBuiltIn,
                            accessschema = aType.AccessSchema,
                            pathschema = aType.PathSchema,
                            storageid = storage.id
                        }).Single();
                        if (null == dataType) throw new Exception();
                    }

                    trans.CommitTransaction();
                }
                catch (Exception e)
                {
                    trans.RollbackTransaction();
                    Log.Debug(e.ToString());
                }
            }
        }
        
    }
}