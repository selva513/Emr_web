using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class RegistrationView
    {
        public long RegisterSeqid { get; set; }
        public string AdminUsername { get; set; }
        public string AdminUserid { get; set; }
        public string AdminPassword { get; set; }
        public string AdminEmailid { get; set; }
        public string ContactNumber { get; set; }
        public string ActivationKey { get; set; }
        public DateTime? CreatedDatetime { get; set; }
        public DateTime? ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool? IsActive { get; set; }
        public string HospitalName { get; set; }
        public long Hospitalid { get; set; }
    }
}
