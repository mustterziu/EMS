using System;

namespace EMS.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Payment { get; set; }
        public Employee Emp { get; set; }
    }
}
