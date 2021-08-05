using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class ClinicView
    {
        public long ClinicID { get; set; }
        public long HospitalID { get; set; }
        public string ClinicIdentifier { get; set; }
        public string ClinicName { get; set; }
        public string ClinicMobileNo { get; set; }
        public string ClinicLandlineNo { get; set; }
        public string ClinicLandlineNo1 { get; set; }
        public string ClinicAddress { get; set; }
        public string ClinicAddress1 { get; set; }
        public string ClinicAddress2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Pin { get; set; }
        public string Prefix { get; set; }
        public byte? CliniclLogo { get; set; }
        public byte? OtherLogo { get; set; }
        public bool TeleConsultation { get; set; }
        public DateTime? CreatedDatetime { get; set; }
        public DateTime? ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool? Isactive { get; set; }
        public IFormFile FileClinic { get; set; }
        public IFormFile FileClinic1 { get; set; }
    }
}
