using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class SpecialityView
    {
        public long SpecialityID { get; set; }
        public string Speciality_Name { get; set; }
        public string Description { get; set; }
        public string CreatedDatetime { get; set; }
        public string ModifiedDatetime { get; set; }
        public string CreatedUser { get; set; }
        public string ModifiedUser { get; set; }
        public bool? Isactive { get; set; }
    }
}
