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
    public class CityController : Controller
    {
        private IDBConnection _IDBConnection;
        private IStateRepo _stateRepo;
        private IErrorlog _errorlog;
        private ICountryRepo _CountryRepo;
        private ICityRepo _cityRepo;
        private IHospitalRepo _hospitalRepo;
        private readonly IEmrRepo _emrRepo;
        public CityController(IDBConnection iDBConnection, IStateRepo stateRepo, IErrorlog errorlog, ICountryRepo countryRepo,ICityRepo cityRepo,IHospitalRepo hospitalRepo, IEmrRepo emrRepo)
        {
            _IDBConnection = iDBConnection;
            _stateRepo = stateRepo;
            _errorlog = errorlog;
            _CountryRepo = countryRepo;
            _cityRepo = cityRepo;
            _hospitalRepo = hospitalRepo;
            _emrRepo = emrRepo;
        }
        public async Task<IActionResult> City()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                var lstCountry = await _CountryRepo.GetAllCountry();
                if (lstCountry != null)
                {
                    lstCountry = lstCountry as List<CountryMaster>;
                    ViewBag.CountryDetail = lstCountry;
                }
                var lst = await _stateRepo.GetAllstate();
                if (lst != null)
                {
                    lst = lst as List<StateMaster>;
                    ViewBag.StateDetail = lst;
                }
                var lstcity = await _cityRepo.GetAllcity();
                if (lstcity != null)
                {
                    lstcity = lstcity as List<CityMaster>;
                    ViewBag.CityDetail = lstcity;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public IActionResult InsertCity([Bind]CityView model)
        {
            int result = 0;
            try
            {
                if (model.CitySeqID == 0)
                    result = InsertNewCity(model, true);
                else
                    result = InsertNewCity(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("City", "City");
        }
        private int InsertNewCity(CityView model, bool validation)
        {
            int result = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                CityMaster cityMaster = new CityMaster();
                cityMaster.CityName = model.CityName;
                cityMaster.CountrySeqID = model.CountrySeqID;
                cityMaster.StateSeqID = model.StateSeqID;
                cityMaster.CityCode = model.CityCode;
                cityMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                cityMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                cityMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                cityMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                cityMaster.Isactive = model.Isactive;
                if (validation == true)
                {
                    result = _cityRepo.CreateNewCity(cityMaster);
                    string EventName = "New City Added-" + model.CityName;
                    CreateEventManagemnt(EventName);
                }
                    
                else
                {
                    cityMaster.CitySeqID = model.CitySeqID;
                    result = _cityRepo.UpdateCity(cityMaster);
                    string EventName = "Update City Master-" + model.CityName;
                    CreateEventManagemnt(EventName);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        [HttpGet]
        public bool UpdateCityStatePin(long Statecode)
        {
            bool result = false;
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                //long PinStatecode = _hospitalRepo.GetCityStatePin(Hospitalid);
                result = _hospitalRepo.UpdateCityStatePin(Statecode, Hospitalid);
                string EventName = "Update City State Code-" + Statecode;
                CreateEventManagemnt(EventName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        [HttpGet]
        public bool UpdateCityCountryPin(long Countrycode)
        {
            bool result = false;
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                //long PinStatecode = _hospitalRepo.GetCityStatePin(Hospitalid);
                result = _hospitalRepo.UpdateCityCountryPin(Countrycode, Hospitalid);
                string EventName = "Update City Country Code-" + Countrycode;
                CreateEventManagemnt(EventName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        //Create Event Method
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