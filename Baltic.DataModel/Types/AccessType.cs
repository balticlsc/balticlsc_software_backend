using Baltic.DataModel.CAL;

namespace Baltic.DataModel.Types
{
    public class AccessType : CalType
    {
        public string AccessSchema { get; set; }
        public string PathSchema { get; set; }
        public string StorageUid { get; set; }
        public string ParentUid { get; set; }
    }
}