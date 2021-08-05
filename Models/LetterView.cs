using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class LetterView
    {
        public long LetterSeqid { get; set; }
        public string PatientId { get; set; }
        public string VisitId { get; set; }
        public string LetterBody { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool IsActive { get; set; }
    }
}
