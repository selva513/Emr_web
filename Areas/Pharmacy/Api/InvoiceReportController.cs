using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/InvoiceReport")]
    [ApiController]
    public class InvoiceReportController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IInvoiceReportRepo _invoiceReportRepo;

        public InvoiceReportController(IDBConnection dBConnection, IErrorlog errorlog, IInvoiceReportRepo invoiceReportRepo)
        {

            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _invoiceReportRepo = invoiceReportRepo;

        }
        [HttpGet("GetInvoiceReportsDeatislByDate")]
        public List<InvoiceReport> GetInvoiceReportsDeatislByDate(string FromDate, string ToDate, string invoiceVatType, string invoiceType)
        {
            List<InvoiceReport> lstresult = new List<InvoiceReport>();
            try
            {
                DateTime FrmDate = DateTime.Now;
                DateTime RunningFromDate = GetDataformat(FromDate, FrmDate);
                FromDate = RunningFromDate.ToString("yyyy-MM-dd");
                DateTime toDate = DateTime.Now;
                DateTime Todate = GetDataformat(ToDate, toDate);
                ToDate = Todate.ToString("yyyy-MM-dd");
                //string toDate = DateTime.Now.ToString("yyyy-MM-dd");
                if (invoiceVatType == "Select")
                {
                    invoiceVatType = "";
                }
                if (invoiceType == "Select")
                {
                    invoiceType = "";
                }
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstresult = _invoiceReportRepo.GetInvoiceReportsDeatislByDate(FromDate, ToDate, invoiceVatType, invoiceType, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstresult;

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


        [HttpGet("Getdata")]
        public List<InvoiceReport> Getdata()
        {
            List<InvoiceReport> lstresult = new List<InvoiceReport>();
            try
            {
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstresult;

        }

    }
}
