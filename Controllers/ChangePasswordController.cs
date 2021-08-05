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

namespace Emr_web.Controllers
{
    public class ChangePasswordController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private ILoginRepo _loginrepo;
        private readonly IEmrRepo _emrRepo;
        public ChangePasswordController(IDBConnection iDBConnection, IErrorlog errorlog, ILoginRepo loginRepo, IEmrRepo emrRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _loginrepo = loginRepo;
            _emrRepo = emrRepo;
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public JsonResult UpdatePassword([FromBody] LoginView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    string OldPwd = data.OldPassword;
                    string CurrentPwd = data.Password;
                    isSuccess = _loginrepo.UpdatePassword(OldPwd, CurrentPwd);
                    if (isSuccess)
                    {
                        string EventName = "Password Changed by" + data.Userid;
                        CreateEventManagemnt(EventName);
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
        //Create Event Method
        private void CreateEventManagemnt(string EventName)
        {
            EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
            eventManagemntInfo.EventName = EventName;
            eventManagemntInfo.UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
            eventManagemntInfo.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            _emrRepo.NewEventCreateion(eventManagemntInfo);
        }
    }
}