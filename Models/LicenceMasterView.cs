using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class LicenceMasterView
    {
        public long LicenceSeqid { get; set; }
        public long HospitalID { get; set; }
        public long UserTypeID { get; set; }
        public string PrimaryUserName { get; set; }
        public long MobileNo { get; set; }
        public string EmailID { get; set; }
        public int ClinicsCount { get; set; }
        public int UsersCount { get; set; }
        public string SearchType { get; set; }
        public string OtherNotes { get; set; }
        public long AgentMobileNo { get; set; }
        public string Hospitalname { get; set; }
        public bool IsActive { get; set; }
        public bool IsSendEmail { get; set; }
        public bool IsChangePassword { get; set; }
        //public string HospitalName { get; set; }
        public string Typename { get; set; }
        public string Password { get; set; }
        public long SNO { get; set; }
        public string Hospital_Uniqueno { get; set; }
        public long UserSeqid { get; set; }
        public long QuestionID { get; set; }
        public string QuestionAnswer { get; set; }
        public long CityId { get; set; }
        public long CountryId { get; set; }
        public string Pincode { get; set; }
    }
}
