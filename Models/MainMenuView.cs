using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class MainMenuView
    {
        public long M_Menu_id { get; set; }
        public string M_Menuname { get; set; }
        public long M_Seq { get; set; }
        public string M_Class { get; set; }
        public string M_Icon { get; set; }
        public string M_Href { get; set; }
        public DateTime? M_CreatedDatetime { get; set; }
        public DateTime? M_ModifiedDatetime { get; set; }
        public string M_CreatedUser { get; set; }
        public string M_ModifiedUser { get; set; }
        public bool? M_IsActive { get; set; }
    }
}
