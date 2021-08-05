using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class CountryView
    {
        public long SNO { get; set; }
        public long CountrySeqId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public int MobileDigits { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool Isactive { get; set; }
    }
}
