using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeApi.Hubs
{
    public class HubNotificacion : Hub
    {
        static List<User> users = new List<User>();
        public void Connect(string User)
        {
            var id = Context.ConnectionId;

            if (users.Count(x => x.ConnectionId == id) == 0)
            {
                users.Add(new User { ConnectionId = id, UserName = User });
            }
        }

        public override Task OnConnectedAsync()
        {
            var item = users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                users.Remove(item);
            }

            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }

    public class User
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }
}
