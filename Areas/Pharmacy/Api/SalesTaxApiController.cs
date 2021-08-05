using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Data;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Emr_web.Common;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesTaxApiController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ISalesTaxRepo _salesTaxRepo;
        private IConfiguration _configuration;

        public SalesTaxApiController(IDBConnection dBConnection, IErrorlog errorlog, ISalesTaxRepo salesTaxRepo, IConfiguration configuration)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _salesTaxRepo= salesTaxRepo;
            _configuration = configuration;
        }
        [HttpGet("GetSalesTax")]
        public string GetSalesTax(string StartDate, string EndDate)
        {
            DataSet dsResult = new DataSet();
            try
            {
                DateTime dtFrom = DateTime.Now;
                StartDate = CommonSetting.getDataformat(StartDate, dtFrom).ToString("yyyy-MM-dd 00:00:00");
                DateTime dtTodate = DateTime.Now;
                EndDate = CommonSetting.getDataformat(EndDate, dtTodate).ToString("yyyy-MM-dd 23:59:59");
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                DataTable dtResult = _salesTaxRepo.getDatewiseCollectedAmountSalesTax(StartDate, EndDate, HospitalId);
                if (dtResult.Rows.Count > 0)
                {
                    dtResult.Columns[0].ReadOnly = false;
                    dtResult.Columns[1].ReadOnly = false;
                    dtResult.Columns[2].ReadOnly = false;

                    for (int count = 0; count < dtResult.Rows.Count; count++)
                    {
                        string FromDate = CommonSetting.getDataformat(dtResult.Rows[count]["Date"].ToString(), dtFrom).ToString("yyyy-MM-dd 00:00:00");
                        string ToDate = CommonSetting.getDataformat(dtResult.Rows[count]["Date"].ToString(), dtFrom).ToString("yyyy-MM-dd 23:59:59");
                        DataTable dtReturn = _salesTaxRepo.getReturnAmountSalesTax(FromDate, ToDate, HospitalId);
                        if (dtReturn.Rows.Count > 0)
                        {
                            decimal Amount = Convert.ToDecimal(dtResult.Rows[count]["Amount"]) - Convert.ToDecimal(dtReturn.Rows[dtReturn.Rows.Count - 1]["Amt"]);
                            decimal Tax = Convert.ToDecimal(dtResult.Rows[count]["Tax"]) - Convert.ToDecimal(dtReturn.Rows[dtReturn.Rows.Count - 1]["Tax"]);
                            DataRow[] foundedRow = dtResult.Select("Date LIKE '%" + dtResult.Rows[count]["Date"].ToString() + "%'");
                            if (foundedRow.Length > 0)
                            {
                                foundedRow[0]["Amount"] = Amount;
                                foundedRow[0]["Tax"] = Tax;
                            }
                        }
                    }
                }
                DataTable dtCollect = _salesTaxRepo.getGroupofCollectAmount(StartDate, EndDate, HospitalId);
                if (dtCollect.Rows.Count > 0)
                {
                    dtCollect.Columns[0].ReadOnly = false;
                    dtCollect.Columns[1].ReadOnly = false;
                    dtCollect.Columns[2].ReadOnly = false;
                    dtCollect.Columns[3].ReadOnly = false;
                    dtCollect.Columns[4].ReadOnly = false;

                   DataTable dtReturnCollect = _salesTaxRepo.getGroupofReturnCollectAmount(StartDate, EndDate, HospitalId);
                    if (dtReturnCollect.Rows.Count > 0)
                    {
                        for (int count = 0; count < dtCollect.Rows.Count; count++)
                        {
                            decimal TaxPercetage = Convert.ToDecimal(dtCollect.Rows[count]["PH_CSHDTL_DRUG_TAXPERCENT"]);
                            string Cat = dtCollect.Rows[count]["Cat"].ToString();
                            DataRow[] foundrow = dtReturnCollect.Select("PH_RETDTL_DRUG_TAXPERCENT=" + TaxPercetage + " AND Cat='" + Cat + "'");
                            if (foundrow.Length > 0)
                            {
                                decimal Amount = Convert.ToDecimal(dtCollect.Rows[count]["Amount"]) - Convert.ToDecimal(foundrow[0]["Amount"]);
                                decimal Tax = Convert.ToDecimal(dtCollect.Rows[count]["Tax"]) - Convert.ToDecimal(foundrow[0]["Tax"]);
                                DataRow[] rowFound = dtCollect.Select("PH_CSHDTL_DRUG_TAXPERCENT=" + TaxPercetage + " And Cat='" + Cat + "'");
                                if (rowFound.Length > 0)
                                {
                                    rowFound[0]["Amount"] = Amount;
                                    rowFound[0]["Tax"] = Tax;
                                }
                            }
                        }
                    }
                }
                decimal TotalAmount = dtResult.AsEnumerable().Sum(x => x.Field<decimal>("Amount"));
                decimal TotalTax = dtResult.AsEnumerable().Sum(x => x.Field<decimal>("Tax"));
                decimal SGST = TotalTax / 2;
                decimal CGST = TotalTax / 2;
                dtResult.Rows.Add("SGST", 0, SGST);
                dtResult.Rows.Add("CGST", 0, CGST);
                dtResult.Rows.Add("Total", TotalAmount, TotalTax);

                decimal GroupTotalAmount = dtCollect.AsEnumerable().Sum(x => x.Field<decimal>("Amount"));
                decimal GroupTotalTax = dtCollect.AsEnumerable().Sum(x => x.Field<decimal>("Tax"));
                dtCollect.Rows.Add("Tot", "Total", 0, GroupTotalAmount, GroupTotalTax);

                dsResult.Tables.Add(dtResult);
                dsResult.Tables[0].TableName = "Sales";
                dsResult.Tables.Add(dtCollect);
                dsResult.Tables[1].TableName = "Group";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return dsResult.GetXml();
        }
        [HttpGet("GetSalesTaxByDate")]
        public JsonResult GetSalesTaxByDate(string StartDate, string EndDate)
        {
            List<SalesTaxInfo> lstResult = new List<SalesTaxInfo>();
            List<SalesTaxInfo> lstReturn = new List<SalesTaxInfo>();
            try
            {
                DateTime ExDate = DateTime.Now;
                DateTime StartingDate = GetDataformat(StartDate, ExDate);
                StartDate = StartingDate.ToString("yyyy-MM-dd 00:00:00");
                DateTime EnDate = GetDataformat(EndDate, ExDate);
                EndDate = EnDate.ToString("yyyy-MM-dd 23:59:59");
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _salesTaxRepo.GetSalesTaxByDate(StartDate, EndDate, HospitalId);
                lstReturn = _salesTaxRepo.GetReturnSalesTaxByDate(StartDate, EndDate, HospitalId);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstResult, Return = lstReturn });
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
