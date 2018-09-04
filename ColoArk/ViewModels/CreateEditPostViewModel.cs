using ColoArk.Models;
using System;
using System.ComponentModel.DataAnnotations;
using static ColoArk.Models.Post;

namespace ColoArk.ViewModels
{
    public class CreateEditPostViewModel
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        [Required]
        public string Description { get; set; }
        public string Picture { get; set; }
        [Required]
        public PostType Type { get; set; }
        public DateTime Date { get; set; }

        public CreateEditPostViewModel()
        {

        }

        public CreateEditPostViewModel(Post post)
        {
            this.ID = post.ID;
            this.Title = post.Title;
            this.Subtitle = post.Subtitle;
            this.Description = post.Description;
            this.Picture = post.Picture;
            this.Type = post.Type;
            this.Date = post.Date;
        }
    }
}
