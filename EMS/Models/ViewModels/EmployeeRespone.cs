using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Models.ViewModels
{
    public class EmployeeResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public long PhoneNumber { get; set; }
        public long PersonalNumber { get; set; }
        public DateTime Birthday { get; set; }
        public string Banka { get; set; }
        public long NrBankes { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Position { get; set; }
        public string Schedule { get; set; }
        public double PaymentPerHour { get; set; }
        public string Holiday { get; set; }
        public bool Status { get; set; }
    }
}
