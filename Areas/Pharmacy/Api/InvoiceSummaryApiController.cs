using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/InvoiceSummary")]
    [ApiController]
    public class InvoiceSummaryApiController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IInvoiceSummaryRepo _invoiceSummaryRepo;

        public InvoiceSummaryApiController(IDBConnection dBConnection, IErrorlog errorlog, IInvoiceSummaryRepo invoiceSummaryRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _invoiceSummaryRepo = invoiceSummaryRepo;
        }
        [HttpGet("getAllInvoicebySuppplierID")]
        public JsonResult getAllInvoicebySuppplierID(string StartDate, string EndDate, int SupplierId, string InvoiceType)
        {
            List<InvoiceSummaryHeader> lstResult = new List<InvoiceSummaryHeader>();
            List<InvoiceSummaryDtl> lstResultDtl = new List<InvoiceSummaryDtl>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _invoiceSummaryRepo.NewInvoiceDetailedReportbySupplierID(StartDate, EndDate, SupplierId, InvoiceType, HospitalId);
                lstResultDtl = _invoiceSummaryRepo.NewSummaryInvoicebySupplierID(StartDate, EndDate, SupplierId, InvoiceType, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstResult, Deatils = lstResultDtl });
        }

        [HttpGet("getAllInvoice")]
        public JsonResult getAllInvoice(string fromyear, string toyear, int frommonth, string tomonth)
        {
            List<InvoiceSummaryHeader> lstResult = new List<InvoiceSummaryHeader>();
            List<InvoiceSummaryDtl> lstResultDtl = new List<InvoiceSummaryDtl>();
            try
            {
                int fromyr = Convert.ToInt32(fromyear);
                int toyr= Convert.ToInt32(toyear);
                int frommth= Convert.ToInt32(frommonth);
                int tomth = Convert.ToInt32(tomonth);
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                var fromstartDate = new DateTime(fromyr, frommth, 1);
                var fromendDate = fromstartDate.AddMonths(1).AddDays(-1);
                var tostartDate = new DateTime(toyr, tomth, 1);
                var toendDate = tostartDate.AddMonths(1).AddDays(-1);
                //lstResult = _invoiceSummaryRepo.NewInvoiceDetailedReportbySupplierID(StartDate, EndDate, SupplierId, InvoiceType, HospitalId);
                //lstResultDtl = _invoiceSummaryRepo.NewSummaryInvoicebySupplierID(StartDate, EndDate, SupplierId, InvoiceType, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstResult, Deatils = lstResultDtl });
        }

        [HttpGet("GetInvoiceSummaryByDate")]
        public JsonResult GetInvoiceSummaryByDate(string StartDate, string EndDate, int SupplierId, string InvoiceType)
        {
            List<InvoiceSummaryInfo> lstResult = new List<InvoiceSummaryInfo>();
            List<InvoiceSumDtl> lstSumary = new List<InvoiceSumDtl>();
            List<RetInvoiceHeaderSummary> lstReturnHeader = new List<RetInvoiceHeaderSummary>();
            List<ReturnInvoiceSummaryDeatils> lstReturnDeatils = new List<ReturnInvoiceSummaryDeatils>();
            try
            {
                DateTime ExDate = DateTime.Now;
                DateTime StartingDate = GetDataformat(StartDate, ExDate);
                StartDate = StartingDate.ToString("yyyy-MM-dd");
                DateTime EnDate = GetDataformat(EndDate, ExDate);
                EndDate = EnDate.ToString("yyyy-MM-dd");
                string RetStartDate= StartingDate.ToString("yyyy-MM-dd 00:00:00");
                string RetEndDate= EnDate.ToString("yyyy-MM-dd 23:59:59");
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if (string.IsNullOrEmpty(InvoiceType))
                    InvoiceType = "";
                if(!InvoiceType.Equals("Select") && SupplierId > 0)
                {
                    lstResult = _invoiceSummaryRepo.GetInvoiceSummaryBySupplier(StartDate, EndDate, HospitalId, InvoiceType, SupplierId);
                    lstSumary = _invoiceSummaryRepo.GetInvoiceTotalSummaryBySupplier(StartDate, EndDate, HospitalId, InvoiceType,SupplierId);
                    lstReturnHeader = _invoiceSummaryRepo.GetRetInvoiceSummaryByInvoiceBySupplier(RetStartDate, RetEndDate, HospitalId, InvoiceType, SupplierId);
                    lstReturnDeatils = _invoiceSummaryRepo.GetRetInvoiceTotalSummaryBySupplier(RetStartDate, RetEndDate, HospitalId, InvoiceType, SupplierId);
                }
                else if (!InvoiceType.Equals("Select") && InvoiceType!="")
                {
                    lstResult = _invoiceSummaryRepo.GetInvoiceSummaryByInvoiceType(StartDate, EndDate, HospitalId, InvoiceType);
                    lstSumary = _invoiceSummaryRepo.GetInvoiceTotalSummaryByInvoiceType(StartDate, EndDate, HospitalId,InvoiceType);
                    lstReturnHeader = _invoiceSummaryRepo.GetRetInvoiceSummaryByInvoiceType(RetStartDate, RetEndDate, HospitalId, InvoiceType);
                    lstReturnDeatils = _invoiceSummaryRepo.GetRetInvoiceTotalSummaryByInvoiceType(RetStartDate, RetEndDate, HospitalId, InvoiceType);
                }
                else
                {
                    lstResult = _invoiceSummaryRepo.GetInvoiceSummaryByDate(StartDate, EndDate, HospitalId);
                    lstSumary = _invoiceSummaryRepo.GetInvoiceTotalSummaryByDate(StartDate, EndDate, HospitalId);
                    lstReturnHeader = _invoiceSummaryRepo.GetRetInvoiceSummaryByDate(RetStartDate, RetEndDate, HospitalId);
                    lstReturnDeatils = _invoiceSummaryRepo.GetRetInvoiceTotalSummaryByDate(RetStartDate, RetEndDate, HospitalId);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstResult, Summary = lstSumary, RetHeader = lstReturnHeader, RetSummary = lstReturnDeatils });
        }
        public static DateTime GetDataformat(string DateValue, DateTime date)
        {
            string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            try
            {
                if (DateValue != null || DateValue != "")
                {
                    IFormatProvider cultureDDMMYYYY = new CultureInfo("fr-Fr", true);
                    IFormatProvider cultureMMDDYYYY = new CultureInfo("en-US", true);
                    DateTime currentDate = DateTime.Now;
                    IFormatProvider culture = cultureDDMMYYYY;
                    DateTime.TryParse(DateValue, culture, DateTimeStyles.NoCurrentDateDefault, out date);
                }
            }
            catch (Exception ex)
            {


            }
            return date;
        }
    }
}
