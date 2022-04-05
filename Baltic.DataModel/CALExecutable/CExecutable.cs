using System.Collections.Generic;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.CALExecutable {
	public abstract class CExecutable {

		public string Uid { get; set; }
		public abstract List<CDataToken> Tokens { get; set; }

		/// <summary>
		/// Simple element: has just one "single" required pin (data token)
		/// </summary>
		public bool IsSimple => 1 == Tokens.FindAll(t =>  DataBinding.RequiredStrong == t.Binding).Count && 
		                        CMultiplicity.Single == Tokens.Find(t => 
			                        DataBinding.RequiredStrong == t.Binding).TokenMultiplicity;

		/// <summary>
		/// Merger type element: has as least one required multiple pin (data token) with at least one "depth" indicator
		/// </summary>
		public bool IsMerger => Tokens.Exists(t => 0 != t.Depths.Count);

		/// <summary>
		/// Splitter type element: has at least on provided multiple pin (data token)
		/// </summary>
		public bool IsSplitter => Tokens.Exists(t =>
			DataBinding.Provided <= t.Binding && CMultiplicity.Multiple == t.TokenMultiplicity);

		/// 
		/// <param name="tokenNo"></param>
		public CDataToken GetToken(long tokenNo)
		{
			return Tokens.Find(token => token.TokenNo == tokenNo || -token.TokenNo == tokenNo);
		}

	}
}