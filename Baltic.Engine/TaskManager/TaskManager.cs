using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;
using Baltic.DataModel.Types;
using Baltic.Engine.JobBroker;
using Baltic.Engine.TaskProcessor;
using Baltic.Types.DataAccess;
using Baltic.Types.Entities;
using Grpc.Core.Logging;
using Serilog;

namespace Baltic.Engine.TaskManager
{
	public class TaskManager : ITaskManager, ICALTranslation {

		private IUnitProcessing _unitRegistry;
		private ITaskManagement _taskRegistry;
		private ITaskProcessor _taskProcessor;
		private IDataModelImplFactory _factory;
		private IDiagram _diagramRegistry;
		private IJobBroker _broker;
		private INetworkBrokerage _networkRegistry;

		/// 
		/// <param name="ur"></param>
		/// <param name="tr"></param>
		/// <param name="tp"></param>
		/// <param name="dmf"></param>
		/// <param name="diag"></param>
		/// <param name="broker"></param>
		/// <param name="nr"></param>
		public TaskManager(IUnitProcessing ur, ITaskManagement tr, ITaskProcessor tp, IDataModelImplFactory dmf,
			IDiagram diag, IJobBroker broker, INetworkBrokerage nr){
			_unitRegistry = ur;
			_taskRegistry = tr;
			_taskProcessor = tp;
			_factory = dmf;
			_diagramRegistry = diag;
			_broker = broker;
			_networkRegistry = nr;
		}

		/// 
		/// <param name="releaseUid"></param>
		/// <param name="par"></param>
		/// <param name="userUid"></param>
		public string InitiateTask(string releaseUid, TaskParameters par, string userUid){
			ComputationApplicationRelease release = (ComputationApplicationRelease) _unitRegistry.GetUnitRelease(releaseUid);
			if (string.IsNullOrEmpty(release.DiagramUid))
				return null;
			DiagramTranslator translator = new DiagramTranslator(release,_unitRegistry,_diagramRegistry);
			translator.TranslateDiagram();
			return InitiateTask(release,par,userUid);
		}

		/// 
		/// <param name="appUid"></param>
		/// <param name="par"></param>
		/// <param name="userUid"></param>
		public string InitiateAppTestTask(string appUid, TaskParameters par, string userUid){
			ComputationApplication app = (ComputationApplication) _unitRegistry.GetUnit(appUid);
			if (string.IsNullOrEmpty(app.DiagramUid))
				return null;
			ComputationApplicationRelease tempRelease = new ComputationApplicationRelease()
			{
				DiagramUid = app.DiagramUid, Uid = app.Uid
			};
			DiagramTranslator translator = new DiagramTranslator(tempRelease,_unitRegistry,_diagramRegistry);
			translator.TranslateDiagramFull();
			return InitiateTask(tempRelease,par,userUid);
		}

		/// 
		/// <param name="release"></param>
		/// <param name="par"></param>
		/// <param name="userUid"></param>
		private string InitiateTask(ComputationApplicationRelease release, TaskParameters par, string userUid){
		
			//*test*
			Log.Debug(ConsoleString() + "Baltic.TaskManager InitiateTask START: " + release.Uid);
			//*test*
		
			CTask task = _factory.CreateCTask();
			task.ReleaseUid = release.Uid;
			task.OwnerUid = userUid;
			CTaskWriter writer = new CTaskWriter(task,_factory);
			TaskExecution taskExec = writer.Task.Execution;
			taskExec.Parameters = par;
			taskExec.Status = ComputationStatus.Idle;
			taskExec.Start = DateTime.Now;
			taskExec.Task = task;
			if (UnitStrength.Strong == par.ClusterAllocation) {
				writer.GenerateJobBatch();
			}
		
			foreach (DeclaredDataPin pin in release.DeclaredPins) {
				pin.TokenNo = writer.GenerateNextToken();
				if (DataBinding.Provided != pin.Binding && null != pin.Outgoing)
					pin.Outgoing.Target.TokenNo = pin.TokenNo;
				if ((DataBinding.Provided <= pin.Binding) && null != pin.Incoming)
				{
					pin.Binding = DataBinding.ProvidedExternal;
					pin.Incoming.Source.Binding = DataBinding.ProvidedExternal;
					pin.Incoming.Source.TokenNo = pin.TokenNo;
				}
				writer.GenerateDataToken(pin);
			}

			TranslateApp(par.ClusterAllocation,release,writer,UnitStrength.Strong == par.ClusterAllocation,par.CustomParameters,true);

			writer.SetDepthLevelsInTask();
			
			//*test*
			Log.Debug(ConsoleString() + "TranslateApp: " + release.Uid + "\n@@ " +
			          writer.Task.ToString());
			//*test*

			DetermineBatchReservationRanges(writer.Task);
			
			_taskRegistry.StoreTask(writer.Task);
		
			//*test*
			Log.Debug(ConsoleString() + "InitiateTask FINISH: " + release.Uid);
			//*test*
    	
			return writer.Task.Uid;
		}

