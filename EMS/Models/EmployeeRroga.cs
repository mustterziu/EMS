using System;
using System.ComponentModel.DataAnnotations;

namespace EMS.Models
{
    public class EmployeeRroga
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Position { get; set; }

        public double Paga { get; set; }
    }
}
