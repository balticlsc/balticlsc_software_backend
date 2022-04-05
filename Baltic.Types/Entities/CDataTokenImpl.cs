using Baltic.DataModel.CALExecutable;

namespace Baltic.Types.Entities
{
    public class CDataTokenImpl : CDataToken
    {
        public string OwnerUid { get; set; }
        private CDataSet _parameters;
        private CDataSet _accessParameters;

        public override CDataSet Data
        {
            get
            {
                if (null == _parameters)
                {
                    // TODO pobrać z bazy danych na podstawie Uid
                }

                return _parameters;
            }
            set => _parameters = value;
        }

        public override CDataSet AccessData {
            get
            {
                if (null == _accessParameters)
                {
                    // TODO pobrać z bazy danych na podstawie Uid
                }

                return _accessParameters;
            }
            set => _accessParameters = value; }
    }
}