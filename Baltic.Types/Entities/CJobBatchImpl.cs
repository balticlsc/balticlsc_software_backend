using System.Collections.Generic;
using System.Text.RegularExpressions;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;

namespace Baltic.Types.Entities
{
    public class CJobBatchImpl : CJobBatch
    {
        public string TaskUid { get; set; }

        private List<CJob> _jobs;

        public override List<CJob> Jobs
        {
            get
            {
                if (null == _jobs)
                {
                    // TODO pobrać Jobs z bazy danych, dla których job.BatchUid==Uid
                }

                return _jobs;
            }
            set => _jobs = value;
        }
        
        private List<CService> _services;

        public override List<CService> Services
        {
            get
            {
                if (null == _services)
                {
                    // TODO pobrać Jobs z bazy danych, dla których job.BatchUid==Uid
                }

                return _services;
            }
            set => _services = value;
        }

        private List<CDataToken> _Tokens;

        public override List<CDataToken> Tokens
        {
            get
            {
                if (null == _Tokens)
                {
                    // TODO pobrać Tokens z bazy danych, dla których token.BatchUid==Uid
                }

                return _Tokens;
            }
            set => _Tokens = value;
        }

        private List<BatchExecution> _BatchInstances;

        public override List<BatchExecution> BatchExecutions
        {
            get
            {
                if (null == _BatchInstances)
                {
                    // TODO pobrać BatchExecutions z bazy danych, dla których instance.BatchUid==Uid
                }

                return _BatchInstances;
            }
            set => _BatchInstances = value;
        }

        public override string ToString()
        {
            var ret = "JobBatch " + Uid + " (depth_level=" + DepthLevel + ")\n";
            foreach (var token in Tokens)
                ret = ret + "\t" + Regex.Replace(token.ToString(), @"\n\t", "\n\t\t");
            foreach (var service in Services)
                ret = ret + "\t" + Regex.Replace(service.ToString(), @"\n\t", "\n\t\t");
            foreach (var job in Jobs)
                ret = ret + "\t" + Regex.Replace(job.ToString(), @"\n\t", "\n\t\t");
            return ret;
        }
    }
}