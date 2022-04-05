using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Types;

namespace Baltic.DataModel.Accounts {
	public class TaskDataSet {
		public string Uid { get; set; }
		public string Name { get; set; }
		public string OwnerUid { get; set; }
		public DataType Type { get; set; }
		public CMultiplicity Multiplicity { get; set; }
		public AccessType Access { get; set; }
		public DataStructure Structure { get; set; }
		public CDataSet Data { get; set; }
		public CDataSet AccessData { get; set; }
	}
}