using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Emr_web.Controllers
{
    public class ApiRtcController : Controller
    {
        public IActionResult ApiRtc()
        {
            return View();
        }
        public IActionResult Sample()
        {
            return View();
        }
    }
}