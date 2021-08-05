using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class LoginView
    {
        public long UserSeqid { get; set; }
        public string UserName { get; set; }
        public string Userid { get; set; }
        public string Password { get; set; }
        public long RoleID { get; set; }
        public long DoctorID { get; set; }
        public long DepartmentID { get; set; }
        public long SpecialityID { get; set; }
        public long UserType { get; set; }
        public long HospitalID { get; set; }
        public long ClinicID { get; set; }
        public string ForgotPwdEmail { get; set; }
        public string ActivationKey { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime? ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool? IsActive { get; set; }
        public bool IsPrimaryUser { get; set; }
        public long LicenceID { get; set; }
        public long QuestionID { get; set; }
        public string QuestionAnswer { get; set; }
        public virtual string HospitalUniqueno { get; set; }
        public virtual string OldPassword { get; set; }
        public virtual string ConfirmPassword { get; set; }
        public bool IsTrailUser { get; set; }
        public DateTime? TrailCreateDate { get; set; }
        public int TrailDays { get; set; }
        public bool IsConnectedHIS { get; set; }
        public bool IsConnectedPharmacy { get; set; }
        public long MobileNumber { get; set; }
    }
}
