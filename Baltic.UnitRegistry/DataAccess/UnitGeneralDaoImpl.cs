using System.Collections.Generic;
using System.Linq;
using Baltic.DataModel.Accounts;
using Baltic.DataModel.CAL;
using Baltic.DataModel.Types;
using Baltic.Types.DataAccess;
using Baltic.UnitRegistry.Tables;

namespace Baltic.UnitRegistry.DataAccess {
	public class UnitGeneralDaoImpl : IUnitGeneral {
		
		private ComputationApplicationTable _cAppTable = null;
		protected ComputationApplicationTable CAppTable =>
			_cAppTable ??= new ComputationApplicationTable();
		
		private ComputationModuleTable _cModuleTable = null;
		protected ComputationModuleTable CModuleTable =>
			_cModuleTable ??= new ComputationModuleTable();
		
		private ComputationUnitTable _cUnitTable = null;
		protected ComputationUnitTable CUnitTable =>
			_cUnitTable ??= new ComputationUnitTable();
		
		private UnitDescriptorTable _uDescTable = null;
		protected UnitDescriptorTable UDescTable =>
			_uDescTable ??= new UnitDescriptorTable();
		
		private ComputationApplicationReleaseTable _cAppRelTable = null;
		protected ComputationApplicationReleaseTable CAppRelTable =>
			_cAppRelTable ??= new ComputationApplicationReleaseTable();
		
		private ComputationModuleReleaseTable _cModuleRelTable = null;
		protected ComputationModuleReleaseTable CModuleRelTable =>
			_cModuleRelTable ??= new ComputationModuleReleaseTable();
		
		private ComputationUnitReleaseTable _cUnitRelTable = null;
		protected ComputationUnitReleaseTable CUnitRelTable =>
			_cUnitRelTable ??= new ComputationUnitReleaseTable();
		
		private ReleaseDescriptorTable _rDescTable = null;
		protected ReleaseDescriptorTable RDescTable =>
			_rDescTable ??= new ReleaseDescriptorTable();
		
		private KeywordTable _keywTable = null;
		protected KeywordTable KeywTable =>
			_keywTable ??= new KeywordTable();
		
		private ProblemClassTable _pClassTable = null;
		protected ProblemClassTable PClassTable =>
			_pClassTable ??= new ProblemClassTable();
		
		private DeclaredDataPinTable _pinTable = null;
		protected DeclaredDataPinTable PinTable =>
			_pinTable ??= new DeclaredDataPinTable();
		
		private AccessTypeTable _aTypeTable = null;
		protected AccessTypeTable ATypeTable =>
			_aTypeTable ??= new AccessTypeTable();
		
		private DataTypeTable _dTypeTable = null;
		protected DataTypeTable DTypeTable =>
			_dTypeTable ??= new DataTypeTable();
		
		private DataStructureTable _dStructTable = null;
		protected DataStructureTable DStructTable =>
			_dStructTable ??= new DataStructureTable();
		
		private UnitParameterTable _uParamTable = null;
		protected UnitParameterTable UParamTable =>
			_uParamTable ??= new UnitParameterTable();
		
		private CommandArgumentTable _cArgTable = null;
		protected CommandArgumentTable CArgTable =>
			_cArgTable ??= new CommandArgumentTable();
		
		private CredentialParameterTable _cParTable = null;
		protected CredentialParameterTable CParTable =>
			_cParTable ??= new CredentialParameterTable();

		private RequiredServiceTable _reqServiceTable = null;
		protected RequiredServiceTable ReqServiceTable =>
			_reqServiceTable ??= new RequiredServiceTable();
		
		private TaskDataSetTable _dataSetTable = null;
		protected TaskDataSetTable DataSetTable =>
			_dataSetTable ??= new TaskDataSetTable();

		/// 
		/// <param name="releaseUid"></param>
		public ComputationUnitRelease GetUnitRelease(string releaseUid)
		{
			var unitRel = CUnitRelTable.Single(new {uid = releaseUid});
			return unitRel.isapplication ? MapApplicationRelease(unitRel) : MapModuleRelease(unitRel);
		}

		/// 
		/// <param name="unitUid"></param>
		public ComputationUnit GetUnit(string unitUid)
		{
			var unit = CUnitTable.Single(new {uid = unitUid});
			return unit.isapplication ? MapApplication(unit) : MapModule(unit);
		}

		/// 
		/// <param name="dtUid"></param>
		public DataType GetDataType(string dtUid)
		{
			var dType = DTypeTable.Single(new {uid = dtUid});
			return MapDataType(dType);
		}

		public List<DataType> GetDataTypes()
		{
			return DTypeTable.All().Select(dt => (DataType) MapDataType(dt)).ToList();
		}

