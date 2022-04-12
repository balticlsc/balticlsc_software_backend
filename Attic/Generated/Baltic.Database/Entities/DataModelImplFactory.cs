/*
 * Created by SharpDevelop.
 * User: smialek
 * Date: 20.04.2020
 * Time: 13:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;

namespace Baltic.Database.Entities
{
	/// <summary>
	/// Description of DataModelImplFactory.
	/// </summary>
	public class DataModelImplFactory : IDataModelImplFactory
	{
		public DataModelImplFactory()
		{
		}

		public CJobBatch CreateCJobBatch(){
			return (CJobBatch) new CJobBatchImpl {
				Uid = Guid.NewGuid().ToString(),
				Jobs = new List<CJob>(),
				Tokens = new List<CDataToken>(),
				BatchInstances = new List<BatchExecution>()
			};

		}

		public CTask CreateCTask(){
			return (CTask) new CTaskImpl {
				Uid = Guid.NewGuid().ToString(),
				Batches = new List<CJobBatch>(),
				Tokens = new List<CDataToken>(),
				Execution = new TaskExecutionImpl()
			};
		}

		public CJob CreateCJob(){
			return (CJob) new CJobImpl {
				Uid = Guid.NewGuid().ToString(),
				Tokens = new List<CDataToken>(),
				JobInstances = new List<JobExecution>()
			};
		}

		public CDataToken CreateCDataToken(){
			return (CDataToken) new CDataTokenImpl {
				Uid = Guid.NewGuid().ToString(),
				Depths = new List<int>()
			};
		}
		
		public BatchExecution CretateBatchExecution(){
			return (BatchExecution) new BatchExecutionImpl();
		}
		
	}
}
