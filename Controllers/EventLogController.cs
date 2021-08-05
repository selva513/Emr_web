using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Emr_web.Controllers
{
    public class EventLogController : Controller
    {
        private IDBConnection _IDBConnection;
        private readonly IEmrRepo _emrRepo;
        private readonly IErrorlog _errorlog;
        public EventLogController(IDBConnection iDBConnection,IEmrRepo emrRepo,IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _emrRepo = emrRepo;
            _errorlog = errorlog;
        }
        public IActionResult EventLog()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetEventLogByUserID()
        {
            long UserID= Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
            long HospitalID= Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<EventManagemntInfo> lstEvent = new List<EventManagemntInfo>();
            try
            {
                lstEvent = _emrRepo.GetEventByUserId(UserID, HospitalID);
                if(lstEvent.Count > 0)
                {
                    for(int i = 0; i < lstEvent.Count(); i++)
                    {
                        string UserName = Encrypt_Decrypt.Decrypt(lstEvent[i].UserName);
                        lstEvent[i].UserName = UserName;
                    }
                }
            }
            catch (Exception)
            {

            }
            return Json(lstEvent);
        }
        #region Habib
        [HttpGet]
        public JsonResult GetEventBySearch(string Search, DateTime FromDate, DateTime ToDate)
        {
            long UserID= Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
            long HospitalID= Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<EventManagemntInfo> lstEvent = new List<EventManagemntInfo>();
            try
            {
                if (Search == null)
                    Search = "";
                string fromdate = FromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string todate = ToDate.ToString("yyyy-MM-dd HH:mm:ss");
                lstEvent = _emrRepo.GetEventBySearch(UserID, HospitalID,Search,fromdate,todate);
                if (lstEvent.Count > 0)
                {
                    for (int i = 0; i < lstEvent.Count(); i++)
                    {
                        string UserName = Encrypt_Decrypt.Decrypt(lstEvent[i].UserName);
                        lstEvent[i].UserName = UserName;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstEvent);
        }
        #endregion
    }
}