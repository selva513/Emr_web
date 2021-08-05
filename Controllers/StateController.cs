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
    public class StateController : Controller
    {
        private IDBConnection _IDBConnection;
        private IStateRepo _stateRepo;
        private IErrorlog _errorlog;
        private ICountryRepo _CountryRepo;
        private IHospitalRepo _hospitalRepo;
        private readonly IEmrRepo _emrRepo;
        public StateController(IDBConnection iDBConnection, IStateRepo stateRepo, IErrorlog errorlog, ICountryRepo countryRepo,IHospitalRepo hospitalRepo, IEmrRepo emrRepo)
        {
            _IDBConnection = iDBConnection;
            _stateRepo = stateRepo;
            _errorlog = errorlog;
            _CountryRepo = countryRepo;
            _hospitalRepo = hospitalRepo;
            _emrRepo = emrRepo;
        }
        public async Task<IActionResult> State()
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
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public IActionResult InsertState([Bind]StateView model)
        {
            int result = 0;
            try
            {
                if (model.StateSeqID == 0)
                    result = InsertNewState(model, true);
                else
                    result = InsertNewState(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("State", "State");
        }
        private int InsertNewState(StateView model, bool validation)
        {
            int result = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                StateMaster stateMaster = new StateMaster();
                stateMaster.StateName = model.StateName;
                stateMaster.StateCode = model.StateCode;
                stateMaster.CountrySeqID = model.CountrySeqID;
                stateMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                stateMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                stateMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                stateMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                stateMaster.Isactive = model.Isactive;
                if (validation == true)
                {
                    result = _stateRepo.CreateNewState(stateMaster);
                    string EventName = "New State Added-" + model.StateName;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    stateMaster.StateSeqID = model.StateSeqID;
                    result = _stateRepo.UpdateState(stateMaster);
                    string EventName = "Update State Master-" + model.StateName;
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
        public bool UpdateStateCountryPin(long Countrycode)
        {
            bool result = false;
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                //long PinStatecode = _hospitalRepo.GetCityStatePin(Hospitalid);
                result = _hospitalRepo.UpdateStateCountryPin(Countrycode, Hospitalid);
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