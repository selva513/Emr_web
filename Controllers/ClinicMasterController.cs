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

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ClinicMasterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IClinicRepo _clinicrepo;
        private IPatientRepo _patientRepo;
        private IErrorlog _errorlog;
        public ClinicMasterController(IDBConnection iDBConnection, IClinicRepo ClinicRepo, IPatientRepo patientRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _clinicrepo = ClinicRepo;
            _patientRepo = patientRepo;
            _errorlog = errorlog;
        }
        public async Task<IActionResult> ClinicMaster()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                GetCity();
                GetAllCountry();
                GetAllHospital();
                string Hospitalid = HttpContext.Session.GetString("Hospitalid");
                var lst = await _clinicrepo.GetAllClinic(Hospitalid);
                if (lst != null)
                {
                    lst = lst as List<ClinicMaster>;
                    ViewBag.ClinicDetails = lst;
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
        private void GetAllHospital()
        {
            try
            {
                List<HospitalMaster> list = _clinicrepo.GetHospitalList();
                ViewBag.Hospital = list;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClinicSubmit([Bind]ClinicView model)
        {
            bool issuccess = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                long Licenceid = Convert.ToInt64(HttpContext.Session.GetString("Licenceid"));
                int Cliniccount = _clinicrepo.GetClinicCount(Licenceid);
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                int TotalClinicCount = _clinicrepo.GetClinicCountByHospital(Hospitalid);
                TotalClinicCount = TotalClinicCount + 1;
                ClinicMaster clinicMaster = new ClinicMaster();
                clinicMaster.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                clinicMaster.ClinicIdentifier = model.ClinicIdentifier;
                clinicMaster.ClinicName = model.ClinicName;
                clinicMaster.ClinicMobileNo = model.ClinicMobileNo;
                clinicMaster.ClinicLandlineNo = model.ClinicLandlineNo;
                clinicMaster.ClinicLandlineNo1 = model.ClinicLandlineNo1;
                clinicMaster.ClinicAddress = model.ClinicAddress;
                clinicMaster.ClinicAddress1 = model.ClinicAddress1;
                clinicMaster.ClinicAddress2 = model.ClinicAddress2;
                clinicMaster.City = model.City;
                clinicMaster.Country = model.Country;
                clinicMaster.Pin = model.Pin;
                if (model.FileClinic != null)
                {
                    if (model.FileClinic.Length > 0)
                        clinicMaster.CliniclLogo = CommonSetting.GetImageBytes(model.FileClinic);
                }
                if (model.FileClinic1 != null)
                {
                    if (model.FileClinic1.Length > 0)
                        clinicMaster.OtherLogo = CommonSetting.GetImageBytes(model.FileClinic1);
                }
                clinicMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                clinicMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                clinicMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                clinicMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                clinicMaster.Isactive = true;
                if (model.ClinicID == 0)
                {
                    if (TotalClinicCount <= Cliniccount)
                    {
                        issuccess = _clinicrepo.InsertNewClinic(clinicMaster);
                    }
                    else
                    {
                        TempData["ClinicFailed"] = "Clinic Count Exceeded.Please Contact Allied Business Solutions";
                    }
                }
                else
                {
                    clinicMaster.ClinicID = model.ClinicID;
                    issuccess = _clinicrepo.UpdateClinic(clinicMaster);
                }

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("ClinicMaster", "ClinicMaster");
        }
    }
}