using Microsoft.AspNetCore.SignalR;
using SignalR.API.Models;
using System.Threading.Tasks;

namespace SignalR.API.Hubs
{
    public class ProductHub : Hub<IProductHub>
    {
        /// <summary>
        /// kendi yazdıgım metodun kullanılışı
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task SendProduct(Product product)
        {            
            await Clients.All.ReceiveProduct(product).ConfigureAwait(false);
        }
    }
}
