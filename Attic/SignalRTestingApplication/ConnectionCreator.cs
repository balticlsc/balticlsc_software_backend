using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRTestingApplication.CommunicationDefiners;

namespace SignalRTestingApplication
{
    public static class ConnectionCreator
    {
        private const int MaxConnectingTime = 30;

        public static void CreateAndRun(CommunicationDefiner communicationDefiner)
        {
            var connection = new HubConnectionBuilder().WithUrl(communicationDefiner.Url).Build();

            try
            {
                connection.StartAsync();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Cannot connect to server");
                return;
            }

            if (connection.State == HubConnectionState.Connected)
            {
                Console.Clear();
                Console.WriteLine($"{communicationDefiner.Url} {communicationDefiner.MethodName}");
            }

            connection.On(communicationDefiner.MethodName, communicationDefiner.ParametersTypes.ToArray(), objects =>
            {
                foreach (var arg in objects)
                    Console.Write(arg + " ");

                Console.WriteLine();
                return Task.CompletedTask;
            });

            Console.ReadKey();
        }
    }
}