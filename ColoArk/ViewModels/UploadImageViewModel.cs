using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ColoArk.ViewModels
{
    public class UploadImageViewModel
    {
        public IFormFile File { get; set; }
        [Display(Name="New Filename")]
        public string Filename { get; set; }
    }
}
