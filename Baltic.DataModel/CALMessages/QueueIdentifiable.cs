using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.CALMessages {
	public abstract class QueueIdentifiable {
		
		public string TaskUid { get; set; }
		public SeqTokenStack QueueSeqStack { get; set; }

		public QueueIdentifiable()
		{
			QueueSeqStack = new SeqTokenStack();
		}

		public abstract string FamilyId { get; }
	}
}