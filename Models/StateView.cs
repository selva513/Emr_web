using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class StateView
    {
        public long SNO { get; set; }
        public long StateSeqID { get; set; }
        public long CountrySeqID { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool Isactive { get; set; }
        public string CountryName { get; set; }
    }
}
