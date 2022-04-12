using System;
using SignalRTestingApplication.CommunicationDefiners;

namespace SignalRTestingApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            if (1 == args.Length && args[0].Equals("--h"))
            {
                Help.PrintHelp();
            }
            else if (0 != args.Length)
            {
                var communicationDefiner = new FromArgumentsCommunicationDefiner();
                if (communicationDefiner.DefineMethodFromArguments(args))
                    ConnectionCreator.CreateAndRun(communicationDefiner);

                else
                    Console.WriteLine("Incorrect arguments - cannot create connection.");
            }
            else
            {
                var communicationDefiner = new InteractiveCommunicationDefiner();
                communicationDefiner.DemandServerUrl();
                communicationDefiner.DemandMethodName();
                communicationDefiner.DemandParametersNumber();
                communicationDefiner.DemandParametersTypes();

                ConnectionCreator.CreateAndRun(communicationDefiner);
            }
        }
    }
}