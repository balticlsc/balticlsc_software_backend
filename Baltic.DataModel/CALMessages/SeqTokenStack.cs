using System.Collections;
using System.Collections.Generic;

namespace Baltic.DataModel.CALMessages {
	public class SeqTokenStack : IEnumerable<SeqToken>
	{
		private List<SeqToken> _seqTokens;

		public SeqToken this[int index]
		{
			get => _seqTokens[_seqTokens.Count - index - 1];
		}

		/// 
		/// <param name="sts1"></param>
		/// <param name="sts2"></param>
		public static bool operator ==(SeqTokenStack sts1, SeqTokenStack sts2)
		{
			if (sts1?.Count != sts2?.Count)
				return false;
			for (int i = 0; i < sts1?.Count; i++)
				if (sts1[i] != sts2[i])
					return false;
			return true;
		}
		
		/// 
		/// <param name="sts1"></param>
		/// <param name="sts2"></param>
		public static bool operator !=(SeqTokenStack sts1, SeqTokenStack sts2)
		{
			if (sts1?.Count != sts2?.Count)
				return true;
			for (int i = 0; i < sts1?.Count; i++)
				if (sts1[i] != sts2[i])
					return true;
			return false;
		}

		public int Count => _seqTokens.Count;

		public SeqTokenStack()
		{
			_seqTokens = new List<SeqToken>();
		}

		public SeqTokenStack(SeqTokenStack sts)
		{
			_seqTokens = new List<SeqToken>();
			foreach (SeqToken st in sts)
				_seqTokens.Add(st.Copy());
		}

		/// 
		/// <param name="seqTokens"></param>
		public SeqTokenStack(List<SeqToken> seqTokens)
		{
			_seqTokens = seqTokens;
		}

		/// 
		/// <param name="st"></param>
		public void Push(SeqToken st)
		{
			_seqTokens.Add(st);
		}

		/// 
		/// <param name="i"></param>
		public void RemoveAt(int i)
		{
			_seqTokens.RemoveAt(_seqTokens.Count - i - 1);
		}

		public bool IsEmpty()
		{
			return 0 == _seqTokens.Count;
		}

		public SeqTokenStack Copy()
		{
			return new SeqTokenStack(this);
		}

		public SeqTokenStack Copy(List<int> depths, bool simple, int depthLevel)
		{
			SeqTokenStack ret = new SeqTokenStack();
			
			// The following loop does 2 things:
			// 1. omit those elements that are indexed by "depths"
			// 2a. (simple == true): copy no more than "depthLevel" elements (the omitted elements don't count)
			// 2b. (simple == false): copy elements irrespective of "depthLevel"
			for (int i = Count - 1, j = 0; 0 <= i && (!simple || j < depthLevel); i--)
				if (null == depths || !depths.Contains(i))
				{
					ret.Push(this[i].Copy());
					j++;
				}
			return ret;
		}

		/// 
		/// <param name="index"></param>
		/// <param name="count"></param>
		public SeqTokenStack SubStack(int index, int count)
		{
			List<SeqToken> ret = _seqTokens.GetRange(_seqTokens.Count - index - count, count);
			ret.Reverse();
			return new SeqTokenStack(ret);
		}
		
		/// 
		/// <param name="index"></param>
		public SeqTokenStack SubStack(int index)
		{
			return SubStack(index, Count - index);
		}
		
		/// <summary>
		/// Split the stack into two parts: a) leave the SeqTokens that will form the queue name
		/// b) return the SeqTokens that can be used for current computations
		/// </summary>
		/// <param name="depths"></param>
		/// <param name="simple"></param>
		/// <param name="depthLevel"></param>
		public SeqTokenStack Split(List<int> depths, bool simple, int depthLevel)
		{
			SeqTokenStack ret = new SeqTokenStack();
			
			// The following loop moves a SeqToken to the "ret" stack in two cases:
			// 1. the token is indexed by the "depths"
			// 2. there is already depthLevel tokens on the current stack (only if "simple")
			for (int i = Count - 1; 0 <= i; i--)
				if ((simple && Count > depthLevel) || null != depths && depths.Contains(i))
				{
					ret.Push(this[i]);
					RemoveAt(i);
				}
			return ret;
		}

		public IEnumerator<SeqToken> GetEnumerator()
		{
			return _seqTokens.GetEnumerator();
		}

		public override string ToString()
		{
			string ret = "";
			foreach (SeqToken st in _seqTokens)
				ret += (0 == _seqTokens.IndexOf(st) ? "#seq_stack: " : ",") + st;
			return ret + "\n";
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void AddRange(SeqTokenStack stack)
		{
			foreach (SeqToken st in stack)
				Push(st);
		}
		
		protected bool Equals(SeqTokenStack other)
		{
			if (Count != other.Count)
				return false;
			for (int i = 0; i < Count; i++)
				if (this[i] != other[i])
					return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SeqTokenStack) obj);
		}

		public override int GetHashCode()
		{
			return (_seqTokens != null ? _seqTokens.GetHashCode() : 0);
		}
	}
}