using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Emr_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Emr_web.Controllers
{
    public class LicenceController : Controller
    {
        private IDBConnection _IDBConnection;
        private ILicenceRepo _licenceRepo;
        private IErrorlog _errorlog;
        private ILoginRepo _loginrepo;
        private IHospitalRepo _hospitalRepo;
        private IClinicRepo _clinicRepo;
        private IUserRoleRepo _userRoleRepo;
        private IPatientRepo _patientRepo;
        private IConfiguration _configuration;

        public LicenceController(IDBConnection iDBConnection, ILicenceRepo licenceRepo, IErrorlog errorlog, ILoginRepo loginRepo, IHospitalRepo hospitalRepo, IUserRoleRepo userRoleRepo, IClinicRepo clinicRepo, IPatientRepo patientRepo, IConfiguration configuration)
        {
            _IDBConnection = iDBConnection;
            _licenceRepo = licenceRepo;
            _errorlog = errorlog;
            _loginrepo = loginRepo;
            _hospitalRepo = hospitalRepo;
            _userRoleRepo = userRoleRepo;
            _clinicRepo = clinicRepo;
            _patientRepo = patientRepo;
            _configuration = configuration;
        }
        public IActionResult Licence()
        {
            return View();
        }
        [HttpPost]
        public JsonResult NewLicenceInsert([FromBody] LicenceMasterView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    string UploadPath = _configuration.GetConnectionString("HospitalLogoInsertPath");
                    string UploadgetPath = _configuration.GetConnectionString("HospitalLogogetPath");
                    string Location = Path.Combine(UploadPath, "NoImage.png");
                    string GetLocation = Path.Combine(UploadgetPath, "NoImage.png");

                    long Hospitalid = 0;
                    long Clinicid = 0;
                    long Liceneid = 0;
                    int value = 0;
                    string rdnvalue = "";
                    Random rnd = new Random();
                    value = rnd.Next(10000, 99999);
                    string HospitalPrefix = _licenceRepo.GetHospitalPrefix();
                    rdnvalue = HospitalPrefix + value.ToString();
                    bool value_ok = false;
                    long MappingRoleid = _licenceRepo.GetRoleidByUserType(2);
                    if (MappingRoleid == 0)
                        MappingRoleid = 1;
                    List<MenuGroupDetail> menuGroupDetails = _licenceRepo.GetMenuGroupByRoleid(MappingRoleid);
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

                    HospitalMaster hospitalMaster = new HospitalMaster();
                    hospitalMaster.Hospital_Uniqueno = rdnvalue;
                    hospitalMaster.HospitalName = data.Hospitalname;
                    hospitalMaster.HospitalMobileNo = data.MobileNo.ToString();
                    hospitalMaster.HospitalLandlineNo = null;
                    hospitalMaster.HospitalLandlineNo1 = null;
                    hospitalMaster.HospitalAddress = null;
                    hospitalMaster.HospitalAddress1 = null;
                    hospitalMaster.HospitalAddress2 = null;
                    hospitalMaster.City = data.CityId.ToString();
                    hospitalMaster.Country = data.CountryId.ToString(); ;
                    hospitalMaster.Pin = data.Pincode;
                    hospitalMaster.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    hospitalMaster.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    hospitalMaster.CreatedUser = "";
                    hospitalMaster.ModifiedUser = "";
                    hospitalMaster.Isactive = true;
                    hospitalMaster.Prefix = data.Hospitalname.Length <= 3 ? data.Hospitalname : data.Hospitalname.Substring(0, 3) + "-";
                    hospitalMaster.HospitalLogoUrl = GetLocation;
                    Hospitalid = _hospitalRepo.InsertHospital(hospitalMaster);

                    ClinicMaster clinicMaster = new ClinicMaster();
                    clinicMaster.HospitalID = Hospitalid;
                    clinicMaster.ClinicName = data.Hospitalname;
                    clinicMaster.ClinicMobileNo = data.MobileNo.ToString();
                    clinicMaster.ClinicLandlineNo = null;
                    clinicMaster.ClinicLandlineNo1 = null;
                    clinicMaster.ClinicAddress = null;
                    clinicMaster.ClinicAddress1 = null;
                    clinicMaster.ClinicAddress2 = null;
                    clinicMaster.City = data.CityId.ToString();
                    clinicMaster.Country = data.CountryId.ToString();
                    clinicMaster.Pin = data.Pincode;
                    clinicMaster.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    clinicMaster.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    clinicMaster.CreatedUser = "";
                    clinicMaster.ModifiedUser = "";
                    clinicMaster.Isactive = true;
                    clinicMaster.TeleConsultation = false;
                    Clinicid = _clinicRepo.InsertNewClinic_FromLicence(clinicMaster);

                    clinicMaster.ClinicName = data.Hospitalname + " " + "TeleConsultation";
                    clinicMaster.TeleConsultation = true;
                    Clinicid = _clinicRepo.InsertNewClinic_FromLicence(clinicMaster);

                    LicenceView licenceMasterView = new LicenceView();
                    licenceMasterView.HospitalID = Hospitalid;
                    licenceMasterView.UserTypeID = 2;
                    licenceMasterView.PrimaryUserName = data.PrimaryUserName;
                    licenceMasterView.MobileNo = data.MobileNo;
                    licenceMasterView.EmailID = data.EmailID;
                    licenceMasterView.ClinicsCount = data.ClinicsCount;
                    licenceMasterView.UsersCount = data.UsersCount;
                    licenceMasterView.SearchType = data.SearchType;
                    licenceMasterView.OtherNotes = data.OtherNotes;
                    licenceMasterView.AgentMobileNo = data.AgentMobileNo;
                    licenceMasterView.IsSendEmail = false;
                    Liceneid = _licenceRepo.InsertLicence(licenceMasterView);

                    RoleMasterDetail roleMasterDetail = new RoleMasterDetail();
                    roleMasterDetail.Rolename = "Administrator";
                    roleMasterDetail.HospitalID = Hospitalid;
                    roleMasterDetail.ClinicID = 0;
                    roleMasterDetail.AllowedDiscount = 0;
                    roleMasterDetail.IsDiscountallowed = null;
                    roleMasterDetail.CreatedByRoleId = 0;
                    roleMasterDetail.IsAdminRole = null;
                    roleMasterDetail.AdminRoleId = 0;
                    roleMasterDetail.MyPatientData = true;
                    roleMasterDetail.AllPatientData = false;
                    roleMasterDetail.AllDoctorData = false;
                    roleMasterDetail.DocumentUpload = false;
                    roleMasterDetail.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    roleMasterDetail.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    roleMasterDetail.CreatedUser = "";
                    roleMasterDetail.ModifiedUser = "";
                    roleMasterDetail.IsActive = data.IsActive;
                    int roleid = _userRoleRepo.CreateNewRole(roleMasterDetail);

                    for (int i = 0; i < menuGroupDetails.Count; i++)
                    {
                        MenuGroupDetail menuGroupDetail = new MenuGroupDetail();
                        menuGroupDetail.G_Roleid = roleid;
                        menuGroupDetail.G_MainMenuid = menuGroupDetails[i].G_MainMenuid;
                        menuGroupDetail.G_SubMenuid = menuGroupDetails[i].G_SubMenuid;
                        menuGroupDetail.G_IsMainMenu = menuGroupDetails[i].G_IsMainMenu;
                        menuGroupDetail.G_Access = menuGroupDetails[i].G_Access;
                        menuGroupDetail.G_Add = menuGroupDetails[i].G_Add;
                        menuGroupDetail.G_Edit = null;
                        menuGroupDetail.G_Delete = menuGroupDetails[i].G_Delete;
                        menuGroupDetail.G_View = menuGroupDetails[i].G_View;
                        menuGroupDetail.G_Verify = menuGroupDetails[i].G_Verify;
                        menuGroupDetail.G_ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        menuGroupDetail.G_CreatedUser = "";
                        menuGroupDetail.G_ModifiedUser = "";
                        menuGroupDetail.G_IsActive = true;
                        isSuccess = _userRoleRepo.InsertMenuGroup(menuGroupDetail);
                    }

                    Login login = new Login();
                    login.UserName = data.PrimaryUserName;
                    login.Userid = data.EmailID;
                    login.Password = rdnvalue;
                    login.RoleID = roleid;
                    login.DepartmentID = 0;
                    login.SpecialityID = 0;
                    login.UserType = data.UserTypeID;
                    login.HospitalID = Hospitalid;
                    login.ClinicID = 0;
                    login.ForgotPwdEmail = null;
                    login.ActivationKey = null;
                    login.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    login.CreatedUser = "";
                    login.ModifiedUser = "";
                    login.IsActive = true;
                    login.IsPrimaryUser = true;
                    login.LicenceID = Liceneid;
                    login.IsTrailUser = false;
                    login.TrailCreateDate = null;
                    login.TrailDays = 0;
                    login.QuestionID = data.QuestionID;
                    login.QuestionAnswer = data.QuestionAnswer;
                    login.MobileNumber = data.MobileNo;
                    isSuccess = _loginrepo.InsertNewUser(login);
                    if (isSuccess == true)
                    {
                        List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                        if (licenceconfig.Count > 0)
                        {
                            string Email = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceToEmail.ToString());
                            string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                            string Password = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                            string MsgSubject = licenceconfig[0].MessageSubject.ToString();
                            string linksmp = "";
                            linksmp += "Hospital Name:" + data.Hospitalname + "";
                            linksmp += "<br/>";
                            linksmp += "<br/>";
                            linksmp += "Primary Username: " + data.PrimaryUserName + @"";
                            linksmp += "<br/>";
                            linksmp += "<br/>";
                            linksmp += "Mobile No: " + data.MobileNo + @"";
                            linksmp += "<br/>";
                            linksmp += "<br/>";
                            linksmp += "Hospital Unique No: " + rdnvalue + @"";
                            linksmp += "<br/>";
                            linksmp += "<br/>";
                            linksmp += "Primary User Email: " + data.EmailID + @"";
                            linksmp += "<br/>";
                            linksmp += "<br/>";
                            linksmp += "No of Clinics: " + data.ClinicsCount + @"";
                            linksmp += "<br/>";
                            linksmp += "<br/>";
                            linksmp += "No of Users: " + data.UsersCount + @"";
                            using (MailMessage mm = new MailMessage(sendmail, Email))
                            {
                                mm.Subject = MsgSubject;
                                mm.Body = linksmp;
                                mm.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = "smtp.gmail.com";
                                smtp.EnableSsl = true;
                                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                                credentials.UserName = sendmail;
                                credentials.Password = Password;
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
            finally { }
            return Json(isSuccess);
        }
        public IActionResult TrailLicence()
        {
            return View();
        }

        [HttpPost]
        public JsonResult NewTrailLicenceInsert([FromBody] LicenceMasterView data)
        {
            bool issuccess = false;
            try
            {
                issuccess = CreateNewTrailUser(data);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(issuccess);
        }
        private bool CreateNewTrailUser(LicenceMasterView data)
        {
            bool isSuccess = false;
            try
            {
                if (!_loginrepo.IsUserNameExists(UserName: data.EmailID))
                {
                    string UploadPath = _configuration.GetConnectionString("HospitalLogoInsertPath");
                    string UploadgetPath = _configuration.GetConnectionString("HospitalLogogetPath");
                    string Location = Path.Combine(UploadPath, "NoImage.png");
                    string GetLocation = Path.Combine(UploadgetPath, "NoImage.png");
                    long Hospitalid = 0;
                    long Clinicid = 0;
                    long Liceneid = 0;
                    int value = 0;
                    string rdnvalue = "";
                    Random rnd = new Random();
                    value = rnd.Next(10000, 99999);
                    string HospitalPrefix = _licenceRepo.GetHospitalPrefix();
                    rdnvalue = HospitalPrefix + value.ToString();
                    bool value_ok = false;
                    long MappingRoleid = _licenceRepo.GetRoleidByUserType(2);
                    if (MappingRoleid == 0)
                        MappingRoleid = 1;
                    List<MenuGroupDetail> menuGroupDetails = _licenceRepo.GetMenuGroupByRoleid(MappingRoleid);
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

                    HospitalMaster hospitalMaster = new HospitalMaster();
                    hospitalMaster.Hospital_Uniqueno = rdnvalue;
                    hospitalMaster.HospitalName = data.Hospitalname;
                    hospitalMaster.HospitalMobileNo = data.MobileNo.ToString();
                    hospitalMaster.HospitalLandlineNo = null;
                    hospitalMaster.HospitalLandlineNo1 = null;
                    hospitalMaster.HospitalAddress = null;
                    hospitalMaster.HospitalAddress1 = null;
                    hospitalMaster.HospitalAddress2 = null;
                    hospitalMaster.City = data.CityId.ToString();
                    hospitalMaster.Country = data.CountryId.ToString();
                    hospitalMaster.Pin = data.Pincode;
                    hospitalMaster.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    hospitalMaster.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    hospitalMaster.CreatedUser = "";
                    hospitalMaster.ModifiedUser = "";
                    hospitalMaster.Isactive = true;
                    hospitalMaster.Prefix = data.Hospitalname.Length <= 3 ? data.Hospitalname : data.Hospitalname.Substring(0, 3) + "-";
                    hospitalMaster.HospitalLogoUrl = GetLocation;
                    Hospitalid = _hospitalRepo.InsertHospital(hospitalMaster);

                    ClinicMaster clinicMaster = new ClinicMaster();
                    clinicMaster.HospitalID = Hospitalid;
                    clinicMaster.ClinicName = data.Hospitalname;
                    clinicMaster.ClinicMobileNo = data.MobileNo.ToString();
                    clinicMaster.ClinicLandlineNo = null;
                    clinicMaster.ClinicLandlineNo1 = null;
                    clinicMaster.ClinicAddress = null;
                    clinicMaster.ClinicAddress1 = null;
                    clinicMaster.ClinicAddress2 = null;
                    clinicMaster.City = data.CityId.ToString();
                    clinicMaster.Country = data.CountryId.ToString();
                    clinicMaster.Pin = data.Pincode;
                    clinicMaster.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    clinicMaster.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    clinicMaster.CreatedUser = "";
                    clinicMaster.ModifiedUser = "";
                    clinicMaster.Isactive = true;
                    clinicMaster.TeleConsultation = false;
                    Clinicid = _clinicRepo.InsertNewClinic_FromLicence(clinicMaster);

                    clinicMaster.ClinicName = data.Hospitalname + " " + "TeleConsultation";
                    clinicMaster.TeleConsultation = true;
                    Clinicid = _clinicRepo.InsertNewClinic_FromLicence(clinicMaster);

                    MobileUsers mobileusers = new MobileUsers();
                    mobileusers.FirstName = data.PrimaryUserName;
                    mobileusers.MobileNo = data.MobileNo;
                    mobileusers.Email = data.EmailID;
                    mobileusers.ZipCode = data.Pincode;
                    mobileusers.UserType = "Doctor"; //_patientRepo.GetUserTypeByID(data.UserTypeID).Trim()
                    mobileusers.CreateDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    mobileusers.IsActive = true;
                    mobileusers.City = _patientRepo.GetCityByID(data.CityId);
                    _patientRepo.CreateNewMobileUser(mobileusers);

                    LicenceView licenceMasterView = new LicenceView();
                    licenceMasterView.HospitalID = Hospitalid;
                    licenceMasterView.UserTypeID = 2;
                    licenceMasterView.PrimaryUserName = data.PrimaryUserName;
                    licenceMasterView.MobileNo = data.MobileNo;
                    licenceMasterView.EmailID = data.EmailID;
                    licenceMasterView.ClinicsCount = data.ClinicsCount;
                    licenceMasterView.UsersCount = data.UsersCount;
                    licenceMasterView.SearchType = data.SearchType;
                    licenceMasterView.OtherNotes = data.OtherNotes;
                    licenceMasterView.AgentMobileNo = data.AgentMobileNo;
                    licenceMasterView.IsSendEmail = true;
                    Liceneid = _licenceRepo.InsertLicence(licenceMasterView);

                    RoleMasterDetail roleMasterDetail = new RoleMasterDetail();
                    roleMasterDetail.Rolename = "Administrator";
                    roleMasterDetail.HospitalID = Hospitalid;
                    roleMasterDetail.ClinicID = 0;
                    roleMasterDetail.AllowedDiscount = 0;
                    roleMasterDetail.IsDiscountallowed = null;
                    roleMasterDetail.CreatedByRoleId = 0;
                    roleMasterDetail.IsAdminRole = null;
                    roleMasterDetail.AdminRoleId = 0;
                    roleMasterDetail.MyPatientData = true;
                    roleMasterDetail.AllPatientData = false;
                    roleMasterDetail.AllDoctorData = false;
                    roleMasterDetail.DocumentUpload = false;
                    roleMasterDetail.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    roleMasterDetail.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    roleMasterDetail.CreatedUser = "";
                    roleMasterDetail.ModifiedUser = "";
                    roleMasterDetail.IsActive = data.IsActive;
                    int roleid = _userRoleRepo.CreateNewRole(roleMasterDetail);

                    for (int i = 0; i < menuGroupDetails.Count; i++)
                    {
                        MenuGroupDetail menuGroupDetail = new MenuGroupDetail();
                        menuGroupDetail.G_Roleid = roleid;
                        menuGroupDetail.G_MainMenuid = menuGroupDetails[i].G_MainMenuid;
                        menuGroupDetail.G_SubMenuid = menuGroupDetails[i].G_SubMenuid;
                        menuGroupDetail.G_IsMainMenu = menuGroupDetails[i].G_IsMainMenu;
                        menuGroupDetail.G_Access = menuGroupDetails[i].G_Access;
                        menuGroupDetail.G_Add = menuGroupDetails[i].G_Add;
                        menuGroupDetail.G_Edit = null;
                        menuGroupDetail.G_Delete = menuGroupDetails[i].G_Delete;
                        menuGroupDetail.G_View = menuGroupDetails[i].G_View;
                        menuGroupDetail.G_Verify = menuGroupDetails[i].G_Verify;
                        menuGroupDetail.G_ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        menuGroupDetail.G_CreatedUser = "";
                        menuGroupDetail.G_ModifiedUser = "";
                        menuGroupDetail.G_IsActive = true;
                        isSuccess = _userRoleRepo.InsertMenuGroup(menuGroupDetail);
                    }

                    long TrialPeriod = _loginrepo.GetTrialPeriod();
                    Login login = new Login();
                    login.UserName = data.PrimaryUserName;
                    login.Userid = data.EmailID;
                    login.Password = rdnvalue;
                    login.RoleID = roleid;
                    login.DepartmentID = 0;
                    login.SpecialityID = 0;
                    login.UserType = data.UserTypeID;
                    login.HospitalID = Hospitalid;
                    login.ClinicID = 0;
                    login.ForgotPwdEmail = null;
                    login.ActivationKey = null;
                    login.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    login.CreatedUser = "";
                    login.ModifiedUser = "";
                    login.IsActive = true;
                    login.IsPrimaryUser = true;
                    login.LicenceID = Liceneid;
                    login.IsTrailUser = true;
                    login.TrailCreateDate = DateTime.Now;
                    login.TrailDays = TrialPeriod;
                    login.MobileNumber = data.MobileNo;
                    int loginid = _loginrepo.InsertNewLogin(login);
                    if (loginid > 0)
                    {
                        string Terms = _loginrepo.GetTerms();
                        string Privacy = _loginrepo.GetPrivacyPolicy();
                        TermsConditions termsConditions = new TermsConditions();
                        termsConditions.Userid = loginid;
                        termsConditions.TermsandConditions = Terms;
                        termsConditions.PrivacyPolicy = Privacy;
                        termsConditions.IsActive = true;
                        termsConditions.CreatedDttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        termsConditions.ModifiedDttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        isSuccess = _loginrepo.InsertNewTerms(termsConditions);
                    }
                    if (isSuccess == true)
                    {
                        List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                        if (licenceconfig.Count > 0)
                        {
                            string Email = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceToEmail.ToString());
                            string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                            string Password = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                            //string MsgSubject = "Trail Licence Activation for Knockdok";
                            string linksmp = "";
                            string EmailTitle = "TrailLicenceActivation";
                            List<EmailMaster> emailMaster = _licenceRepo.GetEmailByTitle(EmailTitle);
                            if (emailMaster.Count > 0)
                            {
                                string EmailSubject = emailMaster[0].EmailSubject;
                                string content = emailMaster[0].EmailBody;
                                content = content.Replace("&lt;Name&gt;",data.PrimaryUserName)
                                          .Replace("&lt;Temporary Password&gt;",rdnvalue);
                                linksmp = content;

                                //linksmp += "Welcome " + data.PrimaryUserName + "!";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "Enjoy the benefits and capabilities of our Knockdok. Please log in with ";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "User Name  <b>" + data.EmailID + "</b>";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "Temporary Password  <b>" + rdnvalue + "</b>. Please change the password once you log in. ";

                                //linksmp += "Hospital Name:" + data.Hospitalname + "";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "Primary User: " + data.PrimaryUserName + @"";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "Mobile No: " + data.MobileNo + @"";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "User Name: " + data.EmailID + @"";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "Password: " + rdnvalue + @"";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "No of Clinics: " + data.ClinicsCount + @"";
                                //linksmp += "<br/>";
                                //linksmp += "<br/>";
                                //linksmp += "No of Users: " + data.UsersCount + @"";
                                using (MailMessage mm = new MailMessage(sendmail, data.EmailID))
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
                }
                else
                {
                    TempData["Trail"] = "Email Already Exists";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return isSuccess;
        }
    }
}