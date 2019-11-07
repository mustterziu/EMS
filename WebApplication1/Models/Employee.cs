using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public int PhoneNumber { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Position { get; set; }
        [Required]
        public string Schedule { get; set; }

        public virtual ICollection<Attendance> Attendance { get; set; }
    }
}
