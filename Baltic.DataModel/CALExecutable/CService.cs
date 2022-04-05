using System.Collections.Generic;
using Baltic.DataModel.CAL;

namespace Baltic.DataModel.CALExecutable {
	public abstract class CService : CJobBatchElement {
		public CJobBatch Batch { get; set; }
		public List<CredentialParameter> CredentialParameters { get; set; }

		public CService()
		{
			CredentialParameters = new List<CredentialParameter>();
		}

		public abstract string GetCredentials();

	}
}