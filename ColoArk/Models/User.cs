using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColoArk.Models
{
    public class User : BaseModel
    {
        public string Email { get; set; }
        public string PSN { get; set; }
        public int Pin { get; set; }
        public int ImplantID { get; set; }
        public AuthType AuthLevel {get;set;}
        public bool IsDonator { get; set; }
        public virtual ICollection<GiveawayReceipt> GiveawayReceipts { get; set; }
        public string Bio { get; set; }
        public string ProfilePic { get; set; }

        public User()
        {
            this.GiveawayReceipts = new HashSet<GiveawayReceipt>();
        }

        public enum AuthType
        {
            Admin,
            Moderator,
            Player
        }

    }
}
