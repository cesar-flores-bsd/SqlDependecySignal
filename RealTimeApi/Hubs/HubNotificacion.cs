using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RealTimeApi.Hubs
{
    public class HubNotificacion : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
