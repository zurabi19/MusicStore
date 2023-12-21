using Microsoft.AspNetCore.Identity;

namespace MusicStore__1_.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string City { get; set; }
    }
}
