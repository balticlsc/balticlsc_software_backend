using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Baltic.Core.Utils;
using Baltic.DataModel.Resources;
using Baltic.DataModel.Types;
using Baltic.NetworkManager.Models;
using Baltic.Types.DataAccess;
using Baltic.Web.Common;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Baltic.NetworkManager.Controllers {
	[ApiController]
	[Authorize]
	[Route("resource")]
	public class NetworkManagerController : BalticController
	{

		private INetworkManagement _manager;

		public NetworkManagerController(IConfiguration config, INetworkManagement manager, IUserSessionRoutine userSession):base(userSession)
		{
			_manager = manager;
		}

		/// <summary>
		/// [CreateCluster] Create a new CCluster record.
		/// </summary>
		/// <param name="cluster">Data of the new CCluster record.</param>
		/// <returns>New CCluster Uid.</returns>
		[HttpPut]
		public IActionResult CreateCluster([FromBody] XCluster cluster)
		{
			try
			{
				CCluster cCluster = DBMapper.Map<CCluster>(cluster, new CCluster());
				// TODO - map also the lists embedded in "cluster"
				string clusterUid = _manager.CreateCluster(cCluster);
				if (null != clusterUid)
					return Ok(clusterUid,"ok");
				return HandleError("Cluster node could not be created", HttpStatusCode.BadRequest);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [UpdateCluster] Update an existing CCluster with changed data.
		/// </summary>
		/// <param name="cluster">Data of the CCluster to be updated.</param>
		[HttpPost]
		public IActionResult UpdateCluster([FromBody] XCluster cluster){
			try
			{
				CCluster cCluster = DBMapper.Map<CCluster>(cluster, new CCluster());
				// TODO - map also the lists embedded in "cluster"
				if (0 == _manager.UpdateCluster(cCluster))
					return Ok();
				return HandleError("Cluster node could not be updated", HttpStatusCode.InternalServerError);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [UpdateClusterStatus] Update an existing CCLuster with new ClusterStatus.
		/// </summary>
		/// <param name="clusterUid">The Uid of an existing CCluster.</param>
		/// <param name="status">The new ClusterStatus of the CCLuster.</param>
		[HttpPost("status")]
		public IActionResult UpdateClusterStatus([FromQuery] string clusterUid, [FromQuery] string status)
		{
			try
			{
				if (!Enum.TryParse(status, out ClusterStatus clusterStatus))
					return HandleError("Improper cluster status literal", HttpStatusCode.BadRequest);
				if (0 == _manager.UpdateClusterStatus(clusterUid, clusterStatus))
					return Ok();
				return HandleError("Cluster node status could not be updated", HttpStatusCode.InternalServerError);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [FindClusters] Get data of CCluster-s according to a ClusterQuery.
		/// </summary>
		/// <param name="query">Query parameters (ClusterQuery).</param>
		/// <returns>A list of CCLuster records.</returns>
		[HttpPost("list")]
		public IActionResult FindClusters([FromQuery] XClusterQuery query)
		{
			try
			{
				if (null == query)
					return HandleError("Query is empty", HttpStatusCode.BadRequest);
				var clusterQuery = DBMapper.Map<ClusterQuery>(query, new ClusterQuery());
				string userUid = UserName;
				var clusters = _manager.FindClusters(clusterQuery);
				List<XCluster> result = clusters.Select(c => DBMapper.Map<XCluster>(c,new XCluster()
				{
					// TODO - map also the lists embedded in "cluster"
				})).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetUserClusters] Get data of CClusters managed by the current user.
		/// </summary>
		/// <returns>A list of CCluster records.</returns>
		[HttpGet("shelf")]
		public IActionResult GetUserClusters()
		{
			try
			{
				string userUid = UserName;
				List<CCluster> userClusters = _manager.GetUserClusters(userUid).ToList();
				List<XCluster> result = userClusters.Select(c => DBMapper.Map<XCluster>(c,new XCluster()
				{
					// TODO - map also the lists embedded in "cluster"
				})).ToList();
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetCluster] Get data of a given CCluster.
		/// </summary>
		/// <param name="clusterUid">The Uid of a CCluster.</param>
		/// <returns>A single CCluster record.</returns>
		[HttpGet()]
		public IActionResult GetCluster([FromQuery] string clusterUid)
		{
			try
			{
				CCluster cluster = _manager.GetCluster(clusterUid);
				if (null == cluster)
					return HandleError("Cluster not found", HttpStatusCode.BadRequest);
				XCluster result = DBMapper.Map<XCluster>(cluster, new XCluster());
				// TODO - map also the lists embedded in "cluster"
				return Ok(result);
			}
			catch (Exception e)
			{
				return HandleError(e);
			}
		}

		/// <summary>
		/// [GetMachine] Get data of a given CMachine.
		/// </summary>
		/// <param name="machineUid">The Uid af a CMachine.</param>
		/// <returns>A single CMachine record.</returns>
		[HttpGet("machine")]
		public IActionResult GetMachine([FromQuery] string machineUid){

			return Ok(null,"ok");
		}

		/// <summary>
		/// [GetBatchesForResource] Get data of CBatchExecution-s run on a given CCluster.
		/// </summary>
		/// <param name="query">Query parameters (ClusterBatchQuery).</param>
		/// <returns>A list of CBatchExecution records.</returns>
		[HttpPost("batch/list")]
		public IActionResult GetBatchesForResource([FromBody] XClusterBatchQuery query){

			return Ok(null,"ok");
		}

	}
}