		/// 
		/// <param name="task"></param>
		private short DetermineBatchReservationRanges(CTask task)
		{
			foreach (CJobBatch batch in task.Batches)
			{
				// TODO - re-think this algorithm for determining batch resource ranges
				ResourceReservationRange range = (batch.DerivedReservationRange = new ResourceReservationRange());
				foreach (CJob job in batch.Jobs)
				{
					ComputationUnitRelease unit = _unitRegistry.GetUnitRelease(job.ModuleReleaseUid);
					range.MaxReservation.Cpus += unit.SupportedResourcesRange.MaxReservation.Cpus;
					range.MaxReservation.Gpus += unit.SupportedResourcesRange.MaxReservation.Gpus;
					range.MaxReservation.Memory += unit.SupportedResourcesRange.MaxReservation.Memory;
					range.MaxReservation.Storage += unit.SupportedResourcesRange.MaxReservation.Storage;
					if (range.MinReservation.Cpus < unit.SupportedResourcesRange.MinReservation.Cpus)
						range.MinReservation.Cpus = unit.SupportedResourcesRange.MinReservation.Cpus;
					if (range.MinReservation.Gpus < unit.SupportedResourcesRange.MinReservation.Gpus)
						range.MinReservation.Gpus = unit.SupportedResourcesRange.MinReservation.Gpus;
					if (range.MinReservation.Memory < unit.SupportedResourcesRange.MinReservation.Memory)
						range.MinReservation.Memory = unit.SupportedResourcesRange.MinReservation.Memory;
					if (range.MinReservation.Storage < unit.SupportedResourcesRange.MinReservation.Storage)
						range.MinReservation.Storage = unit.SupportedResourcesRange.MinReservation.Storage;
				}
			}

			return 0;
		}

		/// 
		/// <param name="taskUid"></param>
		/// <param name="pinUid"></param>
		/// <param name="data"></param>
		/// <param name="accessData"></param>
		public short InjectDataSet(string taskUid, string pinUid, CDataSet data, CDataSet accessData){
			TokenMessage tm = null;
			
			Log.Debug($"{ConsoleString()} Baltic.TaskManager InjectDataSet START:\n" +
			          $"Data: {data?.Values}\n" +
			          $"Access data: {accessData?.Values}");

			IEnumerable<CDataToken> taskTokens = _taskRegistry.GetTaskDataTokens(taskUid);
			if (null == taskTokens)
				return -1;
			CDataToken dt = taskTokens.ToList().Find(dtk => dtk.PinUid == pinUid);
			if (null == dt)
				return -2;
			_taskRegistry.UpdateDataSet(data,accessData,dt.Uid);
			
			tm = new TokenMessage()
			{
				TokenNo = dt.TokenNo,
				TaskUid = taskUid,
				PinName = dt.PinName,
				DataSet = new CDataSet(){Values = data?.Values}
			};

			_taskProcessor.PutTokenMessage(tm);
			
			//*test*
			Log.Debug(ConsoleString() + "Baltic.TaskManager injectDataSet FINISH: " + data?.Values);
			//*test*
			
			return 0;
		}

