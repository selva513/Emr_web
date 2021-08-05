using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class HospitalView
    {
        public long HospitalID { get; set; }
        public string HospitalName { get; set; }
        public string HospitalMobileNo { get; set; }
        public string HospitalLandlineNo { get; set; }
        public string HospitalLandlineNo1 { get; set; }
        public string HospitalAddress { get; set; }
        public string HospitalAddress1 { get; set; }
        public string HospitalAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Pin { get; set; }
        public string Prefix { get; set; }
        public long TimezoneSeqID { get; set; }
        public long PinCity_Stateid { get; set; }
        public long PinCity_Countryid { get; set; }
        public long PinState_Countryid { get; set; }
        public long PinClinicid { get; set; }
        public byte[] HospitalLogo { get; set; }
        public byte[] OtherLogo { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime? ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool? Isactive { get; set; }
        public IFormFile FileHospital { get; set; }
        public IFormFile FileHospital1 { get; set; }
        public long Cityid { get; set; }
        public long Stateid { get; set; }
        public long Countryid { get; set; }
        public virtual string CityCode { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual int MobileDigits { get; set; }
    }
}
