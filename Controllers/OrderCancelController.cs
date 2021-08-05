using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Emr_web.Common;
using Emr_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Controllers
{
    public class OrderCancelController : Controller
    {

        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
        private IConfiguration _configuration;
        private readonly IOrderRepo _orderRepo;

        public OrderCancelController(IDBConnection iDBConnection, IErrorlog errorlog, IConfiguration configuration, IOrderRepo orderRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _configuration = configuration;
            _orderRepo = orderRepo;
        }
        public IActionResult OrderCancel()
        {
            return View();
        }
        public IActionResult PatientOrderCancel(long OrderId, long PatSeqID)
        {
            try
            {
                HttpContext.Session.SetString("CancelOrderId", OrderId.ToString());
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
       

        [HttpGet]
        public JsonResult GetOrderDetails()
        {
            List<OrderDetails> orderDetails = new List<OrderDetails>();
            List<OrderReprint> orderReprint = new List<OrderReprint>();
            long OrderId = 0;
            long HospitalId = 0;
            try
            {
                OrderId = Convert.ToInt64(HttpContext.Session.GetString("CancelOrderId"));
                HospitalId = Convert.ToInt16(HttpContext.Session.GetString("Hospitalid"));
                orderDetails = _orderRepo.GetOrderDetails(OrderId);
                orderReprint = _orderRepo.GetOrderslist(OrderId, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return Json(new { OrdHeader = orderReprint, OrdDet = orderDetails });
        }
    }
}