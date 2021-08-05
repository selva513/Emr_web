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
using Syncfusion.EJ2.Navigations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class HospitalManagementController : Controller
    {
        private IDBConnection _IDBConnection;
        private IHospitalRepo _hospitalRepo;
        private IPatientRepo _patientRepo;
        private IErrorlog _errorlog;
        private IConfiguration _configuration;
        public HospitalManagementController(IDBConnection iDBConnection, IHospitalRepo hospitalRepo, IPatientRepo patientRepo, IErrorlog errorlog, IConfiguration configuration)
        {
            _IDBConnection = iDBConnection;
            _hospitalRepo = hospitalRepo;
            _patientRepo = patientRepo;
            _errorlog = errorlog;
            _configuration = configuration;
        }
        public async Task<IActionResult> HospitalManagement()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                GetCity();
                GetAllCountry();
                GetAllTimezone();
                string Hospitalid = HttpContext.Session.GetString("Hospitalid");
                var lst = await _hospitalRepo.GetAllHospital(Hospitalid);
                if (lst != null)
                {
                    lst = lst as List<HospitalMaster>;
                    ViewBag.HospitalDetails = lst;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        private void GetCity()
        {
            try
            {
                List<CityMaster> lstCity = _patientRepo.GetCityList();
                ViewBag.City = lstCity;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private void GetAllCountry()
        {
            try
            {
                List<CountryMaster> list = _patientRepo.GetAllCountry();
                ViewBag.Country = list;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private void GetAllTimezone()
        {
            try
            {
                List<TimezoneMaster> list = _hospitalRepo.GetAlltimezone();
                ViewBag.Timezone = list;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HospitalMaster([Bind] HospitalView model)
        {
            bool issuccess = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (ModelState.IsValid)
                {
                    string UploadPath = _configuration.GetConnectionString("HospitalLogoInsertPath");
                    string UploadgetPath = _configuration.GetConnectionString("HospitalLogogetPath");
                    string Location = Path.Combine(UploadPath, "NoImage.png");
                    string GetLocation = Path.Combine(UploadgetPath, "NoImage.png");
                    long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    string HospitalName = model.HospitalName;
                    HospitalMaster hospitalMaster = new HospitalMaster
                    {
                        HospitalID = HospitalID,
                        HospitalMobileNo = model.HospitalMobileNo,
                        HospitalLandlineNo = model.HospitalLandlineNo,
                        HospitalLandlineNo1 = model.HospitalLandlineNo1,
                        HospitalAddress = model.HospitalAddress,
                        HospitalAddress1 = model.HospitalAddress1,
                        HospitalAddress2 = model.HospitalAddress2,
                        City = model.City,
                        Country = model.Country,
                        Pin = model.Pin,
                        Prefix = model.Prefix,
                        TimezoneSeqID = model.TimezoneSeqID
                    };
                    if (model.FileHospital != null)
                    {
                        if (model.FileHospital.Length > 0)
                        {
                            hospitalMaster.HospitalLogo = CommonSetting.GetImageBytes(model.FileHospital);
                            string ImageName = HospitalID.ToString() + HospitalName;
                            string urlpath = Path.Combine(UploadPath, ImageName + "Logo.png");
                            string urlgetpath = Path.Combine(UploadgetPath, ImageName + "Logo.png");
                            var filePath = urlpath;
                            if (!System.IO.File.Exists(filePath))
                            {
                                System.IO.File.WriteAllBytes(filePath, hospitalMaster.HospitalLogo);
                            }
                            hospitalMaster.HospitalLogoUrl = urlgetpath;
                        }
                    }
                    else
                    {
                        List<HospitalMaster> lstResult = _hospitalRepo.GetHospitalImagebytes(HospitalID);
                        byte[] img = lstResult[0].HospitalLogo;
                        if (img != null && img.Length > 0)
                        {
                            hospitalMaster.HospitalLogoUrl = lstResult[0].HospitalLogoUrl;
                        }
                        else
                        {
                            string urlgetpath = Path.Combine(UploadgetPath, "NoImage.png");
                            hospitalMaster.HospitalLogoUrl = urlgetpath;
                        }
                    }
                    if (model.FileHospital1 != null)
                    {
                        if (model.FileHospital1.Length > 0)
                            hospitalMaster.OtherLogo = CommonSetting.GetImageBytes(model.FileHospital1);
                    }

                    hospitalMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    hospitalMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    hospitalMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                    hospitalMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    hospitalMaster.Isactive = true;
                    issuccess = _hospitalRepo.UpdateHospital(hospitalMaster);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("HospitalManagement", "HospitalManagement");
        }
    }
}