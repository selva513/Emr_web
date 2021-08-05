using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    public class StockTransferController : Controller
    {
        public IActionResult StockTransfer()
        {
            return View();
        }
    }
}
