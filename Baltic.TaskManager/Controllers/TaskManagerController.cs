using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using AutoMapper;
using Baltic.Core.Utils;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;
using Baltic.DataModel.Types;
using Baltic.Engine.TaskManager;
using Baltic.TaskManager.Models;
using Baltic.Types.DataAccess;
using Baltic.Types.Models;
using Baltic.Web.Common;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Baltic.TaskManager.Controllers {
	
	[ApiController]
	[Authorize]
	[Route("task")]
	public class TaskManagerController : BalticController {

		private ITaskManagement _taskRegistry;
		private ITaskManager _taskManager;
		private IUnitProcessing _unitRegistry;
		private IMapper _mapper;
	
		private static DateTime _statusUpdateTime = DateTime.Now;
		
		public TaskManagerController(ITaskManagement taskRegistry, ITaskManager taskManager,
			IUnitProcessing unitRegistry, IMapper mapper, IUserSessionRoutine userSession):base(userSession)
		{
			_taskRegistry = taskRegistry;
			_taskManager = taskManager;
			_unitRegistry = unitRegistry;
			_mapper = mapper;
		}
		
		/// <summary>
		/// [InitiateTask] Initiate a CTask for a given ComputationApplicationRelease.
		/// </summary>
		/// <param name="releaseUid">The Uid of the ComputationApplicationRelease to be instantiated.</param>
		/// <param name="par">The TaskParameters to control CTask execution.</param>
		/// <returns>The Uid of the newly initiated CTask</returns>
		[HttpPut("initiate")]
		public IActionResult InitiateTask([FromQuery,Required] string releaseUid, [FromBody,Required] XTaskParameters par){
			try
			{
				if (null == par)
					return HandleError("Parameters are empty", HttpStatusCode.BadRequest);
				string userUid = UserName;
				//var taskParameters = _mapper.Map<TaskParameters>(par);
				var taskParameters = DBMapper.Map<TaskParameters>(par, new TaskParameters()
				{
					ClusterAllocation = "strong" == par.ClusterAllocation.ToLower() ? UnitStrength.Strong : UnitStrength.Weak
				});
				var taskUid = _taskManager.InitiateTask(releaseUid,taskParameters,userUid);
				if (string.IsNullOrEmpty(taskUid))
					return HandleError("Corrupted application release", HttpStatusCode.InternalServerError);
				return Ok(taskUid,"ok");
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [InjectDataSet] Inject a TaskDataSet into a specific DeclaredPin of an initiated CTask.
		/// </summary>
		/// <param name="taskUid">The Uid of the CTask.</param>
		/// <param name="pinUid">The Uid of the DeclaredPin to inject the TaskDataSet.</param>
		/// <param name="ds">The actual TaskDataSet record.</param>
		[HttpPut("injectData")]
		public IActionResult InjectDataSet([FromQuery] string taskUid, [FromQuery] string pinUid, [FromBody] XTaskDataSet ds)
		{
			Log.Debug("Attempt to process endpoint injectData");				
			try
			{
				if (null == ds)
					return HandleError("Data Set is empty", HttpStatusCode.BadRequest);
				var parameters = new CDataSet() {Values = ds.Values};
				var accessParameters = new CDataSet() {Values = ds.AccessValues};
				string userUid = UserName;
				Log.Debug($"Send to InjectDataSet: {taskUid}, {pinUid}, {parameters}, {accessParameters}, {userUid}");
				return InjectDataSet(taskUid,pinUid,parameters,accessParameters,userUid);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}
		
		/// <summary>
		/// [InjectDataSet] Inject a TaskDataSet into a specific DeclaredPin of an initiated CTask.
		/// </summary>
		/// <param name="taskUid">The Uid of the CTask.</param>
		/// <param name="pinUid">The Uid of the DeclaredPin to inject the TaskDataSet.</param>
		/// <param name="dataSetUid">The Uid of an existing TaskDataSet record.</param>
		[HttpPost("injectData")]
		public IActionResult InjectDataSet([FromQuery] string taskUid, [FromQuery] string pinUid, [FromQuery] string dataSetUid){
			try
			{
				var taskDataSet = _unitRegistry.GetDataSet(dataSetUid);
				if (null == taskDataSet)
					return HandleError("Data Set not found", HttpStatusCode.BadRequest);
				string userUid = UserName;
				return InjectDataSet(taskUid,pinUid,taskDataSet.Data,taskDataSet.AccessData,userUid);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		private IActionResult InjectDataSet(string taskUid, string pinUid, CDataSet parameters, CDataSet accessParameters, string userUid)
		{
			if (!CheckTaskOwnership(taskUid,userUid))
				return HandleError("Task not available for the user", HttpStatusCode.Unauthorized);
			var ret = _taskManager.InjectDataSet(taskUid, pinUid, parameters, accessParameters);
			if (-1 == ret)
				return HandleError("Task not found", HttpStatusCode.BadRequest);
			if (-2 == ret)
				return HandleError("Pin not found", HttpStatusCode.BadRequest);
			return Ok();
		}

		/// <summary>
		/// [InitiateAppTestTask] Initiate a CTask for a given ComputationApplication in test mode.
		/// </summary>
		/// <param name="appUid">The Uid of the ComputationApplication to be instantiated.</param>
		/// <param name="par">The TaskParameters to control CTask execution.</param>
		/// <returns>The Uid of the newly initiated CTask.</returns>
		[HttpPut("initiateTest")]
		public IActionResult InitiateAppTestTask([FromQuery] string appUid, [FromBody] XTaskParameters par){
			try
			{
				if (null == par)
					return HandleError("Parameters are empty", HttpStatusCode.BadRequest);
				string userUid = UserName;
				//var taskParameters = _mapper.Map<TaskParameters>(par);
				var taskParameters = DBMapper.Map<TaskParameters>(par, new TaskParameters());
				var taskUid = _taskManager.InitiateAppTestTask(appUid, taskParameters,userUid);
				if (string.IsNullOrEmpty(taskUid))
					return HandleError("Corrupted application", HttpStatusCode.InternalServerError);
				return Ok(taskUid,"ok");
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [FindTasks] Get data of CTask-s according to a TaskQuery.
		/// </summary>
		/// <param name="query">Query parameters (TaskQuery).</param>
		/// <returns>A list of CTask records.</returns>
		[HttpPost("list")]
		public IActionResult FindTasks([FromBody] XTaskQuery query){
            // TODO pagination 
            try
            {
	            if (null == query)
		            return HandleError("Query is empty", HttpStatusCode.BadRequest);
	            if ((DateTime.Now-_statusUpdateTime).TotalSeconds > 30)
	            {
		           _statusUpdateTime = DateTime.Now;
		           _taskManager.UpdateActiveTaskStatuses(); // TODO remove MOCK (to be called from Cron)
	            }
	            var taskQuery = DBMapper.Map<TaskQuery>(query, new TaskQuery());

	            string userUid = UserName;
	            taskQuery.UserUid = userUid;
	            
	            return FindTasks(taskQuery);
            }
            catch (Exception e)
            {
	            return HandleError(e);
            }
		}

		/// <summary>
		/// [AbortTask] Change the ComputationStatus of CTask to "Aborted".
		/// </summary>
		/// <param name="taskUid">The Uid of the CTask.</param>
		[HttpDelete("abort")]
		public IActionResult AbortTask([FromQuery] string taskUid) {
			try
			{
				string userUid = UserName;
				if (!CheckTaskOwnership(taskUid, userUid))
					return HandleError("Task not available for the user", HttpStatusCode.Unauthorized);
				if (0 == _taskManager.AbortTask(taskUid))
					return Ok();
				return HandleError("Task could not be aborted", HttpStatusCode.InternalServerError);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetTask] Get data of a given CTask.
		/// </summary>
		/// <param name="taskUid">The Uid of a CTask.</param>
		/// <returns>A single CTask record with all contained CJobBatches and CJobs.</returns>
		[HttpGet]
		public IActionResult GetTask([FromQuery] string taskUid) {
			try {
				if ((DateTime.Now-_statusUpdateTime).TotalSeconds > 30)
				{
					_statusUpdateTime = DateTime.Now;
					_taskManager.UpdateActiveTaskStatuses(); // TODO remove MOCK (to be called from Cron)
				}
				var cTask = _taskRegistry.GetTask(taskUid);
				if (null == cTask) return HandleError("Task not found", HttpStatusCode.BadRequest);
				string userUid = UserName;
				if (!CheckTaskOwnership(cTask, userUid))
					return HandleError("Task not available for the user", HttpStatusCode.Unauthorized);
				return Ok(MapTask(cTask));
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetBatch] Get data of a given CJobBatch.
		/// </summary>
		/// <param name="batchUid">The Uid of a CJobBatch.</param>
		/// <returns>A list of BatchExecution records for the given CJobBatch with all contained JobExecutions.</returns>
		[HttpGet("batch")]
		public IActionResult GetBatch([FromQuery] string batchUid){
			try
			{
				var cJobBatch = _taskRegistry.GetJobBatch(batchUid);
				if (null == cJobBatch) return HandleError("Batch not found", HttpStatusCode.BadRequest);
				string userUid = UserName;
				if (!CheckTaskOwnership(cJobBatch.Task, userUid))
					return HandleError("Batch not available for the user", HttpStatusCode.Unauthorized);
				return Ok(MapBatches(cJobBatch));
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetJob] Get data of a given CJob.
		/// </summary>
		/// <param name="jobUid">The Uid of a CJob.</param>
		/// <returns>A list of JobExecution records for the given CJob.</returns>
		[HttpGet("job")]
		public IActionResult GetJob([FromQuery] string jobUid){
			try
			{
				var cJob = _taskRegistry.GetJob(jobUid);
				if (null == cJob) return HandleError("Job not found", HttpStatusCode.BadRequest);
				string userUid = UserName;
				if (!CheckTaskOwnership(cJob.Batch.Task, userUid))
					return HandleError("Job not available for the user", HttpStatusCode.Unauthorized);
				return Ok(MapJobs(cJob));
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetResourceUsage] Get data of ResourceUsage-s for a given CCluster and UsageQuery. 
		/// </summary>
		/// <param name="query">Query parameters (UsageQuery) including the Uid of the CCluster.</param>
		/// <returns></returns>
		[HttpPost("usage")]
		public IActionResult GetResourceUsage([FromBody] XUsageQuery query){
            // TODO need pagination
            try
            {
	            if (null == query)
		            return HandleError("Query is empty", HttpStatusCode.BadRequest);
	            var usageQuery = DBMapper.Map<UsageQuery>(query, new UsageQuery());
	            var resourceUsages = _taskRegistry.GetResourceUsage(usageQuery);

	            var xResourceUsages = resourceUsages.Select(s =>
		            DBMapper.Map<XResourceUsage>(resourceUsages, new XResourceUsage())).ToList();

	            return Ok(xResourceUsages);
            }
            catch (Exception e)
            {
	            return HandleError(e);
            }
		}

		/// <summary>
		/// [GetSupportedResourceRange] Get ResourceRange for the given ComputationRelease.
		/// </summary>
		/// <param name="releaseUid">The Uid of the ComputationRelease</param>
		/// <returns>A single ResourceRange record.</returns>
		[HttpGet("range")]
		public IActionResult GetSupportedResourceRange([FromQuery] string releaseUid)
        {
	        try
	        {
		        var reservationRange = _taskManager.GetSupportedResourceRange(releaseUid);
		        if (null == reservationRange)
			        return HandleError("Release not found", HttpStatusCode.BadRequest);
		        
		        return Ok(new XReservationRange(reservationRange));
	        }
	        catch (Exception e)
	        {
		        return HandleError(e);
	        }
        }

		/// <summary>
		/// [GetPinsForTask] Get Pins that need to be initiated with TaskDataSets.
		/// </summary>
		/// <param name="taskUid">The Uid of the Task.</param>
		/// <returns>A list of Pin records.</returns>
		[HttpGet("pins")]
		public IActionResult GetPinsForTask([FromQuery] string taskUid)
		{
			try
			{
				string userUid = UserName;
				if (!CheckTaskOwnership(taskUid, userUid))
					return HandleError("Task not available for the user", HttpStatusCode.Unauthorized);
				var tokens = _taskRegistry.GetTaskDataTokens(taskUid);
				if (null == tokens)
					return HandleError("Task not found", HttpStatusCode.BadRequest);
				var result = tokens.Select(t => DBMapper.Map<XPin>(t, new XPin())).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetCompatibleClusters] Get Clusters that can run the given ComputationApplication.
		/// </summary>
		/// <param name="appReleaseUid">The Uid of the ComputationApplication.</param>
		/// <returns>A list of Clusters - just basic data.</returns>
		[HttpGet("clusters")]
		public IActionResult GetCompatibleClusters([FromQuery] string appReleaseUid)
		{
			List<CCluster> clusters = _taskManager.GetCompatibleClusters(appReleaseUid);
			if (clusters == null)
				return HandleError("Compatible clusters not found", HttpStatusCode.BadRequest);
			if (0 == clusters.Count)
				return HandleError("Compatible clusters not found");
			var xClusters = new List<XClusterBasic>();
			foreach (var cluster in clusters)
				xClusters.Add(DBMapper.Map<XClusterBasic>(cluster,new XClusterBasic()));
			return Ok(xClusters);
		}
		
		private bool CheckTaskOwnership(CTask task, string ownerUid)
		{
			return ownerUid == task?.OwnerUid;
		}
		
		private bool CheckTaskOwnership(string taskUid, string ownerUid)
		{
			return CheckTaskOwnership(_taskRegistry.GetTask(taskUid),ownerUid);
		}

		private IActionResult FindTasks(TaskQuery taskQuery)
		{
			var tasks = _taskRegistry.FindTasks(taskQuery);

			if (tasks != null)
			{
				var xTasks = new List<XTask>();
				foreach (var task in tasks)
					xTasks.Add(MapTask(task));
				return Ok(xTasks);
			}

			return HandleError("Tasks not found");
		}
		
		private XTask MapTask(CTask task) {
			var xTask = DBMapper.Map<XTask>(task, new XTask());
			DBMapper.Map<XTask>(task.Execution, xTask);
			if (null != task.Execution.Parameters) 
				xTask.Parameters = DBMapper.Map<XTaskParameters>(task.Execution.Parameters, new XTaskParameters());
			// TODO: substitute with - xTask.Batches = task.Batches.Select(b => MapBatches(b)).Aggregate().ToList();
			List<XBatch> lb = new List<XBatch>();
			foreach (CJobBatch batch in task.Batches)
				lb.AddRange(MapBatches(batch));
			xTask.Batches = lb;
			xTask.TokensReceived = xTask.Batches.Sum(b => b.TokensReceived);
			xTask.TokensProcessed = xTask.Batches.Sum(b => b.TokensProcessed);
			return xTask;
		}
		
		private IEnumerable<XBatch> MapBatches(CJobBatch batch)
		{
			return batch.BatchExecutions.Select(be => DBMapper.Map<XBatch>(
				be, DBMapper.Map<XBatch>(
					batch, new XBatch()
					{
						Jobs = MapJobs(be),
						TokensReceived = be.JobExecutions.Sum(j => j.TokensReceived),
						TokensProcessed = be.JobExecutions.Sum(j => j.TokensProcessed),
						Cluster = null != be.Cluster ? DBMapper.Map<XClusterBasic>(be.Cluster,new XClusterBasic()) : null
					})
				)).ToList();
		}
		
		private IEnumerable<XJob> MapJobs(BatchExecution exec){
			return exec.JobExecutions.Select(i => DBMapper.Map<XJob>(
				i, null != i.Job ? DBMapper.Map<XJob>(
					i.Job, new XJob()) : new XJob()
				{
					CallName = "copy (system)"
				}
			)).ToList();
		}

		private IEnumerable<XJob> MapJobs(CJob job){
			return job.JobExecutions.Select(i => DBMapper.Map<XJob>(
				i, DBMapper.Map<XJob>(
					job, new XJob())
				)).ToList();
		}
		
	}
}