using System.Collections.Generic;

namespace SignalR.API.Models
{
    public class Team
    {
        public Team()
        {
            Users = new List<User>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
