using System;

namespace ColoArk.Models
{
    public class Mailbox : BaseModel
    {
        public int VaultNumber { get; set; }
        public bool IsActive { get; set; }
        public int? UserID { get; set; }
        public virtual User User { get; set; }
        public string Mail_Description { get; set; }
        public DateTime ArrivalDate { get; set; }
    }
}
