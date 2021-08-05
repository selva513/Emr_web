using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Controllers
{
    public class IPRegstrationViewsController : Controller
    {
        public IActionResult IPRegstrationViews()
        {
            return View();
        }
    }
}
