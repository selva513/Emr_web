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
    public class CountryController : Controller
    {
        private IDBConnection _IDBConnection;
        private ICountryRepo _countryRepo;
        private IErrorlog _errorlog;
        private readonly IEmrRepo _emrRepo;
        public CountryController(IDBConnection iDBConnection, ICountryRepo countryRepo, IErrorlog errorlog, IEmrRepo emrRepo)
        {
            _IDBConnection = iDBConnection;
            _countryRepo = countryRepo;
            _errorlog = errorlog;
            _emrRepo = emrRepo;
        }
        public async Task<IActionResult> Country()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                var lst = await _countryRepo.GetAllCountry();
                if (lst != null)
                {
                    lst = lst as List<CountryMaster>;
                    ViewBag.CountryDetail = lst;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public IActionResult InsertCountry([Bind]CountryView model)
        {
            int result = 0;
            try
            {
                if (model.CountrySeqId == 0)
                    result = InsertNewCountry(model, true);
                else
                    result = InsertNewCountry(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("Country", "Country");
        }
        private int InsertNewCountry(CountryView model, bool validation)
        {
            int result = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                CountryMaster countryMaster = new CountryMaster();
                countryMaster.CountryName = model.CountryName;
                countryMaster.CountryCode = model.CountryCode;
                countryMaster.CurrencyCode = model.CurrencyCode;
                countryMaster.MobileDigits = model.MobileDigits;
                countryMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                countryMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                countryMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                countryMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                countryMaster.Isactive = model.Isactive;
                if (validation == true)
                {
                    result = _countryRepo.CreateNewCountry(countryMaster);
                    string EventName = "New Country Added-" + model.CountryName;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    countryMaster.CountrySeqId = model.CountrySeqId;
                    result = _countryRepo.UpdateCountry(countryMaster);
                    string EventName = "Update Country Master-" + model.CountryName;
                    CreateEventManagemnt(EventName);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        private void CreateEventManagemnt(string EventName)
        {
            EventManagemntInfo eventManagemntInfo = new EventManagemntInfo
            {
                EventName = EventName,
                UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
            };
            _emrRepo.NewEventCreateion(eventManagemntInfo);
        }
    }
}