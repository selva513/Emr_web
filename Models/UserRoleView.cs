using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class UserRoleView
    {
        public RoleMasterView roleMasterView { get; set; }
        public MainMenuView mainMenuView { get; set; }
        public SubMenuView subMenuView { get; set; }
        public MenuGroupView menuGroupView { get; set; }
    }
}
