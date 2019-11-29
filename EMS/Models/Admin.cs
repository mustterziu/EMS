using Microsoft.AspNetCore.Identity;

namespace EMS.Models
{
    public class Admin : IdentityUser
    {
        public string FirstName { get; set; }
    }
}
