using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class InvestigationMasterView
    {
        public long Investigation_Seqid { get; set; }
        public string Investigation_Name { get; set; }
        public decimal Investigation_Rate { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public long HospitalID { get; set; }
        public bool Isactive { get; set; }
    }
}
