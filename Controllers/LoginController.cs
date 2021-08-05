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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Emr_web.Controllers
{
    public class LoginController : Controller
    {
        private IDBConnection _IDBConnection;
        private ILoginRepo _loginrepo;
        private IErrorlog _errorlog;
        private IInvestigationRepo _investigationRepo;
        private IHospitalRepo _hospitalRepo;
        private ILicenceRepo _licenceRepo;
        private readonly IEmrRepo _emrRepo;
        public SelectListItem[] TimezoneList { get; set; }
        public LoginController(IDBConnection iDBConnection, ILoginRepo loginrepo, IErrorlog errorlog, IInvestigationRepo investigationRepo, IHospitalRepo hospitalRepo, ILicenceRepo licenceRepo, IEmrRepo emrRepo)
        {
            _IDBConnection = iDBConnection;
            _loginrepo = loginrepo;
            _errorlog = errorlog;
            _investigationRepo = investigationRepo;
            _hospitalRepo = hospitalRepo;
            _licenceRepo = licenceRepo;
            _emrRepo = emrRepo;
        }
        public IActionResult Login()
        {
            try
            {
                bool IsConnectedHIS = _investigationRepo.GetConnectedHIS();
                if (IsConnectedHIS == true)
                    HttpContext.Session.SetString("IsConnectedHIS", "1");
                else
                    HttpContext.Session.SetString("IsConnectedHIS", "0");
                HttpContext.Session.SetString("JWToken", "");
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        private string GenerateJSONWebToken(Claim[] claims)
        {
            var token = new JwtSecurityToken();
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisisalliedonenineSecretKey"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                token = new JwtSecurityToken("AlliedJeenah", "AlliedJeenah",
                  claims: claims,
                  expires: DateTime.Now.AddMinutes(120),

                  signingCredentials: credentials);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost]
        public IActionResult Loginuser([Bind] LoginView loginView)
        {
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);
                if (ModelState.IsValid)
                {
                    //string HospitalUniqueno = loginView.HospitalUniqueno;
                    //long Hospitalid = _hospitalRepo.GetHospIDByUniqueno(HospitalUniqueno);
                    long Licenceid = _licenceRepo.GetLicenceidByUser(loginView.Userid);
                    bool logincheck = _loginrepo.GetLogin(loginView.Userid, loginView.Password);
                    if (logincheck == true)
                    {
                        List<Login> logins = new List<Login>();
                        logins = _loginrepo.GetUserDetails(loginView.Userid,CurrentDatetime);
                        HttpContext.Session.SetObjectAsJsonLsit("LoginDetails", logins.ToArray());
                        bool isTrailUser = logins[0].IsTrailUser;
                        if (isTrailUser)
                        {
                            long TrailDays = logins[0].TrailDays;
                            if (TrailDays > 0)
                            {
                                var claims = new Claim[]
                                {
                                    new Claim("username", "alliedonenine"),
                                    new Claim("role", "admin")
                                };
                                string token = GenerateJSONWebToken(claims);
                                HttpContext.Session.SetString("JWToken", token);
                                if (Licenceid == 0)
                                {
                                    string EventName = "User Login-" + loginView.Userid;
                                    CreateEventManagemnt(EventName, logins[0].UserSeqid, logins[0].HospitalID);
                                    return RedirectToAction("Index", "Home");
                                }
                                else
                                {
                                    string EventName = "User Login-" + loginView.Userid;
                                    CreateEventManagemnt(EventName, logins[0].UserSeqid, logins[0].HospitalID);
                                    return RedirectToAction("ChangePassword", "ChangePassword");
                                }
                            }
                            else
                            {
                                TempData["UserLoginFailed"] = "Trail Licence Expired";
                                return View("Login");
                            }
                        }
                        else
                        {
                            var claims = new Claim[]
                               {
                                        new Claim("username", "alliedonenine"),
                                        new Claim("role", "admin")
                               };
                            string token = GenerateJSONWebToken(claims);
                            HttpContext.Session.SetString("JWToken", token);
                            if (Licenceid == 0)
                            {
                                string EventName = "User Login-" + loginView.Userid;
                                CreateEventManagemnt(EventName, logins[0].UserSeqid, logins[0].HospitalID);
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                string EventName = "User Login-" + loginView.Userid;
                                CreateEventManagemnt(EventName, logins[0].UserSeqid, logins[0].HospitalID);
                                return RedirectToAction("ChangePassword", "ChangePassword");
                            }
                        }
                    }
                    else
                    {
                        TempData["UserLoginFailed"] = "Login Failed.Please Enter Correct Credentials";
                        return View("Login");
                    }
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        public IActionResult LoginSubmit()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register()
        {
            return RedirectToAction("Register", "Register");
        }
        private void CreateEventManagemnt(string EventName, long UserId, long HospitalID)
        {
            EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
            eventManagemntInfo.EventName = EventName;
            eventManagemntInfo.UserID = UserId;
            eventManagemntInfo.HospitalID = HospitalID;
            _emrRepo.NewEventCreateion(eventManagemntInfo);
        }
    }
}