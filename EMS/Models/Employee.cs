using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMS.Models
{
    public class Employee : BaseEntity
    {
        public Employee()
        {
            Attendance = new HashSet<Attendance>();
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        
        [Required (ErrorMessage = "Ju lutem shenoni emrin!")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Emri duhet te jete me i madh se 3 shkronja!")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Perdorni vetem shkronja, ju lutem!")]
        public string FirstName { get; set; }
        
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Mbiemri duhet te jete me i madh se 3 shkronja!")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Perdorni vetem shkronja, ju lutem!")]
        [Required (ErrorMessage = "Ju lutem shenoni mbiemrin!")]
        public string LastName { get; set; }
        
        [Required (ErrorMessage = "Ju lutem zgjidhni gjinine!")]
        public string Gender { get; set; }
        
        [Required (ErrorMessage = "Ju lutem shenoni numrin e telefonit!")]
        public long PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Ju lutem shenoni numrin personal!")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Numri juaj personal duhet te permbaje 10 shifra!")]
        public long PersonalNumber { get; set; }

        [Required (ErrorMessage = "Ju lutem shtoni daten e lindjes")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Ju lutem caktoni banken")]
        public string Banka { get; set; }

        [Required(ErrorMessage = "Ju lutem shtoni numrin e bankes")]
        [RegularExpression(@"\d{16}", ErrorMessage = "Numri juaj i gjirollogarise duhet ti permbaje 16 shifra!")]
        public long NrBankes { get; set; }

        [Required(ErrorMessage = "Ju lutem shtoni email")]
        public string Email { get; set; }

        [Required (ErrorMessage = "Ju lutem shenoni Qytetin!")]
        
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Perdorni vetem shkronja, ju lutem!")]
        public string City { get; set; }
        
        [Required (ErrorMessage = "Ju lutem zgjidhni Shtetin!")]
        public string State { get; set; }
        
        [Required (ErrorMessage = "Ju lutem shenoni poziten!")]
        public string Position { get; set; }
        
        [Required (ErrorMessage = "Ju lutem zgjidhni orarin!")]
        public string Schedule { get; set; }
        
        [Required (ErrorMessage = "Ju lutem caktoni pagen!")]
        public double PaymentPerHour { get; set; }

        public string Holiday { get; set; }
        public bool Status { get; set; }
        public ICollection<Attendance> Attendance { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
