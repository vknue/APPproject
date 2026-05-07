using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AADBDT.ViewModels
{
    public class PhotoUploadViewModel
    {
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Hashtags (comma separated)")]
        public string Hashtags { get; set; }

        [Required]
        [Display(Name = "Choose Image")]
        public IFormFile ImageFile { get; set; }

        public string TargetFormat { get; set; } // requiremets: "jpg", "png", "bmp"
        public bool ResizeToSquare { get; set; }
    }
}