using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using BizLayer.Repo;
using Syncfusion.EJ2.Navigations;
using Emr_web.Common;
using Microsoft.AspNetCore.Authorization;
using Emr_web.Models;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Globalization;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MyPatientController : Controller
    {
        private IDBConnection _IDBConnection;
        private IPatientRepo _patientRepo;
        private IErrorlog _errorlog;
        List<MyPatient> _lstPat = null;
        private IHISPatientRepo _IHISPatientRepo;
        private IDoctorRepo _doctorRepo;
        private ILoginRepo _loginRepo;
        public MyPatientController(IDBConnection iDBConnection, IPatientRepo patientRepo, IErrorlog errorlog, IHISPatientRepo iHISPatientRepo, IDoctorRepo doctorRepo, ILoginRepo loginRepo)
        {
            _IDBConnection = iDBConnection;
            _patientRepo = patientRepo;
            _errorlog = errorlog;
            _IHISPatientRepo = iHISPatientRepo;
            _doctorRepo = doctorRepo;
            _loginRepo = loginRepo;
        }

        public async Task<IActionResult> MyPatient()
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            string CurrentDateTime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
            string CurDatetime = timezoneUtility.Gettimezone(Timezoneid);
            _patientRepo.UpdateStatus(CurDatetime);
            string imagebind = "Disable";
            List<MyPatient> _Todaylistbypatid = null;
            long Userseqid = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
            List<Login> logins = new List<Login>();
            logins = _loginRepo.GetUserDetailsByID(Userseqid, CurDatetime);
            long Doctorid = logins[0].DoctorID;
            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            var lstPat = await _patientRepo.GetMyPatientWithCheck_In(Doctorid, Hospitalid, CurDatetime);
            if (lstPat.Count() > 0 && lstPat != null)
            {
                
                DateTime dtcurrentdatetime = Convert.ToDateTime(CurrentDateTime);
                _lstPat = lstPat as List<MyPatient>;
                _lstPat = _lstPat.GroupBy(i => i.PatientID)
                .Select(g => g.First()).ToList();
                for (int i = 0; i < _lstPat.Count(); i++)
                {
                    long MobileNo = _lstPat[i].MobileNumber;
                    string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                    string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    var Todaylistbypatid = await _patientRepo.TodayMyPatientlistbypatid(Doctorid, Hospitalid, MobileNo, Fromdate, Todate);
                    _Todaylistbypatid = Todaylistbypatid as List<MyPatient>;
                    for (int j = 0; j < _Todaylistbypatid.Count(); j++)
                    {
                        string StartDatetime = _Todaylistbypatid[j].StartDatetime;
                        string EndDatetime = _Todaylistbypatid[j].EndDatetime;
                        if (StartDatetime != null && EndDatetime != null)
                        {
                            DateTime dtstarttime = Convert.ToDateTime(StartDatetime);
                            DateTime dtendtime = Convert.ToDateTime(EndDatetime);
                            if (dtcurrentdatetime >= dtstarttime && dtcurrentdatetime <= dtendtime)
                            {
                                imagebind = _Todaylistbypatid[j].imagebind;
                                _lstPat[i].imagebind = imagebind;
                            }
                        }
                    }
                    //int Interval = _lstPat[i].Interval;
                    //string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
                    //TimeSpan timeSessionend = TimeSpan.Parse(timezoneUtility.GettimeBytimezone(Timezoneid));
                    //timeSessionend = timeSessionend + TimeSpan.FromMinutes(Interval);
                    //string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timeSessionend.ToString();
                }
            }
            ViewBag.MyPatient = _lstPat;
            ViewBag.AllPatient = false;
            ViewBag.AllDoctor = false;
            return View("MyPatient");
        }

        public async Task<IActionResult> Search(bool AllPatient, bool AllDoctor)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            string CurrentDateTime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
            string CurDatetime = timezoneUtility.Gettimezone(Timezoneid);
            DateTime dtcurrentdatetime = Convert.ToDateTime(CurrentDateTime);
            string imagebind = "Disable";
            List<MyPatient> _Todaylistbypatid = null;
            long Doctorid = Convert.ToInt64(HttpContext.Session.GetString("Doctorid"));
            if (AllPatient)
            {
                if (AllDoctor)
                {
                    ViewBag.AllDoctor = true;
                    ViewBag.AllPatient = true;
                    long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    var lstPat = await _patientRepo.GetAllPatientPageIndex(HospitalId, Doctorid, CurDatetime);
                    if (lstPat.Count() > 0)
                    {
                        _lstPat = lstPat as List<MyPatient>;
                        _lstPat = _lstPat.GroupBy(i => i.PatientID)
                        .Select(g => g.First()).ToList();
                        for (int i = 0; i < _lstPat.Count(); i++)
                        {
                            string Chckincheck = _lstPat[i].Status;
                            if (Chckincheck == "Checked-In")
                            {
                                long MobileNo = _lstPat[i].MobileNumber;
                                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                                var Todaylistbypatid = await _patientRepo.TodayMyPatientlistbypatid(Doctorid, HospitalId, MobileNo, Fromdate, Todate);
                                _Todaylistbypatid = Todaylistbypatid as List<MyPatient>;
                                for (int j = 0; j < _Todaylistbypatid.Count(); j++)
                                {
                                    string StartDatetime = _Todaylistbypatid[j].StartDatetime;
                                    string EndDatetime = _Todaylistbypatid[j].EndDatetime;
                                    if (StartDatetime != null && EndDatetime != null)
                                    {
                                        DateTime dtstarttime = Convert.ToDateTime(StartDatetime);
                                        DateTime dtendtime = Convert.ToDateTime(EndDatetime);
                                        if (dtcurrentdatetime >= dtstarttime && dtcurrentdatetime <= dtendtime)
                                        {
                                            imagebind = _Todaylistbypatid[j].imagebind;
                                            _lstPat[i].imagebind = imagebind;
                                        }
                                    }
                                }
                            }
                        }
                        ViewBag.MyPatient = _lstPat;
                    }
                }
                else
                {
                    ViewBag.AllPatient = true;
                    ViewBag.AllDoctor = false;
                    long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    var lstPat = await _patientRepo.GetMyPatient(Doctorid, HospitalId, CurDatetime);
                    if (lstPat.Count() > 0)
                    {
                        _lstPat = lstPat as List<MyPatient>;
                        _lstPat = _lstPat.GroupBy(i => i.PatientID)
                        .Select(g => g.First()).ToList();
                        for (int i = 0; i < _lstPat.Count(); i++)
                        {
                            string Chckincheck = _lstPat[i].Status;
                            if(Chckincheck=="Checked-In")
                            {
                                long MobileNo = _lstPat[i].MobileNumber;
                                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                                var Todaylistbypatid = await _patientRepo.TodayMyPatientlistbypatid(Doctorid, HospitalId, MobileNo, Fromdate, Todate);
                                _Todaylistbypatid = Todaylistbypatid as List<MyPatient>;
                                for (int j = 0; j < _Todaylistbypatid.Count(); j++)
                                {
                                    string StartDatetime = _Todaylistbypatid[j].StartDatetime;
                                    string EndDatetime = _Todaylistbypatid[j].EndDatetime;
                                    if (StartDatetime != null && EndDatetime != null)
                                    {
                                        DateTime dtstarttime = Convert.ToDateTime(StartDatetime);
                                        DateTime dtendtime = Convert.ToDateTime(EndDatetime);
                                        if (dtcurrentdatetime >= dtstarttime && dtcurrentdatetime <= dtendtime)
                                        {
                                            imagebind = _Todaylistbypatid[j].imagebind;
                                            _lstPat[i].imagebind = imagebind;
                                        }
                                    }
                                }
                            }
                        }
                        ViewBag.MyPatient = _lstPat;
                    }
                }
            }
            else
            {
                if (AllDoctor)
                {
                    ViewBag.AllDoctor = true;
                    ViewBag.AllPatient = false;
                    long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    var lstPat = await _patientRepo.GetAllDoctorWithCheck_In(HospitalId, Doctorid, CurDatetime);
                    if (lstPat.Count() > 0)
                    {
                        _lstPat = lstPat as List<MyPatient>;
                        _lstPat = _lstPat.GroupBy(i => i.PatientID)
                        .Select(g => g.First()).ToList();
                        for (int i = 0; i < _lstPat.Count(); i++)
                        {
                            string Chckincheck = _lstPat[i].Status;
                            if (Chckincheck == "Checked-In")
                            {
                                long MobileNo = _lstPat[i].MobileNumber;
                                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                                var Todaylistbypatid = await _patientRepo.TodayMyPatientlistbypatid(Doctorid, HospitalId, MobileNo, Fromdate, Todate);
                                _Todaylistbypatid = Todaylistbypatid as List<MyPatient>;
                                for (int j = 0; j < _Todaylistbypatid.Count(); j++)
                                {
                                    string StartDatetime = _Todaylistbypatid[j].StartDatetime;
                                    string EndDatetime = _Todaylistbypatid[j].EndDatetime;
                                    if (StartDatetime != null && EndDatetime != null)
                                    {
                                        DateTime dtstarttime = Convert.ToDateTime(StartDatetime);
                                        DateTime dtendtime = Convert.ToDateTime(EndDatetime);
                                        if (dtcurrentdatetime >= dtstarttime && dtcurrentdatetime <= dtendtime)
                                        {
                                            imagebind = _Todaylistbypatid[j].imagebind;
                                            _lstPat[i].imagebind = imagebind;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ViewBag.MyPatient = _lstPat;
                }
                else
                {
                    long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    var lstPat = await _patientRepo.GetMyPatientWithCheck_In(Doctorid, HospitalId, CurDatetime);
                    if (lstPat.Count() > 0)
                    {
                        _lstPat = lstPat as List<MyPatient>;
                        _lstPat = _lstPat.GroupBy(i => i.PatientID)
                        .Select(g => g.First()).ToList();
                        for (int i = 0; i < _lstPat.Count(); i++)
                        {
                            string Chckincheck = _lstPat[i].Status;
                            if (Chckincheck == "Checked-In")
                            {
                                long MobileNo = _lstPat[i].MobileNumber;
                                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                                var Todaylistbypatid = await _patientRepo.TodayMyPatientlistbypatid(Doctorid, HospitalId, MobileNo, Fromdate, Todate);
                                _Todaylistbypatid = Todaylistbypatid as List<MyPatient>;
                                for (int j = 0; j < _Todaylistbypatid.Count(); j++)
                                {
                                    string StartDatetime = _Todaylistbypatid[j].StartDatetime;
                                    string EndDatetime = _Todaylistbypatid[j].EndDatetime;
                                    if (StartDatetime != null && EndDatetime != null)
                                    {
                                        DateTime dtstarttime = Convert.ToDateTime(StartDatetime);
                                        DateTime dtendtime = Convert.ToDateTime(EndDatetime);
                                        if (dtcurrentdatetime >= dtstarttime && dtcurrentdatetime <= dtendtime)
                                        {
                                            imagebind = _Todaylistbypatid[j].imagebind;
                                            _lstPat[i].imagebind = imagebind;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ViewBag.MyPatient = _lstPat;
                    ViewBag.AllDoctor = false;
                    ViewBag.AllPatient = false;
                }
            }
            return View("MyPatient");
        }
        [HttpPost]
        public JsonResult StorePatData([FromBody] MyPatient patdet)
        {
            List<MyPatient> lstResult = new List<MyPatient>();
            try
            {
                long LoginDocid = Convert.ToInt64(HttpContext.Session.GetString("Doctorid"));
                long CurrentDocid = patdet.DoctorID;
                long DoctorMobileno = _patientRepo.GetDoctorMobileno(CurrentDocid);
                if (LoginDocid == CurrentDocid)
                    patdet.DoctorCompare = true;
                else
                    patdet.DoctorCompare = false;
                MyPatient patientView = new MyPatient
                {
                    PatientID = patdet.PatientID,
                    FirstName = patdet.FirstName,
                    Gender = patdet.Gender,
                    Age = patdet.Age,
                    RefDoctor = patdet.RefDoctor,
                    VisitID = patdet.VisitID,
                    HISVisitID = patdet.HISVisitID,
                    HISDocID = patdet.HISDocID,
                    HISDeptID = patdet.HISDeptID,
                    VisitDate = patdet.VisitDate,
                    Status = patdet.Status,
                    DoctorID = patdet.DoctorID,
                    DoctorCompare = patdet.DoctorCompare,
                    Speciality_Name = patdet.Speciality_Name,
                    ClinicID = patdet.ClinicID,
                    DoctorMobileno = DoctorMobileno
                };
                lstResult.Add(patientView);
                HttpContext.Session.SetObjectAsJsonLsit("Patientlist", lstResult.ToArray());

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult GetAllPatientByToday()
        {
            List<HISPatient> lstResult = new List<HISPatient>();
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long Doctorid = Convert.ToInt64(HttpContext.Session.GetString("Doctorid"));
                long HISDocid = _doctorRepo.GetHISDocid(Doctorid);
                lstResult = _IHISPatientRepo.GetAllPatientByToday(Fromdate, Todate, HISDocid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                HisPta = lstResult
            });
        }
        [HttpGet]
        public JsonResult GetAllPatientByTodayBySearch(string SearchTerm)
        {
            List<HISPatient> lstResult = new List<HISPatient>();
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                long Doctorid = Convert.ToInt64(HttpContext.Session.GetString("Doctorid"));
                long HISDocid = _doctorRepo.GetHISDocid(Doctorid);
                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                lstResult = _IHISPatientRepo.GetAllPatientByTodayBySearch(Fromdate, Todate, SearchTerm, HISDocid);
                if (string.IsNullOrWhiteSpace(SearchTerm))
                    lstResult = _IHISPatientRepo.GetAllPatientByToday(Fromdate, Todate, HISDocid);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                HisPta = lstResult
            });
        }
        [HttpPost]
        public JsonResult CreateNewHISPatient([FromBody] PatientView model)
        {
            bool isSuccess = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(model.PatientID))
                {
                    if (_patientRepo.IsPatientExists(model.PatientID))
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                            Timezoneid = "India Standard Time";
                        string CurrentDateTime = timezoneUtility.Gettimezone(Timezoneid);

                        string HospitalID = HttpContext.Session.GetString("Hospitalid");
                        _patientRepo.UpdatePatientHospital(HospitalID, model.PatientID);
                        int Docid = Convert.ToInt32(HttpContext.Session.GetString("Doctorid"));
                        model.DoctorID = Docid;
                        if (!_patientRepo.IsTodayDocPatientExists(model.PatientID, (int)Docid,CurrentDateTime))
                            isSuccess = InsertNewVisit(model);
                    }
                    else
                    {
                        isSuccess = InsertNewPatient(model);
                        TempData["Register"] = "Patient Create Success full";
                    }
                }
                else
                {
                    isSuccess = InsertNewPatient(model);
                    TempData["Register"] = "Patient Create Success full";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(isSuccess);
        }
        private bool InsertNewPatient(PatientView model)
        {
            bool isSuccess = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                Patient patient = new Patient();
                if (string.IsNullOrWhiteSpace(model.PatientID))
                {
                    long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    patient.PatientID = _patientRepo.AutoGenreationPatientID(Hospitalid);
                }
                else
                    patient.PatientID = model.PatientID;
                patient.FirstName = model.FirstName;
                patient.SecondName = model.SecondName;
                patient.Gender = model.Gender;
                if (!string.IsNullOrWhiteSpace(model.BirthDate))
                {
                    //patient.BirthDate = Convert.ToDateTime(model.BirthDate);
                    patient.BirthDate = GetDataformat(model.BirthDate, DateTime.Now);
                }

                patient.PhoneNumber = model.PhoneNumber;
                if (model.MobileNumber != null)
                    patient.MobileNumber = (long)model.MobileNumber;
                patient.Email = model.Email;
                patient.RelationType = model.RelationType;
                patient.RelationName = model.RelationName;
                patient.PatientAddress1 = model.PatientAddress1;
                patient.PatientAddress2 = model.PatientAddress2;
                if (model.City != null)
                    patient.City = (int)model.City;
                if (model.State != null)
                    patient.State = (int)model.State;
                if (model.Country != null)
                    patient.Country = (int)model.Country;
                patient.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                patient.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                patient.CreatedUser = HttpContext.Session.GetString("Userseqid");
                patient.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                patient.HospitalID = HttpContext.Session.GetString("Hospitalid");
                patient.IsActive = true;
                isSuccess = _patientRepo.CreateNewPatient(patient);
                if (isSuccess)
                {
                    PatientRegistrationDetails details = new PatientRegistrationDetails();
                    details.PatSeqID = patient.PatSeqID;
                    details.VisitID = Common.CommonSetting.generateStudyID();
                    details.VisitDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    if (model.AgeYear != null)
                        details.AgeYear = (int)model.AgeYear;
                    if (model.AgeMonth != null)
                        details.AgeMonth = (int)model.AgeMonth;
                    if (model.AgeDay != null)
                        details.AgeDay = (int)model.AgeDay;
                    details.DoctorID = Convert.ToInt32(HttpContext.Session.GetString("Doctorid"));
                    details.DeptID = _doctorRepo.GetDeptidByDocid(details.DoctorID);
                    details.Status = "Checked-In";
                    details.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    details.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    details.CreatedUser = HttpContext.Session.GetString("Userseqid");
                    details.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    details.HIS_VisitID = model.HIS_VisitID;
                    details.HIS_DocID = model.HIS_DocID;
                    details.HIS_DeptID = model.HIS_DeptID;
                    details.IsActive = true;
                    int result = _patientRepo.CreateNewVist(details);
                    if (result > 0)
                        isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return isSuccess;
        }
        private bool InsertNewVisit(PatientView model)
        {
            bool isSuccess = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                DataTable dtResult = _patientRepo.GetPatientByID(model.PatientID);
                long patSeqID = 0;
                if (dtResult.Rows.Count > 0)
                    patSeqID = Convert.ToInt64(dtResult.Rows[0]["PatSeqID"]);
                PatientRegistrationDetails details = new PatientRegistrationDetails();
                details.PatSeqID = patSeqID;
                details.VisitID = Common.CommonSetting.generateStudyID();
                details.VisitDatetime = timezoneUtility.Gettimezone(Timezoneid);
                if (model.AgeYear != null)
                    details.AgeYear = (int)model.AgeYear;
                if (model.AgeMonth != null)
                    details.AgeMonth = (int)model.AgeMonth;
                if (model.AgeDay != null)
                    details.AgeDay = (int)model.AgeDay;
                if (model.DoctorID != null)
                    details.DoctorID = (int)model.DoctorID;
                if (model.DeptID != null)
                    details.DeptID = (int)model.DeptID;
                details.Status = "Checked-In";
                details.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                details.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                details.CreatedUser = HttpContext.Session.GetString("Userseqid");
                details.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                details.HIS_VisitID = model.HIS_VisitID;
                details.HIS_DocID = model.HIS_DocID;
                details.HIS_DeptID = model.HIS_DeptID;
                details.IsActive = true;
                int result = _patientRepo.CreateNewVist(details);
                if (result > 0)
                    isSuccess = true;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return isSuccess;
        }
        [HttpGet]
        public JsonResult GetHISConnected()
        {
           string myconfig = "";
            try
            {
                long UserSeqID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                myconfig = _patientRepo.GetHISConnected(UserSeqID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(myconfig);
        }

        public static DateTime GetDataformat(string DateValue, DateTime date)
        {
            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            try
            {
                if (DateValue != null || DateValue != "")
                {
                    IFormatProvider cultureDDMMYYYY = new CultureInfo("fr-Fr", true);
                    IFormatProvider cultureMMDDYYYY = new CultureInfo("en-US", true);
                    DateTime currentDate = DateTime.Now;
                    IFormatProvider culture = cultureDDMMYYYY;
                    DateTime.TryParse(DateValue, culture, DateTimeStyles.NoCurrentDateDefault, out date);
                }
            }
            catch (Exception ex)
            {


            }
            return date;
        }
    }
}