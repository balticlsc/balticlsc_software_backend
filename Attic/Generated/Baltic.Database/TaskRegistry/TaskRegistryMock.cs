/*
 * Created by SharpDevelop.
 * User: smialek
 * Date: 02.04.2020
 * Time: 11:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;

namespace Baltic.Database.TaskRegistry
{
	public class TaskRegistryMock
	{
		public List<CTask> StoredTasks;
	
		public TaskRegistryMock()
		{
			StoredTasks = new List<CTask>();
		}
	}
}