///////////////////////////////////////////////////////////
//  TaskMessageHelper.cs
//  Implementation of the Class TaskMessageHelper
//  Generated by Enterprise Architect
//  Created on:      10-mar-2020 15:44:00
//  Original author: smialek
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Text.RegularExpressions;
using Baltic.DataModel.CALExecutable;

namespace Baltic.Engine.TaskProcessor
{
	public class TaskMessageUtility
	{

		public TaskMessageUtility()
		{

		}

		~TaskMessageUtility()
		{

		}

		/// 
		/// <param name="tm"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public static string GetQueueNameFromToken(TokenMessage tm, List<int> depths, bool simple, int depthLevel){
			return GetQueueNameFromToken(tm,depths,simple,depthLevel,false);
		}
	
		///
		/// <param name="tm"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		/// <param name="negated"></param>
		public static string GetQueueNameFromToken(TokenMessage tm, List<int> depths, bool simple, int depthLevel, bool negated)
		{
			string ret = tm.TaskUid + "." + (negated?-tm.TokenNo:tm.TokenNo);
			for (int i = 0, j = 0; i < tm.SeqStack.Count && (!simple || j <depthLevel); i++)
				if (null == depths || !depths.Contains(tm.SeqStack.Count - i - 1)){
					ret += "." + tm.SeqStack[i].SeqUid
					           + "." + tm.SeqStack[i].No + ((tm.SeqStack[i].IsFinal)?"f":"");
					j++;
				}
			return ret;
		}
	
		/// 
		/// <param name="tm"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public static short RemoveSeqTokensFromToken(TokenMessage tm, List<int> depths, bool simple, int depthLevel){
			for (int i = 0, j = 0; i < tm.SeqStack.Count && (!simple || j <depthLevel); i++)
				if (null == depths || !depths.Contains(tm.SeqStack.Count - i - 1)){
					tm.SeqStack.RemoveAt(i);
					j++;
				}
			return 0;
		}

		///
		/// <param name="tm"></param>
		/// <param name="batchUid"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public static string GetBatchQueueNameFromToken(TokenMessage tm, string batchUid, List<int> depths, bool simple, int depthLevel)
		{
			string ret = tm.TaskUid + "." + batchUid;
			for (int i = 0, j = 0; i < tm.SeqStack.Count && (!simple || j <depthLevel); i++)
				if (null == depths || !depths.Contains(tm.SeqStack.Count - i - 1)){
					ret += "." + tm.SeqStack[i].SeqUid
					           + "." + tm.SeqStack[i].No + ((tm.SeqStack[i].IsFinal)?"f":"");
					j++;
				}
			return ret;
		}
	
		/// 
		/// <param name="name"></param>
		/// <param name="tokenNo"></param>
		public static string UpdateTokenInQueueName(string name, long tokenNo)
		{
			return (new Regex(@"\.[\w-]+\.?")).Replace(name, "." + tokenNo + ".", 1).TrimEnd(new []{'.'});
		}

		/// 
		/// <param name="queueUid"></param>
		/// <param name="batch"></param>
		public static string GetBatchQueueNameFromJob(string queueUid, CJobBatch batch){
			string ret = (new Regex(@"\.[\w-]+\.?")).Replace(queueUid, "." + batch.Uid + ".", 1).TrimEnd(new []{'.'});
			Match m = Regex.Match(ret,@"[\w-]+\.[\w-]+(\.[\w-]+\.[\w-]+)*");
			if (m.Groups[1].Captures.Count > batch.DepthLevel)
				return ret.Substring(0,m.Groups[1].Captures[batch.DepthLevel].Index);
			return ret;
		}
	
		/// 
		/// <param name="qname"></param>
		public static string getQueueNameForNegatedToken(string qname) {
			Group g = Regex.Match(qname,@"\.([\w-]+)\.?").Groups[1];
			return qname.Substring(0,g.Index)+-long.Parse(g.Value)+qname.Substring(g.Index+g.Length);
		}

	}
}//end TaskMessageUtility
