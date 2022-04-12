/*
 * Created by SharpDevelop.
 * User: smialek
 * Date: 20.04.2020
 * Time: 12:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;


namespace Baltic.Database.Entities
{
	
	public class CJobBatchImpl : CJobBatch
	{
		public string TaskUid;
		
		private List<CJob> _Jobs;
		public override List<CJob> Jobs{
			get{
				if (null == _Jobs) {
					// TODO pobrać Jobs z bazy danych, dla których job.BatchUid==Uid
				}
				return _Jobs;
			}
			set{
				_Jobs = value;
			}
		}
		private List<CDataToken> _Tokens;
		public override List<CDataToken> Tokens{
			get{
				if (null == _Tokens) {
					// TODO pobrać Tokens z bazy danych, dla których token.BatchUid==Uid
				}
				return _Tokens;
			}
			set{
				_Tokens = value;
			}
		}
		private List<BatchExecution> _BatchInstances;
		public override List<BatchExecution> BatchInstances{
			get{
				if (null == _BatchInstances) {
					// TODO pobrać BatchInstances z bazy danych, dla których instance.BatchUid==Uid
				}
				return _BatchInstances;
			}
			set{
				_BatchInstances = value;
			}
		}
		
		public CJobBatchImpl()
		{
		}
		
		public override string ToString() {
			string ret = "JobBatch " + Uid + " (depth_level=" + DepthLevel + ")\n";
			foreach (CDataToken token in Tokens)
				ret = ret + "\t" + Regex.Replace(token.ToString(), @"\n\t", "\n\t\t");
			foreach (CJob job in Jobs)
				ret = ret + "\t" + Regex.Replace(job.ToString(), @"\n\t", "\n\t\t");
			return ret;
		}
	}
}
