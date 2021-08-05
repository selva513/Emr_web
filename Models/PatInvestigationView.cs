using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
namespace Emr_web.Models
{
    public class PatInvestigationView
    {
        public long Seqid { get; set; }
        public string Patientid { get; set; }
        public string Visitid { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal GrandDiscount { get; set; }
        public decimal NetTotal { get; set; }
        public long HospitalID { get; set; }
        public long ClinicID { get; set; }
        public string InvestigationNotes { get; set; }
        public long Departmentid { get; set; }
        public string DepartmentRefNotes { get; set; }
        public long Doctorid { get; set; }
        public string DoctorRefNotes { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public int HIS_Orderid { get; set; }
        public PatientInvestDetails[] InvDetails { get; set; }
    }
}
