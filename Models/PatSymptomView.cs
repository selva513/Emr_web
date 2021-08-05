using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
namespace Emr_web.Models
{
    public class PatSymptomView
    {
        public long PshSeqID { get; set; }
        public string PatientId { get; set; }
        public string VisitId { get; set; }
        public string HistoryNotes { get; set; }
        public string PainScale { get; set; }
        public bool IsVerified { get; set; }
        public PatientSympotmDetails[]  SymDetails { get; set; }
        public FunPickListDetails[] FunStatsuDtl { get; set; }
        public string PickListName { get; set; }
    }
}
