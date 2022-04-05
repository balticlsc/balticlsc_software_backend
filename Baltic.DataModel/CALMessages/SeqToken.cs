using System;

namespace Baltic.DataModel.CALMessages
{
    public class SeqToken
    {
        public string SeqUid { get; set; }
        public long No { get; set; }
        public bool IsFinal { get; set; }

        public SeqToken()
        {
            SeqUid = "";
        }

        /// 
        /// <param name="st"></param>
        public SeqToken(SeqToken st)
        {
            SeqUid = st.SeqUid;
            No = st.No;
            IsFinal = st.IsFinal;
        }

        /// 
        /// <param name="st1"></param>
        /// <param name="st2"></param>
        public static bool operator ==(SeqToken st1, SeqToken st2)
        {
            return st1?.SeqUid == st2?.SeqUid && st1?.No == st2?.No && st1?.IsFinal == st2?.IsFinal;
        }
        
        /// 
        /// <param name="st1"></param>
        /// <param name="st2"></param>
        public static bool operator !=(SeqToken st1, SeqToken st2)
        {
            return st1?.SeqUid != st2?.SeqUid || st1?.No != st2?.No || st1?.IsFinal != st2?.IsFinal;
        }
        
        public SeqToken Copy()
        {
            return new SeqToken(this);
        }
        
        protected bool Equals(SeqToken other)
        {
            return SeqUid == other.SeqUid && No == other.No && IsFinal == other.IsFinal;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SeqToken) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SeqUid, No, IsFinal);
        }

        public override string ToString()
        {
            return SeqUid + "=>" + No + (IsFinal ? "f" : "");
        }
    }
}