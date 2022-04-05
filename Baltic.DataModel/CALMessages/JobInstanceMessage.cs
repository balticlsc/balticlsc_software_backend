using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Baltic.DataModel.CALMessages
{
    public class JobInstanceMessage : JobExecutionMessage
    {
        /// <summary>
        /// Dictionary<pin_uid: string, token_no: long>
        /// </summary>
        public IDictionary<string, long> ProvidedPinTokens { get; set; }

        /// <summary>
        /// This job is a "merging" job (receives many tokens on any of the required pins)
        /// </summary>
        public bool IsMerger { get; set; }
        /// <summary>
        /// This job is a "splitting" job (sends many tokens on any of the provided pins)
        /// </summary>
        public bool IsSplitter { get; set; }
        public bool IsSimple { get; set; }
        public BalticModuleBuild Build { get; set; }
        public bool IsMultitasking { get; set; }

        public List<string> RequiredAccessTypes { get; set; }
        
        public JobInstanceMessage()
        {
            JobUid = "";
            ProvidedPinTokens = new ConcurrentDictionary<string, long>();
            IsMerger = false;
            IsSplitter = false;
            IsSimple = false;
            RequiredAccessTypes = new List<string>();
        }

        public override string ToString()
        {
            var i = 0;
            var ret = "JobInstanceMessage TaskUid - " + TaskUid + " MsgUid - " + MsgUid + "\n";
            foreach (var q in RequiredPinQueues)
            {
                ret = ret + "\t  Queue " + i + ": " + q.ToString() + "\n";
                i++;
            }

            foreach (var q in ProvidedPinTokens)
            {
                ret = ret + "\t  Token " + i + ": " + q.ToString() + "\n";
                i++;
            }

            ret += Build.ToString();

            return ret;
        }
    }
}