using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using System.Text.RegularExpressions;
using System.Data;
using Emr_web.Common;


namespace Emr_web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/OrderApi")]
    public class OrderApiController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IOrderRepo _orderRepo;
        private IPatientRepo _patientRepo;
        public OrderApiController(IDBConnection dBConnection, IErrorlog errorlog, IOrderRepo orderRepo, IPatientRepo patientRepo)
        {
            _IDBConnection = dBConnection;
            _errorlog = errorlog;
            _orderRepo = orderRepo;
            _patientRepo = patientRepo;
        }
        [HttpGet("GetOrderStatus")]
        public OrderStatus[] GetOrderStatus()
        {
            List<OrderStatus> lstorderstatus = new List<OrderStatus>();
            try
            {
                lstorderstatus = _orderRepo.GetOrderStatus();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstorderstatus.ToArray();
        }
        [HttpGet("GetOrderTestBySearch")]
        public List<TestBinding> GetOrderTestBySearch()
        {
            List<TestBinding> lstResult = new List<TestBinding>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _orderRepo.GetOrderTestBySearch(EnteredName, HospitalId);
            return lstResult;
        }

        [HttpGet("GetPackageBySearch")]
        public List<PackageHeader> GetPackageBySearch()
        {
            List<PackageHeader> lstResult = new List<PackageHeader>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _orderRepo.GetPackageBySearch(EnteredName, HospitalId);
            return lstResult;
        }
        public string GetEnteredData()
        {
            string Data = "";
            try
            {
                var query = Request.Query;
                string filter = query["$filter"];
                Match matchString = Regex.Match(filter, @"'(.*)',tolower");
                string[] seperators = { "(", ")", ",", "'", "'" };
                string[] split = matchString.Value.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                string QueryString = split[0];
                Data = QueryString;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Data;
        }
        [HttpGet("CancelService")]
        public string CancelService(long TestID, long OrderID, string Reason, decimal TestAmount)
        {
            string result = "";
            try
            {
                OrderDetails OrdDtl = new OrderDetails()
                {
                    OrderId = OrderID,
                    TestID = TestID,
                    CancelReason = Reason,
                    TestAmount = TestAmount
                };
                _orderRepo.UpdateTestCancelDetails(OrdDtl);

                Refund refund = new Refund()
                {

                };

                _orderRepo.CreateNewRefund(refund);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return result;
        }

        [HttpGet("GetClinicByHospitalID")]
        public List<ClinicMaster> GetClinicByHospitalID()
        {
            List<ClinicMaster> lstClinic = new List<ClinicMaster>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstClinic = _patientRepo.GetClinicByHospitalID(HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstClinic;
        }
        [HttpGet("CreateOrUpdateClinicPin")]
        public bool CreateOrUpdateClinicPin(long ClinicId)
        {
            long result = 0;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";

                OrderScreenPin orderScreenPin = new OrderScreenPin()
                {
                    ClinicId = ClinicId,
                    UserId = Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                    CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid)
                };
                result = _orderRepo.CreateOrUpdateClinicPin(orderScreenPin);
                if (result > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return false;
        }
        [HttpGet("GetClinicPin")]
        public JsonResult GetPinnedClinic()
        {
            long UserId = 0;
            List<OrderScreenPin> lstPinClinic = new List<OrderScreenPin>();
            try
            {
                UserId = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                lstPinClinic = _orderRepo.GetPinnedClinic(UserId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstPinClinic);
        }
    }
}