		/// 
		/// <param name="dsUid"></param>
		public DataStructure GetDataStructure(string dsUid)
		{
			var dStruct = DStructTable.Single(new {uid = dsUid});
			return MapDataStructure(dStruct);
		}

		public List<DataStructure> GetDataStructures()
		{
			return DStructTable.All().Select(ds => (DataStructure) MapDataStructure(ds)).ToList();
		}

		/// 
		/// <param name="atUid"></param>
		public AccessType GetAccessType(string atUid)
		{
			var aType = ATypeTable.Single(new {uid = atUid});
			return MapAccessType(aType);
		}

		public List<AccessType> GetAccessTypes()
		{
			return ATypeTable.All().Select(at => (AccessType) MapAccessType(at)).ToList();
		}
		
		protected ComputationModule MapModule(dynamic unit)
		{
			if (null == unit)
				return null;
			var pClass = PClassTable.Single(new {id = unit.classid});
			if (null == pClass)
				return null;
			
			UnitDescriptor descriptor = GetUnitDescriptor(unit.id);
			if (null == descriptor)
				return null;
			
			// TODO - handle ForkParentUid
			var module = CModuleTable.Single(new {computationunitid = unit.id});
			if (null == module)
				return null;
			var mReleases = CUnitRelTable.All(new {unitid = unit.id});
			ComputationModule mRet = new ComputationModule()
			{
				Name = unit.name,
				Uid = unit.uid,
				AuthorUid = unit.authoruid,
				IsService = module.isservice,
				Descriptor = descriptor,
				Class = new ProblemClass() { Name = pClass.name },
			};
			mRet.Releases = mReleases.Select(r => (ComputationUnitRelease) MapModuleRelease(r, mRet)).ToList();
			return mRet;
		}

		protected ComputationApplication MapApplication(dynamic unit)
		{
			if (null == unit)
				return null;
			var pClass = PClassTable.Single(new {id = unit.classid});
			if (null == pClass)
				return null;
			
			UnitDescriptor descriptor = GetUnitDescriptor(unit.id);
			if (null == descriptor)
				return null;
			
			// TODO - handle ForkParentUid
			var app = CAppTable.Single(new {computationunitid = unit.id});
			if (null == app)
				return null;
			var aReleases = CUnitRelTable.All(new {unitid = unit.id});
			ComputationApplication aRet = new ComputationApplication()
			{
				Name = unit.name,
				Uid = unit.uid,
				AuthorUid = unit.authoruid,
				DiagramUid = app.diagramuid,
				Descriptor = descriptor,
				Class = new ProblemClass() { Name = pClass.name }
			};
			aRet.Releases = aReleases.Select(r => (ComputationUnitRelease) MapApplicationRelease(r, aRet)).ToList();
			return aRet;
		}

		protected ComputationApplicationRelease MapApplicationRelease(dynamic uRelease, ComputationUnit unit = null)
		{
			if (null == uRelease)
				return null;
			ReleaseDescriptor descriptor = GetReleaseDescriptor(uRelease.id);
			if (null == descriptor)
				return null;
			var aRelease = CAppRelTable.Single(new {computationunitreleaseid = uRelease.id});
			if (null == aRelease)
				return null;
			// TODO - SupportedResourcesRange
			return new ComputationApplicationRelease()
			{
				Unit = unit ?? MapApplication(CUnitTable.Single(new
				{
					id = uRelease.unitid
				})),
				Version = uRelease.version,
				Uid = uRelease.uid,
				Status = (UnitReleaseStatus)uRelease.status,
				Descriptor = descriptor,
				DeclaredPins = GetDeclaredDataPins(uRelease.id),
				Parameters = GetUnitParameters(uRelease.id),
				DiagramUid = aRelease.diagramuid
			};
		}
		
		protected ComputationUnitRelease MapModuleRelease(dynamic uRelease, ComputationUnit unit = null)
		{
			if (null == uRelease)
				return null;
			ReleaseDescriptor descriptor = GetReleaseDescriptor(uRelease.id);
			if (null == descriptor)
				return null;
			var mRelease = CModuleRelTable.Single(new {computationunitreleaseid = uRelease.id});
			if (null == mRelease)
				return null;
			var requiredServices = ReqServiceTable.All(new {moduleid = mRelease.id});
			if (null == requiredServices)
				return null;
			// TODO - SupportedResourcesRange
			return new ComputationModuleRelease()
			{
				Unit = unit ?? MapModule(CUnitTable.Single(new
				{
					id = uRelease.unitid
				})),
				Version = uRelease.version,
				Uid = uRelease.uid,
				Status = (UnitReleaseStatus)uRelease.status,
				Descriptor = descriptor,
				DeclaredPins = GetDeclaredDataPins(uRelease.id),
				Parameters = GetUnitParameters(uRelease.id),
				Image = mRelease.image,
				Command = mRelease.command,
				IsMultitasking = mRelease.ismultitasking,
				CommandArguments = CArgTable.All(new{computationmodulereleaseid = mRelease.id}).
					Select(ca => new string(ca.value)).ToList(),
				CredentialParameters = CParTable.All(new {computationmodulereleaseid = mRelease.id}).Select(cp =>
					new CredentialParameter()
					{
						AccessCredentialName = cp.accesscredentialname,
						DefaultCredentialValue = cp.defaultcredentialvalue,
						EnvironmentVariableName = cp.environmentvariablename
					}).ToList(),
				RequiredServiceUids = requiredServices.Select(rs => (string) GetRequiredServiceUid(rs)).ToList()
			};
		}

