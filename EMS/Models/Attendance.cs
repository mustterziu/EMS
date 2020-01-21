using Newtonsoft.Json;
using System;

namespace EMS.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [JsonPropertyAttribute("pagesa")]
        public double? payment { get; set; }
        public Employee Emp { get; set; }
        public Payment? Payment { get; set; }
        public int? PaymentId {get; set;}
    }
}
