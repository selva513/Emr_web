using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Emr_web.Common;
using Syncfusion.EJ2.Navigations;
using BizLayer.Domain;
using Microsoft.AspNetCore.Http;
using BizLayer.Utilities;
using BizLayer.Interface;
using Emr_web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authorization;
using System.Data;
namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DrugController : Controller
    {
        private IDBConnection _IDBConnection;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDrugRepo _drugRepo;

        public DrugController(IDBConnection iDBConnection, IHostingEnvironment hostingEnvironment,IDrugRepo drugRepo)
        {
            _IDBConnection = iDBConnection;
            _hostingEnvironment = hostingEnvironment;
            _drugRepo = drugRepo;
        }
        public IActionResult DrugMaster()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DrugMastreSaveAsync([Bind] DrugMaster model)
        {
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                List<Login> logins = myComplexObject;
                var path = Path.Combine(
                   Directory.GetCurrentDirectory(), "wwwroot\\Files",
                   model.FilePatientDocment.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.FilePatientDocment.CopyToAsync(stream);
                }
                if (CommonSetting.IsValidImportCSVfile(path.ToString(), ','))
                {
                    DataTable dtDrug = CommonSetting.ParseCSVFile(path.ToString(), ',', true);
                    if (dtDrug.Rows.Count > 0)
                    {
                        for(int count = 0; count < dtDrug.Rows.Count; count++)
                        {
                            DrugMasterInfo drugMasterInfo = new DrugMasterInfo
                            {
                                DrugName = dtDrug.Rows[count]["DrugName"].ToString(),
                                Category = dtDrug.Rows[count]["Category"].ToString(),
                                Uom = dtDrug.Rows[count]["Uom"].ToString(),
                                Gst = Convert.ToDecimal(dtDrug.Rows[count]["Gst"]),
                                ScheduleType= dtDrug.Rows[count]["ScheduleType"].ToString(),
                                HSnCode= dtDrug.Rows[count]["HSnCode"].ToString(),
                                Company= dtDrug.Rows[count]["Company"].ToString(),
                                Type= dtDrug.Rows[count]["Type"].ToString(),
                                CreateUser= logins[0].UserSeqid.ToString(),
                                ModifieDatetime= timezoneUtility.Gettimezone(Timezoneid),
                                ModifedUser =logins[0].UserSeqid.ToString(),
                                IsActive=true,
                                HospitalID= Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                            };
                            long Result = _drugRepo.CreateNewDrug(drugMasterInfo);
                        }
                    }
                }
                List<MyPatient> lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                TempData["Upload"] = "Upload Successfull";
            }
            catch
            {
                TempData["Upload"] = "Upload Successfull";
            }
            return RedirectToAction("DrugMaster", "Drug");
        }
        [HttpPost]
        public JsonResult CreateNewDrugMaster([FromBody] DrugMaster model)
        {
            string Result = "";
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            DrugMasterInfo drugMasterInfo = new DrugMasterInfo
            {
                DrugName = model.DrugName,
                Category = model.Category,
                Uom = model.Uom,
                Gst = model.Gst,
                ScheduleType = model.ScheduleType,
                HSnCode = model.HSnCode,
                Company = model.Company,
                Type = model.Type,
                CreateUser = HttpContext.Session.GetString("Userseqid").ToString(),
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifedUser = HttpContext.Session.GetString("Userseqid").ToString(),
                IsActive = true,
                HospitalID= Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))

            };
            long count = _drugRepo.CreateNewDrug(drugMasterInfo);
            if (count > 0)
                Result = "Drug Added Successful";
            else
                Result = "Drug Already Exists";
            return Json(Result);
        }
        [HttpGet]
        public JsonResult GetDrugTop100()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            List<DrugMasterInfo> lstResult = new List<DrugMasterInfo>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _drugRepo.GetDrugTop100(HospitalID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                Drug = lstResult,
            });
        }
        [HttpGet]
        public JsonResult GetDrugSearch(string Search)
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            List<DrugMasterInfo> lstResult = new List<DrugMasterInfo>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _drugRepo.GetDrugBySearch(Search, HospitalID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                Drug = lstResult,
            });
        }
        [HttpPost]
        public JsonResult UpdateDrugMaster([FromBody] DrugMaster model)
        {
            string Result = "";
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            DrugMasterInfo drugMasterInfo = new DrugMasterInfo
            {
                SeqID=model.SeqID,
                DrugName = model.DrugName,
                Category = model.Category,
                Uom = model.Uom,
                Gst = model.Gst,
                ScheduleType = model.ScheduleType,
                HSnCode = model.HSnCode,
                Company = model.Company,
                Type = model.Type,
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifedUser = HttpContext.Session.GetString("Userseqid").ToString(),
                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
            };
            long count = _drugRepo.UpdateDrugMaster(drugMasterInfo);
            if (count > 0)
                Result = "Drug Updated Successful";
            else
                Result = "Drug Already Exists";
            return Json(Result);
        }
    }
}