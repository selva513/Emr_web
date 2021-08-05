using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
namespace Emr_web.Models
{
    public class PatDiagnosisView
    {
        public long Seqid { get; set; }
        public string PatientId { get; set; }
        public string VisitId { get; set; }
        public long HospitalID { get; set; }
        public long ClinicID { get; set; }
        public string Comments { get; set; }
        public string CreateDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifieDatetime { get; set; }
        public string ModifiedUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public string DignosisType { get; set; }
        public PatientDiagnosisDetail[] DiagDetails { get; set; }
        public long TreatmentHdrSeqID { get; set; }
    }
}
