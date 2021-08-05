using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    public class UserChangePasswordController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private ILoginRepo _loginrepo;
        public UserChangePasswordController(IDBConnection iDBConnection, IErrorlog errorlog, ILoginRepo loginRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _loginrepo = loginRepo;
        }
        public IActionResult UserChangePassword()
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
                    long Userseqid = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                    string OldPwd = data.OldPassword;
                    string CurrentPwd = data.Password;
                    long Userid = _loginrepo.CheckUserExist(Userseqid, OldPwd);
                    if (Userid > 0)
                    {
                        isSuccess = _loginrepo.UpdateLoginPassword(OldPwd, CurrentPwd, Userseqid);
                        isSuccess = true;
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
    }
}