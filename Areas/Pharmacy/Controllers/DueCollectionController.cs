using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Emr_web.Areas.Pharmacy.Controllers
{
    public class DueCollectionController : Controller
    {
        public IActionResult DueCollection()
        {
            return View();
        }
    }
}