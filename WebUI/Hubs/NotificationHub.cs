using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebUI.Hubs
{
    [HubName("notificationHub")]
    public class NotificationHub : Hub
    {
        public bool Register(string userName)
        {
            

            return true;
        }
    }
}