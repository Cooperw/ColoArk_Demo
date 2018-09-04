using ColoArk.Models;
using System.ComponentModel.DataAnnotations;
using static ColoArk.Models.GiveawayDrop;

namespace ColoArk.ViewModels
{
    public class CreateEditGiveawayDropViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int LowerBound { get; set; }
        [Required]
        public int HigherBound { get; set; }
        [Required]
        public DropType Type { get; set; }

        public CreateEditGiveawayDropViewModel()
        {

        }

        public CreateEditGiveawayDropViewModel(GiveawayDrop drop)
        {
            this.ID = drop.ID;
            this.Name = drop.Name;
            this.LowerBound = drop.LowerBound;
            this.HigherBound = drop.HigherBound;
            this.Type = drop.Type;
        }
    }
}
