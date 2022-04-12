using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Baltic.Database;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;
using Baltic.UnitRegistry.Tables;
using Microsoft.Extensions.Configuration;
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

        private IConfiguration _configuration;

        public UnitManagementDaoImpl(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
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
        /// <param name="releaseUid"></param>
        public string AddReleaseToUnit(string unitUid, ComputationUnitRelease release, string releaseUid = null)
        {
            var unit = CUnitTable.Single(new {uid = unitUid});
            if (null == unit)
                return null;

            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    release.Uid = releaseUid ?? Guid.NewGuid().ToString();
                    var uRel = trans.Insert(CUnitRelTable, new
                    {
                        uid = release.Uid,
                        unitid = unit.id,
                        isapplication = unit.isapplication,
                        version = release.Version,
                        status = (int)release.Status,
                    }).Single();
                    if (null == uRel) throw new Exception();

                    if (null == release.Descriptor)
                        release.Descriptor = new ReleaseDescriptor();
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

                    var dPins = trans.Insert(PinTable, release.DeclaredPins.Select(pin => new
                    {
                        computationunitreleaseid = uRel.id,
                        name = pin.Name,
                        uid = pin.Uid,
                        binding = (int)pin.Binding,
                        datamultiplicity = (int)pin.DataMultiplicity,
                        tokenmultiplicity = (int)pin.TokenMultiplicity,
                        // TODO - typeid, structureid, accessid - check for nulls
                        typeid =  DTypeTable.Single(new {uid = pin.Type.Uid}).id,
                        structureid = null != pin.Structure ? DStructTable.Single(new {uid = pin.Structure.Uid}).id : null,
                        accessid = null != pin.Access ? ATypeTable.Single(new {uid = pin.Access.Uid}).id : null
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
                        classid = 0 // TODO - proper class handling
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
            return units.ToList().FindAll(u => "system" != u.authoruid).Select(u => u.isapplication ? 
                (ComputationUnit) MapApplication(u) : (ComputationUnit) MapModule(u)).ToList();
        }
        
        /// 
        /// <param name="query"></param>
        public List<ComputationUnitRelease> FindUnitReleases(UnitQuery query)
        {
            IEnumerable<dynamic> releases;
            // TODO - OnlyLastRelease
            dynamic dbQuery = new ExpandoObject();
            if (!query.AllUnits)
                dbQuery.isapplication = query.IsApp;
            releases = CUnitRelTable.All(dbQuery);

            if (!string.IsNullOrEmpty(query.UserUid))
            {
                MightyOrm shelfTable;
                if (query.IsInToolbox)
                    shelfTable = ToolTable;
                else
                    shelfTable = AShelfTable;
                var shelf = shelfTable.All(new
                    {
                        useruid = query.UserUid
                    }).ToList();
                releases = releases.ToList()
                    .FindAll(r => shelf.Exists(tr => r.id == tr.computationunitreleaseid));
            }

            // TODO - optimise
            List<ComputationUnitRelease> ret = releases.Select(u => u.isapplication ? 
                (ComputationUnitRelease) MapApplicationRelease(u) : (ComputationUnitRelease) MapModuleRelease(u)).ToList();
            if (!string.IsNullOrEmpty(query.AuthorUid))
                ret = ret.FindAll(r => r.Unit.AuthorUid == query.AuthorUid);
            
            return ret;
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
                    currUnit.authoruid = unit.AuthorUid ?? currUnit.authoruid;
                    currUnit.classid = null != unit.Class ? PClassTable.Single(new {name = unit.Class.Name}).id : 0;
                    int uRes = trans.Update(CUnitTable,currUnit);
                    if (1 != uRes) throw new Exception();

                    descr.shortdescription = unit.Descriptor.ShortDescription;
                    descr.longdescription = unit.Descriptor.LongDescription;
                    descr.icon = unit.Descriptor.Icon;
                    uRes = trans.Update(UDescTable, descr);
                    if (1 != uRes) throw new Exception();

                    trans.Delete(KeywTable, keywords.ToArray());
                    if (null != unit.Descriptor.Keywords)
                    {
                        keywords = trans.Insert(KeywTable, unit.Descriptor.Keywords.Select(k => new
                        {
                            unitdescriptorid = descr.id,
                            value = k
                        }));
                        if (null == keywords || keywords.Count() != unit.Descriptor.Keywords.Count())
                            throw new Exception();
                    }

                    // TODO - ratings

                    if (unit is ComputationModule module)
                    {
                        unitExt.isservice = module.IsService;
                        uRes = trans.Update(CModuleTable, unitExt);
                    }
                    else
                    {
                        unitExt.diagramuid = ((ComputationApplication) unit).DiagramUid ?? unitExt.diagramuid;
                        uRes = trans.Update(CAppTable, unitExt);
                    }

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
                    currRelease.status = (int)release.Status;
                    int uRes = trans.Update(CUnitRelTable, currRelease);
                    if (1 != uRes) throw new Exception();

                    descr.description = release.Descriptor.Description;
                    descr.isopensource = release.Descriptor.IsOpenSource;
                    descr.usagecounter = release.Descriptor.UsageCounter;
                    uRes = trans.Update(RDescTable, descr);
                    if (1 != uRes) throw new Exception();
                    
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
                                computationunitreleaseid = currRelease.id,
                                nameorpath = up.NameOrPath,
                                defaultvalue = up.DefaultValue,
                                type = (int)up.Type,
                                ismandatory = up.IsMandatory,
                                targetparameterid = null != up.TargetParameterUid ? UParamTable.Single(new{uid = up.TargetParameterUid}).id : null
                            });
                        }
                        else
                        {
                            unitParam.nameorpath = up.NameOrPath;
                            unitParam.defaultvalue = up.DefaultValue;
                            unitParam.type = up.Type;
                            unitParam.ismandatory = up.IsMandatory;
                            unitParam.targetparameterid = UParamTable.Single(new {uid = up.TargetParameterUid}).id;
                            uRes = trans.Update(UParamTable, unitParam);
                        }
                        if (1 != uRes)
                            throw new Exception();
                    }

                    if (release is ComputationModuleRelease moduleRelease)
                    {
                        releaseExt.image = moduleRelease.Image;
                        releaseExt.command = moduleRelease.Command;
                        releaseExt.ismultitasking = moduleRelease.IsMultitasking;
                        uRes = trans.Update(CModuleRelTable, releaseExt);
                        if (1 != uRes) throw new Exception();

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
                        releaseExt.diagramuid = ((ComputationApplicationRelease) release).DiagramUid ?? releaseExt.diagramuid;
                        uRes = trans.Update(CAppRelTable, releaseExt);
                        if (1 != uRes) throw new Exception();
                    }

                    if (null == release.DeclaredPins)
                        return 0;
                    
                    var dataPins = PinTable.All(new {computationunitreleaseid = currRelease.id});
                    trans.Delete(PinTable, dataPins.ToArray());
                    dataPins = trans.Insert(PinTable, release.DeclaredPins.Select(dp => new
                    {
                        computationunitreleaseid = currRelease.id,
                        name = dp.Name,
                        uid = dp.Uid,
                        binding = (int)dp.Binding,
                        datamultiplicity = (int)dp.DataMultiplicity,
                        tokenmultiplicity = (int)dp.TokenMultiplicity,
                        // TODO - typeid, structureid, accessid - check for nulls
                        typeid =  DTypeTable.Single(new {uid = dp.Type.Uid}).id,
                        structureid = null != dp.Structure ? DStructTable.Single(new {uid = dp.Structure.Uid}).id : null,
                        accessid = null != dp.Access ? ATypeTable.Single(new {uid = dp.Access.Uid}).id : null
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
            return 1 == CUnitTable.Delete(cUnit) ? (short)0 : (short)-2;
        }

        /// 
        /// <param name="unitRelUid"></param>
        public short DeleteUnitRelease(string unitRelUid)
        {
            var cUnitRel = CUnitRelTable.Single(new {uid = unitRelUid});
            if (null == cUnitRel)
                return -1;
            return 1 == CUnitRelTable.Delete(cUnitRel) ? (short)0 : (short)-2;
        }

        /// 
        /// <param name="releaseUid"></param>
        /// <param name="userUid"></param>
        /// <param name="toToolbox"></param>
        public short AddUnitToShelf(string releaseUid, string userUid, bool toToolbox)
        {
            var unitRel = CUnitRelTable.Single(new {uid = releaseUid});
            if (null == unitRel || (short)UnitReleaseStatus.Deprecated <= unitRel.status)
                return -1;

            MightyOrm table;
            if (toToolbox)
                table = ToolTable;
            else
                table = AShelfTable;

            if (null != table.Single(new
            {
                useruid = userUid,
                computationunitreleaseid = unitRel.id
            }))
                return -2;
            
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
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
        
        private UnitRegistryInit ResetUnitRegistry()
        {
            UnitRegistryInit unitRegistryInit = new UnitRegistryInit(_configuration);
            
            // Enter a default ProblemClass
            var pClass = PClassTable.Single(new {Id = 0});
            // TODO - make secure for deletion of pclasstable records
            if (null == pClass)
                PClassTable.Insert(new {Name = "General"});
            
            var dSets = DataSetTable.All();
            foreach (var dSet in dSets)
                DataSetTable.Delete(dSet);
            var dPins = PinTable.All();
            foreach (var dPin in dPins)
                PinTable.Delete(dPin);
            var dTypes = DTypeTable.All();
            foreach (var dType in dTypes)
                DTypeTable.Delete(dType);
            var aTypes = ATypeTable.All();
            foreach (var aType in aTypes)
                ATypeTable.Delete(aType);
            var dStructs = DStructTable.All();
            foreach (var dStruct in dStructs)
                DStructTable.Delete(dStruct);
            var units = CUnitTable.All();
            foreach (var unit in units)
                CUnitTable.Delete(unit);
            
            CreateService(unitRegistryInit.GetMongoDbService());
            CreateService(unitRegistryInit.GetFtpService());
            
            CreateDataTypes(unitRegistryInit.GetInitDataTypes());
            CreateDataStructures(unitRegistryInit.GetInitDataStructures());
            CreateAccessTypes(unitRegistryInit.GetInitAccessTypes());

            return unitRegistryInit;
        }

        private void RestoreUnitRegistry()
        {
            UnitRegistryInit unitRegistryInit = ResetUnitRegistry();
            
            List<ComputationModule> modules = unitRegistryInit.GetInitModules();
            foreach (ComputationModule module in modules)
            {
                string mUid = CreateModule(module.Name, "user1");
                module.Uid = mUid;
                UpdateUnit(module);
                foreach (ComputationUnitRelease release in module.Releases)
                {
                    AddReleaseToUnit(mUid, release, release.Uid);
                    UpdateUnitRelease(release);
                }
            }
            
            List<ComputationApplication> apps = unitRegistryInit.GetInitApplications();
            foreach (ComputationApplication app in apps)
            {
                string mUid = CreateApp(app.Name, app.DiagramUid,"user1");
                app.Uid = mUid;
                UpdateUnit(app);
                foreach (ComputationUnitRelease release in app.Releases)
                {
                    AddReleaseToUnit(mUid, release, release.Uid);
                    UpdateUnitRelease(release);
                }
            }
        }
        
        private void CreateService(ComputationModule service)
        {
            string uid = CreateModule(service.Name, service.AuthorUid);
            foreach (ComputationUnitRelease release in service.Releases)
                AddReleaseToUnit(uid, release, release.Uid);
        }

        private void CreateDataTypes(List<DataType> dTypes)
        {
            using (var trans = DBTransaction.BeginTransaction())
            {
                try
                {
                    foreach (DataType dType in dTypes){
                        var dataType = trans.Insert(DTypeTable, new
                        {
                            uid = dType.Uid,
                            name = dType.Name,
                            version = dType.Version,
                            isbuiltin = dType.IsBuiltIn,
                            isstructured = dType.IsStructured
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
                        var dataType = trans.Insert(DStructTable, new
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
                    int storageId;
                    foreach (AccessType aType in aTypes)
                    {
                        if (string.IsNullOrEmpty(aType.StorageUid))
                            storageId = 0; // TODO - maybe rework the DB
                        else
                        {
                            var unitRel = CUnitRelTable.Single(new {uid = aType.StorageUid});
                            var moduleRel = CModuleRelTable.Single(new {computationunitreleaseid = unitRel.id});
                            storageId = moduleRel.id;
                        }

                        var dataType = trans.Insert(ATypeTable, new
                        {
                            uid = aType.Uid,
                            name = aType.Name,
                            version = aType.Version,
                            isbuiltin = aType.IsBuiltIn,
                            accessschema = aType.AccessSchema,
                            pathschema = aType.PathSchema,
                            storageid = storageId
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