using System;
using System.Collections.Generic;
using Baltic.Core.Utils;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;

namespace Baltic.UnitRegistry.DataAccess
{
    public class UnitManagementDaoImplMock : UnitGeneralDaoImplMock, IUnitManagement
    {
        private IDictionary<string, List<string>> _toolbox;
        private IDictionary<string, List<string>> _appShelf;
        
        public UnitManagementDaoImplMock(UnitRegistryMock registry) : base(registry)
        {
            _toolbox = registry.Toolbox;
            _appShelf = registry.AppShelf;
        }

        /// 
        /// <param name="appName"></param>
        /// <param name="diagramUid"></param>
        /// <param name="userUid"></param>
        public string CreateApp(string appName, string diagramUid, string userUid)
        {
            ComputationApplication app = new ComputationApplication()
            {
                Name = appName, DiagramUid = diagramUid, AuthorUid = userUid,
                Descriptor = new UnitDescriptor()
                {
                    Icon = "https://www.balticlsc.eu/model/_icons/default.png"
                },
                Uid = Guid.NewGuid().ToString(),
                Releases = new List<ComputationUnitRelease>()
            };
            _cas.Add(app);
            return app.Uid;
        }
        
        /// 
        /// <param name="moduleName"></param>
        /// <param name="userUid"></param>
        public string CreateModule(string moduleName, string userUid)
        {
            ComputationModule module = new ComputationModule()
            {
                Name = moduleName, AuthorUid = userUid,
                Descriptor = new UnitDescriptor(){
                    Icon = "https://www.balticlsc.eu/model/_icons/default.png"
                },
                Uid = Guid.NewGuid().ToString()
            };
            _cms.Add(module);
            return module.Uid;
        }

        /// 
        /// <param name="unitUid"></param>
        /// <param name="release"></param>
        /// <param name="releaseUid"></param>
        public string AddReleaseToUnit(string unitUid, ComputationUnitRelease release, string releaseUid = null)
        {
            ComputationUnit unit;
            if (release is ComputationApplicationRelease)
                unit = _cas.Find(a => a.Uid == unitUid);
            else
                unit = _cms.Find(m => m.Uid == unitUid);
            if (null != unit)
            {
                release.Uid = Guid.NewGuid().ToString();
                if (null == release.Descriptor)
                    release.Descriptor = new ReleaseDescriptor();
                release.Descriptor.Date = DateTime.Now;
                unit.Releases.Add(release);
                release.Unit = unit;
                if (release is ComputationApplicationRelease)
                    _cars.Add((ComputationApplicationRelease) release);
                else 
                    _cmrs.Add((ComputationModuleRelease) release);
                return release.Uid;
            }
            return null;
        }

        /// 
        /// <param name="query"></param>
        public List<ComputationUnit> FindUnits(UnitQuery query)
        {
            List<ComputationUnit> result = new List<ComputationUnit>();
            if (query.AllUnits || query.IsApp) 
                result.AddRange(_cas);
            if (query.AllUnits || !query.IsApp)
                result.AddRange(_cms.FindAll(m => "system" != m.AuthorUid));
            if (!string.IsNullOrEmpty(query.AuthorUid))
                result = result.FindAll(r => r.AuthorUid == query.AuthorUid);
            if (!query.OnlyLastRelease)
                return result;
            List<ComputationUnit> filtered = new List<ComputationUnit>();
            foreach (ComputationUnit unit in result)
            {
                ComputationUnit fUnit;
                if (unit is ComputationApplication)
                    fUnit = new ComputationApplication();
                else
                    fUnit = new ComputationModule();
                fUnit = DBMapper.Map<ComputationUnit>(unit, fUnit);
                ComputationUnitRelease latest = null;
                if (null != unit.Releases)
                    foreach(ComputationUnitRelease rel in unit.Releases)
                        if (null == latest || rel.Descriptor.Date > latest.Descriptor.Date)
                            latest = rel;
                fUnit.Releases = new List<ComputationUnitRelease>();
                if (null != latest)
                    fUnit.Releases.Add(latest);
                filtered.Add(fUnit);
            }
            return filtered;
        }
        
