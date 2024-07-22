using Microsoft.AspNetCore.Identity;

namespace ParadiseVilla_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
