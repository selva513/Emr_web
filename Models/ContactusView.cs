using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class ContactusView
    {
        public long SeqID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public long CustomerMobileNo { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedDatetime { get; set; }
    }
}
