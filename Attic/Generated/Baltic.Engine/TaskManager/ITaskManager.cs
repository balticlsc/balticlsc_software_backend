///////////////////////////////////////////////////////////
//  ITaskManager.cs
//  Implementation of the Interface ITaskManager
//  Generated by Enterprise Architect
//  Created on:      02-kwi-2020 16:30:47
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;

namespace Baltic.Engine.TaskManager
{
	public interface ITaskManager {
		
		/// 
		/// <param name="releaseUid"></param>
		/// <param name="par"></param>
		string InitiateTask(string releaseUid, TaskParameters par);

		/// 
		/// <param name="appUid"></param>
		/// <param name="par"></param>
		string InitiateAppTestTask(string appUid, TaskParameters par);
	
		/// 
		/// <param name="taskUid"></param>
		/// <param name="pinUid"></param>
		/// <param name="ds"></param>
		void InjectDataSet(string taskUid, string pinUid, CDataSet ds);

	}
}//end ITaskManager