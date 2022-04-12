using System.Collections.Generic;
using System.Linq;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.CALMessages;
using Baltic.DataModel.Execution;
using BalticModuleBuild = Baltic.DataModel.CALMessages.BalticModuleBuild;

namespace Baltic.Types.Entities
{
    public class CJobImpl : CJob
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

        private List<JobExecution> _jobExecutions;

        public override List<JobExecution> JobExecutions
        {
            get
            {
                if (null == _jobExecutions)
                {
                    // TODO pobrać z bazy danych na podstawie Uid
                }

                return _jobExecutions;
            }
            set => _jobExecutions = value;
        }

        private List<JobInstance> _jobInstances;

        public override List<JobInstance> JobInstances
        {
            get
            {
                if (null == _jobInstances)
                {
                    // TODO pobrać z bazy danych na podstawie Uid
                }

                return _jobInstances;
            }
            set => _jobInstances = value;
        }

        /*
        private string _batchUid;
*/
        private CJobBatch _batch;

        public override CJobBatch Batch
        {
            get
            {
                if (null == _batch)
                {
                    // TODO pobrać z bazy danych na podstawie _batchUid
                }
                return _batch;
            }
            set => _batch = value;
        }
        
        public override BalticModuleBuild GetBuild(string pinsConfigMountPath)
        {
           BalticModuleBuild ret = GetBaseBuild();
           List<PinsConfig> pinConfigs = Tokens.Select(t => new PinsConfig(t)).ToList();
           ret.FinalizePinConfigFile(pinConfigs,pinsConfigMountPath);

           return ret;
        }

        public override string ToString()
        {
            var ret = "Job " + ModuleReleaseUid + " " + Uid + "\n";
            foreach (var token in Tokens)
                ret = ret + "\t" + token.ToString();
            return ret;
        }
        
    }
}