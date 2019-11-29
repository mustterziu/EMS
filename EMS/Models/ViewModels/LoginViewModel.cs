using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
