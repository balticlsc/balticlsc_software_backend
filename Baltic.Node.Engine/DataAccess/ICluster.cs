using System;
using System.Collections.Generic;
using Baltic.DataModel.Execution;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Node.Engine.DataAccess
{
	public interface ICluster {
		/// 
		/// <param name="build"></param>
		/// <param name="jobInstanceUid"></param>
		/// <param name="batchInstanceUid"></param>
		IJob StartJob(BalticModuleBuild build, string jobInstanceUid, string batchInstanceUid);

		/// 
		/// <param name="batchInstanceUid"></param>
		/// <param name="quota"></param>
		/// <param name="serviceBuilds"></param>
		short StartBatch(string batchInstanceUid, ResourceReservation quota, List<BalticModuleBuild> serviceBuilds);

		/// <summary>
		/// string - jobUid
		/// </summary>
		/// <param name="batchInstanceId"></param>
		Dictionary<string,ResourceUsage> GetCurrentResourceUsage(string batchInstanceId);

		/// 
		/// <param name="batchInstanceId"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		Dictionary<string,ResourceUsage> GetRangeResourceUsage(string batchInstanceId, DateTime startTime, DateTime endTime);

		/// 
		/// <param name="jobInstanceUid"></param>
		/// <param name="batchInstanceUid"></param>
		short StopJob(string jobInstanceUid, string batchInstanceUid);

		/// 
		/// <param name="batchInstanceId"></param>
		short StopBatch(string batchInstanceId);
	}

}