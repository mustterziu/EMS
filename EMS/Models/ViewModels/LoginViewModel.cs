using System;
using System.ComponentModel.DataAnnotations;

namespace EMS.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public String Username { get; set; }
        [Required]
        public String Password { get; set; }

    }
}
