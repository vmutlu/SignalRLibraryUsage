using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR.API.Data;
using SignalR.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalR.API.Hubs
{
    public class HubClass : Hub
    {
        private readonly AppDbContext _appDbContext;
        public HubClass(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        //clientların çagıracağı metodlar

        private static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;
        public static int TeamCount { get; set; } = 10;
        public async Task SendNameAsync(string name)
        {
            if (Names.Count >= TeamCount)
            {
                //11.client istek attıgında caller metodu sayesnde 11.cliente aşagıdaki mesaj gönderilecek
                await Clients.Caller.SendAsync("Error", $"Takımlar en fazla {TeamCount} kişi olabilir yav");
            }
            else
            {
                Names.Add(name);
                await Clients.All.SendAsync("ReceiveName", name).ConfigureAwait(false);  //bu huba baglı clientlara bildirim gönderecek
            }
        }
        public async Task GetNamesAsync()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }
        public async override Task OnConnectedAsync() //hub'a client baglandıgında otomatik çalışacak metodtur. Baglanan client sayısını tutmak için ClientCount adında static bir değişken tanımlaması yapıldı.
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception exception) // buda tam tersi baglantı koptugunda otomatik çalışacak metod
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnDisconnectedAsync(exception);
        }

        #region Groups Methods

        /// <summary>
        /// group'a girmek isterse
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        /// <summary>
        /// isim ekleme işlemi. Kullanıcıyı hangi gruba eklenecekse ekler.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public async Task SendNameByGroup(string userName, string teamName)
        {
            var team = _appDbContext.Teams.Where(x => x.Name == teamName).FirstOrDefault();
            if (team != null)
                team.Users.Add(new User { Name = userName });
            else
            {
                //gelen takım isminde takım yoksa önce takım oluştur
                var newTeam = new Team { Name = teamName };
                newTeam.Users.Add(new User { Name = userName });

                _appDbContext.Teams.Add(newTeam);
            }

            await _appDbContext.SaveChangesAsync();

            await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup", userName, team.Id);
        }

        /// <summary>
        /// gruptaki tüm kayıtlı kullanıcı isimlerini alır
        /// </summary>
        /// <returns></returns>
        public async Task GetNamesByGroup()
        {
            var teams = _appDbContext.Teams.Include(x => x.Users).Select(x => new
            {
                teamId = x.Id,
                Users = x.Users.ToList()
            });

            await Clients.All.SendAsync("ReceiveNamesByGroup", teams);
        }

        /// <summary>
        /// grouptan çıkma isterse
        /// </summary>
        /// <param name="teamName"></param>
        /// <returns></returns>
        public async Task RemoveToGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }

        #endregion

        public async Task SendProduct(Product product)
        {
            await Clients.All.SendAsync("ReceiveProduct", product);
        }
    }
}
