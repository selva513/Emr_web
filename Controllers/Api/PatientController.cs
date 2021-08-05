using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using BizLayer.Repo;
using Emr_web.Common;
using System.Text.RegularExpressions;
using Emr_web.Models;
using System.Data;
using System.Data.SqlClient;
using Syncfusion.EJ2.Base;
using System.Globalization;
using System.Text;

namespace Emr_web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Patient")]
    public class PatientController : Controller
    {
        private IDBConnection _IDBConnection;
        private IPatientRepo _patientRepo;
        private IHISPatientRepo _HISPatientRepo;
        private IUserRoleRepo _userRoleRepo;
        private ILoginRepo _loginRepo;
        private IErrorlog _errorlog;
        private ILicenceRepo _licenceRepo;
        private IHospitalRepo _hospitalRepo;
        private ICountryRepo _countryRepo;
        private IStateRepo _stateRepo;
        private ICityRepo _cityRepo;
        private IInvestRepo _investRepo;
        private IDiagnosisRepo _diagnosisRepo;
        private IPatientDashboardRepo _patientdashboardrepo;
        private IDepartmentRepo _departmentRepo;
        private IEmrRepo _emrRepo;
        private IVitlasRepo _vitalsRepo;
        private IPatientDocmentsRepo _patientDocmentsRepo;
        private IDoctorRepo _doctorRepo;
        
        public PatientController(IDBConnection iDBConnection, IPatientRepo patientRepo, IHISPatientRepo HISPatientRepo, IUserRoleRepo userRoleRepo, ILoginRepo loginRepo, IErrorlog errorlog, ILicenceRepo licenceRepo, IHospitalRepo hospitalRepo, ICountryRepo countryRepo,IStateRepo stateRepo,ICityRepo cityRepo,IInvestRepo investRepo,IDiagnosisRepo diagnosisRepo, IPatientDashboardRepo patientdashboardrepo,IDepartmentRepo departmentRepo,IEmrRepo emrRepo,IVitlasRepo vitalsRepo,IPatientDocmentsRepo patientDocmentsRepo,IDoctorRepo doctorRepo)
        {
            _IDBConnection = iDBConnection;
            _patientRepo = patientRepo;
            _HISPatientRepo = HISPatientRepo;
            _userRoleRepo = userRoleRepo;
            _loginRepo = loginRepo;
            _errorlog = errorlog;
            _licenceRepo = licenceRepo;
            _hospitalRepo = hospitalRepo;
            _countryRepo = countryRepo;
            _stateRepo = stateRepo;
            _cityRepo = cityRepo;
            _investRepo = investRepo;
            _diagnosisRepo = diagnosisRepo;
            _patientdashboardrepo = patientdashboardrepo;
            _departmentRepo = departmentRepo;
            _emrRepo = emrRepo;
            _vitalsRepo = vitalsRepo;
            _patientDocmentsRepo = patientDocmentsRepo;
            _doctorRepo = doctorRepo;
        }
        [HttpGet("DepartmentByDocId")]
        public JsonResult GetAllList(string DocID)
        {
            if (!string.IsNullOrWhiteSpace(DocID))
            {
                long docSeqID = Convert.ToInt64(DocID);
                DepartmentMaster department = _patientRepo.GetDepartmentByDoctID(docSeqID);
                return Json(department);
            }
            else
            {
                return Json(null);

            }
        }
        [HttpGet("GetAge")]
        public JsonResult GetAge(string BirthDate)
        {
            if (!string.IsNullOrWhiteSpace(BirthDate))
            {
                DateTime ageDate = DateTime.Now;
                DateTime dob = getDataformat(BirthDate, ageDate);
                Age age = new Age();
                age = CommonSetting.GetAge(dob, DateTime.Today);
                return Json(new { Years = age.years, Months = age.months, Days = age.days });
            }
            else
            {
                return Json(null);
            }
        }
        [HttpGet("GetPatientIDList")]
        public List<Patient> GetPatientIDList()
        {
            List<Patient> lstResult = new List<Patient>();
            try
            {
                string EnteredName = GetEnteredData();
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long Doctorid = Convert.ToInt64(HttpContext.Session.GetString("Doctorid"));
                if (EnteredName != "tolower")
                    lstResult = _patientRepo.GetPatientIDList(EnteredName, Hospitalid);
                else
                    lstResult = _patientRepo.GetEmptyPatientIDList(EnteredName, Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetPatientByID")]
        public JsonResult GetPatientByID(string PatientID)
        {
            DataTable dtResult = new DataTable();
            dtResult = _patientRepo.GetPatientByID(PatientID);
            PatientView patientView = new PatientView();
            if (dtResult.Rows.Count > 0)
            {
                for (int count = 0; count < dtResult.Rows.Count; count++)
                {
                    patientView.PatSeqID = Convert.ToInt64(dtResult.Rows[count]["PatSeqID"]);
                    patientView.PatientID = Convert.ToString(dtResult.Rows[count]["PatientID"]);
                    patientView.FirstName = Convert.ToString(dtResult.Rows[count]["FirstName"]);
                    patientView.SecondName = Convert.ToString(dtResult.Rows[count]["SecondName"]);
                    patientView.Gender = Convert.ToString(dtResult.Rows[count]["Gender"]);
                    patientView.BirthDate = Convert.ToString(dtResult.Rows[count]["BirthDate"]);
                    patientView.PhoneNumber = Convert.ToString(dtResult.Rows[count]["PhoneNumber"]);
                    patientView.MobileNumber = Convert.ToInt64(dtResult.Rows[count]["MobileNumber"]);
                    patientView.Email = Convert.ToString(dtResult.Rows[count]["Email"]);
                    patientView.RelationName = Convert.ToString(dtResult.Rows[count]["RelationName"]);
                    patientView.RelationType = Convert.ToString(dtResult.Rows[count]["RelationType"]);
                    patientView.PatientAddress1 = Convert.ToString(dtResult.Rows[count]["PatientAddress1"]);
                    patientView.PatientAddress2 = Convert.ToString(dtResult.Rows[count]["PatientAddress2"]);
                    patientView.City = Convert.ToInt32(dtResult.Rows[count]["City"]);
                    patientView.State = Convert.ToInt32(dtResult.Rows[count]["State"]);
                    patientView.Country = Convert.ToInt32(dtResult.Rows[count]["Country"]);
                    patientView.DeptID = Convert.ToInt32(dtResult.Rows[count]["DeptID"]);
                    patientView.DoctorID = Convert.ToInt32(dtResult.Rows[count]["DoctorID"]);
                    patientView.ClinicID = Convert.ToInt64(dtResult.Rows[count]["ClinicID"]);
                    patientView.CityCode = dtResult.Rows[count]["CityCode"].ToString();
                    patientView.CountryCode = dtResult.Rows[count]["CountryCode"].ToString();
                    patientView.SpecialityID = Convert.ToInt64(dtResult.Rows[count]["SpecialityID"].ToString());
                    if (!string.IsNullOrWhiteSpace(dtResult.Rows[count]["BloodGroup"].ToString()))
                        patientView.BloodGroup = Convert.ToInt32(dtResult.Rows[count]["BloodGroup"].ToString());

                    if (!string.IsNullOrWhiteSpace(patientView.BirthDate))
                    {
                        DateTime ageDate = DateTime.Now;
                        DateTime dob = getDataformat(patientView.BirthDate, ageDate);
                        Age age = new Age();
                        age = CommonSetting.GetAge(dob, DateTime.Today);
                        patientView.AgeYear = age.years;
                        patientView.AgeMonth = age.months;
                        patientView.AgeDay = age.days;
                    }
                    else
                    {
                        patientView.AgeYear = Convert.ToInt32(dtResult.Rows[count]["AgeYear"]);
                        patientView.AgeMonth = Convert.ToInt32(dtResult.Rows[count]["AgeMonth"]);
                        patientView.AgeDay = Convert.ToInt32(dtResult.Rows[count]["AgeDay"]);
                    }
                }
            }
            return Json(patientView);
        }
        [HttpGet("GetHISPatientByID")]
        public JsonResult GetHISPatientByID(string PatientID)
        {
            DataTable dtResult = new DataTable();
            dtResult = _patientRepo.GetHISPatientByID(PatientID);
            PatientView patientView = new PatientView();
            if (dtResult.Rows.Count > 0)
            {
                for (int count = 0; count < dtResult.Rows.Count; count++)
                {
                    patientView.PatSeqID = Convert.ToInt64(dtResult.Rows[count]["PatSeqID"]);
                    patientView.PatientID = Convert.ToString(dtResult.Rows[count]["PatientID"]);
                    patientView.FirstName = Convert.ToString(dtResult.Rows[count]["FirstName"]);
                    patientView.SecondName = Convert.ToString(dtResult.Rows[count]["SecondName"]);
                    patientView.Gender = Convert.ToString(dtResult.Rows[count]["Gender"]);
                    patientView.BirthDate = Convert.ToString(dtResult.Rows[count]["BirthDate"]);
                    patientView.PhoneNumber = Convert.ToString(dtResult.Rows[count]["PhoneNumber"]);
                    patientView.MobileNumber = Convert.ToInt64(dtResult.Rows[count]["MobileNumber"]);
                    patientView.Email = Convert.ToString(dtResult.Rows[count]["Email"]);
                    patientView.RelationName = Convert.ToString(dtResult.Rows[count]["RelationName"]);
                    patientView.RelationType = Convert.ToString(dtResult.Rows[count]["RelationType"]);
                    patientView.PatientAddress1 = Convert.ToString(dtResult.Rows[count]["PatientAddress1"]);
                    patientView.PatientAddress2 = Convert.ToString(dtResult.Rows[count]["PatientAddress2"]);
                    patientView.City = Convert.ToInt32(dtResult.Rows[count]["City"]);
                    patientView.State = Convert.ToInt32(dtResult.Rows[count]["State"]);
                    patientView.Country = Convert.ToInt32(dtResult.Rows[count]["Country"]);
                    patientView.DeptID = Convert.ToInt32(dtResult.Rows[count]["DeptID"]);
                    patientView.DoctorID = Convert.ToInt32(dtResult.Rows[count]["DoctorID"]);
                    if (!string.IsNullOrWhiteSpace(patientView.BirthDate) && patientView.BirthDate != "01/01/1900")
                    {
                        DateTime ageDate = DateTime.Now;
                        DateTime dob = getDataformat(patientView.BirthDate, ageDate);
                        Age age = new Age();
                        age = CommonSetting.GetAge(dob, DateTime.Today);
                        patientView.AgeYear = age.years;
                        patientView.AgeMonth = age.months;
                        patientView.AgeDay = age.days;
                    }
                    else
                    {
                        patientView.AgeYear = Convert.ToInt32(dtResult.Rows[count]["AgeYear"]);
                        patientView.AgeMonth = Convert.ToInt32(dtResult.Rows[count]["AgeMonth"]);
                        patientView.AgeDay = Convert.ToInt32(dtResult.Rows[count]["AgeDay"]);
                    }
                }
            }
            return Json(patientView);
        }

        public string GetEnteredData()
        {
            string Data = "";
            try
            {
                var query = Request.Query;
                string filter = query["$filter"];
                Match matchString = Regex.Match(filter, @"'(.*)',tolower");
                string[] seperators = { "(", ")", ",", "'", "'" };
                string[] split = matchString.Value.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                string QueryString = split[0];
                Data = QueryString;
            }
            catch (Exception ex)
            {

            }
            return Data;
        }
        [HttpGet("CheckSplPwd")]
        public string CheckSplPwd(string splpawd)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckSpecialPwd(splpawd);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("CheckUsername")]
        public string CheckUsername(string admusernm)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckUsername(admusernm);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("CheckUserid")]
        public string CheckUserid(string admuserid)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckUserid(admuserid);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("CheckUseremail")]
        public string CheckUseremail(string admuseremail)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckUseremail(admuseremail);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("CheckRole")]
        public string CheckRole(string Role)
        {
            string Success = "";
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                bool validate = _patientRepo.CheckUserRole(Role, Hospitalid);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("GetRoles")]
        public RoleMasterView[] GetRoles()
        {
            DataTable dtResult = new DataTable();
            List<RoleMasterView> lstrole = new List<RoleMasterView>();
            try
            {
                dtResult = _patientRepo.GetAllRoleNames();
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        RoleMasterView roleMasterView = new RoleMasterView();
                        roleMasterView.Roleseqid = Convert.ToInt64(dtResult.Rows[i]["Roleseqid"]);
                        roleMasterView.Rolename = dtResult.Rows[i]["Rolename"].ToString();
                        lstrole.Add(roleMasterView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstrole.ToArray();
        }
        [HttpGet("GetHospitals")]
        public HospitalView[] GetHospitals()
        {
            DataTable dtResult = new DataTable();
            List<HospitalView> lsthos = new List<HospitalView>();
            try
            {
                dtResult = _patientRepo.GetAllHospitals();
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        HospitalView hospitalView = new HospitalView();
                        hospitalView.HospitalID = Convert.ToInt64(dtResult.Rows[i]["HospitalID"]);
                        hospitalView.HospitalName = dtResult.Rows[i]["HospitalName"].ToString();
                        lsthos.Add(hospitalView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lsthos.ToArray();
        }
        [HttpGet("GetClinic")]
        public JsonResult GetClinic(long hospitalid)
        {
            DataTable dtResult = new DataTable();
            ClinicView clinicView = new ClinicView();
            List<ClinicView> lstclinic = new List<ClinicView>();
            try
            {
                hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                dtResult = _patientRepo.GetAllClinic(hospitalid);
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        clinicView = new ClinicView();
                        clinicView.ClinicID = Convert.ToInt64(dtResult.Rows[i]["ClinicID"]);
                        clinicView.ClinicName = dtResult.Rows[i]["ClinicName"].ToString();
                        lstclinic.Add(clinicView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstclinic.ToArray());
        }
        [HttpGet("GetMenus")]
        public string GetMenus()
        {
            DataSet ds = new DataSet();
            try
            {
                if (ModelState.IsValid)
                {
                    ds = _userRoleRepo.GetAllMenu();
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        [HttpGet("GetMenuGroup")]
        public string GetMenuGroup(long Roleid)
        {
            DataSet ds = new DataSet();
            try
            {
                if (ModelState.IsValid)
                {
                    ds = _userRoleRepo.GetAllMenuGroup(Roleid);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        [HttpGet("CheckLoginUsername")]
        public string CheckLoginUsername(string UserName)
        {
            string Success = "";
            try
            {
                UserName = Encrypt_Decrypt.Encrypt(UserName);
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                bool validate = _patientRepo.CheckLoginUsername(UserName, Hospitalid);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("CheckLoginUserid")]
        public string CheckLoginUserid(string UserID)
        {
            string Success = "";
            try
            {
                UserID = Encrypt_Decrypt.Encrypt(UserID);
                //long Hospitalid= Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                bool validate = _patientRepo.CheckLoginUserID(UserID);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        
        [HttpGet("GetDepartment")]
        public DepartmentView[] GetDepartment()
        {
            DataTable dtResult = new DataTable();
            DepartmentView departmentView = new DepartmentView();
            List<DepartmentView> lstdept = new List<DepartmentView>();
            try
            {
                dtResult = _patientRepo.GetAllDepartmentNames();
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        departmentView = new DepartmentView();
                        departmentView.DeptSeqID = Convert.ToInt64(dtResult.Rows[i]["DeptSeqID"]);
                        departmentView.DepartmentName = dtResult.Rows[i]["DepartmentName"].ToString();
                        lstdept.Add(departmentView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstdept.ToArray();
        }
        [HttpGet("GetSpeciality")]
        public SpecialityView[] GetSpeciality()
        {
            DataTable dtResult = new DataTable();
            SpecialityView specialityView = new SpecialityView();
            List<SpecialityView> lstspeciality = new List<SpecialityView>();
            try
            {
                dtResult = _patientRepo.GetAllSpecialityNames();
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        specialityView = new SpecialityView();
                        specialityView.SpecialityID = Convert.ToInt64(dtResult.Rows[i]["SpecialityID"]);
                        specialityView.Speciality_Name = dtResult.Rows[i]["Speciality_Name"].ToString();
                        lstspeciality.Add(specialityView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstspeciality.ToArray();
        }
        [HttpGet("GetUserType")]
        public UserTypeView[] GetUserType()
        {
            DataTable dtResult = new DataTable();
            UserTypeView userTypeView = new UserTypeView();
            List<UserTypeView> lstusertype = new List<UserTypeView>();
            try
            {
                dtResult = _patientRepo.GetAllUserTypeNames();
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        userTypeView = new UserTypeView();
                        userTypeView.Type_Seqid = Convert.ToInt64(dtResult.Rows[i]["Type_Seqid"]);
                        userTypeView.Type_Name = dtResult.Rows[i]["Type_Name"].ToString();
                        lstusertype.Add(userTypeView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstusertype.ToArray();
        }
        [HttpGet("GetAllCity_Country")]
        public JsonResult GetAllCity_Country()
        {
            //DataTable dtResult = new DataTable();
            //UserTypeView userTypeView = new UserTypeView();
            //List<UserTypeView> lstusertype = new List<UserTypeView>();
            List<CityMaster> lstCity = new List<CityMaster>();
            List<CountryMaster> lstCountry = new List<CountryMaster>();
            try
            {
                lstCity = _patientRepo.GetCityList();
                lstCountry = _patientRepo.GetAllCountry();
                //dtResult = _patientRepo.GetAllUserTypeNames();
                //if (dtResult.Rows.Count > 0)
                //{
                //    for (int i = 0; i < dtResult.Rows.Count; i++)
                //    {
                //        userTypeView = new UserTypeView();
                //        userTypeView.Type_Seqid = Convert.ToInt64(dtResult.Rows[i]["Type_Seqid"]);
                //        userTypeView.Type_Name = dtResult.Rows[i]["Type_Name"].ToString();
                //        lstusertype.Add(userTypeView);
                //    }
                //}
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                AllCity = lstCity.ToArray(),
                AllCountry = lstCountry.ToArray()

            });
        }
        [HttpGet("Checkemail")]
        public string Checkemail(string useremail)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.Checkemail(useremail);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("GetuserDetailsByUserid")]
        public string GetuserDetailsByUserid(long Roleid, long ModUserid)
        {
            DataSet ds = new DataSet();
            try
            {
                if (ModelState.IsValid)
                {
                    ds = _loginRepo.GetUserDetailsByUserid(Roleid, ModUserid);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string Username = Encrypt_Decrypt.Decrypt(ds.Tables[0].Rows[0]["UserName"].ToString());
                        ds.Tables[0].Rows[0]["UserName"] = Username;
                        string Userid = Encrypt_Decrypt.Decrypt(ds.Tables[0].Rows[0]["Userid"].ToString());
                        ds.Tables[0].Rows[0]["Userid"] = Userid;
                        string Password = Encrypt_Decrypt.Decrypt(ds.Tables[0].Rows[0]["Password"].ToString());
                        ds.Tables[0].Rows[0]["Password"] = Password;
                        string ForgotEmail = ds.Tables[0].Rows[0]["ForgotPwdEmail"].ToString();
                        if (ForgotEmail != null && ForgotEmail != "")
                        {
                            string Email = Encrypt_Decrypt.Decrypt(ds.Tables[0].Rows[0]["ForgotPwdEmail"].ToString());
                            ds.Tables[0].Rows[0]["ForgotPwdEmail"] = Email;
                        }
                        string Key = ds.Tables[0].Rows[0]["ActivationKey"].ToString();
                        if (Key != null && Key != "")
                        {
                            string ActivationKey = Encrypt_Decrypt.Decrypt(ds.Tables[0].Rows[0]["ActivationKey"].ToString());
                            ds.Tables[0].Rows[0]["ActivationKey"] = ActivationKey;
                        }
                        ds.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        [HttpGet("GetUserid")]
        public LoginView[] GetUserid(long Roleid)
        {
            DataTable dtResult = new DataTable();
            LoginView loginView = new LoginView();
            List<LoginView> lstuserid = new List<LoginView>();
            try
            {
                dtResult = _patientRepo.GetAllUseridNames(Roleid);
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        loginView = new LoginView();
                        loginView.UserSeqid = Convert.ToInt64(dtResult.Rows[i]["UserSeqid"]);
                        loginView.Userid = Encrypt_Decrypt.Decrypt(dtResult.Rows[i]["Userid"].ToString());
                        lstuserid.Add(loginView);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstuserid.ToArray();
        }
        [HttpGet("CheckHospitalname")]
        public string CheckHospitalname(string HosName)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckHospital(HosName);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("CheckClinicname")]
        public string CheckClinicname(string ClinicName, long Hosid)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckClinic(ClinicName, Hosid);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("GetHospitalname")]
        public List<HospitalMaster> GetHospitalname()
        {
            List<HospitalMaster> lst = new List<HospitalMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lst = _hospitalRepo.GetHospitalDetails(Hospitalid);
                //string Userseqid = HttpContext.Session.GetString("Userseqid");
                //string Hospitalname = _patientRepo.GetHospitalname(Userseqid);
                //Success = Hospitalname;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }

        [HttpGet("GetAllHISPatientByDate")]
        public async Task<JsonResult> GetAllHISPatientByDate()
        {
            string stdatetime = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            string enddatetime = DateTime.Now.ToString("yyyy-MM-dd");
            var lst = await _HISPatientRepo.GetAllHISPatientByDate(stdatetime, enddatetime);
            if (lst != null)
            {
                lst = lst as List<HISPatient>;
                return Json(new { result = lst, count = lst.Count() });
            }
            else
            {
                return Json(null);
            }
        }
        [HttpGet("GetDoctors")]
        public DoctorMaster[] GetDoctors()
        {
            List<DoctorMaster> lstdoctor = new List<DoctorMaster>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstdoctor = _patientRepo.GetAllDoctorNames(HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstdoctor.ToArray();
        }
        [HttpGet("GetDoctorandDept")]
        public string GetDoctorandDept()
        {
            DataSet ds = new DataSet();
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);

                long Userseqid = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                List<Login> logins = new List<Login>();
                logins = _loginRepo.GetUserDetailsByID(Userseqid,CurrentDatetime);
                long Doctorid = logins[0].DoctorID;
                ds = _patientRepo.GetDoctorandDept(Doctorid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        #region Raffi for bind questions to Dropdown
        [HttpGet("GetQuestions")]
        public QuestionMaster[] GetQuestions()
        {
            List<QuestionMaster> lstquestion = new List<QuestionMaster>();
            try
            {
                lstquestion = _licenceRepo.GetQuestions();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstquestion.ToArray();
        }
        #endregion

        #region Raffi Check user id in login table whether exist or not
        [HttpGet("CheckUseridExist")]
        public string CheckUseridExist(string Userid)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckUseridExist(Userid);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        #endregion
        [HttpGet("GetHISDoctorname")]
        public List<DoctorMaster> GetHISDoctorname(long HISDoctorid)
        {
            List<DoctorMaster> lstdoctor = new List<DoctorMaster>();
            try
            {
                if (HISDoctorid != 0)
                    lstdoctor = _patientRepo.GetHISDoctorname(HISDoctorid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstdoctor;
        }
        [HttpGet("GetDoctorDetails")]
        public List<DoctorMaster> GetDoctorDetails(long Doctorid)
        {
            List<DoctorMaster> lstdoctor = new List<DoctorMaster>();
            try
            {
                 lstdoctor = _patientRepo.GetDoctorDetails(Doctorid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstdoctor;
        }
        [HttpGet("Check_MedicalCouncilExist")]
        public bool Check_MedicalCouncilExist(string MedicalCouncil)
        {
            bool result = false;
            try
            {
                 result = _doctorRepo.Check_MedicalCouncilExist(MedicalCouncil);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        [HttpPost("InsertMedicalCouncil")]
        public long InsertMedicalCouncil(string MedicalCouncil,bool IsActive)
        {
            long result = 0;
            MedicalCouncilMaster medicalCouncilMaster = new MedicalCouncilMaster();
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                medicalCouncilMaster.CouncilName = MedicalCouncil;
                medicalCouncilMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                medicalCouncilMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                medicalCouncilMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                medicalCouncilMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                medicalCouncilMaster.IsActive = IsActive;
                result = _doctorRepo.InsertMedicalCouncil(medicalCouncilMaster);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        #region Get Config details for hospital setting
        [HttpGet("GetConfig")]
        public List<Config> GetConfig()
        {
            List<Config> lstconfig = new List<Config>();
            try
            {
                lstconfig = _patientRepo.GetConfig();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstconfig;
        }
        #endregion
        [HttpGet("CheckSpeciality")]
        public string CheckSpeciality(string Speciality)
        {
            string result = "";
            try
            {
                string SpecialityCheck = _patientRepo.GetSpeciality(Speciality);
                result = SpecialityCheck;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        [HttpGet("GetRoleConfig")]
        public List<RoleMaster> GetRoleConfig()
        {
            List<RoleMaster> lst = new List<RoleMaster>();
            try
            {
                long Roleid = Convert.ToInt64(HttpContext.Session.GetString("Roleid"));
                lst = _userRoleRepo.GetRoleConfig(Roleid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetHISConfig")]
        public List<Login> GetHISConfig()
        {
            List<Login> lst = new List<Login>();
            try
            {
                lst = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetDoctorByUserid")]
        public long GetDoctorByUserid()
        {
            long Doctorid = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);
                long Userseqid = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                List<Login> logins = new List<Login>();
                logins = _loginRepo.GetUserDetailsByID(Userseqid, CurrentDatetime);
                Doctorid = logins[0].DoctorID;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Doctorid;
        }
        [HttpGet("GetCityPins")]
        public List<HospitalMaster> GetCityPins()
        {
            List<HospitalMaster> lst = new List<HospitalMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lst = _hospitalRepo.GetPins(Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetStatePins")]
        public List<HospitalMaster> GetStatePins()
        {
            List<HospitalMaster> lst = new List<HospitalMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lst = _hospitalRepo.GetStatePins(Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCityChangeDetails")]
        public List<HospitalMaster> GetCityChangeDetails(long CityID)
        {
            List<HospitalMaster> lst = new List<HospitalMaster>();
            try
            {
                lst = _hospitalRepo.GetCityChangeDetails(CityID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetStateChangeDetails")]
        public List<HospitalMaster> GetStateChangeDetails(long StateID)
        {
            List<HospitalMaster> lst = new List<HospitalMaster>();
            try
            {
                lst = _hospitalRepo.GetStateChangeDetails(StateID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCountryChangeDetails")]
        public List<HospitalMaster> GetCountryChangeDetails(long CountryID)
        {
            List<HospitalMaster> lst = new List<HospitalMaster>();
            try
            {
                lst = _hospitalRepo.GetCountryChangeDetails(CountryID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCountryCodelist")]
        public List<string> GetCountryCodelist()
        {
            List<string> lstResult = new List<string>();
            try
            {
                string EnteredName = GetEnteredData();
                lstResult = _patientRepo.GetCountryCodelist(EnteredName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }

        [HttpGet("GetCityCodelist")]
        public List<string> GetCityCodelist()
        {
            List<string> lstResult = new List<string>();
            try
            {
                string EnteredName = GetEnteredData();
                lstResult = _patientRepo.GetCityCodelist(EnteredName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetCountryNameByCode")]
        public string GetCountryNameByCode(string CountryCode)
        {
            string Countryname = "";
            try
            {
                Countryname = _countryRepo.GetCountryNameByCode(CountryCode);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Countryname;
        }
        [HttpGet("GetClinicPins")]
        public List<HospitalMaster> GetClinicPins()
        {
            List<HospitalMaster> lst = new List<HospitalMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lst = _hospitalRepo.GetClinicPins(Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCountryName")]
        public List<CountryMaster> GetCountryName(string CountryName)
        {
            List<CountryMaster> lst = new List<CountryMaster>();
            try
            {
                lst = _countryRepo.GetCountryByName(CountryName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCountryCode")]
        public List<CountryMaster> GetCountryCode(string CountryCode)
        {
            List<CountryMaster> lst = new List<CountryMaster>();
            try
            {
                lst = _countryRepo.GetCountryByCode(CountryCode);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCurrencyCode")]
        public List<CountryMaster> GetCurrencyCode(string CurrencyCode)
        {
            List<CountryMaster> lst = new List<CountryMaster>();
            try
            {
                lst = _countryRepo.GetCurrencyCode(CurrencyCode);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetStateName")]
        public List<StateMaster> GetStateName(string StateName)
        {
            List<StateMaster> lst = new List<StateMaster>();
            try
            {
                lst = _stateRepo.GetStateByName(StateName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCityName")]
        public List<CityMaster> GetCityName(string CityName)
        {
            List<CityMaster> lst = new List<CityMaster>();
            try
            {
                lst = _cityRepo.GetCityByName(CityName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetCityCode")]
        public List<CityMaster> GetCityCode(string CityCode)
        {
            List<CityMaster> lst = new List<CityMaster>();
            try
            {
                lst = _cityRepo.GetCityByCode(CityCode);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetInvestName")]
        public List<InvestigationMaster> GetInvestName(string InvestName)
        {
            List<InvestigationMaster> lst = new List<InvestigationMaster>();
            try
            {
                lst = _investRepo.GetInvestigationByName(InvestName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }

        [HttpGet("GetDiagnosisName")]
        public List<DiagnosisMaterCls> GetDiagnosisName(string DiagnosiName)
        {
            List<DiagnosisMaterCls> lst = new List<DiagnosisMaterCls>();
            try
            {
                lst = _diagnosisRepo.GetDiagnosisByName(DiagnosiName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("GetPatientDetailsById")]
        public JsonResult GetPatientDetailsById(string PatientId)
        {
            //DataSet ds = new DataSet();
            //DataTable dt1 = new DataTable();
            //DataTable dt2 = new DataTable();
            ////DataTable dt3 = new DataTable();
            List<PatientDashboard> lstPatDetails = new List<PatientDashboard>();
            List<PatientDashboard> lstPatDiagnosis = new List<PatientDashboard>();
            List<PatientDashboard> lstSymptomLatestVisit = new List<PatientDashboard>();
            List<PatientDashboard> lstInvestigationLatestVisit = new List<PatientDashboard>();
            List<PatientDashboard> lstTreatmentLatestVisit = new List<PatientDashboard>();
            List<PatientDashboard> lstCoMorbitiesLatestVisit = new List<PatientDashboard>();
            List<PatientDashboard> lstMedicationLatestVisit = new List<PatientDashboard>();
            List<PatientDashboard> lstSurgicalHxLatestVisit = new List<PatientDashboard>();
            List<PatientDashboard> lstGenitalHxLatestVisit = new List<PatientDashboard>();
            List<string> lstAllergies = new List<string>();
            string Allergies = "";

            try
            {
                lstPatDetails = _patientdashboardrepo.GetPatientDetailsById(PatientId);
                lstPatDiagnosis = _patientdashboardrepo.GetPatientDiagnosis(PatientId);
                lstSymptomLatestVisit = _patientdashboardrepo.GetSymptomLatestVisit(PatientId);
                lstInvestigationLatestVisit = _patientdashboardrepo.GetInvestigationLatestVisit(PatientId);
                lstTreatmentLatestVisit = _patientdashboardrepo.GetTreatmentLatestVisit(PatientId);
                lstCoMorbitiesLatestVisit = _patientdashboardrepo.GetCoMorbitiesLatestVisit(PatientId);
                lstMedicationLatestVisit = _patientdashboardrepo.GetMedicationLatestVisit(PatientId);
                lstSurgicalHxLatestVisit = _patientdashboardrepo.GetSurgicalHxLatestVisit(PatientId);
                lstGenitalHxLatestVisit = _patientdashboardrepo.GetGenitalHxLatestVisit(PatientId);
                lstAllergies = _emrRepo.GetPatinetAllergiesByPatient(PatientId);
                if (lstAllergies.Count > 0)
                    Allergies = string.Join(",", lstAllergies.ToArray());
                //dt2 = _patientdashboardrepo.GetPatientVitals(PatientId);
                //ds.Tables.Add(dt1);
                //ds.Tables.Add(dt2);
                //ds.Tables.Add(dt3);
                //ViewBag.PatientDetails = list
                //issuccess = true;
            }
            catch (Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            ////return ds.GetXml();
            return Json(new
            {
                PatDetails = lstPatDetails,
                PatDiagnosis = lstPatDiagnosis,
                LatestSymptom = lstSymptomLatestVisit,
                LatestInvestigation = lstInvestigationLatestVisit,
                LatestTreatment = lstTreatmentLatestVisit,
                LatestCoMorbities = lstCoMorbitiesLatestVisit,
                LatestMedication = lstMedicationLatestVisit,
                LatestSurgicalHx = lstSurgicalHxLatestVisit,
                LatestGenitalHx = lstGenitalHxLatestVisit,
                Allergy = Allergies
            });
        }

        [HttpGet("GetVitlasByLastVisit")]
        public JsonResult GetVitlasByLastVisit(string PatientId)
        {
            List<VitalsView> lstResult = new List<VitalsView>();
            try
            {
                lstResult = _vitalsRepo.GetVitalsByLatestVisit(PatientId);
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return Json(lstResult);
        }
        [HttpGet("GetDiagnosisByVisitId")]
        public JsonResult GetDiagnosisByVisitId(string VisitId)
        {
            //DataSet ds = new DataSet();
            //DataTable dt = new DataTable();
            List<PatientDashboard> lstDiagnosis = new List<PatientDashboard>();
            try
            {
                lstDiagnosis = _patientdashboardrepo.GetDiagnosisByVisitId(VisitId);
                //ds.Tables.Add(dt);
            }
            catch (Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            //return ds.GetXml();
            return Json(new { PatDiagnosis = lstDiagnosis });
        }

        [HttpGet("GetEmrViewDataByVisitId")]
        public JsonResult GetEmrViewDataByVisitId(string VisitId)
        {
            List<PatientDashboard> lstSymptom = new List<PatientDashboard>();
            List<PatientDashboard> lstInvestigation = new List<PatientDashboard>();
            List<PatientDashboard> lstTreatment = new List<PatientDashboard>();
            List<PatientDashboard> lstCoMorbities = new List<PatientDashboard>();
            List<PatientDashboard> lstMedication = new List<PatientDashboard>();
            List<PatientDashboard> lstSurgicalHistory = new List<PatientDashboard>();
            List<VitalsView> lstVitals = new List<VitalsView>();
            List<PatientDashboard> lstGenitalHx = new List<PatientDashboard>();

            try
            {
                lstSymptom = _patientdashboardrepo.GetSymptomsByVisitId(VisitId);
                lstInvestigation = _patientdashboardrepo.GetInvestigationByVisitId(VisitId);
                lstTreatment = _patientdashboardrepo.GetTreatmentByVisitId(VisitId);
                lstCoMorbities = _patientdashboardrepo.GetCoMorbitiesByVisitId(VisitId);
                lstMedication = _patientdashboardrepo.GetMedicationByVisitId(VisitId);
                lstSurgicalHistory = _patientdashboardrepo.GetSurgicalHistoryByVisitId(VisitId);
                lstVitals = _patientdashboardrepo.GetVitalsByVisitId(VisitId);
                lstGenitalHx = _patientdashboardrepo.GetGenitalHxByVisitId(VisitId);
            }
            catch (Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return Json(new
            {
                PatSymptom = lstSymptom,
                PatInvestigation = lstInvestigation,
                PatTreatment = lstTreatment,
                PatCoMorbities = lstCoMorbities,
                PatMedication = lstMedication,
                PatSurgicalHistory = lstSurgicalHistory,
                PatGenitalHx = lstGenitalHx,
                PatVitals = lstVitals
            }) ;
        }
        [HttpGet("GetDeptName")]
        public List<DepartmentMaster> GetDeptName(string DeptName)
        {
            List<DepartmentMaster> lst = new List<DepartmentMaster>();
            try
            {
                lst = _departmentRepo.GetDepartmentByName(DeptName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lst;
        }
        [HttpGet("CheckIdentifierExist")]
        public string CheckIdentifierExist(string Identifier, long Hosid)
        {
            string Success = "";
            try
            {
                bool validate = _patientRepo.CheckIdentifier(Identifier, Hosid);
                if (validate == true)
                    Success = "success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
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
        [HttpGet("GetDescriptionList")]
        public List<DocumentDescriptionMaster> GetDescriptionList()
        {
            List<DocumentDescriptionMaster> lstResult = new List<DocumentDescriptionMaster>();
            try
            {
                string EnteredName = GetEnteredData();
                lstResult = _patientDocmentsRepo.GetDescriptionList();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        
        [HttpGet("CheckDoctorEmailId")]
        public bool CheckDoctorEmailId(string EmailId)
        {
            bool result = false;
            try
            {
                result = _doctorRepo.CheckDoctorEmailId(EmailId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        [HttpGet("Check_UserEmail")]
        public JsonResult Check_UserEmail(string UserID)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            List<Login> logins = new List<Login>();
            string[] result = new string[4];
            long HospitalID;
            bool validate;
            long Licenceid;
            int Usercount;
            int TotalUserCount;
            bool IsExists;
            long MobileNo = 0;
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);

                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                UserID = Encrypt_Decrypt.Encrypt(UserID);
                validate = _patientRepo.CheckLoginUserID(UserID);
                if (validate)
                {
                    IsExists = _loginRepo.IsUserIdExistsInHospital(UserID, HospitalID);
                    if (IsExists)
                    {
                        result[0] = "Update";
                        logins = _loginRepo.GetUserDetails(UserID, CurrentDatetime);
                        result[1] = logins[0].MobileNumber.ToString();
                    }
                    else
                    {
                        result[0] = "Error";
                        result[1] = "Email Id Already Exists";
                    }
                }
                else
                {
                    Licenceid = Convert.ToInt64(HttpContext.Session.GetString("Licenceid"));
                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    Usercount = _loginRepo.GetUserCount(Licenceid);
                    TotalUserCount = _loginRepo.GetUserByHospital(HospitalID);
                    TotalUserCount = TotalUserCount + 1;
                    if (TotalUserCount <= Usercount)
                        result[0] = "Insert";
                    else
                    {
                        result[0] = "Error";
                        result[1] = "User Count Exceeded.Please Contact Allied Business Solutions.";
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(result);
        }

        [HttpGet("GetTermsFromConfig")]
        public string GetTermsFromConfig()
        {
            string Success = "";
            try
            {
                Success = _loginRepo.GetTerms();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("GetTPrivacyPolicyFromConfig")]
        public string GetTPrivacyPolicyFromConfig()
        {
            string Success = "";
            try
            {
                Success = _loginRepo.GetPrivacyPolicy();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }

        #region Habib
        [HttpGet("GetCityBySearch")]
        public List<CityMaster> GetCityBySearch(string Search)
        {
            List<CityMaster> lstResult = new List<CityMaster>();
            try
            {
                if (string.IsNullOrWhiteSpace(Search))
                {
                    Search = "";
                }
                lstResult = _patientRepo.GetCityBySearch(Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetStateBySearch")]
        public List<StateMaster> GetStateBySearch(string Search)
        {
            List<StateMaster> lstResult = new List<StateMaster>();
            try
            {
                if (string.IsNullOrWhiteSpace(Search))
                {
                    Search = "";
                }
                lstResult = _patientRepo.GetStateBySearch(Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetCountryBySearch")]
        public List<CountryMaster> GetCountryBySearch(string Search)
        {
            List<CountryMaster> lstResult = new List<CountryMaster>();
            try
            {
                if (string.IsNullOrWhiteSpace(Search))
                {
                    Search = "";
                }
                lstResult = _patientRepo.GetCountryBySearch(Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        #endregion

        #region Habib_HIS_API
        [HttpGet("GetReprintPatients")]
        public string GetReprintPatients(string Search, int PageIndex, int PageSize)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                if (Search == null)
                {
                    Search = "";
                }
                dt = _patientRepo.GetReprintPatients(Search, PageIndex, PageSize);
                ds.Tables.Add(dt);
                ds.Tables[0].TableName = "PatientsList";

                DataTable dtPager = new DataTable("Pager");
                dtPager.Columns.Add("PageIndex");
                dtPager.Columns.Add("PageSize");
                dtPager.Columns.Add("RecordCount");
                dtPager.Rows.Add();
                dtPager.Rows[0]["PageIndex"] = PageIndex;
                dtPager.Rows[0]["PageSize"] = PageSize;
                dtPager.Rows[0]["RecordCount"] = dt.Rows.Count;
                ds.Tables.Add(dtPager);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("HIS GetReprintPatients" + ex.ToString());
            }

            return ds.GetXml();
        }

        [HttpGet("GetFinalBillReprint")]
        public string GetFinalBillReprint(string AdmitId, string PatientID, string BillNo,bool IsHeader)
        {
            DataSet ds = new DataSet();
            DataSet Dslab = new DataSet();
            DataTable dtAdvance = new DataTable();
            DataTable dtBed = new DataTable();
            DataTable dtOrderTotal = new DataTable();
            DataTable dtPatientDet = new DataTable();
            DataTable dtLastBed = new DataTable();
            string LocationId = "1";
            string LocationName = "Erode";
            string LastBedName = "";
            decimal BedTotalAmount = 0;
            decimal GrandTotal = 0;
            decimal OrderTotal = 0;
            StringBuilder sb = new StringBuilder();
            try
            {
                dtPatientDet = _patientRepo.GetFinalBillDetails(AdmitId, BillNo);
                ds.Tables.Add(dtPatientDet);
                dtBed = _patientRepo.GetIPBedDetails(AdmitId);
                ds.Tables.Add(dtBed);
                //ds.Tables.Add(_patientRepo.GetIPFinalBillOrderDetails(AdmitId, BillNo, PatientID));
                ds.Tables.Add(_patientRepo.GetIPStamentHeaderByAdmitID(PatientID, AdmitId));
                Dslab = _patientRepo.labheaderclass(LocationId);
                bool IsHeaderPrint = _patientRepo.Print_Header_YesorNO();
                string surgerdate = _patientRepo.GetSurgery_Date(AdmitId);

                sb.Append(@"<html>");
                if (IsHeader)
                {
                    sb.Append("<head>");
                    sb.Append("<table style='width:95%;'");
                    sb.Append("<tr>");

                    sb.Append("<td style = 'width:65%;border-color: white;'>");
                    sb.Append("<table ");
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:left;font-size:20px;;border-color: white;'>" + Dslab.Tables[0].Rows[0]["LIS_LABNAME"].ToString() + "</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:left;font-size:12px;;border-color:white;'>" + Dslab.Tables[0].Rows[0]["LIS_LAB_ADDRESS"].ToString() + "</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:left;font-size:12px;;border-color: white;'>" + Dslab.Tables[0].Rows[0]["LIS_LAB_ADDRESS1"].ToString() + "</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");
                    sb.Append("</td>");

                    sb.Append("<td style = 'width:15%;border-color: white;' >");
                    sb.Append("<table ");
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:right;font-size:12px;border-color: white;'>Phone:" + Dslab.Tables[0].Rows[0]["LIS_LABPHONE"].ToString() + "</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:right;font-size:12px;border-color: white;'>Email:" + Dslab.Tables[0].Rows[0]["LIS_LABEMAIL"].ToString() + "</td>");
                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    sb.Append("<td style='text-align:right;font-size:12px;border-color: white;'>Website:" + Dslab.Tables[0].Rows[0]["Lis_Website"].ToString() + "</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");
                    sb.Append("</td>");

                    sb.Append("<td style = 'width:20%;border-color: white;'>");
                    sb.Append("<table ");
                    sb.Append("<tr>");
                    sb.Append("<td  style=';border-color: white;'><img src='/images/TOSH_LOGO.jpg' width='90%' style='margin-left: 25px;'></td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");
                    sb.Append("</td>");


                    sb.Append("</tr>");
                    sb.Append("</table>");
                    sb.Append("<style>table, th, td {border: 1px solid lightgrey;border-collapse: collapse;}</style>");
                    sb.Append("</head>");

                    sb.Append(@"<body>");
                    sb.Append(@"<hr/>");
                }
                else
                {
                    sb.Append("<style>table, th, td {border: 1px solid lightgrey;border-collapse: collapse;}</style>");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    dtLastBed = _patientRepo.GetLastBedByAdmitId(AdmitId);
                    if (dtLastBed.Rows.Count > 0)
                    {
                        LastBedName = dtLastBed.Rows[0]["HIS_M_BEDDESC"].ToString();
                    }
                    //sb.Append(@"<div><span>Location- " + LocationName + "</span><span style='font-size:12px;float: right;'>" + AdmitId + "</span></div>");
                    sb.Append(@"<table style='width:100%;border:white;'><tr>");
                    sb.Append(@"<td width='30%' style='border:white;'></td>");
                    sb.Append(@"<td style='text-align:Center;font-size:20px;border:white;' width='20%'>IP Final Bill</td>");
                    //sb.Append(@"<td style='text-align:right;font-size:20px;border-bottom:white;border-top:white;border-left:white;border-right:white;' width='5%'></td>");
                    sb.Append(@"<td style='width:10%;border:white;font-size:15px;'>Bill No: " + ds.Tables[0].Rows[0]["HIS_ip_FINAL_BillNO"] + "</td>");
                    sb.Append(@"<td style='width:20%;text-align:right;border:white;font-size:15px;'>" + ds.Tables[0].Rows[0]["finalbilldate"] + "</td>");
                    sb.Append(@"</tr></table></hr>");
                    sb.Append(@"<table  style='font-size: 14px; width: 96%;'>");
                    sb.Append(@"<tr><td style='width:20%'><b>Patient ID</b></td><td style='width:30%'>" + PatientID + "</td><td style='width:20%'><b>Admit ID</b></td><td style='width:30%'>" + AdmitId + "</td></tr>");
                    sb.Append(@"<tr><td><b>Patient Name</b></td><td>" + ds.Tables[0].Rows[0]["PATIENT_NAME"].ToString() + "</td><td><b>Room No</b></td><td>" + LastBedName + "</td></tr>");
                    sb.Append(@"<tr><td><b>Gender/Age</b></td><td>" + ds.Tables[0].Rows[0]["LIS_SEX"] + "/" + ds.Tables[0].Rows[0]["AGE"] + "</td><td><b>Address</b></td><td>" + ds.Tables[0].Rows[0]["ADDRESS"] + "</td></tr>");
                    sb.Append(@"<tr><td><b>Admit Date</b></td><td>" + ds.Tables[0].Rows[0]["HIS_AdmitDtTime"] + "</td><td><b>Discharge Date</b></td><td>" + ds.Tables[0].Rows[0]["discharge_date"] + "</td></tr>");
                    //sb.Append("<tr><td><b>Bill No</b></td><td><div style='float:left;width:50%'>" + ds.Tables[0].Rows[0]["HIS_ip_FINAL_BillNO"] + "</div><div style='float:right;width:50%;text-align: center;'>" + insbillno + "</div></td><td><b>Bill Date</b></td><td>" + ds.Tables[0].Rows[0]["finalbilldate"] + "</td></tr>");
                    //sb.Append(@"<tr><td><b>Bill No</b></td><td><div style='float:left;width:50%'>" + ds.Tables[0].Rows[0]["HIS_ip_FINAL_BillNO"] + "</div></td><td><b>Bill Date</b></td><td>" + ds.Tables[0].Rows[0]["finalbilldate"] + "</td></tr>");
                    sb.Append(@"<tr><td><b>Doctor</b></td><td>" + ds.Tables[0].Rows[0]["ADMITDOC"] + "</td><td><b>Discharge Type</b></td><td>" + ds.Tables[0].Rows[0]["HIS_DISCHARGE_TYPE"] + "</td>");

                    if (surgerdate != "")
                        sb.Append("<tr><td><b>Surgery Date</b></td><td>" + surgerdate + "</td><td><b></b></td><td></td></tr>");
                    if (ds.Tables[0].Rows[0]["his_patienttype"].ToString() == "CMP")
                    {
                        sb.Append("<tr><td><b>Company Name</b></td><td>" + ds.Tables[0].Rows[0]["HIS_CompanyName"] + "</td><td><b>Staff ID</b></td><td>" + ds.Tables[0].Rows[0]["HIS_Staffid_idold"] + "</td></tr>");
                    }

                    sb.Append(" </table>");
                }

                sb.Append("</br>");

                sb.Append(@" <table style='font-family:sans-serif;font-size: 12px;font-weight: 500;width:94%;margin-left: 1%;'>");
                sb.Append(@"<thead><tr>
                            <td style='width: 30px !important;'><b>SNO</b></td>
                            <td style='width: 170px !important;'><b>DATE</b></td>
                            <td style='width: 350px !important;'><b>PARTICULARS</b></td>
                            <td style='width: 30px !important;text-align:right;'><b>QTY</b></td>
                            <td  style='width: 60px !important;text-align:right;'><b>RATE</b></td>
                            <td  style='width: 70px !important;text-align:right;'><b>AMOUNT</b></td>
                        </tr></thead>");

                sb.Append("<tbody>");
                if (dtBed.Rows.Count > 0)
                {
                    int sno = 0;
                    BedTotalAmount = 0;

                    sb.Append("<tr><td colspan='3'><b>BED AND NURSING CHARGES</b></td></tr>");
                    for (int bed = 0; bed < dtBed.Rows.Count; bed++)
                    {
                        sno = sno + 1;
                        string str_NursingAmount = dtBed.Rows[bed]["NurseAmount"].ToString();

                        sb.Append("<tr>");
                        sb.Append("<td>" + (sno) + "</td>");
                        sb.Append("<td>" + dtBed.Rows[bed]["INDATE"].ToString() + " To " + dtBed.Rows[bed]["OUTDATE"].ToString() + "</td>");
                        sb.Append("<td>" + dtBed.Rows[bed]["HIS_IP_BEDID"].ToString() + "</td>");
                        sb.Append("<td style='text-align:right;'>" + dtBed.Rows[bed]["days_count"].ToString() + "</td>");
                        sb.Append("<td style='text-align:right;'>" + dtBed.Rows[bed]["HIS_M_Category_Rate"].ToString() + "</td>");
                        sb.Append("<td style='text-align:right;'>" + dtBed.Rows[bed]["BedAmount"].ToString() + "</td>");
                        sb.Append("</tr>");

                        if (str_NursingAmount != "0")
                        {
                            sno = sno + 1;
                            sb.Append("<tr>");
                            sb.Append("<td>" + (sno) + "</td>");
                            sb.Append("<td>" + dtBed.Rows[bed]["INDATE"].ToString() + " To " + dtBed.Rows[bed]["OUTDATE"].ToString() + "</td>");
                            sb.Append("<td>Nursing Charges</td>");
                            sb.Append("<td style='text-align:right;'>" + dtBed.Rows[bed]["days_count"].ToString() + "</td>");
                            sb.Append("<td style='text-align:right;'>" + dtBed.Rows[bed]["HIS_M_NursCharge_Rate"].ToString() + "</td>");
                            sb.Append("<td style='text-align:right;'>" + dtBed.Rows[bed]["NurseAmount"].ToString() + "</td>");
                            sb.Append("</tr>");
                        }
                        BedTotalAmount = BedTotalAmount + Convert.ToDecimal(dtBed.Rows[bed]["RowTotalAmount"].ToString());
                    }

                    sb.Append("<tr>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td><b>Total</b></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td style='text-align:right;'>" + BedTotalAmount + "</td>");
                    sb.Append("</tr>");
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    for (int serv = 0; serv < ds.Tables[2].Rows.Count; serv++)
                    {
                        int ipsno = 0;
                        string ServiceName = ds.Tables[2].Rows[serv]["HIS_M_ServiceName"].ToString();
                        sb.Append("<tr><td colspan='3'><b>" + ServiceName.ToUpper() + "</b></td></tr>");
                        DataTable IPdetails = _patientRepo.GetServiceWiseTestDetails(ServiceName, AdmitId);
                        if (IPdetails.Rows.Count > 0)
                        {
                            for (int det = 0; det < IPdetails.Rows.Count; det++)
                            {
                                ipsno = det + 1;
                                sb.Append("<tr>");
                                sb.Append("<td>" + ipsno + "</td>");
                                sb.Append("<td>" + dtPatientDet.Rows[0]["AdmitDate"].ToString() + " To " + dtPatientDet.Rows[0]["discharge_date"].ToString() + "</td>");
                                sb.Append("<td>" + IPdetails.Rows[det]["LIS_TestName"].ToString() + "</td>");
                                sb.Append("<td style='text-align:right;'>" + IPdetails.Rows[det]["Qty"].ToString() + "</td>");
                                sb.Append("<td style='text-align:right;'>" + IPdetails.Rows[det]["Rate"].ToString() + "</td>");
                                sb.Append("<td style='text-align:right;'>" + IPdetails.Rows[det]["Amount"].ToString() + "</td>");
                                sb.Append("</tr>");
                            }
                        }
                        sb.Append("<tr>");
                        sb.Append("<td></td>");
                        sb.Append("<td></td>");
                        sb.Append("<td><b>Total</b></td>");
                        sb.Append("<td></td>");
                        sb.Append("<td></td>");
                        sb.Append("<td style='text-align:right;'>" + ds.Tables[2].Rows[serv]["Amount"].ToString() + "</td>");
                        sb.Append("</tr>");
                    }
                }

                sb.Append("</tbody>");

                dtAdvance = _patientRepo.GetAdvanceByAdmitID(AdmitId);
                ds.Tables.Add(dtAdvance);

                if (dtAdvance.Rows.Count > 0)
                {
                    //sb.Append("<br/>");
                    //sb.Append("<br/>");
                    sb.Append("<table style='font-size: 16px; width: 96%;margin-top:20px;'>");
                    sb.Append("<tr><td colspan = '6' align='center'><b>ADVANCE DETAILS</b></td></tr>");
                    sb.Append("<tr><td><b>SNO</b></td><td><b>Date</b></td><td style='text-align:right;'><b>Amount</b></td></tr>");
                    sb.Append("<tr>");
                    sb.Append("<td>1</td>");
                    sb.Append("<td>" + dtPatientDet.Rows[0]["AdmitDate"].ToString() + " To " + dtPatientDet.Rows[0]["discharge_date"].ToString() + "</td>");
                    sb.Append("<td style='text-align:right;'>" + dtAdvance.Rows[0]["Amount"].ToString() + "</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");
                }

                dtOrderTotal = _patientRepo.GetOrderTotalByAdmitID(AdmitId, PatientID);

                //sb.Append("<br/>");
                //sb.Append("<br/>");
                sb.Append("<table style='font-size: 16px; width: 96%;margin-top:20px;'> ");
                sb.Append("<tbody>");
                sb.Append("<tr>");
                sb.Append("<td scope='col'><b>Total Bill Amount </b></td>");
                sb.Append("<td style='text-align:right;'>" + dtOrderTotal.Rows[0]["HIS_IP_Final_Amount"].ToString() + "</td>");
                sb.Append("</tr>");

                if (dtAdvance.Rows.Count > 0)
                {
                    sb.Append("<tr>");
                    sb.Append("<td scope='col'><b>Total Advance Received </b></td>");
                    sb.Append("<td style='text-align:right;'>" + dtAdvance.Rows[0]["Amount"].ToString() + "</td>");
                    sb.Append("</tr>");
                }

                if (dtOrderTotal.Rows[0]["HIS_IP_Final_Tot_Disc"].ToString() != "0.00")
                {
                    sb.Append("<tr>");
                    sb.Append("<td scope='col'><b>Discounted Amount</b></td>");
                    sb.Append("<td style='text-align:right;'>" + dtOrderTotal.Rows[0]["HIS_IP_Final_Tot_Disc"].ToString() + "</td>");
                    sb.Append("</tr>");
                }

                sb.Append("<tr>");
                sb.Append("<td scope='col'><b>Grand Total Amount</b></td>");
                sb.Append("<td style='text-align:right;'>" + dtOrderTotal.Rows[0]["HIS_IP_Final_NetAmt"].ToString() + "</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td scope='col'><b>Collected Amount</b></td>");
                sb.Append("<td style='text-align:right;'>" + dtOrderTotal.Rows[0]["CollectedAmount"].ToString() + "</td>");
                sb.Append("</tr>");
                if (dtOrderTotal.Rows[0]["HIS_CurrentPendingAmount"].ToString() != "0.00")
                {
                    sb.Append("<tr>");
                    sb.Append("<td scope='col'><b>Pending To Pay Amount</b></td>");
                    sb.Append("<td style='text-align:right;'>" + dtOrderTotal.Rows[0]["HIS_CurrentPendingAmount"].ToString() + "</td>");
                    sb.Append("</tr>");
                }

                if (dtOrderTotal.Rows[0]["HIS_IP_Current_Refund"].ToString() != "0.00")
                {
                    sb.Append("<tr>");
                    sb.Append("<td scope='col'><b>Refund Amount</b></td>");
                    sb.Append("<td style='text-align:right;'>" + dtOrderTotal.Rows[0]["HIS_IP_Current_Refund"].ToString() + "</td>");
                    sb.Append("</tr>");
                }



                //sb.Append("<tr>");
                //sb.Append("<td scope='col'><b>Payment Mode</b></td>");
                //sb.Append("<td style='text-align:right;'>" + dtOrderTotal.Rows[0]["HIS_PaymentMode"].ToString() + "</td>");
                //sb.Append("</tr>");
                decimal CollectedAmount = Convert.ToDecimal(dtOrderTotal.Rows[0]["CollectedAmount"].ToString());
                string Words_CollectedAmount = _patientRepo.NumberToWords(CollectedAmount);
                sb.Append("<tr>");
                sb.Append("<td colspan='2'><b> Rupees " + Words_CollectedAmount + " Only</b></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append(@"</html>");


            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}