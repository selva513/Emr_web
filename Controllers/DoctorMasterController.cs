using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
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
using RestSharp;
using Syncfusion.EJ2.Navigations;
using Syncfusion.EJ2.PivotView;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DoctorMasterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IDoctorRepo _doctorRepo;
        private IErrorlog _errorlog;
        private IDepartmentRepo _departmentRepo;
        private IPatientRepo _patientRepo;
        private ILicenceRepo _licenceRepo;
        private readonly ILoginRepo _loginRepo;
        private IHospitalRepo _hospitalRepo;
        private IConfiguration _configuration;
        public DoctorMasterController(IDBConnection iDBConnection, IDoctorRepo doctorRepo, IErrorlog errorlog, IDepartmentRepo departmentRepo, IPatientRepo patientRepo, ILoginRepo loginRepo, ILicenceRepo licenceRepo, IHospitalRepo hospitalRepo, IConfiguration configuration)
        {
            _IDBConnection = iDBConnection;
            _doctorRepo = doctorRepo;
            _errorlog = errorlog;
            _departmentRepo = departmentRepo;
            _patientRepo = patientRepo;
            _loginRepo = loginRepo;
            _licenceRepo = licenceRepo;
            _hospitalRepo = hospitalRepo;
            _configuration = configuration;
        }
        public async Task<IActionResult> DoctorMaster()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                var lstDepartment = await _departmentRepo.GetAllDepartment();
                if (lstDepartment != null)
                {
                    lstDepartment = lstDepartment as List<DepartmentMaster>;
                    ViewBag.DeptDetail = lstDepartment;
                }
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                var lst = await _doctorRepo.GetAllDoctor(HospitalID);
                if (lst != null)
                {
                    lst = lst as List<DoctorMaster>;
                    ViewBag.DoctorDetail = lst;
                }
                List<Login> myconfig = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                bool HISConnected = myconfig[0].IsConnectedHIS;
                if (HISConnected == true)
                {
                    var HISDoclst = await _doctorRepo.GetAllHISDoctor();
                    if (HISDoclst != null)
                    {
                        HISDoclst = HISDoclst as List<DoctorMaster>;
                        ViewBag.HISDocDetail = HISDoclst;
                    }
                }
                else
                    ViewBag.HISDocDetail = null;

                List<SpecialityMaster> list = new List<SpecialityMaster>();
                list = _patientRepo.GetAllSpeciality();
                ViewBag.Speciality = list;
                GetCity();
                GetCountry();
                GetGender();
                GetMedicalCouncil();
                GetAllClinic();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public IActionResult InsertDoctor([Bind] DoctorView model)
        {
            long result = 0;
            try
            {
                if (model.DoctorSeqID == 0)
                {
                    result = InsertNewDoctor(model, true);
                    if (result > 0)
                        TempData["Doctor"] = "Registration Successfull.We will check your Registration Number and Send an Email to you Once it is confirmed.";
                    else
                        TempData["Doctor"] = "Registration Fails";
                }
                else
                {
                    result = InsertNewDoctor(model, false);
                    TempData["Doctor"] = null;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("DoctorMaster", "DoctorMaster");
        }
        private long InsertNewDoctor(DoctorView model, bool validation)
        {
            long result = 0;
            string urlpath = "";
            string signurlpath = "";
            string urlgetpath = "";
            string signurlgetpath = "";
            try
            {
                string UploadPath = _configuration.GetConnectionString("DoctorPhotoInsertPath");
                string SignUploadPath = _configuration.GetConnectionString("DoctorSignInsertPath");
                string UploadgetPath = _configuration.GetConnectionString("DoctorPhotogetPath");
                string SignUploadgetPath = _configuration.GetConnectionString("DoctorSignGetPath");
                List<HospitalMaster> lsthospitalMaster = _hospitalRepo.GetHospitalDetails(Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")));
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                //if (model.DepartmentID==0)
                //{
                //    DepartmentMaster departmentMaster = new DepartmentMaster();
                //    departmentMaster.DepartmentName = model.DepartmentName;
                //    departmentMaster.HospitalID = HttpContext.Session.GetString("Hospitalid");
                //    departmentMaster.CreatedDate = timezoneUtility.Gettimezone(Timezoneid);
                //    departmentMaster.ModifyDate = timezoneUtility.Gettimezone(Timezoneid);
                //    departmentMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                //    departmentMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                //    departmentMaster.Isactive = true;
                //    int result1 = _departmentRepo.CreateNewDepartment(departmentMaster);
                //    model.DepartmentID = result1;
                //}
                if (model.SpecialityID == 0)
                {
                    SpecialityMaster specialityMaster = new SpecialityMaster();
                    specialityMaster.Speciality_Name = model.Speciality_Name;
                    specialityMaster.Description = "";
                    specialityMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    specialityMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                    specialityMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    specialityMaster.Isactive = true;
                    int result2 = _loginRepo.InsertNewSpeciality(specialityMaster);
                    model.SpecialityID = result2;
                }

                DoctorMaster doctorMaster = new DoctorMaster();
                doctorMaster.DoctorName = model.DoctorName;
                doctorMaster.DoctorDegree = model.DoctorDegree;
                doctorMaster.DepartmentID = model.DepartmentID;
                doctorMaster.SpecialityID = model.SpecialityID;
                doctorMaster.HospitalID = HttpContext.Session.GetString("Hospitalid");
                doctorMaster.CreatedDate = timezoneUtility.Gettimezone(Timezoneid);
                doctorMaster.ModifyDate = timezoneUtility.Gettimezone(Timezoneid);
                doctorMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                doctorMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                doctorMaster.HISDoctorID = model.HISDoctorID;
                doctorMaster.Isactive = model.Isactive;
                doctorMaster.ConsultingFees = model.ConsultingFees;
                doctorMaster.RegistrationNumber = model.RegistrationNumber;
                doctorMaster.RegistrationCouncil = model.RegistrationCouncil;
                doctorMaster.PassedOutYear = model.PassedOutYear;
                doctorMaster.City = model.City;
                doctorMaster.Country = model.Country;
                doctorMaster.Gender = model.Gender;
                doctorMaster.PostGraduationDegree = model.PostGraduationDegree;
                doctorMaster.SuperSpecialityDegree = model.SuperSpecialityDegree;
                doctorMaster.SpecialityExperience = model.SpecialityExperience;
                doctorMaster.OverallExperience = model.OverallExperience;
                doctorMaster.BankAccountNumber = model.BankAccountNumber;
                doctorMaster.IFSC_Code = model.IFSC_Code;
                doctorMaster.Others = model.Others;
                doctorMaster.Comments = model.Comments;
                doctorMaster.Alternative_Number = model.Alternative_Number;
                doctorMaster.EmailId = model.EmailId;
                doctorMaster.Resume = model.Resume;
                doctorMaster.MobileNumber = model.MobileNumber;
                doctorMaster.DoctorAddress1 = model.DoctorAddress1;
                doctorMaster.DoctorAddress2 = model.DoctorAddress2;
                doctorMaster.Area = model.Area;
                doctorMaster.Pincode = model.Pincode;
                doctorMaster.CountryCode = model.CountryCode;
                //doctorMaster.Clinic_HospitalName = model.Clinic_HospitalName;
                doctorMaster.GatewayCharges = model.GatewayCharges;
                doctorMaster.NetConsultFees = model.NetConsultFees;
                doctorMaster.BankName = model.BankName;
                doctorMaster.DirectConsultFees = model.DirectConsultFees;
                doctorMaster.DirectNetFees = model.DirectNetFees;
                doctorMaster.DirectGatewayCharges = model.DirectGatewayCharges;
                doctorMaster.GatewayPercentage = lsthospitalMaster[0].GatewayPercentage;
                doctorMaster.GatewayAmount = lsthospitalMaster[0].GatewayAmount;

                if (model.DoctorPhoto != null)
                {
                    if (model.DoctorPhoto.Length > 0)
                        doctorMaster.DoctorPhoto = CommonSetting.GetImageBytes(model.DoctorPhoto);

                }
                if (model.DoctorSignature != null)
                {
                    if (model.DoctorSignature.Length > 0)
                        doctorMaster.DoctorSignature = CommonSetting.GetImageBytes(model.DoctorSignature);
                }

                if (validation == true)
                {
                    doctorMaster.IsConfirmRegistration = false;
                    result = _doctorRepo.CreateNewDoctor(doctorMaster);
                    if (result > 0)
                    {
                        model.DoctorSeqID = result;
                        if (model.DoctorPhoto != null)
                        {
                            string ImageName = model.DoctorSeqID.ToString() + Regex.Replace(doctorMaster.DoctorName, @"\s", "");
                            urlpath = Path.Combine(UploadPath, ImageName + "Logo.png");
                            urlgetpath = Path.Combine(UploadgetPath, ImageName + "Logo.png");
                            var filePath = urlpath;
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                                System.IO.File.WriteAllBytes(filePath, doctorMaster.DoctorPhoto);
                            }
                        }
                        else
                        {
                            urlpath = Path.Combine(UploadPath, "NoImage.png");
                            urlgetpath = Path.Combine(UploadgetPath, "NoImage.png");
                        }
                        if (model.DoctorSignature != null)
                        {
                            string ImageName = model.DoctorSeqID.ToString() + Regex.Replace(doctorMaster.DoctorName, @"\s", "");
                            signurlpath = Path.Combine(SignUploadPath, ImageName + "Sign.png");
                            signurlgetpath= Path.Combine(SignUploadgetPath, ImageName + "Sign.png");
                            var filePathsign = signurlpath;
                            if (System.IO.File.Exists(filePathsign))
                            {
                                System.IO.File.Delete(filePathsign);
                                System.IO.File.WriteAllBytes(filePathsign, doctorMaster.DoctorSignature);
                            }
                        }
                        else
                        {
                            signurlpath = Path.Combine(SignUploadPath, "NoImage.png");
                            signurlgetpath = Path.Combine(SignUploadgetPath, "NoImage.png");
                        }

                        doctorMaster.DoctorPhotoUrl = urlgetpath;
                        doctorMaster.DoctorSignUrl = signurlgetpath;
                        int UpdateDoctorPhotoUrl = _doctorRepo.UpdateDoctorPhotoUrl(doctorMaster.DoctorPhotoUrl, doctorMaster.DoctorSignUrl, model.DoctorSeqID);

                        if (model.IsUserExist)
                            UpdateLoginUser(model);
                        //else
                        //    CreateNewLoginUser(model);

                        SendMailToEmployee(doctorMaster);
                        SendMailToDoctor(doctorMaster);
                        //CreateNewMobileUser(model);
                    }
                }
                else
                {
                    List<DoctorMaster> lstResult = _patientRepo.GetDoctorDetails(model.DoctorSeqID);
                    doctorMaster.DoctorSeqID = model.DoctorSeqID;
                    if (model.DoctorPhoto != null)
                    {
                        string ImageName = doctorMaster.DoctorSeqID.ToString() + Regex.Replace(doctorMaster.DoctorName, @"\s", "");
                        urlpath = Path.Combine(UploadPath, ImageName + "Logo.png");
                        urlgetpath = Path.Combine(UploadgetPath, ImageName + "Logo.png");
                        var filePath = urlpath;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                            System.IO.File.WriteAllBytes(filePath, doctorMaster.DoctorPhoto);
                        }
                    }
                    else
                    {
                        byte[] img = lstResult[0].DoctorPhoto;
                        if (img != null && img.Length > 0)
                        {
                            urlgetpath = lstResult[0].DoctorPhotoUrl;
                        }
                        else
                        {
                            urlpath = Path.Combine(UploadPath, "NoImage.png");
                            urlgetpath = Path.Combine(UploadgetPath, "NoImage.png");
                        }

                    }
                    if (model.DoctorSignature != null)
                    {
                        string ImageName = doctorMaster.DoctorSeqID.ToString() + Regex.Replace(doctorMaster.DoctorName, @"\s", "");
                        signurlpath = Path.Combine(SignUploadPath, ImageName + "Sign.png");
                        signurlgetpath = Path.Combine(SignUploadgetPath, ImageName + "Sign.png");
                        var filePathsign = signurlpath;
                        if (!System.IO.File.Exists(filePathsign))
                        {
                            System.IO.File.WriteAllBytes(filePathsign, doctorMaster.DoctorSignature);
                            //System.IO.File.Delete(filePathsign);
                        }
                    }
                    else
                    {
                        byte[] img = lstResult[0].DoctorSignature;
                        if (img != null && img.Length > 0)
                        {
                            signurlgetpath = lstResult[0].DoctorSignUrl;
                        }
                        else
                        {
                            signurlpath = Path.Combine(SignUploadPath, "NoImage.png");
                            signurlgetpath = Path.Combine(SignUploadgetPath, "NoImage.png");
                        }
                    }

                    doctorMaster.DoctorPhotoUrl = urlgetpath;
                    doctorMaster.DoctorSignUrl = signurlgetpath;
                    result = _doctorRepo.UpdateDoctor(doctorMaster);
                    UpdateLoginUser(model);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }
        private void CreateNewLoginUser(DoctorView data)
        {
            bool isSuccess = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                List<Login> logins = myComplexObject;

                Login login = new Login();
                login.UserName = data.DoctorName;
                login.Userid = data.EmailId;
                int value = 0;
                string rdnvalue = "";
                Random rnd = new Random();
                value = rnd.Next(10000, 99999);
                string HospitalPrefix = _licenceRepo.GetHospitalPrefix();
                rdnvalue = HospitalPrefix + value.ToString();
                bool value_ok = false;
                while (!value_ok)
                {
                    string CheckRdnexist = _licenceRepo.RandomvalExist(rdnvalue);
                    if (CheckRdnexist != null && CheckRdnexist != "")
                    {
                        value = rnd.Next(10000, 99999);
                        rdnvalue = HospitalPrefix + value.ToString();
                    }
                    else
                        value_ok = true;
                }

                login.Password = rdnvalue;
                login.RoleID = Convert.ToInt64(HttpContext.Session.GetString("Roleid"));
                login.DoctorID = data.DoctorSeqID;
                login.DepartmentID = data.DepartmentID;
                login.SpecialityID = data.SpecialityID;
                login.UserType = 1;
                login.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                login.ClinicID = Convert.ToInt64(data.Clinic_HospitalName);
                login.ForgotPwdEmail = null;
                login.ActivationKey = null;
                login.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                login.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                login.CreatedUser = HttpContext.Session.GetString("Userseqid");
                login.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                login.IsActive = true;
                login.IsPrimaryUser = false;
                login.LicenceID = Convert.ToInt64(HttpContext.Session.GetString("Licenceid"));
                login.QuestionID = 0;
                login.QuestionAnswer = null;
                login.MobileNumber = (long)data.MobileNumber;
                if (logins[0].IsTrailUser)
                {
                    long TrialPeriod = _loginRepo.GetTrialPeriod();
                    login.IsTrailUser = true;
                    login.TrailCreateDate = logins[0].TrailCreateDate;
                    login.TrailDays = TrialPeriod;
                }
                isSuccess = _loginRepo.InsertNewUser(login);
                if (isSuccess == true)
                {
                    List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                    if (licenceconfig.Count > 0)
                    {
                        string MsgSubject = "";
                        string Email = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceToEmail.ToString());
                        string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                        string Password = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                        if (logins[0].IsTrailUser)
                            MsgSubject = "Trail Licence Activation for Knockdok";
                        else
                            MsgSubject = "Licence Activation for Knockdok";
                        string linksmp = "";
                        linksmp += "Welcome " + login.UserName + "!";
                        linksmp += "<br/>";
                        linksmp += "<br/>";
                        linksmp += "Enjoy the benefits and capabilities of our Knockdok. Please log in with ";
                        linksmp += "<br/>";
                        linksmp += "<br/>";
                        linksmp += "User Name  <b>" + login.Userid + "</b>";
                        linksmp += "<br/>";
                        linksmp += "<br/>";
                        linksmp += "Temporary Password  <b>" + rdnvalue + "</b>. Please change the password once you log in. ";

                        using (MailMessage mm = new MailMessage(sendmail, login.Userid))
                        {
                            mm.Subject = MsgSubject;
                            mm.Body = linksmp;
                            mm.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                EnableSsl = true
                            };
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential
                            {
                                UserName = sendmail,
                                Password = Password
                            };
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = credentials;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private bool UpdateLoginUser(DoctorView data)
        {
            bool isSuccess = false;
            List<Login> lstlogin = new List<Login>();
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);
                lstlogin = _loginRepo.GetUserDetails(data.EmailId, CurrentDatetime);

                Login login = new Login();
                login.UserName = data.DoctorName;
                login.UserSeqid = lstlogin[0].UserSeqid;
                login.RoleID = lstlogin[0].RoleID;
                login.DoctorID = data.DoctorSeqID;
                login.DepartmentID = data.DepartmentID;
                login.SpecialityID = data.SpecialityID;
                login.UserType = 1;
                login.HospitalID = lstlogin[0].HospitalID;
                login.ClinicID = Convert.ToInt64(data.Clinic_HospitalName);
                login.ForgotPwdEmail = lstlogin[0].ForgotPwdEmail;
                login.ActivationKey = null;
                login.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                login.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                //login.MobileNumber = (long)data.MobileNumber;
                login.IsActive = true;
                isSuccess = _loginRepo.UpdateUser(login);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return isSuccess;
        }
        private void CreateNewMobileUser(DoctorView model)
        {
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");

                MobileUsers mobileUsers = new MobileUsers();
                mobileUsers.FirstName = model.DoctorName;
                mobileUsers.Gender = _patientRepo.GetGenderByID(Convert.ToInt64(model.Gender));
                mobileUsers.MobileNo = (long)model.MobileNumber;
                mobileUsers.Email = model.EmailId;
                mobileUsers.IsActive = true;
                mobileUsers.UserType = "Doctor";
                if (model.City != null)
                    mobileUsers.City = _patientRepo.GetCityByID(Convert.ToInt64(model.City));
                mobileUsers.ZipCode = model.Pincode;
                mobileUsers.CreateDatetime = timezoneUtility.Gettimezone(Timezoneid);
                _patientRepo.CreateNewMobileUser(mobileUsers);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private void GetCity()
        {
            List<CityMaster> lstCity = _patientRepo.GetCityList();
            ViewBag.City = lstCity;
        }
        private void GetCountry()
        {
            List<CountryMaster> lstCountry = _patientRepo.GetAllCountry();
            ViewBag.Country = lstCountry;
        }
        private void GetGender()
        {
            List<GenderMaster> lstGender = _patientRepo.GetAllGender();
            ViewBag.Gender = lstGender;
        }
        private void GetMedicalCouncil()
        {
            List<MedicalCouncilMaster> lstMedicalCouncil = _patientRepo.GetAllMedicalCouncil();
            ViewBag.MedicalCouncil = lstMedicalCouncil;
        }
        private void GetAllClinic()
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<ClinicMaster> list = new List<ClinicMaster>();
            list = _patientRepo.GetClinicByHospitalID(HospitalId);
            ViewBag.Clinic = list;
        }
        private void SendMailToEmployee(DoctorMaster doctorMaster)
        {
            try
            {
                List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                if (licenceconfig.Count > 0)
                {
                    string Email = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceToEmail.ToString());
                    string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                    string Passwordd = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                    string CouncilName = (_doctorRepo.GetMedicalCouncilDetails(Convert.ToInt64(doctorMaster.RegistrationCouncil)))[0].CouncilName;
                    string MsgSubject = "Alert! Doctor Inserted";
                    string linksmp = "";
                    linksmp += "Doctor " + doctorMaster.DoctorName + "";
                    linksmp += "<br/>";
                    linksmp += "Has been registered in our EMR. Please confirm the register number provided and kindly approve as soon as possible. ";
                    linksmp += "<br/>";
                    linksmp += "<br/>";
                    linksmp += "Doctor Email : <b>" + doctorMaster.EmailId + "</b>";
                    linksmp += "<br/>";
                    linksmp += "Registration Number : <b>" + doctorMaster.RegistrationNumber + "</b>";
                    linksmp += "<br/>";
                    linksmp += "Medical Council : <b>" + CouncilName + "</b>";
                    linksmp += "<br/>";

                    List<EmployeeEmailMaster> Emaillst = _doctorRepo.GetEmployeeEmailList();
                    if (Emaillst.Count > 0)
                    {
                        for (int i = 0; i < Emaillst.Count(); i++)
                        {
                            using (MailMessage mm = new MailMessage(sendmail, Emaillst[i].EmployeeEmail))
                            {
                                mm.Subject = MsgSubject;
                                mm.Body = linksmp;
                                mm.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient
                                {
                                    Host = "smtp.gmail.com",
                                    EnableSsl = true
                                };
                                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential
                                {
                                    UserName = sendmail,
                                    Password = Passwordd
                                };
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = credentials;
                                smtp.Port = 587;
                                smtp.Send(mm);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private void SendMailToDoctor(DoctorMaster doctorMaster)
        {
            try
            {
                List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                if (licenceconfig.Count > 0)
                {
                    string Email = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceToEmail.ToString());
                    string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                    string Passwordd = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                    string MsgSubject = "Thank you Doctor " + doctorMaster.DoctorName + " for choosing to use Knockdok";
                    string linksmp = "";
                    linksmp += "Thank you Doctor " + doctorMaster.DoctorName + " for choosing to use Knockdok. The benefit of using Knockdok is multifaceted including your contribution to the society as a whole. There are few other informations have to be provided by you to list you in the doctor's outreach program. Kindly do it as early as possible and participate in the program.";

                    using (MailMessage mm = new MailMessage(sendmail, doctorMaster.EmailId))
                    {
                        mm.Subject = MsgSubject;
                        mm.Body = linksmp;
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            EnableSsl = true
                        };
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential
                        {
                            UserName = sendmail,
                            Password = Passwordd
                        };
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = credentials;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
        }
        private void ApprovedMailToDoctor(List<DoctorMaster> lstdoctor)
        {
            try
            {
                List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                if (licenceconfig.Count > 0)
                {
                    string Email = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceToEmail.ToString());
                    string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                    string Passwordd = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                    //string MsgSubject = "Knockdok – Profile Activation";
                    string EmailSubject = "";
                    string content = "";
                    string linksmp = "";
                    string addr1 = lstdoctor[0].DoctorAddress1;
                    string addr2 = lstdoctor[0].DoctorAddress2;
                    string HospitalName = _hospitalRepo.GetHospitalDetails(Convert.ToInt64(lstdoctor[0].HospitalID))[0].HospitalName;
                    if (addr2 == null)
                        addr2 = "";
                    else
                        addr2 += ", ";
                    if (lstdoctor[0].Alternative_Number == null)
                        lstdoctor[0].Alternative_Number = "";
                    if (lstdoctor[0].PostGraduationDegree == null)
                        lstdoctor[0].PostGraduationDegree = "";
                    if (lstdoctor[0].SuperSpecialityDegree == null)
                        lstdoctor[0].SuperSpecialityDegree = "";
                    if (lstdoctor[0].Speciality_Name == null)
                        lstdoctor[0].Speciality_Name = "";
                    if (lstdoctor[0].SpecialityExperience == null)
                        lstdoctor[0].SpecialityExperience = "";
                    if (lstdoctor[0].Resume == null)
                        lstdoctor[0].Resume = "";

                    string EmailTitle = "DoctorApproval";
                    List<EmailMaster> emailMaster = _licenceRepo.GetEmailByTitle(EmailTitle);
                    if (emailMaster.Count > 0)
                    {
                        EmailSubject = emailMaster[0].EmailSubject;
                        content = emailMaster[0].EmailBody;

                        content = content.Replace("&lt;Dr Name&gt;", lstdoctor[0].DoctorName)
                                .Replace("&lt;Gender&gt;", lstdoctor[0].GenderName)
                                .Replace("&lt;Email&gt;", lstdoctor[0].EmailId)
                                .Replace("&lt;Country Code&gt;", lstdoctor[0].CountryCode)
                                .Replace("&lt;Mobile No&gt;", lstdoctor[0].MobileNumber.ToString())
                                .Replace("&lt;Clinic Name&gt;", HospitalName)
                                .Replace("&lt;Address 1&gt;", lstdoctor[0].DoctorAddress1)
                                .Replace("&lt;Address 2&gt;", addr2)
                                .Replace("&lt;Area&gt;", lstdoctor[0].Area)
                                .Replace("&lt;City&gt;", lstdoctor[0].CityName)
                                .Replace("&lt;Pincode&gt;", lstdoctor[0].Pincode)
                                .Replace("&lt;Alternate Number&gt;", lstdoctor[0].Alternative_Number)
                                .Replace("&lt;Country&gt;", lstdoctor[0].CountryName)
                                .Replace("&lt;Graduation Degree&gt;", lstdoctor[0].DoctorDegree)
                                .Replace("&lt;Post Graduation&gt;", lstdoctor[0].PostGraduationDegree)
                                .Replace("&lt;Super Specialty&gt;", lstdoctor[0].SuperSpecialityDegree)
                                .Replace("&lt;Specialisation&gt;", lstdoctor[0].Speciality_Name)
                                .Replace("&lt;Specialty Experience&gt;", lstdoctor[0].SpecialityExperience)
                                .Replace("&lt;Over all Experience&gt;", lstdoctor[0].OverallExperience)
                                .Replace("&lt;Medical council No&gt;", lstdoctor[0].RegistrationNumber)
                                .Replace("&lt;Medical council Name&gt;", lstdoctor[0].RegistrationCouncilName)
                                .Replace("&lt;Consulting Fees&gt;", lstdoctor[0].ConsultingFees.ToString())
                                .Replace("&lt;Net Fees&gt;", lstdoctor[0].NetConsultFees.ToString())
                                .Replace("&lt;Bank account No&gt;", lstdoctor[0].BankAccountNumber)
                                .Replace("&lt;IFSC&gt;", lstdoctor[0].IFSC_Code)
                                .Replace("&lt;Profile&gt;", lstdoctor[0].Resume)
                                .Replace("&lt;Signature&gt;", "");
                        //.Replace("&lt;Signature&gt;", lstdoctor[0].DoctorSignature);
                        linksmp += content;

                        using (MailMessage mm = new MailMessage(sendmail, lstdoctor[0].EmailId))
                        {
                            mm.Subject = EmailSubject;
                            mm.Body = linksmp;
                            mm.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                EnableSsl = true
                            };
                            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential
                            {
                                UserName = sendmail,
                                Password = Passwordd
                            };
                            smtp.UseDefaultCredentials = true;
                            smtp.Credentials = credentials;
                            smtp.Port = 587;
                            smtp.Send(mm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async Task<IActionResult> DoctorApproval()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                var lst = await _doctorRepo.GetAllDoctorofAllHospital();
                if (lst != null)
                {
                    lst = lst as List<DoctorMaster>;
                    ViewBag.AllHospital_Doctor = lst;
                }
                List<Login> myconfig = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                List<SpecialityMaster> list = new List<SpecialityMaster>();
                list = _patientRepo.GetAllSpeciality();
                ViewBag.Speciality = list;
                GetCity();
                GetCountry();
                GetGender();
                GetMedicalCouncil();


            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public IActionResult ApproveDoctor([Bind] DoctorView model)
        {
            List<DoctorMaster> lstdoctor = new List<DoctorMaster>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            List<Config> Config = new List<Config>();
            string Identifier = "Doctor Profile Activation";
            List<MessageMaster> lst = new List<MessageMaster>();
            string MsgContent = "";
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string Approvedby = HttpContext.Session.GetString("UserName");
                bool isApproved = _doctorRepo.ApproveDoctor(model.DoctorSeqID, Approvedby);
                if (isApproved)
                {
                    _errorlog.WriteErrorLog("Inside IsApproved");
                    lstdoctor = _patientRepo.GetDoctorDetails(model.DoctorSeqID);
                    if (lstdoctor.Count() > 0)
                    {
                        _errorlog.WriteErrorLog("Inside Count");
                        _errorlog.WriteErrorLog(lstdoctor[0].EmailId);
                        _errorlog.WriteErrorLog(model.DoctorSeqID.ToString());
                        string EmailId = Encrypt_Decrypt.Encrypt(lstdoctor[0].EmailId);
                        bool result = _doctorRepo.UpdateDoctorID_Login(EmailId, model.DoctorSeqID);
                        _errorlog.WriteErrorLog("Completd");
                    }
                    ApprovedMailToDoctor(lstdoctor);
                    lst = _loginRepo.GetMessageInfo(Identifier);
                    if (lst.Count() > 0)
                    {
                        MsgContent = lst[0].PushNotification;
                        MsgContent = MsgContent.Replace("<Doctor Name>", lstdoctor[0].DoctorName);
                        _loginRepo.SendPushNotification(lst[0].MsgHeader, MsgContent, Convert.ToInt64(lstdoctor[0].MobileNumber));
                        bool Isupdated = InsertNotification(lst[0].MsgHeader, MsgContent, Convert.ToInt64(lstdoctor[0].MobileNumber), lst[0].UserType);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("DoctorApproval", "DoctorMaster");
        }
        public bool InsertNotification(string MsgHeader, string MsgContent, long MobileNumber, string UserType)
        {
            bool Isupdated = false;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                NotificationList notificationList = new NotificationList
                {
                    NotificationHeading = MsgHeader,
                    NotificationContent = MsgContent,
                    MobileNo = MobileNumber,
                    CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    IsActive = true,
                    UserType = UserType
                };
                Isupdated = _loginRepo.InsertNewNotification(notificationList);
            }
            catch (Exception ex)
            {

            }
            return Isupdated;
        }
        [HttpGet]
        public JsonResult ProfileReject(long DoctorID, string Reason, string Email, long MobileNo, string DoctorName)
        {
            string Status = "0";
            string EmailSubject = "";
            string content = "";
            string linksmp = "";
            string Identifier = "Profile not Activated";
            List<MessageMaster> lst = new List<MessageMaster>();
            string MsgContent = "";
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string UserID = HttpContext.Session.GetString("Userid");
                string CreatedTime = timezoneUtility.Gettimezone(Timezoneid);
                bool IsUpdated = _doctorRepo.UpdateProfileRejection(DoctorID, Reason, UserID, CreatedTime);
                if (IsUpdated == true)
                {
                    List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                    if (licenceconfig.Count > 0)
                    {
                        string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                        string Passwordd = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                        string EmailTitle = "Profile not Activated";
                        List<EmailMaster> emailMaster = _licenceRepo.GetEmailByTitle(EmailTitle);
                        if (emailMaster.Count > 0)
                        {
                            EmailSubject = emailMaster[0].EmailSubject;
                            content = emailMaster[0].EmailBody;
                            content = content.Replace("Text message", Reason);
                            linksmp += content;

                            using (MailMessage mm = new MailMessage(sendmail, Email))
                            {
                                mm.Subject = EmailSubject;
                                mm.Body = linksmp;
                                mm.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient
                                {
                                    Host = "smtp.gmail.com",
                                    EnableSsl = true
                                };
                                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential
                                {
                                    UserName = sendmail,
                                    Password = Passwordd
                                };
                                smtp.UseDefaultCredentials = true;
                                smtp.Credentials = credentials;
                                smtp.Port = 587;
                                smtp.Send(mm);
                            }
                        }
                        lst = _loginRepo.GetMessageInfo(Identifier);
                        if (lst.Count() > 0)
                        {
                            MsgContent = lst[0].PushNotification;
                            MsgContent = MsgContent.Replace("<Doctor Name>", DoctorName);
                            _loginRepo.SendPushNotification(lst[0].MsgHeader, MsgContent, Convert.ToInt64(MobileNo));
                            bool Isupdated = InsertNotification(lst[0].MsgHeader, MsgContent, Convert.ToInt64(MobileNo), lst[0].UserType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Status);
        }
    }
}