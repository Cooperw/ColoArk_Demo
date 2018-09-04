using System;

namespace ColoArk.Models
{
    public class GiveawayReceipt : BaseModel
    {
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public string Description { get; set; }
        public int MailboxID { get; set; }
        public virtual Mailbox Mailbox { get; set; }
        public DateTime EntryDate { get; set; }
        public bool IsInMailbox { get; set; }
        public bool IsPickedUp { get; set; }
        public DateTime ReadyForPickUpDate { get; set; }
    }
}
