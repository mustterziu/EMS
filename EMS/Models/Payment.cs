using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Models
{
    public class Payment
    {
        public Payment()
        {
            days = new HashSet<Attendance>();
        }

        public int Id { get; set; }
        public Employee employee { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public float paymentNeto { get; set; }
        public float paymentBruto { get; set; }
        public bool paid { get; set; }

        public ICollection<Attendance> days { get; set; }
    }
}
