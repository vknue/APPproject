using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string Description { get; set; }

        public string Hashtags { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}