        /// 
        /// <param name="query"></param>
        public List<ComputationUnitRelease> FindUnitReleases(UnitQuery query)
        {
            // TODO - onlyLastRelease
            List<ComputationUnitRelease> result = new List<ComputationUnitRelease>();
            if (query.AllUnits || query.IsApp) 
                result.AddRange(_cars);
            if (query.AllUnits || !query.IsApp)
                result.AddRange(_cmrs);
            if (!string.IsNullOrEmpty(query.AuthorUid))
                result = result.FindAll(r => r.Unit.AuthorUid == query.AuthorUid);
            if (!string.IsNullOrEmpty(query.UserUid))
            {
                if (query.IsInToolbox) 
                    result = result.FindAll(r => _toolbox.ContainsKey(query.UserUid) && _toolbox[query.UserUid].Contains(r.Uid));
                else
                    result = result.FindAll(r => _appShelf.ContainsKey(query.UserUid) && _appShelf[query.UserUid].Contains(r.Uid));
            }
            return result;
        }
        
        /// 
        /// <param name="unit"></param>
        public short UpdateUnit(ComputationUnit unit)
        {
            ComputationUnit currUnit = GetUnit(unit.Uid);
            if (null == currUnit) return -1;
            if ((unit is ComputationApplication) != (currUnit is ComputationApplication)) return -2;
            
            currUnit.Name = unit.Name;
            currUnit.Class = unit.Class;
            if (currUnit is ComputationModule cm)
                cm.IsService = ((ComputationModule) unit).IsService;
            DBMapper.Map<UnitDescriptor>(unit.Descriptor, currUnit.Descriptor);
            return 0;
        }

        /// 
        /// <param name="release"></param>
        public short UpdateUnitRelease(ComputationUnitRelease release)
        {
            ComputationUnitRelease currRel = GetUnitRelease(release.Uid);
            if (null == currRel) return -1;
            DBMapper.Map<ComputationUnitRelease>(release, currRel);
            DBMapper.Map<ReleaseDescriptor>(release.Descriptor, currRel.Descriptor);
            DBMapper.Map<ResourceReservation>(release.SupportedResourcesRange.MaxReservation,
                currRel.SupportedResourcesRange.MaxReservation);
            DBMapper.Map<ResourceReservation>(release.SupportedResourcesRange.MinReservation,
                currRel.SupportedResourcesRange.MinReservation);
            return 0;
        }

        /// 
        /// <param name="unitUid"></param>
        public short DeleteUnit(string unitUid)
        {
            ComputationUnit currUnit = GetUnit(unitUid);
            if (null == currUnit)
                return -1;
            if (null != currUnit.Releases)
                foreach (ComputationUnitRelease rel in currUnit.Releases)
                    DeleteUnitRelease(rel.Uid);
            if (currUnit is ComputationApplication)
                _cas.Remove((ComputationApplication) currUnit);
            else 
                _cms.Remove((ComputationModule) currUnit);
            return 0;
        }

        /// 
        /// <param name="unitRelUid"></param>
        public short DeleteUnitRelease(string unitRelUid)
        {
            ComputationUnitRelease currRel = GetUnitRelease(unitRelUid);
            if (null == currRel)
                return -1;
            if (currRel is ComputationApplicationRelease)
                _cars.Remove((ComputationApplicationRelease) currRel);
            else 
                _cmrs.Remove((ComputationModuleRelease) currRel);
            if (null != currRel.Unit && null != currRel.Unit.Releases)
                currRel.Unit.Releases.Remove(currRel);
            return 0;
        }

        /// 
        /// <param name="releaseUid"></param>
        /// <param name="userUid"></param>
        /// <param name="toToolbox"></param>
        public short AddUnitToShelf(string releaseUid, string userUid, bool toToolbox)
        {
            ComputationUnitRelease release = GetUnitRelease(releaseUid);
            if (UnitReleaseStatus.Deprecated <= release.Status)
                return -1;
            if (toToolbox)
            {
                if (!_toolbox.ContainsKey(userUid)) 
                    _toolbox.Add(userUid, new List<string>());
                _toolbox[userUid].Add(releaseUid);
            }
            else
            {
                if (!_appShelf.ContainsKey(userUid)) 
                    _appShelf.Add(userUid, new List<string>());
                _appShelf[userUid].Add(releaseUid);
            }
            return 0;
        }

        /// 
        /// <param name="releaseUid"></param>
        /// <param name="userUid"></param>
        /// <param name="fromToolbox"></param>
        public short RemoveUnitFromShelf(string releaseUid, string userUid, bool fromToolbox)
        {
            if (fromToolbox)
            {
                if (!_toolbox.ContainsKey(userUid)) 
                    return -1;
                _toolbox[userUid].Remove(releaseUid);
            }
            else
            {
                if (!_appShelf.ContainsKey(userUid)) 
                    return -1;
                _appShelf[userUid].Remove(releaseUid);
            }
            return 0;
        }
        
    }
}