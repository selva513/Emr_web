using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Dapper;
using Emr_web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Emr_web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Holiday")]
    public class HolidayController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private IHolidayRepo _holidayrepo;
        private IServiceRepo _servicerepo;

        public HolidayController(IDBConnection _dBConnection, IHolidayRepo holidayRepo, IErrorlog errorlog, IServiceRepo servicerepo)
        {
            _IDBConnection = _dBConnection;
            _holidayrepo = holidayRepo;
            _errorlog = errorlog;
            _servicerepo = servicerepo;

        }

        [HttpGet("BindFestival")]
        public JsonResult BindFestival()
        {
            List<HolidayMaster> lstholidaymaster = new List<HolidayMaster>();
            try
            {
                lstholidaymaster = _holidayrepo.BindHolidayDetails();
            }
            catch (Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return Json( new { lstholidaymaster } );
        }

        [HttpGet("DeleteFestival")]
        public bool DeleteFestival(int HolidayId,bool IsActive)
        {
            bool issucess = false;
            try
            {
                issucess = _holidayrepo.DeleteHoliday(HolidayId,IsActive);
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }
            return issucess;
            
        }

        [HttpGet("CheckHolidayExist")]
        public bool CheckHolidayExist(string HolidayName)
        {
            bool issucess = false;
            try
            {
                issucess = _holidayrepo.Check_HolidayExist(HolidayName);
            }
            catch(Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return issucess;
        }

        [HttpGet("GetTestByService")]
        public string GetTestByService(int ServiceId, int PageIndex, int PageSize)
        {
            DataSet ds = new DataSet();
            try
            {
                int HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                HttpContext.Session.SetString("Serviceid", ServiceId.ToString());
                ds = _servicerepo.BindTestByService(HospitalId, ServiceId, PageIndex, PageSize);
            }
            catch (Exception exp)
            {
                _errorlog.WriteErrorLog(exp.ToString());
            }

            return ds.GetXml();
        }
    }
}