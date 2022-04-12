using System.Collections.Generic;
using System.Linq;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Types;
using Baltic.Types.Entities;

namespace Baltic.Engine.TaskManager
{
	public class CTaskWriter {
		public CTask Task { get; set; }
		private long _tokenNo;
		private CJobBatch _curBatch;
		private CJob _curJob;
		private IDataModelImplFactory _factory;
		private int _batchCounter;

		/// 
		/// <param name="ct"></param>
		/// <param name="dmf"></param>
		public CTaskWriter(CTask ct, IDataModelImplFactory dmf){
			Task = ct;
			_tokenNo = 1;
			_curBatch = null;
			_curJob = null;
			_factory = dmf;
			_batchCounter = 1;
		}

		public long GenerateNextToken(){
			long ret = _tokenNo;
			_tokenNo = _tokenNo + 1;
			return ret;
		}

		public void GenerateJobBatch(){
			CJobBatch batch = _factory.CreateCJobBatch();
			Task.Batches.Add(batch);
			batch.Task = Task;
			_curBatch = batch;
			_curJob = null;
			batch.SerialNo = _batchCounter;
			_batchCounter++;
		}
		
		/// <summary>
		/// Generates a new job within the current job batch
		/// </summary>
		/// <param name="unit">The unit release that defines the job</param>
		/// <param name="callName">The name of the unit call that caused the job generation</param>
		/// <param name="jobParams">The parameters to be used as environment variables or config files</param>
		public void GenerateJob(ComputationModuleRelease unit, string callName, List<UnitCallParameter> jobParams = null){
			CJob job = _factory.CreateCJob();
			job.ModuleReleaseUid = unit.Uid;
			job.Image = unit.Image;
			job.Command = unit.Command;
			job.CommandArguments = unit.CommandArguments.Select(ca => new string(ca)).ToList();
			job.Multiplicity = 1;
			job.Batch = _curBatch;
			job.CallName = callName;
			job.IsMultitasking = unit.IsMultitasking;
			if (null == jobParams)
				job.Parameters = unit.Parameters.Select(p => new CParameter(p)).ToList();
			else
				job.Parameters = jobParams.Select(p => new CParameter(p)).ToList();
			
			_curBatch.Jobs.Add(job);
			_curJob = job;
		}
		
		/// <summary>
		/// Generates a new infrastructural service (usually - a storage service) within the current job batch
		/// if not yet generated previously.
		/// </summary>
		/// <param name="serviceModule">The unit (module) release that defines the service</param>
		public CService GenerateService(ComputationModuleRelease serviceModule)
		{
			CService service = _curBatch.Services.Find(s => s.ModuleReleaseUid == serviceModule.Uid);
			if (null != service)
				return service;
			
			// Create the service and set its basic attributes
			service = _factory.CreateCService();
			service.ModuleReleaseUid = serviceModule.Uid;
			service.Image = serviceModule.Image;
			service.Command = serviceModule.Command;
			service.CommandArguments = serviceModule.CommandArguments.Select(ca => new string(ca)).ToList();
			service.Batch = _curBatch;

			// Prepare a list of the service's 'credentials'
			// (except for the "Host" credential which is not allowed to be set by the developer)
			service.CredentialParameters = serviceModule.CredentialParameters.FindAll(
				cp => "Host - RevertBackToHost" != cp.AccessCredentialName).Select(
				cp => new CredentialParameter(cp)).ToList();
			// TODO - revert back to "Host" (mocked for temporary FTP service, that sets an external Host)

			// Prepare a list of the service's 'parameters' (environment variables or config files)
			service.Parameters = serviceModule.Parameters.Select(p => new CParameter(p)).ToList();

			_curBatch.Services.Add(service);
			return service;
		}
		
		public void GenerateSystemJob(ComputedDataPin pin, ComputationModuleRelease copyUnit,
			CService inputService, CService outputService, bool isCopyOut)
		{
			CJob previousJob = _curJob;
			GenerateJob(copyUnit,copyUnit.Unit.Name + 
			                     " (" + pin.Incoming?.Source.TokenNo + pin.Outgoing?.Target.TokenNo + ")");
			foreach (DeclaredDataPin copyPin in copyUnit.DeclaredPins)
			{
				CService service = "input" == copyPin.Name ? inputService : outputService;
				// Consider the pin for which the copy module has to be generated
				// If this is a "required" (input) pin...
				if (null != pin.Incoming)
					copyPin.TokenNo = "output" == copyPin.Name ? pin.TokenNo : 
						(pin.Incoming.Source is DeclaredDataPin dataPin ? dataPin.PrecedingTokenNo : 
							pin.Incoming.Source.TokenNo);
				// If this is a "provided" (output) pin...
				else if (null != pin.Outgoing)
					copyPin.TokenNo = "input" == copyPin.Name ? pin.TokenNo : pin.Outgoing.Target.TokenNo;
				GenerateDataToken(copyPin, service,true, isCopyOut);
			}
			_curJob = previousJob;
		}

