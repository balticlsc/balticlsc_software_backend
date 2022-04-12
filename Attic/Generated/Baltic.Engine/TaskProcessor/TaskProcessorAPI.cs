///////////////////////////////////////////////////////////
//  TaskProcessorAPI.cs
//  Implementation of the Interface TaskProcessorAPI
//  Generated by Enterprise Architect
//  Created on:      02-kwi-2020 15:05:39
//  Original author: smialek
///////////////////////////////////////////////////////////


using Baltic.DataModel.CALExecutable;

namespace Baltic.Engine.TaskProcessor
{
	public interface TaskProcessorAPI {

		/// 
		/// <param name="tm"></param>
		short putTokenMessage(TokenMessage tm);
	}
}//end TaskProcessorAPI