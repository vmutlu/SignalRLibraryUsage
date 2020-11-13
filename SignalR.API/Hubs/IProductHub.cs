using SignalR.API.Models;
using System.Threading.Tasks;

namespace SignalR.API.Hubs
{
    public interface IProductHub
    {
        //kendi hub metodum
        Task ReceiveProduct(Product product);
    }
}
