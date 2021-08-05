using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
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
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class LicenceActivationController : Controller
    {
        private IDBConnection _IDBConnection;
        private ILicenceRepo _licenceRepo;
        private IErrorlog _errorlog;
        public LicenceActivationController(IDBConnection iDBConnection, ILicenceRepo licenceRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _licenceRepo = licenceRepo;
            _errorlog = errorlog;
        }
        public async Task<IActionResult> LicenceActivation()
        {
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
                ViewBag.mainMenuItems = myComplexObject;
                var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
                ViewBag.AccountMenuItems = myComplexObjectaccount;
                var lst = await _licenceRepo.GetAllLicence();
                if (lst != null)
                {
                    lst = lst as List<LicenceView>;
                    ViewBag.LicenceDetail = lst;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpPost]
        public JsonResult UpdateLicence([FromBody] LicenceView data)
        {
            bool Result = true;
            try
            {
                List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                if (licenceconfig.Count > 0)
                {
                    long licenceseqid = Convert.ToInt64(data.LicenceSeqid);
                    string Email = data.EmailID;
                    string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                    string Password = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());
                    string MsgSubject = licenceconfig[0].ActivationEmailBody.ToString();
                    string linksmp = "";
                    linksmp += "Welcome " + data.PrimaryUserName + "!";
                    linksmp += "<br/>";
                    linksmp += "<br/>";
                    linksmp += "Enjoy the benefits and capabilities of our Knockdok. Please log in with ";
                    linksmp += "<br/>";
                    linksmp += "<br/>";
                    linksmp += "User Name  <b>"+ data.EmailID + "</b>";
                    linksmp += "<br/>";
                    linksmp += "<br/>";
                    linksmp += "Temporary Password  <b>" + data.Hospital_Uniqueno + "</b>. Please change the password once you log in. ";
                    //linksmp += "Hospital Name : " + data.Hospitalname + "";
                    //linksmp += "<br/>";
                    //linksmp += "<br/>";
                    //linksmp += "Mobile No : " + data.MobileNo + @"";
                    //linksmp += "<br/>";
                    //linksmp += "<br/>";
                    //linksmp += "<b>Your Username : " + data.EmailID + @"</b>";
                    //linksmp += "<br/>";
                    //linksmp += "<br/>";
                    //linksmp += "<b>Your Password : " + data.Hospital_Uniqueno + @"</b>";
                    //linksmp += "<br/>";
                    //linksmp += "<br/>";
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
                    if (data.IsActive == true)
                        Result = _licenceRepo.UpdateLicence(licenceseqid);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
                Result = false;
            }
            return Json(Result);
        }
    }
}