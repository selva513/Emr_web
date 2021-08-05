using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class RoleMasterView
    {
        public long Roleseqid { get; set; }
        public string Rolename { get; set; }
        public long HospitalID { get; set; }
        public long ClinicID { get; set; }
        public decimal? AllowedDiscount { get; set; }
        public bool? IsDiscountallowed { get; set; }
        public long CreatedByRoleId { get; set; }
        public bool? IsAdminRole { get; set; }
        public long AdminRoleId { get; set; }
        public bool MyPatientData { get; set; }
        public bool AllPatientData { get; set; }
        public bool AllDoctorData { get; set; }
        public bool DocumentUpload { get; set; }
        public DateTime? CreatedDatetime { get; set; }
        public DateTime? ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool? IsActive { get; set; }
        public MenuGroupView[] Menugroup { get; set; }
    }
}
