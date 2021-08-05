using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Emr_web.Common;
using Emr_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class HospitalSettingController : Controller
    {
        private IDBConnection _IDBConnection;
        private IPatientRepo _patientRepo;
        private IErrorlog _errorlog;
        
        public HospitalSettingController(IDBConnection iDBConnection, IPatientRepo patientRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _patientRepo = patientRepo;
            _errorlog = errorlog;
        }
        public IActionResult HospitalSetting()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            return View();
        }
        [HttpPost]
        public IActionResult UpdateConfig([Bind]ConfigView model)
        {
            try
            {
                bool ConnectedHIS = model.IsConnectedHIS;
                bool ConnectedPharmacy = model.IsConnectedPharmacy;
                bool result = _patientRepo.UpdateConfigDetails(ConnectedHIS, ConnectedPharmacy);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("HospitalSetting", "HospitalSetting");
        }
    }
}