using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Models
{
    public class BaseEntity
    {
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
