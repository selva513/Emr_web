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

namespace Emr_web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/UserRoleApi")]
    public class UserRoleApiController : Controller
    {
        private IDBConnection _IDBConnection;
        private IUserRoleRepo _userRoleRepo;
        private ILoginRepo _loginRepo;
        private IErrorlog _errorlog;
        public UserRoleApiController(IDBConnection iDBConnection, IUserRoleRepo userRoleRepo, ILoginRepo loginRepo, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _userRoleRepo = userRoleRepo;
            _loginRepo = loginRepo;
            _errorlog = errorlog;
        }

        [HttpGet("GetHospitals")]
        public HospitalMaster[] GetHospitals()
        {
            List<HospitalMaster> lsthos = new List<HospitalMaster>();
            try
            {
                long Hospitalid= Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lsthos = _userRoleRepo.GetDefaultHospital(Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lsthos.ToArray();
        }
        [HttpGet("GetRoles")]
        public RoleMaster[] GetRoles()
        {
            List<RoleMaster> lstrole = new List<RoleMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstrole = _userRoleRepo.GetDefaultRoles(Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstrole.ToArray();
        }
    }
}