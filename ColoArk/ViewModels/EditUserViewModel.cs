using ColoArk.Models;
using System.ComponentModel.DataAnnotations;
using static ColoArk.Models.User;

namespace ColoArk.ViewModels
{
    public class EditUserViewModel
    {
        public int ID { get; set; }
        public string Email { get; set; }
        [Required]
        public string PSN { get; set; }
        [RegularExpression(@"^(\d{4})$", ErrorMessage = "Enter a valid 4 digit pin")]
        [Display(Name = "Mailbox Pin")]
        [Required]
        public int Pin { get; set; }
        [Required]
        public int ImplantID { get; set; }
        public AuthType AuthLevel { get; set; }
        public bool IsDonator { get; set; }
        public string Bio { get; set; }
        public string ProfilePic { get; set; }

        public EditUserViewModel()
        {

        }

        public EditUserViewModel(User user)
        {
            this.ID = user.ID;
            this.Email = user.Email;
            this.PSN = user.PSN;
            if (user.Pin != 0)
            {
                this.Pin = user.Pin;
            }
            if (user.ImplantID != 0)
            {
                this.ImplantID = user.ImplantID;
            }
            this.AuthLevel = user.AuthLevel;
            this.IsDonator = user.IsDonator;
            this.Bio = user.Bio;
            this.ProfilePic = user.ProfilePic;
        }
    }
}
