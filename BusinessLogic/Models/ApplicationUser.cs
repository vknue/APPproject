using BusinessLogic.Enums;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Models
{
    public class ApplicationUser : IdentityUser
    {
        public PackageType Package { get; set; } = PackageType.FREE;
        public PackageType? PendingPackage { get; set; }
        public DateTime? LastPackageChangeDate { get; set; }

        // Extra Properties for the Profile
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }

        public override string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}