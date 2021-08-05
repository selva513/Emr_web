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

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountPostingApiController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IAccountPostingRepo _accountPostingRepo;
        private IConfiguration _configuration;

        public AccountPostingApiController(IDBConnection dBConnection, IErrorlog errorlog, 
            IAccountPostingRepo accountPostingRepo,IConfiguration configuration)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _accountPostingRepo = accountPostingRepo;
            _configuration = configuration;
        }
        [HttpGet("GetPostingData")]
        public string GetPostingData(string fromdate, string todate, string GetDataType)
        {
            DataSet ds = new DataSet();
            try
            {
                string FromDate = "";
                string ToDate = "";
                if (GetDataType == "All")
                {
                    DateTime startDate = DateTime.Now;
                    DateTime expiryDate = startDate.AddDays(-10);
                    FromDate = expiryDate.ToString("yyyy-MM-dd");
                    ToDate = startDate.ToString("yyyy-MM-dd");
                }
                else if (GetDataType == "Current")
                {
                    DateTime startDate = DateTime.Now;
                    FromDate = startDate.ToString("yyyy-MM-dd");
                    ToDate = startDate.ToString("yyyy-MM-dd");
                }
                else
                {
                    if (fromdate != "")
                    {
                        DateTime frmdattm = DateTime.ParseExact(fromdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        FromDate = frmdattm.ToString("yyyy-MM-dd");
                    }
                    if (todate != "")
                    {
                        DateTime todatm = DateTime.ParseExact(todate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ToDate = todatm.ToString("yyyy-MM-dd");
                    }
                }
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                ds = _accountPostingRepo.GetACcountPostingData(FromDate, ToDate, HospitalId);
                DataTable DataTableval = ds.Tables["Receipt"];


                DataTable dt = new DataTable("Receipt");
                dt.Columns.Add("RowNumber", typeof(string));
                dt.Columns.Add("TRANS_DATE", typeof(string));
                dt.Columns.Add("DATE", typeof(string));
                dt.Columns.Add("SHIFT", typeof(string));
                dt.Columns.Add("OPPHAMT", typeof(string));
                dt.Columns.Add("OPPHTAX", typeof(string));
                dt.Columns.Add("OPPHTOTAL", typeof(string));
                dt.Columns.Add("IPPHAMT", typeof(string));
                dt.Columns.Add("IPPHTAX", typeof(string));
                dt.Columns.Add("IPPHTOTAL", typeof(string));
                dt.Columns.Add("PHARMAMT", typeof(string));
                dt.Columns.Add("PHARMTAX", typeof(string));
                dt.Columns.Add("PHARMTOTAL", typeof(string));
                dt.Columns.Add("RoundPHARMTOTAL", typeof(string));
                dt.Columns.Add("FREETOT", typeof(string));
                dt.Columns.Add("FREETAX", typeof(string));
                dt.Columns.Add("UPDATE", typeof(string));

                if(ds.Tables[0].Rows.Count>0)
                {
                    for (int i = 0; i < DataTableval.Rows.Count; i++)
                    {
                        string RowNumber = DataTableval.Rows[i]["RowNumber"].ToString();
                        DateTime BILLDT = Convert.ToDateTime(DataTableval.Rows[i]["TRANS_DATE"].ToString());
                        string TRANS_DATE = BILLDT.ToString("yyyy-MM-dd");

                        string OPPHAMT = DataTableval.Rows[i]["OPPHAMT"].ToString();
                        string OPPHTAX = DataTableval.Rows[i]["OPPHTAX"].ToString();
                        string OPPHTOTAL = DataTableval.Rows[i]["OPPHTOTAL"].ToString();
                        string IPPHAMT = DataTableval.Rows[i]["IPPHAMT"].ToString();
                        string IPPHTAX = DataTableval.Rows[i]["IPPHTAX"].ToString();
                        string IPPHTOTAL = DataTableval.Rows[i]["IPPHTOTAL"].ToString();
                        string PHARMAMT = DataTableval.Rows[i]["PHARMAMT"].ToString();
                        string PHARMTAX = DataTableval.Rows[i]["PHARMTAX"].ToString();
                        string PHARMTOTAL = DataTableval.Rows[i]["PHARMTOTAL"].ToString();
                        string RoundPHARMTOTAL = DataTableval.Rows[i]["RoundPHARMTOTAL"].ToString();
                        string FREETOT = DataTableval.Rows[i]["FREETOT"].ToString();
                        string FREETAX = DataTableval.Rows[i]["FREETAX"].ToString();

                        DateTime Datetime = DateTime.Now;
                        string datee = Datetime.ToString("yyyy-MM-dd");
                        string Shift = "M";
                        string Status = "";

                        dt.Rows.Add(RowNumber, TRANS_DATE, datee, Shift, OPPHAMT, OPPHTAX, OPPHTOTAL, IPPHAMT, IPPHTAX, IPPHTOTAL, PHARMAMT, PHARMTAX, PHARMTOTAL, RoundPHARMTOTAL, FREETOT, FREETAX, Status);
                    }
                }
                
                ds.Tables.Add(dt);

                #region Creating DBF File in Core
                //Dbf dbf = new Dbf();

                //dbf.Read("F:\\Done\\PRECEIPT.DBF");
                //foreach (DbfField field in dbf.Fields)
                //{
                //    Console.WriteLine(field.Name);
                //}
                //foreach (DbfRecord record in dbf.Records)
                //{
                //    for (int i = 0; i < dbf.Fields.Count; i++)
                //    {
                //        Console.WriteLine(record[i]);
                //    }
                //}


                //DbfField field = new DbfField("TEST", DbfFieldType.Character, 12);
                //dbf.Fields.Add(field);
                //DbfRecord record = dbf.CreateRecord();
                //record.Data[0] = "HELLO";
                //dbf.Write("F:\\Done\\test.dbf", DbfVersion.VisualFoxPro);

                //billHeaders = _accountPostingRepo.GetPostingData(FromDate, Todate);

                #endregion
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return ds.GetXml();
        }
        [HttpPost("CreateSalesLedger")]
        public long CreateSalesLedger([FromBody] AccountPosting AccountPosting)
        {
            long retvalue = 0;
            try
            {
                string Date = "";

                string FileName = "";
                if (AccountPosting.TRANS_DATE != "")
                {
                    string[] Datesplit = AccountPosting.TRANS_DATE.Split('-');
                    Date = Datesplit[2].ToString() + Datesplit[1].ToString() + Datesplit[0].ToString();
                    FileName = "PH-" + Datesplit[0].ToString() + Datesplit[1].ToString() + Datesplit[2].ToString();
                }

                string XMLs = _accountPostingRepo.GetPharmacyTallyXml();

                decimal TaxableValue = Convert.ToDecimal(AccountPosting.PHARMAMT) + Convert.ToDecimal(AccountPosting.FREETOT);
                string ReplaceXMl = XMLs.Replace("TaxableSalesvalue", TaxableValue.ToString());

                decimal TaxCollectedValue = Convert.ToDecimal(AccountPosting.PHARMTAX) + Convert.ToDecimal(AccountPosting.FREETAX);
                ReplaceXMl = ReplaceXMl.Replace("TaxCollectedValue", TaxCollectedValue.ToString());

                ReplaceXMl = ReplaceXMl.Replace("DateValue", Date.ToString());

                decimal RountoffValue = Convert.ToDecimal(AccountPosting.PHARMTOTAL) - (Convert.ToDecimal(AccountPosting.PHARMAMT) + Convert.ToDecimal(AccountPosting.PHARMTAX));

                ReplaceXMl = ReplaceXMl.Replace("RoundofACValue", RountoffValue.ToString());


                ReplaceXMl = ReplaceXMl.Replace("FreeMedicinesValue", "-" + AccountPosting.FREETOT);


                decimal CashValue = Convert.ToDecimal(AccountPosting.PHARMTOTAL) + Convert.ToDecimal(AccountPosting.FREETAX);
                ReplaceXMl = ReplaceXMl.Replace("CashValue", "-" + CashValue.ToString());

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(ReplaceXMl);
                
                string Dir = _configuration.GetConnectionString("PharmacyAccountDir");
                Dir = Dir + FileName + ".xml";
                xd.Save(Dir);
                retvalue = 1;
            }
            catch (Exception ex)
            {
                retvalue = 0;
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retvalue;
        }
    }
}
