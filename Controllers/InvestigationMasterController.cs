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
    public class InvestigationMasterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IInvestigationRepo _investigationRepo;
        private IErrorlog _errorlog;
        private readonly IEmrRepo _emrRepo;
        public InvestigationMasterController(IDBConnection iDBConnection, IInvestigationRepo investigationRepo, IErrorlog errorlog, IEmrRepo emrRepo)
        {
            _IDBConnection = iDBConnection;
            _investigationRepo = investigationRepo;
            _errorlog = errorlog;
            _emrRepo = emrRepo;
        }
        public async Task<IActionResult> InvestigationMaster()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                var lst = await _investigationRepo.GetAllInvestigation(HospitalID);
                if (lst != null)
                {
                    lst = lst as List<InvestigationMaster>;
                    ViewBag.InvestDetail = lst;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult InsertInvestigation([Bind]InvestigationMasterView model)
        {
            int result = 0;
            try
            {
                if (model.Investigation_Seqid == 0)
                    result = InsertNewInvestigation(model, true);
                else
                    result = InsertNewInvestigation(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("InvestigationMaster", "InvestigationMaster");
        }
        private int InsertNewInvestigation(InvestigationMasterView model, bool validation)
        {
            int result = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                InvestigationMaster investigationMaster = new InvestigationMaster();
                investigationMaster.Investigation_Name = model.Investigation_Name;
                investigationMaster.Investigation_Rate = model.Investigation_Rate;
                investigationMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                investigationMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                investigationMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                investigationMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                investigationMaster.HospitalID = HospitalID;
                investigationMaster.Isactive = model.Isactive;
                if (validation == true)
                {
                    result = _investigationRepo.CreateNewInvestigation(investigationMaster);
                    string EventName = "New Investigation Added-" + model.Investigation_Name;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    investigationMaster.Investigation_Seqid = model.Investigation_Seqid;
                    result = _investigationRepo.UpdateInvestigation(investigationMaster);
                    string EventName = "Update Investigation Master-" + model.Investigation_Name;
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