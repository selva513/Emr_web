using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Emr_web.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Role()
        {
            return View();
        }
    }
}