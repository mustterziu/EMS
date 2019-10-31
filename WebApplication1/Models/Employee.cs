using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int PhoneNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Position { get; set; }
        public string Schedule { get; set; }

        public virtual ICollection<Attendance> Attendance { get; set; }
    }
}
