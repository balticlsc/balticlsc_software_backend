///////////////////////////////////////////////////////////
//  TaskManagerAdapter.cs
//  Implementation of the Class TaskManagerAdapter
//  Generated by Enterprise Architect
//  Created on:      02-kwi-2020 16:30:13
//  Original author: smialek
///////////////////////////////////////////////////////////



using System.Collections.Generic;
using Baltic.Engine.TaskManager;
using Baltic.Database.TaskRegistry;
using Baltic.Service.Models;

namespace Baltic.Service.Controllers
{
	public class TaskManagerController{

		private ITaskManagement TaskRegistry;
		private ITaskManager TaskManager;

		public TaskManagerController(){

		}

		~TaskManagerController(){

		}

		/// 
		/// <param name="releaseUid"></param>
		/// <param name="strength"></param>
		public string InitiateTask(string releaseUid, string strength){

			return "";
		}

		/// 
		/// <param name="taskUid"></param>
		/// <param name="pinUid"></param>
		/// <param name="ds"></param>
		public void InjectDataSet(string taskUid, string pinUid, XDataSet ds){

		}

		/// 
		/// <param name="appUid"></param>
		/// <param name="binding"></param>
		public string InitiateAppTestTask(string appUid, string strength){

			return "";
		}

		/// 
		/// <param name="query"></param>
		public IEnumerable<XTask> FindTasks(XTaskQuery query){
			
			return null;
		}

		/// 
		/// <param name="taskUid"></param>
		public void AbortTask(string taskUid){

		}

		/// 
		/// <param name="taskUid"></param>
		public XTask GetTask(string taskUid){

			return null;
		}

		/// 
		/// <param name="batchUid"></param>
		public XBatch GetBatch(string batchUid){

			return null;
		}

		/// 
		/// <param name="jobUid"></param>
		public XJob GetJob(string jobUid){

			return null;
		}

	}
}//end TaskManagerAdapter