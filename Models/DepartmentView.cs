using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class DepartmentView
    {
        public long SNO { get; set; }
        public long DeptSeqID { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public string HospitalID { get; set; }
        public bool Isactive { get; set; }
        public virtual long SpecialityID { get; set; }
        public virtual string Speciality_Name { get; set; }
    }
}
