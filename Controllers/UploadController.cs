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
    public class UploadController : Controller
    {
        private IDBConnection _IDBConnection;
        private IPatientDocmentsRepo _patientDocmentsRepo;
        private IConfiguration _configuration;
        private readonly IPatientRepo _patientRepo;
        private readonly IMobileUserRepo _mobileUserRepo;
        public UploadController(IDBConnection iDBConnection, IPatientDocmentsRepo patientDocmentsRepo, IConfiguration configuration,IPatientRepo patientRepo,IMobileUserRepo mobileUserRepo)
        {
            _IDBConnection = iDBConnection;
            _patientDocmentsRepo = patientDocmentsRepo;
            _configuration = configuration;
            _patientRepo = patientRepo;
            _mobileUserRepo = mobileUserRepo;
        }
        public IActionResult Upload()
        {
            List<MyPatient> lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
            string HISVstid = lstresult[0].HISVisitID;
            if (HISVstid != null && HISVstid != "")
                HttpContext.Session.SetString("IsHISPatient", "1");
            else
                HttpContext.Session.SetString("IsHISPatient", "0");
            string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
            string IsConnectedHIS = HttpContext.Session.GetString("IsConnectedHIS");


            return View();
        }

       [HttpPost]
        [Route("/Upload/PatientDocmentSaveAsync", Name = "NamedRoute")]
        public async Task<IActionResult> PatientDocmentSaveAsync([Bind] UploadModel model)
        {
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string UploadPath = _configuration.GetConnectionString("UploadPath");
                //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", model.FilePatientDocment.FileName);
                string urlpath = Path.Combine(UploadPath, model.FilePatientDocment.FileName); 
                using (var stream = new FileStream(urlpath, FileMode.Create))
                {
                    await model.FilePatientDocment.CopyToAsync(stream);
                }
                model.DescriptionID = _patientDocmentsRepo.GetDescriptionIDByName(model.DescriptionName);
                List<MyPatient> lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                string HISVstid = lstresult[0].VisitID;
                string PatientID = lstresult[0].PatientID;
                long DoctorID = Convert.ToInt64(lstresult[0].DoctorID);
                DataTable dtResult = new DataTable();
                dtResult = _patientRepo.GetPatientByID(PatientID);
                MobileUserInfo mobileUser = new MobileUserInfo();
                if (dtResult.Rows.Count > 0)
                {
                    long mobileNo = Convert.ToInt64(dtResult.Rows[0]["MobileNumber"]);
                    mobileUser = _mobileUserRepo.GetMobileUserInfoByMobile(mobileNo);
                }
                PatientDocments patientDocments = new PatientDocments
                {
                    PatientId = PatientID,
                    PatientVisitId = HISVstid,
                    DoctorID = DoctorID,
                    TagName = model.TagName,
                    DescriptionID = model.DescriptionID,
                    PatientScanDocments = null,
                    PatientDocmentsPath = model.FilePatientDocment.FileName,
                    FileName = model.FilePatientDocment.FileName,
                    ContentType = model.FilePatientDocment.ContentType,
                    ContentDisposition = model.FilePatientDocment.ContentDisposition,
                    CreateUser = HttpContext.Session.GetString("Userseqid"),
                    CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    MobileUserSeqID = mobileUser.UserSeqId,
                    FamilyMemberSeqID = 0
                };
                int Result = _patientDocmentsRepo.CreateNewPatientDocment(patientDocments);
                if (Result > 0)
                {
                    if (patientDocments.DoctorID > 0)
                    {
                       
                        if (!string.IsNullOrWhiteSpace(patientDocments.PatientId))
                        {
                            patientDocments.PatientId = patientDocments.PatientId;
                            patientDocments.DoctorID = patientDocments.DoctorID;
                        }
                        _patientDocmentsRepo.InsertDocumentMapping(patientDocments);
                    }
                }
                TempData["Upload"] = "Upload Successfull";
            }
            catch
            {
                TempData["Upload"] = "Upload Successfull";
            }
            return RedirectToAction("EmrView", "Emr");
        }
        [HttpGet]
        public JsonResult GetPatientDocumentByPatID(string PatientId)
        {
            List<PatientDocments> lstResult = new List<PatientDocments>();
            List<MyPatient> lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
            long DoctorID = Convert.ToInt64(lstresult[0].DoctorID);
            try
            {
                lstResult = _patientDocmentsRepo.GetPatientDocumentsByPatientID(PatientId, DoctorID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatDoc = lstResult
            });
        }
        [HttpGet]
        public JsonResult GetPatientDocumentBySearch(string Search, string PatientID)
        {
            List<PatientDocments> lstResult = new List<PatientDocments>();
            try
            {
                lstResult = _patientDocmentsRepo.GetPatientDocumentsBySearch(Search, PatientID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatDoc = lstResult
            });
        }
        public IActionResult PdfView()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }
        [HttpGet]
        public JsonResult DeletePatinetDocument(long SeqID)
        {
            int Result = _patientDocmentsRepo.DeletePatientBySeqId(SeqID);
            return Json(Result);
        }
        public static string UploadPath()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = builder.Build();
            string constring = config.GetConnectionString("UploadPath");
            return constring;
        }
    }
}