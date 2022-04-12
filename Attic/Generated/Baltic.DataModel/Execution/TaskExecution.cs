///////////////////////////////////////////////////////////
//  TaskExecution.cs
//  Implementation of the Class TaskExecution
//  Generated by Enterprise Architect
//  Created on:      16-kwi-2020 15:45:17
//  Original author: smialek
///////////////////////////////////////////////////////////




using Baltic.DataModel.Execution;
namespace Baltic.DataModel.Execution {
	public abstract class TaskExecution : ExecutionRecord {

		public float ConsumedCredits;
		
		public TaskParameters Parameters;

		public TaskExecution(){

		}

		~TaskExecution(){

		}

	}//end TaskExecution

}//end namespace Baltic.DataModel.Execution