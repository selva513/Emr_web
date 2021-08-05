using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class SubMenuView
    {
        public long S_Menu_id { get; set; }
        public string S_MenuName { get; set; }
        public long S_MainMenu_id { get; set; }
        public bool? S_Access { get; set; }
        public bool? S_Add { get; set; }
        public bool? S_Modify { get; set; }
        public bool? S_Delete { get; set; }
        public bool? S_View { get; set; }
        public bool? S_Verify { get; set; }
        public long S_Seq { get; set; }
        public string S_Menu_Link { get; set; }
        public string S_Class { get; set; }
        public DateTime? S_CreatedDatetime { get; set; }
        public DateTime? S_ModifiedDatetime { get; set; }
        public string S_CreatedUser { get; set; }
        public string S_ModifiedUser { get; set; }
        public bool? S_IsActive { get; set; }
        public bool S_IsReflectto_Screen { get; set; }
    }
}
