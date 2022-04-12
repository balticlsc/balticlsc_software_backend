using System;
using System.Collections.Generic;
using Baltic.DataModel.CAL;
using Baltic.DataModel.CALExecutable;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Types.Entities
{
    public class CServiceImpl : CService
    {
        private List<CDataToken> _tokens;
        
        public override List<CDataToken> Tokens
        {
            get
            {
                if (null == _tokens)
                {
                    // TODO pobrać z bazy danych na podstawie Uid
                }

                return _tokens;
            }
            set => _tokens = value;
        }

        public CServiceImpl()
        {
            Uid = Guid.NewGuid().ToString();
            _tokens = new List<CDataToken>();
        }
        
        public override string ToString()
        {
            var ret = "Service " + ModuleReleaseUid + " " + Uid + "\n";
            foreach (var token in Tokens)
                ret = ret + "\t" + token.ToString();
            return ret;
        }

        public override BalticModuleBuild GetBuild()
        {
            BalticModuleBuild ret = GetBaseBuild();
            // TODO - perhaps use the service execution Uid
            ret.ModuleId = Uid;
            if (!CredentialParameters.Exists(cp => "Host" == cp.AccessCredentialName))
            {
                CredentialParameters.Add(new CredentialParameter()
                {
                    AccessCredentialName = "Host",
                    DefaultCredentialValue = Uid
                });
            }

            foreach (CredentialParameter credential in CredentialParameters)
                // Add an environment variable based on a credential parameter (if not already added from unit parameters)
                if (null != credential.EnvironmentVariableName &&
                    !ret.EnvironmentVariables.ContainsKey(credential.EnvironmentVariableName))
                    ret.EnvironmentVariables.Add(credential.EnvironmentVariableName, credential.DefaultCredentialValue);
            return ret;
        }
        
        public override string GetCredentials()
        {
            // TODO - add overriding of credentials based on unit parameters (potentially user-defined)
            string ret = "";
            foreach (CredentialParameter param in CredentialParameters)
            {
                string paramText = param.ToString();
                if ("" != paramText)
                    ret += param + ",\n";
            }

            if (CredentialParameters.Count > 0)
                ret = ret.Substring(0, ret.Length - 2);

            return "{\n" + ret + "\n}";
        }

    }
}