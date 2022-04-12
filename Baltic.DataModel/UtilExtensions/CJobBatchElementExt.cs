using System.Collections.Generic;
using System.Linq;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

// ReSharper disable CheckNamespace

namespace Baltic.DataModel.CALExecutable
{
    public abstract partial class CJobBatchElement
    {
        public CJobBatchElement()
        {
            CommandArguments = new List<string>();
            Parameters = new List<CParameter>();
        }

        protected BalticModuleBuild GetBaseBuild()
        {
            BalticModuleBuild ret = new BalticModuleBuild()
            {
                Image = Image,
                Command = Command,
                CommandArguments = CommandArguments.Select(ca => new string(ca)).ToList()
            };
            ret.InitializeParameters(Parameters);
            return ret;
        }
    }
}