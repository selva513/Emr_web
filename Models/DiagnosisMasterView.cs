using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class DiagnosisMasterView
    {
        public long Diagnosis_Seqid { get; set; }
        public string Diagnosis_Name { get; set; }
        public string ICD10 { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public long HospitalID { get; set; }
        public bool IsActive { get; set; }
    }
}
