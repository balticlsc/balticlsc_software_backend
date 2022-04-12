using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;
using Baltic.DataModel.Resources;

namespace Baltic.Engine.TaskManager
{
	public interface ITaskManager  {
		/// 
		/// <param name="releaseUid"></param>
		/// <param name="par"></param>
		/// <param name="userUid"></param>
		string InitiateTask(string releaseUid, TaskParameters par, string userUid);

		/// 
		/// <param name="appUid"></param>
		/// <param name="par"></param>
		/// <param name="userUid"></param>
		string InitiateAppTestTask(string appUid, TaskParameters par, string userUid);

		/// 
		/// <param name="taskUid"></param>
		/// <param name="pinUid"></param>
		/// <param name="data"></param>
		/// <param name="accessData"></param>
		short InjectDataSet(string taskUid, string pinUid, CDataSet data, CDataSet accessData);

		/// 
		/// <param name="releaseUid"></param>
		ResourceReservationRange GetSupportedResourceRange(string releaseUid);

		short UpdateActiveTaskStatuses();

		/// 
		/// <param name="taskUid"></param>
		short AbortTask(string taskUid);

		///
		/// <param name="taskUid"></param>
		short ArchiveTask(string taskUid);
		
		/// 
		/// <param name="taskUid"></param>
		List<CCluster> GetCompatibleClusters(string taskUid);

	}
}