using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emr_web.Common;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    public class PurchaseViewController : Controller
    {
        public IActionResult PurchaseView()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            return View();
        }
    }
}