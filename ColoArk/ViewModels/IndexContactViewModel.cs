using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ColoArk.ViewModels
{
    public class IndexContactViewModel
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        [Display(Name ="Preferred Contact")]
        public ContactType PreferredContact { get; set; }
        public IssueType Reason { get; set; }

        public enum ContactType
        {
            PSN,
            Email
        }

        public enum IssueType
        {
            Feedback,
            Issue,
            Question,
            [Display(Name = "Website Help")]
            WebsiteHelp,
            [Display(Name = "Game Help")]
            GameHelp,
            Other
        }
    }
}
