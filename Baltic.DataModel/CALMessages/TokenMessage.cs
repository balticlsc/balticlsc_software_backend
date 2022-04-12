using Baltic.DataModel.CALExecutable;

namespace Baltic.DataModel.CALMessages
{
    public class TokenMessage : Message {

        public long TokenNo { get; set; }
        public string PinName { get; set; }
        public string SenderUid { get; set; }
        public CDataSet DataSet { get; set; }
        public SeqTokenStack TokenSeqStack { get; set; }
        public string AccessType { get; set; } // TODO - consider removing
        public string TargetAccessType { get; set; } // TODO - consider removing
		
        public override string FamilyId => TokenNo.ToString();

        public TokenMessage() {
            SenderUid = "";
            TokenSeqStack = new SeqTokenStack();
        }
		
        public override string ToString()
        {
           return "TokenMessage=" + MsgUid + " TokenNo=" + TokenNo + " PinName=" + PinName + " SenderUid=" + SenderUid + " DataSet=" + DataSet.Values + " " + QueueSeqStack;
        }

    }
}