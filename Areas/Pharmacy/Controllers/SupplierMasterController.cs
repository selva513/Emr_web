using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;

namespace Emr_web.Areas.Pharmacy.Controllers
{
    [Area("Pharmacy")]
    public class SupplierMasterController : Controller
    {
       
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ISupplierMasterRepo _supplierMasterRepo;
        public SupplierMasterController( IDBConnection dBConnection, IErrorlog errorlog, ISupplierMasterRepo supplierMasterRepo)
        {
            
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _supplierMasterRepo = supplierMasterRepo;
        }

        public IActionResult SupplierMaster()
        {
            return View();
        }
        
        //public JsonResult CreateSupplierMaster([FromBody] SupplierMaster supplierMaster)
        //{
        //    List<SupplierMaster> lstResult = new List<SupplierMaster>();
        //    try
        //    {

        //        lstResult = _supplierMasterRepo.CreateSupplierMaster(supplierMaster);
               
        //    }


        //    catch (Exception ex)
        //    {
        //        _errorlog.WriteErrorLog(ex.ToString());
        //    }
        //    return Json(lstResult);
        //}
    }
 
}