		protected string GetRequiredServiceUid(dynamic reqService)
		{
			var serviceModule = CModuleRelTable.Single(new {id = reqService.serviceid});
			if (null == serviceModule)
				return null;
			var serviceUnit = CUnitRelTable.Single(new {id = serviceModule.computationunitreleaseid});
			return serviceUnit?.uid;
		}

		protected List<DeclaredDataPin> GetDeclaredDataPins(int releaseId)
		{
			var pins = PinTable.All(new {computationunitreleaseid = releaseId});
			return pins?.Select(p => new DeclaredDataPin()
			{
				Name = p.name,
				Uid = p.uid,
				Binding = (DataBinding)p.binding,
				DataMultiplicity = (CMultiplicity)p.datamultiplicity,
				TokenMultiplicity = (CMultiplicity)p.tokenmultiplicity,
				Access = null != p.accessid ? MapAccessType(ATypeTable.Single(new {id = p.accessid})) : null,
				Type =  MapDataType(DTypeTable.Single(new {id = p.typeid})),
				Structure = null != p.structureid ? MapDataStructure(DStructTable.Single(new {id = p.structureid})) : null
			}).ToList();
		}

		protected List<UnitParameter> GetUnitParameters(int releaseId)
		{
			var uParams = UParamTable.All(new {computationunitreleaseid = releaseId});
			return uParams?.Select(p => new UnitParameter()
			{
				NameOrPath = p.nameorpath,
				DefaultValue = p.defaultvalue,
				Type = (UnitParamType)p.type,
				IsMandatory = p.ismandatory,
				Uid = p.uid,
				TargetParameterUid = null != p.targetparameterid ? UParamTable.Single(new{id = p.targetparameterid})?.uid : null
			}).ToList();
		}
		
		protected AccessType MapAccessType(dynamic aType)
		{
			if (null == aType)
				return null;
			var storage = CModuleRelTable.Single(new {id = aType.storageid});
			var storageUnit = null != storage ? 
				CUnitRelTable.Single(new {id = storage.computationunitreleaseid}) : null;
			// TODO - Parent
			return new AccessType()
			{
				Name = aType.name,
				Uid = aType.uid,
				Version = aType.version,
				AccessSchema = aType.accessschema,
				PathSchema = aType.pathschema,
				IsBuiltIn = aType.isbuiltin,
				StorageUid = null != storageUnit ? storageUnit.uid : null
			};
		}
		
		protected DataType MapDataType(dynamic dType)
		{
			if (null == dType)
				return null;
			// TODO - Parent, CompatibleAccessTypes - List<string>
			return new DataType()
			{
				Name = dType.name,
				Uid = dType.uid,
				Version = dType.version,
				IsBuiltIn = dType.isbuiltin,
				IsStructured = dType.isstructured
			};
		}
		
		protected DataStructure MapDataStructure(dynamic dStruct)
		{
			if (null == dStruct)
				return null;
			return new DataStructure()
			{
				Name = dStruct.name,
				Uid = dStruct.uid,
				Version = dStruct.version,
				IsBuiltIn = dStruct.isbuiltin,
				DataSchema = dStruct.dataschema
			};
		}


		protected UnitDescriptor GetUnitDescriptor(int unitId)
		{
			var desc = UDescTable.Single(new {computationunitid = unitId});
			if (null == desc)
				return null;
			var keyw = KeywTable.All(new {unitdescriptorid = desc.id});
			// TODO - add ratings table support!
			return new UnitDescriptor()
			{
				Icon = desc.icon,
				LongDescription = desc.longdescription,
				ShortDescription = desc.shortdescription,
				Keywords = null != keyw ? keyw.Select(k => new string(k.value)).ToList() : new List<string>()
			};
		}

		protected ReleaseDescriptor GetReleaseDescriptor(int releaseId)
		{
			var desc = RDescTable.Single(new {computationunitreleaseid = releaseId});
			if (null == desc)
				return null;
			return new ReleaseDescriptor()
			{
				Date = desc.date,
				Description = desc.description,
				UsageCounter = desc.usagecounter,
				IsOpenSource = desc.isopensource
			};
		}

	}
}