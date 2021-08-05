using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class UserTypeView
    {
        public long Type_Seqid { get; set; }
        public string Type_Name { get; set; }
        public DateTime? CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool? Isactive { get; set; }
    }
}
