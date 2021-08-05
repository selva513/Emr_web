using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class DoctorView
    {
        public long SNO { get; set; }
        public long DoctorSeqID { get; set; }
        public string DoctorName { get; set; }
        public string DoctorDegree { get; set; }
        public long DepartmentID { get; set; }
        public long SpecialityID { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public string HospitalID { get; set; }
        public bool Isactive { get; set; }
        public string CreatedDate { get; set; }
        public string ModifyDate { get; set; }
        public virtual string Departmentname { get; set; }
        public virtual string HISDoctorName { get; set; }
        public virtual long HISDoctorID { get; set; }
        public virtual string DoctorwithDegree { get; set; }
        public virtual string DepartmentName { get; set; }
        public virtual string Speciality_Name { get; set; }
        public decimal? ConsultingFees { get; set; }
        public decimal? GatewayCharges { get; set; }
        public decimal? NetConsultFees { get; set; }
        public decimal? DirectConsultFees { get; set; }
        public decimal? DirectGatewayCharges { get; set; }
        public decimal? DirectNetFees { get; set; }
        public int? CityId { get; set; }
        public int? GenderId { get; set; }
        public string RegistrationNumber { get; set; }
        public string RegistrationCouncil { get; set; }
        public string PassedOutYear { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public string PostGraduationDegree { get; set; }
        public string SuperSpecialityDegree { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSC_Code { get; set; }
        public string SpecialityExperience { get; set; }
        public string OverallExperience { get; set; }
        public string Others { get; set; }
        public string Comments { get; set; }
        public string Alternative_Number { get; set; }
        public string Resume { get; set; }
        public string EmailId { get; set; }
        public IFormFile DoctorPhoto { get; set; }
        public IFormFile DoctorSignature { get; set; }
        public long? MobileNumber { get; set; }
        public string DoctorAddress1 { get; set; }
        public string DoctorAddress2 { get; set; }
        public string Area { get; set; }
        public string Clinic_HospitalName { get; set; }
        public string Pincode { get; set; }
        public string CountryCode { get; set; }
        public bool IsUserExist { get; set; }
    }
}