		/// 
		/// <param name="releaseUid"></param>
		public ResourceReservationRange GetSupportedResourceRange(string releaseUid)
		{
			ComputationUnitRelease release = _unitRegistry.GetUnitRelease(releaseUid);
			if (null == release) return null;
			ResourceReservationRange result = release.SupportedResourcesRange;
			if (null == result)
			{
				result = new ResourceReservationRange();
				// MOCK - set constantly the same range
				result.MinReservation.Memory = 64;
				result.MaxReservation.Memory = 128;
				result.MinReservation.Storage = 1024;
				result.MaxReservation.Storage = 4096;
				result.MinReservation.Cpus = 2;
				result.MaxReservation.Cpus = 20;
				result.MinReservation.Gpus = 10;
				result.MaxReservation.Gpus = 100;
				// MOCK end
				// TODO calculate the range from bathes and jobs
			}
			return result;
		}

		public short UpdateActiveTaskStatuses() // TODO call from Cron
		{
			TaskQuery query = new TaskQuery()
			{
				Statuses = new List<ComputationStatus>()
					{ComputationStatus.Working, ComputationStatus.Idle, ComputationStatus.Neglected}
			};
			List<CTask> tasks = _taskRegistry.FindTasks(query).ToList();
			foreach (CTask t in tasks)
				foreach (CJobBatch jb in t.Batches)
					foreach (BatchExecution je in jb.BatchExecutions)
						_broker.UpdateStatusesForBatchInstance(je.BatchMsgUid);
			return 0;
		}

		public short AbortTask(string taskUid)
		{
			return _taskProcessor.AbortTask(taskUid);
		}

		public List<CCluster> GetCompatibleClusters(string appReleaseUid)
		{
			ComputationUnitRelease release = _unitRegistry.GetUnitRelease(appReleaseUid);
			if (!(release is ComputationApplicationRelease))
				return null;
			return _networkRegistry.GetMatchingClusters(release.SupportedResourcesRange);
		}

		/// 
		/// <param name="version"></param>
		/// <param name="diagramUid"></param>
		public ComputationApplicationRelease CreateAppRelease(string version, string diagramUid)
		{
			ComputationApplicationRelease appRelease = new ComputationApplicationRelease()
			{
				Version = version,
				DiagramUid = diagramUid
			};
			DiagramTranslator translator = new DiagramTranslator(appRelease, _unitRegistry, _diagramRegistry);
			translator.TranslateDiagramPartial();
			return appRelease;
		}

		/// <summary>
		/// Translates a unit call to CAL-exec: generates a job batch with data tokens if needed,
		/// generates a job if this is a module call, or translates a CAL diagram and calls TranslateApp recursively if it is an application call
		/// </summary>
		/// <param name="strength">Strength of the app that contains the unit call</param>
		/// <param name="unitCall">The actual unit call to be processed</param>
		/// <param name="task">The task to be updated</param>
		/// <param name="customParams">A list custom parameters</param>
		private void TranslateUnitCall(UnitStrength strength, UnitCall unitCall, CTaskWriter task, List<UnitCallParameter> customParams)
		{
			bool addCopyInJobs = false;
			
			// Generate a new job batch if the current call is 'strong' and within a 'weak' environment (application)
			if ((unitCall.Strength == UnitStrength.Strong || unitCall.Unit is ComputationModuleRelease)
			    && strength == UnitStrength.Weak) {
				task.GenerateJobBatch();
				foreach (ComputedDataPin pin in unitCall.Pins)
					task.GenerateDataToken(pin);
				addCopyInJobs = true;
			}
			
			// *** Process a Module Release call *********************
			if (unitCall.Unit is ComputationModuleRelease) {
				List<UnitCallParameter> callParams = CreateJobParameters(unitCall,customParams);
				
				task.GenerateJob((ComputationModuleRelease) unitCall.Unit, unitCall.Name, callParams);
				// TODO - add services based on ComputationModuleRelease.RequiredServiceUids
				// TODO - solve credential passing for services generated according to TODO above
				foreach (ComputedDataPin pin in unitCall.Pins)
					ProcessUnitCallPin(pin, task);
			}
			// *** Process an Application Release call ******************
			else
			{
				List<UnitCallParameter> callParams = new List<UnitCallParameter>();
				UnitCallParameter newParam;
				UnitParameterValue newParamValue;
				ComputationApplicationRelease release = (ComputationApplicationRelease)unitCall.Unit;
				
				DiagramTranslator translator = new DiagramTranslator(release, _unitRegistry, _diagramRegistry);
				translator.TranslateDiagram();
				
				// copy token numbers from the computed pins of the call to the app's declared pins 
				foreach (ComputedDataPin pin in unitCall.Pins)
					pin.Declared.TokenNo = pin.TokenNo;
				
				foreach(UnitParameter par in release.Parameters){
					if (null != (newParam = customParams.Find(inv => par.Uid == inv.InvariantUid))) // TODO - remove the 'invariant'
					    callParams.Add(new UnitCallParameter{Value = newParam.Value, InvariantUid = par.TargetParameterUid});
					else if (null!= (newParamValue = unitCall.ParameterValues.Find(val => par.Uid == val.Declaration.Uid)))
						callParams.Add(new UnitCallParameter{Value = newParamValue.Value, InvariantUid = par.TargetParameterUid});
				}
				
				if (strength == UnitStrength.Weak)
					TranslateApp(unitCall.Strength,release,task,addCopyInJobs,callParams,false);
				else TranslateApp(UnitStrength.Strong,release,task,addCopyInJobs,callParams,false);
			}
		}

