using Baltic.Core.Utils;
using Baltic.DataModel.CAL;

namespace Baltic.UnitManager.Models
{
    public class XEnvironmentVariableMapping
    {
        public string EnvironmentVariableName { get; set; }

        public string AccessVariableName { get; set; }

        public string DefaultVariableValue { get; set; }

        public XEnvironmentVariableMapping(CredentialParameter parameter = null)
        {
            if (null == parameter)
                return;
            DBMapper.Map<XEnvironmentVariableMapping>(parameter, this);
        }

        public CredentialParameter ToModelObject()
        {
            return DBMapper.Map<CredentialParameter>(this, new CredentialParameter());
        }
    }
}