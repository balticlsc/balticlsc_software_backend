using System;
using System.Collections.Generic;
using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.CALMessages {
	public class QueueId : QueueIdentifiable
	{

		/// <summary>
		/// for TokenMessage queues - tokenNo
		/// for BatchMessage queues - batchUid
		/// for JobMessage queues - batchUid.jobUid
		/// </summary>
		private string _familyId;

		public override string FamilyId => _familyId;
		public QueueId(string taskUid, string familyId, SeqTokenStack queueSeqStack)
		{
			TaskUid = taskUid;
			_familyId = familyId;
			QueueSeqStack = queueSeqStack;
		}

		/// 
		/// <param name="qi"></param>
		public QueueId(QueueId qi)
		{
			TaskUid = qi.TaskUid;
			_familyId = qi.FamilyId;
			QueueSeqStack = qi.QueueSeqStack.Copy();
		}

		/// 
		/// <param name="qi"></param>
		/// <param name="targetDepth"></param>
		public QueueId(QueueId qi, int targetDepth)
		{
			TaskUid = qi.TaskUid;
			_familyId = qi.FamilyId;
			QueueSeqStack = qi.QueueSeqStack.Copy(new List<int>(), true, targetDepth);
		}

		public QueueId(Message msg)
		{
			TaskUid = msg.TaskUid;
			_familyId = msg.FamilyId;
			QueueSeqStack = msg.QueueSeqStack.Copy();
		}
		
		/// 
		/// <param name="tm"></param>
		/// <param name="familyId"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public QueueId(TokenMessage tm, string familyId, List<int> depths, bool simple, int depthLevel)
		{
			TaskUid = tm.TaskUid;
			_familyId = familyId;
			QueueSeqStack = tm.QueueSeqStack.Copy(depths, simple, depthLevel);
		}
		
		/// 
		/// <param name="source"></param>
		/// <param name="familyId"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public QueueId(QueueId source, string familyId, List<int> depths, bool simple, int depthLevel)
		{
			TaskUid = source.TaskUid;
			_familyId = familyId;
			QueueSeqStack = source.QueueSeqStack.Copy(depths, simple, depthLevel);
		}
		
		/// 
		/// <param name="tm"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public QueueId(TokenMessage tm, List<int> depths, bool simple, int depthLevel)
		{
			TaskUid = tm.TaskUid;
			_familyId = tm.TokenNo.ToString();
			QueueSeqStack = tm.QueueSeqStack.Copy(depths, simple, depthLevel);
		}

		/// <summary>
		/// Create a queue for JobMessages
		/// </summary>
		/// <param name="tokenQueue"></param>
		/// <param name="batch"></param>
		/// <param name="jobUid"></param>
		public QueueId(QueueId tokenQueue, CJobBatch batch, string jobUid)
		{
			TaskUid = tokenQueue.TaskUid;
			_familyId = batch.Uid + "." + jobUid;
			QueueSeqStack = tokenQueue.QueueSeqStack.Copy(new List<int>(), true, batch.DepthLevel);
		}
		
		/// <summary>
		/// Create a queue for BatchMessages
		/// </summary>
		/// <param name="tokenQueue"></param>
		/// <param name="batch"></param>
		public QueueId(QueueId tokenQueue, CJobBatch batch)
		{
			TaskUid = tokenQueue.TaskUid;
			_familyId = batch.Uid;
		}

		/// 
		/// <param name="str"></param>
		public static QueueId Parse(string str)
		{
			string[] parts = str.Split(new []{'.'});
			if (2 > parts.Length)
				return null;
			string familyId = 0 == parts.Length % 2 ? parts[1] : parts[1] + "." + parts[2];
			SeqTokenStack seqStack = new SeqTokenStack();
			for (int i = 2 + parts.Length % 2; i < parts.Length; i += 2)
			{
				bool isFinal = parts[i + 1].EndsWith('f');
				seqStack.Push(new SeqToken()
				{
					SeqUid = parts[i],
					IsFinal = isFinal,
					No = long.Parse(isFinal ? parts[i+1].Substring(0,parts[i+1].Length-1) : parts[i+1])
				});
			}
			return new QueueId(parts[0],familyId,seqStack);
		}

		/// 
		/// <param name="qi1"></param>
		/// <param name="qi2"></param>
		public static bool operator ==(QueueId qi1, QueueId qi2)
		{
			if (qi1?.TaskUid != qi2?.TaskUid || qi1?.FamilyId != qi2?.FamilyId || qi1?.QueueSeqStack != qi2?.QueueSeqStack)
				return false;
			return true;
		}
		
		/// 
		/// <param name="qi1"></param>
		/// <param name="qi2"></param>
		public static bool operator !=(QueueId qi1, QueueId qi2)
		{
			if (qi1?.TaskUid != qi2?.TaskUid || qi1?.FamilyId != qi2?.FamilyId || qi1?.QueueSeqStack != qi2?.QueueSeqStack)
				return true;
			return false;
		}
		
		public QueueId Copy()
		{
			return new QueueId(this);
		}
		
		/// 
		/// <param name="newFamilyId"></param>
		public QueueId Copy(string newFamilyId)
		{
			QueueId ret = new QueueId(this);
			ret._familyId = newFamilyId;
			return ret;
		}

		/// 
		/// <param name="targetDepth"></param>
		public QueueId Copy(int targetDepth){
			return new QueueId(this, targetDepth);
		}

		/// 
		/// <param name="familyId"></param>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public QueueId Copy(string familyId, List<int> depths, bool simple, int depthLevel)
		{
			return new QueueId(this,familyId,depths, simple,depthLevel);
		}

		protected bool Equals(QueueIdentifiable other)
		{
			return TaskUid == other.TaskUid && QueueSeqStack == other.QueueSeqStack && FamilyId == other.FamilyId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((QueueIdentifiable) obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(TaskUid, QueueSeqStack, FamilyId);
		}

		public override string ToString()
		{
			string ret = TaskUid + "." + FamilyId;
			foreach (SeqToken st in QueueSeqStack)
				ret += "." + st.SeqUid + "." + st.No + (st.IsFinal ? "f" : "");
			return ret;
		}
	}
}