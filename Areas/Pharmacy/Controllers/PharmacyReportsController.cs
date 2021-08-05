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
    public class PharmacyReportsController : Controller
    {
        public IActionResult PharmacyReports()
        {
            return View();
        }
    }
}
