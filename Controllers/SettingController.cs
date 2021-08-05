using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Emr_web.Models;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using Microsoft.AspNetCore.Http;
using Syncfusion.EJ2.Navigations;
using Emr_web.Common;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SettingController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private ISettingRepo _settingRepo;
        public SettingController(IDBConnection iDBConnection, IErrorlog errorlog,ISettingRepo settingRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _settingRepo = settingRepo;
        }
        public IActionResult Setting()
        {
            return View();
        }
        public IActionResult ReportSetting()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            return View();
        }
        [HttpGet]
        public ReportView[] GetReportConfig()
        {
            DataTable dtResult = new DataTable();
            List<ReportView> lstreportconfig = new List<ReportView>();
            try
            {
                dtResult = _settingRepo.GetAllReportConfig();
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        ReportView reportView = new ReportView();
                        reportView.ReportSeqID = Convert.ToInt64(dtResult.Rows[i]["ReportSeqID"]);
                        reportView.Report_Name = dtResult.Rows[i]["Report_Name"].ToString();
                        reportView.IsActive = Convert.ToBoolean( dtResult.Rows[i]["IsActive"]);
                        lstreportconfig.Add(reportView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstreportconfig.ToArray();
        }
        [HttpGet]
        public ReportView[] GetAllReportConfig()
        {
            DataTable dtResult = new DataTable();
            List<ReportView> lstreportconfig = new List<ReportView>();
            try
            {
                dtResult = _settingRepo.GetAllReportConfig();
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        ReportView reportView = new ReportView();
                        reportView.ReportSeqID = Convert.ToInt64(dtResult.Rows[i]["ReportSeqID"]);
                        reportView.Report_Name = dtResult.Rows[i]["Report_Name"].ToString();
                        reportView.AllReport_IsActive = Convert.ToBoolean(dtResult.Rows[i]["AllReport_IsActive"]);
                        lstreportconfig.Add(reportView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstreportconfig.ToArray();
        }
        [HttpPost]
        public JsonResult UpdateConfigDetails([FromBody] ReportView[] ReportArray)
        {
            string Result = "SaveResult";
            if (ReportArray.Length > 0)
            {
                HttpContext.Session.SetObjectAsJsonLsit("VisitConfigDetails", ReportArray);
                for (int count = 0; count < ReportArray.Length; count++)
                {
                    long ReportSeqid = ReportArray[count].ReportSeqID;
                    string ReportName = ReportArray[count].Report_Name;
                    bool Isactive = ReportArray[count].IsActive;
                    bool IsSuccess = _settingRepo.UpdateConfigDetails(ReportSeqid, ReportName, Isactive);
                }
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult UpdateAllConfigDetails([FromBody] ReportView[] ReportArray)
        {
            string Result = "SaveResult";
            if (ReportArray.Length > 0)
            {
                for (int count = 0; count < ReportArray.Length; count++)
                {
                    long ReportSeqid = ReportArray[count].ReportSeqID;
                    string ReportName = ReportArray[count].Report_Name;
                    bool Isactive = ReportArray[count].AllReport_IsActive;
                    bool IsSuccess = _settingRepo.UpdateAllConfigDetails(ReportSeqid, ReportName, Isactive);
                }
            }
            return Json(Result);
        }
    }
}