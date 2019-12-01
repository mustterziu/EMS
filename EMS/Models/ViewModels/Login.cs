using System;
using System.ComponentModel.DataAnnotations;

namespace EMS.Models.ViewModels
{
    public class Login
    {
        [Required]
        public String Username { get; set; }
        [Required]
        public String Password { get; set; }

    }
}
