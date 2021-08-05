using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class CityView
    {
        public long SNO { get; set; }
        public long CitySeqID { get; set; }
        public long CountrySeqID { get; set; }
        public long StateSeqID { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool Isactive { get; set; }
        public long Cityid { get; set; }
        public long Countryid { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
    }
}
