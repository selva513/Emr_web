using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Emr_web.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    public class DrugDispenseController : Controller
    {
        public IActionResult DrugDispense()
        {
            return View();
        }
    }
}