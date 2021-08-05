using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Repo;
using BizLayer.Utilities;
using Emr_web.Models;
using Emr_web.Common;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Navigations;
using Microsoft.AspNetCore.Http;
using System.Globalization;


namespace Emr_web.Controllers
{
    public class CalendarController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private ICalenderRepo _calenderRepo;
        public CalendarController(IDBConnection iDBConnection,IErrorlog errorlog,ICalenderRepo calenderRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _calenderRepo = calenderRepo;
        }
        public IActionResult Calendar()
        {
            //var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            //ViewBag.mainMenuItems = myComplexObject;
            //var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            //ViewBag.AccountMenuItems = myComplexObjectaccount;
            //ViewBag.maxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            return View();
        }
        [HttpGet]
        public JsonResult AppointmentByDoctor(long DoctorID, long HospitalID)
        {
            List<AppointmentHistory> LstAppointmentHistory = new List<AppointmentHistory>();
            LstAppointmentHistory = _calenderRepo.GetAppointHistoryByDoctorID(DoctorID, HospitalID);
            return Json(LstAppointmentHistory);
        }
    }
}