using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DueController : Controller
    {
        public IActionResult Due()
        {
            return View();
        }
        public IActionResult PatientDue()
        {
            return View();
        }
    }
}
