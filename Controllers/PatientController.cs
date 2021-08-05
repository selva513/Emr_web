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
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Navigations;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class PatientController : Controller
    {
        private IDBConnection _IDBConnection;
        private IPatientRepo _patientRepo;
        private IHISPatientRepo _HISPatientRepo;
        private IErrorlog _errorlog;
        private IHospitalRepo _hospitalRepo;
        private ICountryRepo _countryRepo;
        private IStateRepo _stateRepo;
        private ICityRepo _cityRepo;
        private IEmrRepo _emrRepo;
        private IMobileUserRepo _mobileUserRepo;
        private IPatientAppointmentRepo _patientAppointmentRepo;
        public PatientController(IDBConnection iDBConnection, IPatientRepo patientRepo, IHISPatientRepo HISPatientRepo, IErrorlog errorlog, IHospitalRepo hospitalRepo, ICountryRepo countryRepo, IStateRepo stateRepo, ICityRepo cityRepo, IEmrRepo emrRepo, IMobileUserRepo mobileUserRepo, IPatientAppointmentRepo patientAppointmentRepo)
        {
            _IDBConnection = iDBConnection;
            _patientRepo = patientRepo;
            _HISPatientRepo = HISPatientRepo;
            _errorlog = errorlog;
            _hospitalRepo = hospitalRepo;
            _countryRepo = countryRepo;
            _stateRepo = stateRepo;
            _cityRepo = cityRepo;
            _emrRepo = emrRepo;
            _patientAppointmentRepo = patientAppointmentRepo;
            _mobileUserRepo = mobileUserRepo;
        }
        public IActionResult PatientRegistration()
        {
            GetGenderValue();
            GetReleationType();
            GetDoctorByHospitalID();
            GetDepartmentByHospitalID();
            GetCity();
            GetAllState();
            GetAllCountry();
            GetAllClinic();
            GetAllSpeciality();
            GetBloodGroup();
            //GetGender();
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            ViewBag.maxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            return View();
        }
        private void GetCity()
        {
            List<CityMaster> lstCity = _patientRepo.GetCityList();
            ViewBag.City = lstCity;
        }
        private void GetGenderValue()
        {
            List<string> GenderLst = new List<string>();
            GenderLst.Add("Male");
            GenderLst.Add("Female");
            GenderLst.Add("Others");
            ViewBag.Gender = GenderLst;
        }
        private void GetReleationType()
        {
            List<string> RelType = new List<string>();
            RelType.Add("S/o");
            RelType.Add("D/o");
            RelType.Add("W/o");
            RelType.Add("C/o");
            ViewBag.RelType = RelType;
        }
        private void GetBloodGroup()
        {
            List<BloodGroupMaster> lstBloodGroup = new List<BloodGroupMaster>();
            lstBloodGroup = _patientRepo.GetAllBloodGroup();
            ViewBag.BloodGroup = lstBloodGroup;
        }
        private void GetGender()
        {
            List<GenderMaster> lstGender = new List<GenderMaster>();
            lstGender = _patientRepo.GetAllGender();
            ViewBag.Gender = lstGender;
        }
        private void GetDoctorByHospitalID()
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DoctorMaster> list = new List<DoctorMaster>();
            list = _patientRepo.GetDoctorByHospitalID(HospitalId);
            ViewBag.Doctor = list;
        }
        private void GetDepartmentByHospitalID()
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DepartmentMaster> list = new List<DepartmentMaster>();
            list = _patientRepo.GetDepartmentByHospitalID();
            ViewBag.Department = list;
        }
        private void GetAllState()
        {
            List<StateMaster> list = _patientRepo.GetAllState();
            ViewBag.State = list;
        }
        private void GetAllCountry()
        {
            List<CountryMaster> list = _patientRepo.GetAllCountry();
            ViewBag.Country = list;
        }
        private void GetAllClinic()
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<ClinicMaster> list = new List<ClinicMaster>();
            list = _patientRepo.GetClinicByHospitalID(HospitalId);
            ViewBag.Clinic = list;
        }
        private void GetAllSpeciality()
        {
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                List<SpecialityMaster> list = new List<SpecialityMaster>();
                list = _patientRepo.GetAllSpeciality();
                ViewBag.Speciality = list;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        [HttpPost]
        public JsonResult UpdatePatientMaster([FromBody] PatientView model)
        {
            List<PatientView> lstResult = new List<PatientView>();
            try
            {
                if (model != null)
                {
                    UpdatePatient(model);
                    UpdatePatientRegDetail(model);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstResult });
        }
        [HttpPost]
        public IActionResult PatientRegistration([Bind] PatientView model)
        {
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                string ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                if (model.Country == null || model.Country == 0)
                {
                    string Countryname = model.CountryName;
                    string Countrycode = model.CountryCode;
                    string Userid = HttpContext.Session.GetString("Userseqid");
                    int CountrySeqId = _countryRepo.InsertnewCountry(Countryname, Countrycode, Userid, CreatedDatetime, ModifiedDatetime);
                    model.Country = CountrySeqId;
                }
                if (model.State == null || model.State == 0)
                {
                    string Statename = model.StateName;
                    int CountryID = Convert.ToInt32(model.Country);
                    string Userid = HttpContext.Session.GetString("Userseqid");
                    int StateSeqId = _stateRepo.InsertnewState(Statename, CountryID, Userid, CreatedDatetime, ModifiedDatetime);
                    model.State = StateSeqId;
                }
                if (model.City == null || model.City == 0)
                {
                    string Cityname = model.CityName;
                    int CountryID = Convert.ToInt32(model.Country);
                    int StateID = Convert.ToInt32(model.State);
                    string Userid = HttpContext.Session.GetString("Userseqid");
                    int CitySeqId = _cityRepo.InsertnewCity(Cityname, CountryID, StateID, Userid, CreatedDatetime, ModifiedDatetime);
                    model.City = Convert.ToInt32(CitySeqId);
                }
                if (!string.IsNullOrWhiteSpace(model.PatientID))
                {
                    if (_patientRepo.IsPatientExists(model.PatientID))
                    {
                        if (!_patientRepo.IsTodayDocPatientExists(model.PatientID, (int)model.DoctorID, CreatedDatetime))
                            InsertNewVisit(model);
                    }
                    else
                    {
                        InsertNewPatient(model);
                        TempData["Register"] = "Patient Create Success full";
                    }
                }
                else
                {
                    InsertNewPatient(model);
                    TempData["Register"] = "Patient Create Success full";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("PatientRegistration", "Patient");
        }
        public async Task<IActionResult> UrlDatasource([FromBody] DataManagerRequest dm)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            int Pageindex = dm.Skip;
            if (Pageindex > 0)
                Pageindex = Pageindex / dm.Take + 1;
            else
                Pageindex = 1;
            int Pagesize = dm.Take;
            string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
            string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
            IEnumerable DataSource = await _HISPatientRepo.GetAllPatientByPageIndex(Pageindex, Pagesize, Fromdate, Todate);
            DataOperations operation = new DataOperations();
            int count = 0;
            try
            {
                count = 500;
                if (dm.Search != null && dm.Search.Count > 0)
                {
                    string key = dm.Search[0].Key;
                    DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
                }
                if (dm.Where != null && dm.Where.Count > 0) //Filtering
                {
                    DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                }


            }
            catch (Exception ex)
            {

                throw;
            }

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        private void InsertNewPatient(PatientView model)
        {
            try
            {
                Patient patient = new Patient();
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");

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
                string DOB = model.BirthDate;
                if (!string.IsNullOrWhiteSpace(model.BirthDate))
                {
                    DateTime ageDate = DateTime.Now;
                    DateTime dob1 = getDataformat(model.BirthDate, ageDate);
                    patient.BirthDate = dob1;
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
                patient.CityCode = model.CityCode;
                patient.CountryCode = model.CountryCode;
                patient.BloodGroup = model.BloodGroup;
                bool isSuccess = _patientRepo.CreateNewPatient(patient);
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
                    if (model.DoctorID != null)
                        details.DoctorID = (int)model.DoctorID;
                    if (model.DeptID != null)
                        details.DeptID = (int)model.DeptID;
                    if (model.SpecialityID != 0)
                        details.SpecialityID = model.SpecialityID;
                    details.ClinicID = model.ClinicID;
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
                    string EventName = "Patient Registration Success and UHID-" + patient.PatientID;
                    long Mob = Convert.ToInt64(model.MobileNumber);
                    bool isUserExists = _mobileUserRepo.IsUserAlreadyExist(Convert.ToInt64(model.MobileNumber));
                    if (isUserExists)
                    {
                        MobileUserInfo mobileUserInfo = new MobileUserInfo()
                        {
                            FirstName = model.FirstName + " " + model.SecondName,
                            Email = model.Email,
                            MobileNo = (long)model.MobileNumber,
                            IsActive = true,
                            UserType = "Patient",
                            City = model.CityName,
                            ZipCode = ""
                        };
                        int rowaffected = _mobileUserRepo.CreateNewMobileUser(mobileUserInfo);
                    }
                    string DoctorName = _patientRepo.GetDoctorNameByID((long)model.DoctorID);
                    MobileUserInfo mobileUser = _mobileUserRepo.GetMobileUserInfoByMobile((long)model.MobileNumber);
                    PatientAppointment appointment = new PatientAppointment
                    {
                        DoctorID = (long)model.DoctorID,
                        HospitalID = Convert.ToInt64(model.HospitalID),
                        PatientSeqID = mobileUser.UserSeqId,
                        SlotId = 0,
                        ConsultationType = "Directconsultation",
                        Status = 2,
                        CallDuration = "",
                        IsAppointmentEnd = false,
                        CreatedDate = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                        CreateUserID = DoctorName,
                        ModifiedUser = DoctorName,
                        VisitID = details.VisitID
                    };
                    int rowaffceted = _patientAppointmentRepo.CreateNewAppinntment(appointment);

                    CreateEventManagemnt(EventName);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private void InsertNewVisit(PatientView model)
        {
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
                if (model.SpecialityID != 0)
                    details.SpecialityID = model.SpecialityID;
                details.ClinicID = model.ClinicID;
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
                string DoctorName = _patientRepo.GetDoctorNameByID((long)model.DoctorID);
                PatientAppointment appointment = new PatientAppointment
                {
                    DoctorID = (long)model.DoctorID,
                    HospitalID = Convert.ToInt64(model.HospitalID),
                    PatientSeqID = (long)model.DoctorID,
                    SlotId = 0,
                    ConsultationType = "Directconsultation",
                    Status = 2,
                    CallDuration = "",
                    IsAppointmentEnd = false,
                    CreatedDate = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                    CreateUserID = DoctorName,
                    ModifiedUser = DoctorName,
                    VisitID = details.VisitID
                };
                int rowaffceted = _patientAppointmentRepo.CreateNewAppinntment(appointment);
                string EventName = "Patient Visit Created and UHID-" + model.PatientID;

                CreateEventManagemnt(EventName);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private bool UpdatePatient(PatientView model)
        {
            bool isSuccess = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                string ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                Patient patient = new Patient();
                patient.PatientID = model.PatientID;
                patient.FirstName = model.FirstName;
                patient.SecondName = model.SecondName;
                patient.Gender = model.Gender;
                if (!string.IsNullOrWhiteSpace(model.BirthDate))
                {
                    DateTime ageDate = DateTime.Now;
                    DateTime dob1 = getDataformat(model.BirthDate, ageDate);
                    patient.BirthDate = dob1;
                }
                patient.PhoneNumber = model.PhoneNumber;
                if (model.MobileNumber != null)
                    patient.MobileNumber = (long)model.MobileNumber;
                patient.Email = model.Email;
                patient.RelationType = model.RelationType;
                patient.RelationName = model.RelationName;
                patient.PatientAddress1 = model.PatientAddress1;
                patient.PatientAddress2 = model.PatientAddress2;
                if (model.Country == null || model.Country == 0)
                {
                    string Countryname = model.CountryName;
                    string Countrycode = model.CountryCode;
                    string Userid = HttpContext.Session.GetString("Userseqid");
                    int CountrySeqId = _countryRepo.InsertnewCountry(Countryname, Countrycode, Userid, CreatedDatetime, ModifiedDatetime);
                    model.Country = CountrySeqId;
                }
                if (model.State == null || model.State == 0)
                {
                    string Statename = model.StateName;
                    int CountryID = Convert.ToInt32(model.Country);
                    string Userid = HttpContext.Session.GetString("Userseqid");
                    int StateSeqId = _stateRepo.InsertnewState(Statename, CountryID, Userid, CreatedDatetime, ModifiedDatetime);
                    model.State = StateSeqId;
                }
                if (model.City == null || model.City == 0)
                {
                    string Cityname = model.CityName;
                    int CountryID = Convert.ToInt32(model.Country);
                    int StateID = Convert.ToInt32(model.State);
                    string Userid = HttpContext.Session.GetString("Userseqid");
                    int CitySeqId = _cityRepo.InsertnewCity(Cityname, CountryID, StateID, Userid, CreatedDatetime, ModifiedDatetime);
                    model.City = Convert.ToInt32(CitySeqId);
                }
                if (model.City != null)
                    patient.City = (int)model.City;
                if (model.State != null)
                    patient.State = (int)model.State;
                if (model.Country != null)
                    patient.Country = (int)model.Country;
                patient.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                patient.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                patient.CityCode = model.CityCode;
                patient.CountryCode = model.CountryCode;
                patient.BloodGroup = model.BloodGroup;
                isSuccess = _patientRepo.UpdatePatient(patient);
                string EventName = "Patient Deatils Updated  and UHID-" + model.PatientID;
                CreateEventManagemnt(EventName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return isSuccess;
        }
        private void UpdatePatientRegDetail(PatientView model)
        {
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
                details.VisitID = _patientRepo.GetVisitid(details.PatSeqID, (int)model.DoctorID);
                if (model.AgeYear != null)
                    details.AgeYear = (int)model.AgeYear;
                if (model.AgeMonth != null)
                    details.AgeMonth = (int)model.AgeMonth;
                if (model.AgeDay != null)
                    details.AgeDay = (int)model.AgeDay;
                if (model.DeptID != null)
                    details.DeptID = (int)model.DeptID;
                if (model.SpecialityID != 0)
                    details.SpecialityID = model.SpecialityID;
                details.ClinicID = model.ClinicID;
                details.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                details.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                details.IsActive = true;
                int result = _patientRepo.UpdateVist(details);
                string EventName = "Patient Visit Deatils Updated  and UHID-" + model.PatientID;
                CreateEventManagemnt(EventName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        public async Task<IActionResult> EMRUrlDatasource([FromBody] DataManagerRequest dm)
        {
            //DateTime stdatetime = DateTime.Now;
            //DateTime enddatetime = DateTime.Now.AddYears(-10);
            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            long Doctorid = Convert.ToInt64(HttpContext.Session.GetString("Doctorid"));
            int Pageindex = dm.Skip;
            if (Pageindex > 0)
                Pageindex = Pageindex / dm.Take + 1;
            else
                Pageindex = 1;
            int Pagesize = dm.Take;
            IEnumerable DataSource = null;
            int count = 0;
            if (dm.Search != null && dm.Search.Count > 0)
            {
                string key = dm.Search[0].Key;
                DataSource = await _patientRepo.GetEmrAllPatientbySearch(Pageindex, Pagesize, key, Hospitalid, Doctorid);
                count = DataSource.Cast<MyPatient>().Count();
            }
            else
            {
                DataSource = await _patientRepo.GetEmrAllPatient(Pageindex, Pagesize, Hospitalid, Doctorid);
                count = _patientRepo.PatientCount();
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        [HttpGet]
        public bool UpdateClinicPin(long Cliniccode)
        {
            bool result = false;
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                result = _hospitalRepo.UpdateClinicPin(Cliniccode, Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        [HttpGet]
        public JsonResult GetAllEmrPatinetByHospoital()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<MyPatient> lstPatient = _patientRepo.GetAllEmrPatinetByHospoital(HospitalID);
            return Json(lstPatient);
        }
        [HttpGet]
        public JsonResult GetAllEmrPatinetBySearch(string SearchTerm)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<MyPatient> lstPatient = null;
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                lstPatient = _patientRepo.GetAllEmrPatinetBySearch(HospitalID, SearchTerm);
            else
                lstPatient = _patientRepo.GetAllEmrPatinetByHospoital(HospitalID);
            return Json(lstPatient);
        }
        private void CreateEventManagemnt(string EventName)
        {
            try
            {
                EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
                eventManagemntInfo.EventName = EventName;
                eventManagemntInfo.UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                eventManagemntInfo.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                _emrRepo.NewEventCreateion(eventManagemntInfo);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        public static DateTime getDataformat(string DateValue, DateTime date)
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