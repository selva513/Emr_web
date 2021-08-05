using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Emr_web.Models;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using Microsoft.AspNetCore.Http;
using Syncfusion.EJ2.Navigations;
using Emr_web.Common;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Net.Mail;

namespace Emr_web.Controllers
{
    public class ForgotPasswordController : Controller
    {
        private IDBConnection _IDBConnection;
        private ILoginRepo _loginrepo;
        private IErrorlog _errorlog;
        private IInvestigationRepo _investigationRepo;
        private IHospitalRepo _hospitalRepo;
        private ILicenceRepo _licenceRepo;

        public ForgotPasswordController(IDBConnection iDBConnection, ILoginRepo loginrepo, IErrorlog errorlog, IInvestigationRepo investigationRepo, IHospitalRepo hospitalRepo, ILicenceRepo licenceRepo)
        {
            _IDBConnection = iDBConnection;
            _loginrepo = loginrepo;
            _errorlog = errorlog;
            _investigationRepo = investigationRepo;
            _hospitalRepo = hospitalRepo;
            _licenceRepo = licenceRepo;
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpGet]
        public JsonResult SentEmail(string Userid)
        {
            bool isresult = false;
            string rdnvalue = "";
            try
            {
                bool value_ok = false;
                int value = 0;
                Random rnd = new Random();
                value = rnd.Next(10000, 99999);
                string HospitalPrefix = _licenceRepo.GetHospitalPrefix();
                rdnvalue = HospitalPrefix + value.ToString();
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
                isresult = _loginrepo.UpdateActivationKey(Userid, rdnvalue);
                if (isresult == true)
                {
                    List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                    if (licenceconfig.Count > 0)
                    {
                        string Email = Userid;
                        string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                        string Password = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                        string MsgSubject = "Change Password for Jeenah EMR";
                        string linksmp = "";
                        linksmp += "User ID : " + Userid + @"";
                        linksmp += "<br/>";
                        linksmp += "<br/>";
                        linksmp += "Your Temporary Password : " + rdnvalue + @"";
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
                else
                    rdnvalue = "";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(rdnvalue);
        }
        [HttpGet]
        public JsonResult UpdatePassword(string Userid,string Password,string TempPassword)
        {
            bool isresult = false;
            try
            {
                isresult = _loginrepo.UpdateTempPassword(Userid, Password, TempPassword);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(isresult);
        }
    }
}