		/// <summary>
		/// Creates a list of parameters for the new job.
		/// Substitutes the default values by custom (user-defined) values of the call parameters (if found)
		/// </summary>
		/// <param name="unitCall"></param>
		/// <param name="customParams"></param>
		/// <returns></returns>
		private List<UnitCallParameter> CreateJobParameters(UnitCall unitCall, List<UnitCallParameter> customParams)
		{
			List<UnitCallParameter> ret = new List<UnitCallParameter>();
			bool existsPort = false;

			foreach(UnitParameter unitParam in unitCall.Unit.Parameters){
				UnitCallParameter newParam;
				UnitParameterValue newParamValue;
				// Custom (user defined) parameter matches a Unit parameter? If so, add values from the Custom parameter
				if (null != (newParam = customParams.Find(par => unitParam.Uid == par.Declaration.Uid)))
				{
					newParam.NameOrPath = unitParam.NameOrPath; // just in case
					newParam.Type = unitParam.Type; // just in case
				}
				// UnitCall parameter matches a Unit parameter? If so, add values from the UnitCall parameter
				else if (null != (newParamValue =
					unitCall.ParameterValues.Find(val => unitParam.Uid == val.Declaration.Uid)))
					newParam = new UnitCallParameter()
					{
						NameOrPath = newParamValue.Declaration.NameOrPath,
						Value = newParamValue.Value,
						Type = newParamValue.Declaration.Type
					};
				else
				// No match - copy the current parameter (default value) and add to the list
					newParam = new UnitCallParameter()
					{
						NameOrPath = unitParam.NameOrPath,
						Value = unitParam.DefaultValue,
						Type = unitParam.Type
					};
				ret.Add(newParam);
				if (UnitParamType.Port == newParam.Type)
					existsPort = true;
			}
			
			// Force at least one port mapping for k8s compatibility
			if (!existsPort)
				ret.Add(new UnitCallParameter()
				{
					Type = UnitParamType.Port,
					Value = "80",
					NameOrPath = ""
				});
			
			return ret;
		}

