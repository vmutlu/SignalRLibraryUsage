using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR.Web
{
    /// <summary>
    /// MVC uygulamasında Hub kullanımı
    /// </summary>
    public class HubClass:Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
