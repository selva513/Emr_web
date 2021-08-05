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
using System.Net.Mail;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : Controller
    {
        private IDBConnection _IDBConnection;
        private ILoginRepo _loginrepo;
        private IErrorlog _errorlog;
        private ILicenceRepo _licenceRepo;
        private IPatientRepo _patientRepo;

        public UserController(IDBConnection iDBConnection, ILoginRepo loginrepo, IErrorlog errorlog, ILicenceRepo licenceRepo,IPatientRepo patientRepo)
        {
            _IDBConnection = iDBConnection;
            _loginrepo = loginrepo;
            _errorlog = errorlog;
            _licenceRepo = licenceRepo;
            _patientRepo = patientRepo;
        }
        public IActionResult LoginUser()
        {
            return View();
        }
        [HttpPost]
        public JsonResult InsertNewUser([FromBody] LoginView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    TimezoneUtility timezoneUtility = new TimezoneUtility();
                    string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                    if (Timezoneid == "" || Timezoneid == null)
                        Timezoneid = "India Standard Time";
                    var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                    List<Login> logins = myComplexObject;
                    long Licenceid = Convert.ToInt64(HttpContext.Session.GetString("Licenceid"));
                    int Usercount = _loginrepo.GetUserCount(Licenceid);
                    long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    long DocID = _loginrepo.GetDoctorID(data.Userid, Hospitalid);
                    int TotalUserCount = _loginrepo.GetUserByHospital(Hospitalid);
                    TotalUserCount = TotalUserCount + 1;
                    if (TotalUserCount <= Usercount)
                    {
                        Login login = new Login();
                        login.UserName = data.UserName;
                        login.Userid = data.Userid;
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
                        //login.Password = data.Password;
                        login.Password = rdnvalue;
                        login.RoleID = data.RoleID;
                        login.DoctorID = DocID;
                        login.DepartmentID = data.DepartmentID;
                        login.SpecialityID = data.SpecialityID;
                        login.UserType = data.UserType;
                        login.HospitalID = data.HospitalID;
                        login.ClinicID = data.ClinicID;
                        login.ForgotPwdEmail = data.ForgotPwdEmail;
                        login.ActivationKey = null;
                        login.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        login.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        login.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        login.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                        login.IsActive = true;
                        login.IsPrimaryUser = false;
                        login.LicenceID = Licenceid;
                        login.QuestionID = 0;
                        login.QuestionAnswer = null;
                        login.MobileNumber = data.MobileNumber;
                        if (logins[0].IsTrailUser)
                        {
                            long TrialPeriod = _loginrepo.GetTrialPeriod();
                            login.IsTrailUser = true;
                            login.TrailCreateDate = logins[0].TrailCreateDate;
                            login.TrailDays = TrialPeriod;
                        }
                        isSuccess = _loginrepo.InsertNewUser(login);
                        if (isSuccess == true)
                        {
                            string CheckMobileuserexist = _loginrepo.MobileUserExist(data.Userid, data.MobileNumber);
                            if (CheckMobileuserexist == null || CheckMobileuserexist == "")
                            {
                                MobileUsers mobileUsers = new MobileUsers();
                                mobileUsers.FirstName = data.UserName;
                                mobileUsers.Gender = "";
                                mobileUsers.MobileNo = data.MobileNumber;
                                mobileUsers.Email = data.Userid;
                                mobileUsers.IsActive = true;
                                mobileUsers.UserType = "Patient";
                                mobileUsers.City = "";
                                mobileUsers.ZipCode = null;
                                mobileUsers.CreateDatetime = timezoneUtility.Gettimezone(Timezoneid);
                                _patientRepo.CreateNewMobileUser(mobileUsers);
                            }
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
                    else
                    {
                        TempData["UserFailed"] = "User Count Exceeded.Please Contact Allied Business Solutions";
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
                TempData["UserFailed"] = "Something went wrong.Please contact Allied Business solutions";
            }
            finally { }
            return Json(isSuccess);
        }
        [HttpPost]
        public JsonResult UpdateUser([FromBody] LoginView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    TimezoneUtility timezoneUtility = new TimezoneUtility();
                    string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                    if (Timezoneid == "" || Timezoneid == null)
                        Timezoneid = "India Standard Time";
                    Login login = new Login();
                    login.UserName = data.UserName;
                    login.UserSeqid = data.UserSeqid;
                    login.RoleID = data.RoleID;
                    login.DoctorID = data.DoctorID;
                    login.DepartmentID = data.DepartmentID;
                    login.SpecialityID = data.SpecialityID;
                    login.UserType = data.UserType;
                    login.HospitalID = data.HospitalID;
                    login.ClinicID = data.ClinicID;
                    login.ForgotPwdEmail = data.ForgotPwdEmail;
                    login.ActivationKey = null;
                    login.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    login.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    login.MobileNumber = data.MobileNumber;
                    login.IsActive = true;
                    isSuccess = _loginrepo.UpdateUser(login);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Json(isSuccess);
        }

        [HttpPost]
        public JsonResult InsertSpeciality([FromBody] SpecialityMaster specialityMasters)
        {
            int isSuccess = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    TimezoneUtility timezoneUtility = new TimezoneUtility();
                    string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                    if (Timezoneid == "" || Timezoneid == null)
                        Timezoneid = "India Standard Time";
                    SpecialityMaster specialityMaster = new SpecialityMaster();
                    specialityMaster.Speciality_Name = specialityMasters.Speciality_Name;
                    specialityMaster.Description = specialityMasters.Description;
                    specialityMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                    specialityMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                    specialityMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                    specialityMaster.Isactive = true;
                    isSuccess = _loginrepo.InsertNewSpeciality(specialityMaster);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Json(isSuccess);
        }
    }
}