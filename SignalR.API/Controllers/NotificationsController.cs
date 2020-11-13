using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.API.Hubs;
using System.Threading.Tasks;

namespace SignalR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        /// <summary>
        /// hub üzerinden clientlera mesaj göndermek için IHubContexti DI ya eklemem gerek
        /// </summary>
        private readonly IHubContext<HubClass> _hubContext;
        public NotificationsController(IHubContext<HubClass> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("{teamCount}")]
        public async Task<IActionResult> SetTeamCount(int teamCount)
        {
            HubClass.TeamCount = teamCount;
            await _hubContext.Clients.All.SendAsync("Notify", $"Takımlar baya dolu biladerlerim {teamCount} 'er kişilik");

            return Ok();
        }
    }
}
