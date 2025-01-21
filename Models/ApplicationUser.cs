using Microsoft.AspNetCore.Identity;

namespace SkaftoBageriA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
