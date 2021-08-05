using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class MenuGroupView
    {
        public long G_Seqid { get; set; }
        public long G_Roleid { get; set; }
        public long G_MainMenuid { get; set; }
        public long G_SubMenuid { get; set; }
        public bool? G_IsMainMenu { get; set; }
        public bool? G_Access { get; set; }
        public bool? G_Add { get; set; }
        public bool? G_Edit { get; set; }
        public bool? G_Delete { get; set; }
        public bool? G_View { get; set; }
        public bool? G_Verify { get; set; }
        public DateTime? G_CreatedDatetime { get; set; }
        public DateTime? G_ModifiedDatetime { get; set; }
        public string G_CreatedUser { get; set; }
        public string G_ModifiedUser { get; set; }
        public bool? G_IsActive { get; set; }
    }
}
