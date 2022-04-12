#pragma warning disable 1591
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Baltic.Server.Hubs
{
    public class TimeMachineHub : Hub
    {
        public async Task SendTime(DateTime timestamp)
        {
            await Clients.All.SendAsync("CurrentTime", timestamp.ToLongTimeString());
        }
    }
}
