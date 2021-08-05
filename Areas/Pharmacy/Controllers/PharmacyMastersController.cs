using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emr_web.Common;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    public class PharmacyMastersController : Controller
    {
        public IActionResult PharmacyMastersView()
        {
            return View();
        }
        public IActionResult PHM()
        {
            return View();
        }
    }
}