		/// <param name="pin"></param>
		/// <param name="service"></param>
		/// <param name="b"></param>
		/// <param name="system"></param>
		/// <param name="isCopyOut"></param>
		public void GenerateDataToken(DataPin pin, CService service = null, bool system = false, bool isCopyOut = false)
		{
			DataPin previousPin = pin.Incoming?.Source, nextPin = pin.Outgoing?.Target;
			CDataToken dt = _factory.CreateCDataToken();

			// for this pin we need to generate a "Required" batch data token
			if (null != _curBatch && null == _curJob && null != previousPin)
				dt.TokenNo = previousPin.TokenNo;
			else if (null != _curBatch && null == _curJob && nextPin is DeclaredDataPin) // TODO - check if "outermost app" checking is needed
				dt.TokenNo = nextPin.TokenNo;
			else
				dt.TokenNo = pin.TokenNo;

			dt.PinName = pin.Name;
			if (DataBinding.ProvidedExternal == pin.Binding && null != _curJob)
			{
				if (!system)
					dt.Binding = DataBinding.Provided;
				else
					dt.Binding = DataBinding.RequiredStrong;
			}
			else if (system && DataBinding.Provided == pin.Binding && isCopyOut)
				dt.Binding = DataBinding.ProvidedExternal;
			else
				dt.Binding = pin.Binding;

			dt.DataMultiplicity = pin.DataMultiplicity;
			dt.TokenMultiplicity = pin.TokenMultiplicity;
			dt.DataType = pin.Type?.Name;
			dt.AccessType = pin.Access?.Name;
			dt.Direct = pin.IsDirect;
			dt.Service = service;
			
			if (pin is ComputedDataPin) {
				dt.PinUid = ((ComputedDataPin) pin).Declared.Uid;
				PinGroup pg = ((ComputedDataPin) pin).Group;
				if (null != pg)
					dt.Depths.AddRange(pg.Depths);
			} else
				dt.PinUid = pin.Uid;
			
			if (null == _curBatch)
				Task.Tokens.Add(dt);
			else if (null == _curJob)
				_curBatch.Tokens.Add(dt);
			else 
				_curJob.Tokens.Add(dt);
		}

		/// <summary>
		/// Sets the depth levels of job batches as a post-processing action
		/// </summary>
		public void SetDepthLevelsInTask()
		{
			// TODO consider also special SeqTokens
			if (1 == Task.Batches.Count)
			{
				Task.Batches[0].DepthLevel = 0;
				return;
			}
			// iterate through all the tokens that initiate a task
			foreach (CDataToken token in Task.Tokens.FindAll(t => DataBinding.RequiredStrong == t.Binding))
				SetBatchDepthLevel(token.TokenNo,0);
		}

		private void SetBatchDepthLevel(long tokenNo, int currentDepthLevel)
		{
			// find the batch that is started by this data token
			CJobBatch batch = Task.Batches.Find(b => b.Tokens.Exists(t => t.TokenNo == tokenNo && DataBinding.RequiredStrong == t.Binding));
			if (null == batch || -1 != batch.DepthLevel) // already processed (not -1)?
				return;

			CDataToken batchToken = batch.Tokens.Find(t => t.TokenNo == tokenNo);
			if (batch.IsSimple)
				batch.DepthLevel = 0;
			else
				batch.DepthLevel = currentDepthLevel - batchToken.Depths.Count;
			
			// iterate through all the tokens that initiate an initial batch
			foreach (CDataToken jobToken in batch.Tokens.FindAll(t => DataBinding.RequiredStrong == t.Binding))
				ProcessJobDepthLevel(jobToken.TokenNo,batch,currentDepthLevel);
		}

		private void ProcessJobDepthLevel(long tokenNo, CJobBatch batch, int currentDepthLevel)
		{
			// find the job that is started by this data token
			CJob job = batch.Jobs.Find(j => j.Tokens.Exists(t => t.TokenNo == tokenNo && DataBinding.RequiredStrong == t.Binding));
			if (null == job)
				return;
			CDataToken jobToken = job.Tokens.Find(t => t.TokenNo == tokenNo);
			int nextDepthLevel = currentDepthLevel - jobToken.Depths.Count + (job.IsSplitter ? 1 : 0);
			foreach (CDataToken token in job.Tokens.FindAll(t => DataBinding.Provided == t.Binding))
			{
				if (batch.Jobs.Exists(j => j.Tokens.Exists(t => t.TokenNo == token.TokenNo && DataBinding.RequiredStrong == t.Binding)))
					ProcessJobDepthLevel(token.TokenNo,batch,nextDepthLevel);
				else
					SetBatchDepthLevel(token.TokenNo,nextDepthLevel);
			}
		}
		
	}
}