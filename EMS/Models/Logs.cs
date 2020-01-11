using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Models
{
    public class Logs
    {
        public int Id { get; set; }

        public String mesazhi { get; set; }

        public String createdBy { get; set; }

        public DateTime createdAt { get; set; }
    }
}
