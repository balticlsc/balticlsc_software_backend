using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;

namespace Baltic.DataModel.Execution {
	public abstract class InstantiableExecution : ExecutionRecord {
		public SeqTokenStack SeqStack { get; set; }
	}
}