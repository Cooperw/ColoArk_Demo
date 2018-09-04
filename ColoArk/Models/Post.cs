using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ColoArk.Models
{
    public class Post : BaseModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public PostType Type { get; set; }
        public DateTime Date { get; set; }

        public string DescriptionTrimmed
        {
            get
            {
                string trim;
                try
                {
                    trim = this.Description.Substring(0, 20);

                }
                catch
                {
                    trim = this.Description;
                }
                return trim;
            }
        }

        public enum PostType
        {
            General,
            Labyrinth,
            Giveaway,
            [Display(Name="Scavenger Hunt")]
            ScavengerHunt,
            [Display(Name = "Admin War")]
            AdminWar,
            Rules
        }
    }
}
