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
    public class DiagnosisMasterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IDiagnosisRepo _diagnosisRepo;
        private IErrorlog _errorlog;
        public DiagnosisMasterController(IDBConnection iDBConnection, IDiagnosisRepo diagnosisRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _diagnosisRepo = diagnosisRepo;
            _errorlog = errorlog;
        }
        public async Task<IActionResult> DiagnosisMaster()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                var lst = await _diagnosisRepo.GetAllDiagnosis(HospitalID);
                if (lst != null)
                {
                    lst = lst as List<DiagnosisMaterCls>;
                    ViewBag.DiagnosisDetail = lst;
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
        public IActionResult InsertDiagnosis([Bind]DiagnosisMasterView model)
        {
            int result = 0;
            try
            {
                if (model.Diagnosis_Seqid == 0)
                    result = InsertNewDiagnosis(model, true);
                else
                    result = InsertNewDiagnosis(model, false);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("DiagnosisMaster", "DiagnosisMaster");
        }
        private int InsertNewDiagnosis(DiagnosisMasterView model, bool validation)
        {
            int result = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                DiagnosisMaterCls diagnosisMaterCls = new DiagnosisMaterCls();
                diagnosisMaterCls.Diagnosis_Name = model.Diagnosis_Name;
                diagnosisMaterCls.ICD10 = model.ICD10;
                diagnosisMaterCls.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                diagnosisMaterCls.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                diagnosisMaterCls.CreatedUser = HttpContext.Session.GetString("Userseqid");
                diagnosisMaterCls.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                diagnosisMaterCls.HospitalID = HospitalID;
                diagnosisMaterCls.IsActive = model.IsActive;
                if (validation == true)
                    result = _diagnosisRepo.CreateNewDiagnosis(diagnosisMaterCls);
                else
                {
                    diagnosisMaterCls.Diagnosis_Seqid = model.Diagnosis_Seqid;
                    result = _diagnosisRepo.UpdateDiagnosis(diagnosisMaterCls);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
    }
}