		/// <summary>
		/// Processes one pin in a unit call: if needed, generates an infrastructural service and a data-copier job  
		/// </summary>
		/// <param name="pin">The pin to be processed</param>
		/// <param name="task">The task writer, to which new elements will be added</param>
		private void ProcessUnitCallPin(ComputedDataPin pin, CTaskWriter task)
		{
			CService service = null;
			if (null != pin.Access) // the pin is not for "direct data", and might need a storage service
			{
				// Generate a storage Service in the current batch, if needed
				// and generate appropriate pin configuration parameters (with credentials inserted)
				CService inputService = null, outputService = null;
				ComputationModuleRelease storage =
					(ComputationModuleRelease) _unitRegistry.GetUnitRelease(pin.Access.StorageUid);
				if (null != storage)
					service = task.GenerateService(storage);

				// Generate a data-copier job if the pin is "external" (these conditions hold only for "non-direct" pins)
				ComputationModuleRelease copyModule = null;
				DataPin inputPin = null, outputPin = null;
				bool isCopyOut = false;
				if (null != pin.Incoming && pin.Incoming.Source.TokenNo != pin.TokenNo)
				{
					inputPin = pin.Incoming.Source;
					outputPin = pin;
					outputService = service;
					// TODO - find service from the previous batch (if inter-batch copier service)
				}
				else if (null != pin.Outgoing && pin.Outgoing.Target.TokenNo < pin.TokenNo)
				{
					inputPin = pin;
					outputPin = pin.Outgoing.Target;
					inputService = service;
					isCopyOut = true;
				}

				if (null != inputPin && null != outputPin)
					copyModule =
						_unitRegistry.FindSystemUnit("data-copier", inputPin, outputPin);

				if (null != copyModule)
				{
					DataPin modulePin = copyModule.DeclaredPins.Find(p => p.Binding == DataBinding.RequiredStrong);
					modulePin.DataMultiplicity = inputPin.DataMultiplicity;
					modulePin.TokenMultiplicity = CMultiplicity.Single;
					modulePin.Access = inputPin.Access;

					modulePin = copyModule.DeclaredPins.Find(p => p.Binding >= DataBinding.Provided);
					modulePin.DataMultiplicity = outputPin.DataMultiplicity;
					modulePin.TokenMultiplicity = inputPin.DataMultiplicity > outputPin.DataMultiplicity
						? CMultiplicity.Multiple
						: CMultiplicity.Single;
					modulePin.Access = outputPin.Access;

					task.GenerateSystemJob(pin, copyModule, inputService, outputService, isCopyOut);
				}
			}

			task.GenerateDataToken(pin, service);
		}

		/// <summary>
		/// Translate a CAL app to a CAL-Executable specification
		/// </summary>
		/// <param name="strength"></param>
		/// <param name="release"></param>
		/// <param name="task"></param>
		/// <param name="addCopyInJobs"></param>
		/// <param name="pars"></param>
		/// <param name="outermost"></param>
		private void TranslateApp(UnitStrength strength, ComputationApplicationRelease release, CTaskWriter task,
			bool addCopyInJobs, List<UnitCallParameter> pars, bool outermost)
		{
			foreach (DataFlow flow in release.Flows) {
				bool isTargetModule = flow.Target is ComputedDataPin && ((ComputedDataPin) flow.Target).Call.Unit is ComputationModuleRelease;
				if (flow.Source is DeclaredDataPin)
				{
					// check if we have just created a new CJobBatch and the current flow's source pin is not "direct"
					if ((addCopyInJobs || UnitStrength.Weak == strength && isTargetModule) && !flow.Source.IsDirect)
						// set a new token number for the flow's target (prepares to create a copy-in job) 
						flow.Target.TokenNo = task.GenerateNextToken();
					else
						// copy the token number from the app's declared pin to the computed pin of the flow's target
						flow.Target.TokenNo = flow.Source.TokenNo;
				}
				else if (flow.Target is DeclaredDataPin)
				{
					if (!flow.Target.IsDirect && outermost)
						flow.Source.TokenNo = task.GenerateNextToken();
					else
						flow.Source.TokenNo = flow.Target.TokenNo;
				}
				else {
					flow.Source.TokenNo = task.GenerateNextToken();
					if (UnitStrength.Weak == strength && isTargetModule && (!flow.Target.IsDirect || !flow.Source.IsDirect))
						flow.Target.TokenNo = task.GenerateNextToken();
					else
						flow.Target.TokenNo = flow.Source.TokenNo;
				}
			}
			foreach (UnitCall call in release.Calls)
				TranslateUnitCall(strength,call,task,pars);
		}

		private string ConsoleString(){
			return "## SERVER.TASKMGR ## " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.ffff") + " ## ";
		}
	
	}
}