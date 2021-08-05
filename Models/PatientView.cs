using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class PatientView
    {
        public long? PatSeqID { get; set; }
        public string PatientID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string StateCode { get; set; }
        public string CityCode { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public long? MobileNumber { get; set; }
        public string Email { get; set; }
        public string RelationName { get; set; }
        public string RelationType { get; set; }
        public string PatientAddress1 { get; set; }
        public string PatientAddress2 { get; set; }
        public int? City { get; set; }
        public int? State { get; set; }
        public int? Country { get; set; }
        public string VisitID { get; set; }
        public DateTime? VisitDatetime { get; set; }
        public int? AgeYear { get; set; }
        public int? AgeMonth { get; set; }
        public int? AgeDay { get; set; }
        public int? DoctorID { get; set; }
        public int? DeptID { get; set; }
        public string Status { get; set; }
        public string RefDoctor { get; set; }
        public string Age { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public string LetterBody { get; set; }
        public string HIS_VisitID { get; set; }
        public int HIS_DocID { get; set; }
        public int HIS_DeptID { get; set; }
        public string HospitalID { get; set; }
        public long ClinicID { get; set; }
        public long SpecialityID { get; set; }
        public virtual string CityName { get; set; }
        public virtual string StateName { get; set; }
        public virtual string CountryName { get; set; }
        public int BloodGroup { get; set; }
    }
}
