namespace SignalRTestingApplication.CommunicationDefiners
{
    public class FromArgumentsCommunicationDefiner : CommunicationDefiner
    {
        public bool DefineMethodFromArguments(string[] args)
        {
            if (args.Length < 4)
                return false;

            string url = null;
            string methodName = null;
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals("--url") && !args[i + 1].Equals("--mn"))
                    url = args[i + 1];
                if (args[i].Equals("--mn") && !args[i + 1].Equals("--url"))
                    methodName = args[i + 1];
            }

            if (null == url || null == methodName)
                return false;

            Url = url;
            MethodName = methodName;
            ParametersTypes.Add(typeof(string));

            return true;
        }
    }
}