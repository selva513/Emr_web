using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Interface;
using BizLayer.Utilities;
using BizLayer.Domain;
using Emr_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Emr_web.Common;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class HolidayMasterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private IHolidayRepo _holidayrepo;

        public HolidayMasterController(IDBConnection iDBConnection, IHolidayRepo holidayRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _holidayrepo = holidayRepo;
        }
        public IActionResult HolidayMaster()
        {
            List<HolidayMaster> lstholidaymaster = new List<HolidayMaster>();
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                lstholidaymaster = _holidayrepo.BindHolidayDetails();
                if (lstholidaymaster != null)
                {
                    ViewBag.HolidayList = lstholidaymaster;
                }
            }
            catch (Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return View();
        }

        [HttpPost]
        public bool SaveFestival([FromBody] HolidayView model)
        {
            bool issucess = false;
            bool exist;
            HolidayMaster holidaymaster = new HolidayMaster();
            try
            {
                exist = _holidayrepo.Check_HolidayExist(model.HolidayName);
                if (exist)
                {
                    holidaymaster.HolidayName = model.HolidayName;
                    holidaymaster.HolidayDate = model.HolidayDate.ToString("yyyy-MM-dd");
                    holidaymaster.Content = model.Content;
                    holidaymaster.CreatedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    holidaymaster.IsActive = true;

                    issucess = _holidayrepo.InsertHoliday(holidaymaster);
                }
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return issucess;
        }

        [HttpPost]
        public bool UpdateFestival([FromBody] HolidayView model)
        {
            HolidayMaster holidaymaster = new HolidayMaster();
            try
            {
                holidaymaster.HolidayId = model.HolidayId;
                holidaymaster.HolidayName = model.HolidayName;
                holidaymaster.HolidayDate = model.HolidayDate.ToString("yyyy-MM-dd");
                holidaymaster.Content = model.Content;
                holidaymaster.ModifiedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                return (_holidayrepo.UpdateHoliday(holidaymaster));
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }
            return false;
        }
    }
}