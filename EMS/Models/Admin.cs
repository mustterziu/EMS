using Microsoft.AspNetCore.Identity;

namespace EMS.Models
{
    public class Admin : IdentityUser
    {
        public bool PasswordChangeRequired { get; set; }
    }
}
