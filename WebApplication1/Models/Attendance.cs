using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Attendance
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Payment { get; set; }

        public virtual Employee Emp { get; set; }
    }


}
