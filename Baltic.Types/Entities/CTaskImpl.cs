using System.Collections.Generic;
using System.Text.RegularExpressions;
using Baltic.DataModel.CALExecutable;
using Baltic.DataModel.Execution;

namespace Baltic.Types.Entities
{
    public class CTaskImpl : CTask
    {
        public string AccountUid { get; set; }
        private List<CJobBatch> _batches;

        public override List<CJobBatch> Batches
        {
            get
            {
                if (null == _batches)
                {
                    // TODO pobrać z bazy danych na podstawie Uid
                }

                return _batches;
            }
            set => _batches = value;
        }

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

        private TaskExecution _execution;

        public override TaskExecution Execution
        {
            get
            {
                if (null == _execution)
                {
                    // TODO pobrać z bazy danych na podstawie Uid
                }

                return _execution;
            }
            set => _execution = value;
        }

        public override string ToString()
        {
            var ret = "Task " + Uid + "\n";
            foreach (var token in Tokens)
                ret = ret + "\t" + Regex.Replace(token.ToString(), @"\n\t", "\n\t\t");
            foreach (var batch in Batches)
                ret = ret + "\t" + Regex.Replace(batch.ToString(), @"\n\t", "\n\t\t");
            return ret;
        }
    }
}