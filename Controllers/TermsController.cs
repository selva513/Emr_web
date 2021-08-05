using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Emr_web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.PivotView;

namespace Emr_web.Controllers
{
    public class TermsController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private IPatientRepo _patientRepo;
        private ILicenceRepo _licenceRepo;
        public TermsController(IDBConnection iDBConnection, IErrorlog errorlog, IPatientRepo patientRepo, ILicenceRepo licenceRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _patientRepo = patientRepo;
            _licenceRepo = licenceRepo;
        }
        public IActionResult Terms()
        {
            return View();
        }
        public IActionResult PrivacyPolicy()
        {
            return View();
        }
        public IActionResult Contactus()
        {
            return View();
        }
        [HttpPost]
        public IActionResult InsertSuggestion([Bind]ContactusView model)
        {
            int result = 0;
            try
            {
                string Userid = HttpContext.Session.GetString("Userid");
                string Username = HttpContext.Session.GetString("UserName");
                string Mobileno= HttpContext.Session.GetString("MobileNumber");
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (Mobileno == null || Mobileno == "")
                    Mobileno = "0";
                ContactUs contactUs = new ContactUs();
                contactUs.CustomerName = Username;
                contactUs.CustomerEmail = Userid;
                contactUs.CustomerMobileNo = Convert.ToInt64(Mobileno);
                contactUs.Description = model.Description;
                contactUs.IsActive = true;
                contactUs.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);

                result = _patientRepo.CreateNewSuggestion(contactUs);
                if (result > 0)
                {
                    List<Config> licenceconfig = _licenceRepo.GetConfigDetails();
                    if (licenceconfig.Count > 0)
                    {
                        string MsgSubject = "";
                        string Email = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceToEmail.ToString());
                        string sendmail = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromEmail.ToString());
                        string Password = Encrypt_Decrypt.Decrypt(licenceconfig[0].LicenceFromPwd.ToString());

                        MsgSubject = "New Request/Compliant From Client";

                        string linksmp = "";
                        linksmp += "Customer Name : " + contactUs.CustomerName + "";
                        linksmp += "<br/>";
                        linksmp += "<br/>";
                        linksmp += "Customer Email : "+ contactUs.CustomerEmail + " ";
                        linksmp += "<br/>";
                        linksmp += "<br/>";
                        linksmp += "Customer Mobile : " + contactUs.CustomerMobileNo + " ";
                        linksmp += "<br/>";
                        linksmp += "<br/>";
                        linksmp += "Request/Complaint : "+ contactUs.Description + "</b>";
                        linksmp += "<br/>";
                        

                        using (MailMessage mm = new MailMessage(sendmail, Email))
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
                    TempData["UserFailed"] = "Request Send Successfully.";
                }
                else
                {
                    TempData["UserFailed"] = "Request Send Failed.Please Contact Allied Business Solutions.";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return RedirectToAction("Contactus", "Terms");
        }
    }
}