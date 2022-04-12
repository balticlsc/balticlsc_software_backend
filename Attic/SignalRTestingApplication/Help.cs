using System;

namespace SignalRTestingApplication
{
    public static class Help
    {
        public static void PrintHelp()
        {
            Console.WriteLine("SignalR client to printing communicates from registered server.");
            Console.WriteLine("By default application is working in interactive mode.");
            Console.WriteLine("You can run it from command line with parameters (both are necessary):");
            Console.WriteLine("--url <SERVER_ADDRESS>");
            Console.WriteLine("--mn <METHOD_NAME>");
            Console.WriteLine("Method must have one parameter of type string.");
        }
    }
}