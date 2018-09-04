using System.ComponentModel.DataAnnotations;
using static ColoArk.Models.Mailbox;
using static ColoArk.Models.GiveawayDrop;

namespace ColoArk.ViewModels
{
    public class EnterGiveawayViewModel
    {
        [Required]
        [Display(Name="Drop Table")]
        public DropType Type { get; set; }
    }
}
