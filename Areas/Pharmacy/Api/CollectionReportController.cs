using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/CollectionReportApi")]
    [ApiController]
    public class CollectionReportController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ICollectionReportRepo _collectionReportRepo;

        public CollectionReportController(IDBConnection dBConnection, IErrorlog errorlog, ICollectionReportRepo collectionReportRepo)
        {

            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _collectionReportRepo = collectionReportRepo;

        }

        [HttpGet("GetCollectionReportDetails")]
        public string GetCollectionReportDetails(string FromDate, string fromTime, string ToDate, string toTime, string Type)
        {
            DataTable GetStockDetails = null;
            string Htmltext = "";
            try
            {
                DateTime frmdattm = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todatm = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string frmdat = frmdattm.ToString("yyyy-MM-dd");
                string todat = todatm.ToString("yyyy-MM-dd");
                string Frd = frmdattm.ToString("dd-MM-yyyy");
                string tod = todatm.ToString("dd-MM-yyyy");
                string CashType = "";
                //if (Type == "IP")
                //{
                //    GetStockDetails = _collectionReportRepo.GetCollectionReportbyIP(frmdat, todat);
                //    CashType = "IP";
                //}
                //else if (Type == "CASHOP")
                //{
                //    GetStockDetails = _collectionReportRepo.GetCollectionReportbyCashandOP(frmdat, todat);
                //    // GetReturnStockDetails = StockTrans.GetCollectionReturnReportbyCashandOP(frmdat, todat);
                //    CashType = "CASH/OP";
                //}
                //else if (Type == "All")
                //{
                //    GetStockDetails = _collectionReportRepo.GetCollectionReportbyOPandIP(frmdat, todat,Type);
                //}

                CashType = Type;
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                GetStockDetails = _collectionReportRepo.GetCollectionReportbyOPandIP(frmdat, todat, Type, HospitalId);


                StringBuilder strHTMLBuilder = new StringBuilder();
                StringBuilder strHTMLBuilder1 = new StringBuilder();
                string Headerfiles = "";
                strHTMLBuilder.Append("<html >");
                strHTMLBuilder.Append("<head>");
                strHTMLBuilder.Append(" <style> .section { display: table; width: 100%;  } .section > div {display: table-cell; overflow: hidden; } .section ~ .section > div:before { content: '';  display: block;  margin-bottom: -10.5em; / inverse of header height /; } .section > div > div { page-break-inside: avoid; display: inline-block; width: 100%; vertical-align: top;  } .headers { height: 10.5em; / header must have a fixed height /; }.content {/ this rule set just adds space between sections and is not required / margin-bottom: 1.25em; } </style>");
                //strHTMLBuilder.Append("<style media='print'> .bill{width: 63px !important;} .id{width: 83px !important;} .name{width: 177px !important;}  .amount{width: 77px !important;} .tax{width: 35px !important;} .total{width: 123px !important;}  .collect{width: 123px !important;} </style>   ");
                strHTMLBuilder.Append("</head>");
                strHTMLBuilder.Append("<body>");

                strHTMLBuilder1.Append(" <div class='headers'>");
                strHTMLBuilder1.Append("<h3 style='text-align: -webkit-center;'>ST.ROCK'S HOSPITAL PHARMACY(CLUNY)</h3>");
                strHTMLBuilder1.Append("<h4 style='text-align: -webkit-center;'>Colletion Report - " + CashType + "</h4>");
                strHTMLBuilder1.Append("<h5 style='text-align: -webkit-center;'>FromDate  : " + Frd + "  ToDate  : " + tod + " </h5>");

                strHTMLBuilder1.Append("<table  cellpadding='1' cellspacing='1' style='margin-top: 1px;border: 0;' >");
                strHTMLBuilder1.Append("<tr >");


                strHTMLBuilder1.Append("<td >");
                strHTMLBuilder1.Append("<div style='width: 37px !important;'>S.NO</div>");
                strHTMLBuilder1.Append("</td>");

                foreach (DataColumn myColumn in GetStockDetails.Columns)
                {
                    string HeaderName = "";
                    string S = "";

                    if (myColumn.ColumnName == "PH_CSH_BILLNO")
                    {
                        HeaderName = "B.NO";
                        S = "class='bill'  style='width: 63px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_BILLDT")
                    {
                        HeaderName = "BILL DATE";
                        S = "class='bill'  style='width: 90px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_PATID")
                    {
                        HeaderName = "PAT-ID";
                        S = "class='id'  style='width: 83px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_PATNAME")
                    {
                        HeaderName = "NAME";
                        S = "class='name'   style='width: 177px !important;'";

                    }
                    else if (myColumn.ColumnName == "PH_CSH_NETTAMOUNT")
                    {
                        HeaderName = "AMOUNT";
                        S = " class='amount' style='width: 80px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_TAXAMOUNT")
                    {
                        HeaderName = "TAX";
                        S = "class='tax'  style='width: 50px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_TOTAMOUNT")
                    {
                        HeaderName = "TOTAMOUNT";
                        S = "class='total'   style='width: 123px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_CASHRECIVEDAMT")
                    {
                        HeaderName = "COLLECTED";
                        S = "class='collect' style='width: 152px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_CONCESSION")
                    {
                        HeaderName = "DISCOUNT";
                        S = "class='collect' style='width: 152px !important;'";
                    }
                    else if (myColumn.ColumnName == "PH_CSH_PROCESSKEY")
                    {
                        HeaderName = "STORE";
                        S = "class='collect' style='width: 152px !important;'";
                    }
                    if (HeaderName != "")
                    {
                        strHTMLBuilder1.Append("<td >");
                        strHTMLBuilder1.Append("<div " + S + ">");
                        strHTMLBuilder1.Append(HeaderName);
                        strHTMLBuilder1.Append("</div></td>");
                    }
                }
                strHTMLBuilder1.Append("</tr>");
                strHTMLBuilder1.Append("</table>");
                strHTMLBuilder1.Append(" </div>");
                Headerfiles = strHTMLBuilder1.ToString();


                int count = 1;
                // int chk1 = 0;
                int chk = 0;
                int ss = 0;
                for (int x = 0; x < GetStockDetails.Rows.Count; x++)
                {


                    if (ss == 0)
                    {
                        if (chk != 0)
                        {

                            strHTMLBuilder.Append("</table>");
                            strHTMLBuilder.Append("</div>");
                            strHTMLBuilder.Append(" </div> </div> </div>");
                        }


                        strHTMLBuilder.Append(" <div class='section'> <div> <div>");
                        strHTMLBuilder.Append(Headerfiles);
                        strHTMLBuilder.Append(" <div class='content'>");
                        strHTMLBuilder.Append("<table>");

                        chk = 1;

                    }
                    else
                    {

                    }

                    strHTMLBuilder.Append("<tr >");
                    strHTMLBuilder.Append("<td style='width: 37px !important;'>" + count++ + "");
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 60px !important;'>");
                    string BillNo = GetStockDetails.Rows[x]["PH_CSH_BILLNO"].ToString();
                    strHTMLBuilder.Append(BillNo);
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 90px !important;'>");
                    DateTime BillDate = Convert.ToDateTime(GetStockDetails.Rows[x]["PH_CSH_BILLDT"].ToString());
                    string Datet = BillDate.ToString("dd-MM-yyyy");
                    strHTMLBuilder.Append(Datet);
                    strHTMLBuilder.Append("</td>");



                    strHTMLBuilder.Append("<td style='width: 85px !important;'>");
                    string PATID = GetStockDetails.Rows[x]["PH_CSH_PATID"].ToString();
                    strHTMLBuilder.Append(PATID);
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 180px !important;'>");
                    string PATNAME = GetStockDetails.Rows[x]["PH_CSH_PATNAME"].ToString();
                    strHTMLBuilder.Append(PATNAME);
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 76px !important;'>");
                    string TOTAMOUNT = GetStockDetails.Rows[x]["PH_CSH_TOTAMOUNT"].ToString();
                    // string PH_CSH_ROUNDOFF1 = GetStockDetails.Rows[x]["PH_CSH_ROUNDOFF"].ToString();
                    //  double Amt = Convert.ToDouble(TOTAMOUNT) + Convert.ToDouble(PH_CSH_ROUNDOFF1);
                    // strHTMLBuilder.Append(Amt);
                    strHTMLBuilder.Append(TOTAMOUNT);
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 80px !important;'>");
                    string TAXAMOUNT = GetStockDetails.Rows[x]["PH_CSH_TAXAMOUNT"].ToString();
                    strHTMLBuilder.Append(TAXAMOUNT);
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 145px !important;'>");
                    string PH_CSH_CONCESSION = GetStockDetails.Rows[x]["PH_CSH_CONCESSION"].ToString();
                    strHTMLBuilder.Append(PH_CSH_CONCESSION);
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 116px !important;'>");
                    string NETTAMOUNT = GetStockDetails.Rows[x]["PH_CSH_NETTAMOUNT"].ToString();
                    //string PH_CSH_ROUNDOFF = GetStockDetails.Rows[x]["PH_CSH_ROUNDOFF"].ToString();
                    // double Amt = Convert.ToDouble(TOTAMOUNT) + Convert.ToDouble(PH_CSH_ROUNDOFF);
                    // decimal ssss = Convert.ToDecimal(Amt);
                    strHTMLBuilder.Append(NETTAMOUNT);
                    strHTMLBuilder.Append("</td>");

                    strHTMLBuilder.Append("<td style='width: 145px !important;'>");
                    string CASHRECIVEDAMT = GetStockDetails.Rows[x]["PH_CSH_CASHRECIVEDAMT"].ToString();
                    strHTMLBuilder.Append(CASHRECIVEDAMT);
                    strHTMLBuilder.Append("</td>");


                    strHTMLBuilder.Append("<td >");
                    string PH_CSH_PROCESSKEY = GetStockDetails.Rows[x]["PH_CSH_PROCESSKEY"].ToString();
                    if (PH_CSH_PROCESSKEY == "PHBYSOP" || PH_CSH_PROCESSKEY == "PHBYORD")
                    {
                        PH_CSH_PROCESSKEY = "OP";
                    }
                    else if (PH_CSH_PROCESSKEY == "PHBYSIP" || PH_CSH_PROCESSKEY == "PHBYIRD")
                    {
                        PH_CSH_PROCESSKEY = "IP";
                    }
                    strHTMLBuilder.Append(PH_CSH_PROCESSKEY);
                    strHTMLBuilder.Append("</td>");


                    strHTMLBuilder.Append("</tr>");

                    string Isfreeavailable = "False";
                    if (x == GetStockDetails.Rows.Count - 1)
                    {
                        // decimal Total1 = GetStockDetails.AsEnumerable().Where(c => c.Field<bool>("PH_CSH_ISactive") == true).Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                        decimal Total = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                        // decimal Total1 = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_ROUNDOFF"));
                        //decimal Amts = Convert.ToDecimal(Total) + Convert.ToDecimal(Total1);
                        decimal Tax = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TAXAMOUNT"));
                        decimal subTotal = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CASHRECIVEDAMT"));
                        decimal NetTotal = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_NETTAMOUNT"));
                        decimal ConcessionAmt = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CONCESSION"));
                        strHTMLBuilder.Append("<table><hr><tr><td style='width: 160px !important;'>Total</td><td style='width: 60px !important;'></td><td style='width: 90px !important;'></td><td style='width: 136px !important;'></td><td  style='width: 78px !important;'>" + Total + "</td><td style='width: 85px !important;'>" + Tax + "</td><td style='width: 126px !important;'>" + ConcessionAmt + "</td><td style='width: 146px !important;'>" + NetTotal + "</td><td style='width: 116px !important;'>" + subTotal + "</td></tr></table>");


                        decimal FreeNetAmount = 0;
                        decimal FreeCollectedamt = 0;
                        decimal FreeTaxAmount = 0;
                        decimal FreeTotalAmount = 0;
                        decimal Reducefreeamt = Total - FreeNetAmount;
                        decimal ReducefreeTotal = subTotal + FreeCollectedamt;
                        //strHTMLBuilder.Append("<table><hr><tr><td style='width:120px;'></td><td  style='width:140px;'>Free Amount</td><td style='width:120px;'>Free Tax</td><td style='width:120px;'>Free NetAmount</td><td>Free CollectedAmount</td></tr>");
                        string FreeHeader = "<table><hr><tr><td style='width:120px;'></td><td  style='width:140px;'>Free Amount</td><td style='width:120px;'>Free Tax</td><td style='width:120px;'>Free NetAmount</td><td>Free CollectedAmount</td></tr>";
                        StringBuilder freestrbuilder = new StringBuilder();

                        DataTable GetfreeTable = _collectionReportRepo.getFreegroup(HospitalId);

                        if (Type == "IP")
                        {
                            string FreeType = "";
                            for (int q = 0; q < GetfreeTable.Rows.Count; q++)
                            {
                                FreeType = GetfreeTable.Rows[q]["GroupName"].ToString();
                                DataTable dtfreecollectionop = _collectionReportRepo.GetFreeCollectionReportbyCashandIP(frmdat, todat, FreeType,HospitalId);
                                FreeNetAmount = dtfreecollectionop.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_NETTAMOUNT"));
                                FreeTaxAmount = dtfreecollectionop.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TAXAMOUNT"));
                                FreeTotalAmount = dtfreecollectionop.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                                decimal FreeTotalRoundoff = dtfreecollectionop.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_ROUNDOFF"));

                                //  FreeTotalAmount = FreeTotalAmount + FreeTotalRoundoff;

                                FreeCollectedamt = dtfreecollectionop.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CASHRECIVEDAMT"));


                                if (FreeNetAmount != 0)
                                {
                                    Isfreeavailable = "True";
                                    freestrbuilder.Append("<tr><td style='width:120px;'>" + FreeType + "</td><td  style='width:140px;'>" + FreeTotalAmount + "</td><td style='width:120px;'>" + FreeTaxAmount + "</td><td style='width:120px;'>" + FreeNetAmount + "</td><td>" + FreeCollectedamt + "</td></tr>");
                                }
                            }
                            DataTable getwithoutfree = _collectionReportRepo.GetFreeCollectionReportbyCashandIPwithallfree(frmdat, todat, HospitalId);
                            FreeNetAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_NETTAMOUNT"));
                            FreeTaxAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TAXAMOUNT"));
                            FreeTotalAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                            // decimal FreeTotalRoundoff1 = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_ROUNDOFF"));
                            //  FreeTotalAmount = FreeTotalAmount + FreeTotalRoundoff1;
                            FreeCollectedamt = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CASHRECIVEDAMT"));

                            //  strHTMLBuilder.Append("<tr><td style='width:120px;'>Collected</td><td  style='width:140px;'>" + Amts + "</td><td style='width:120px;'>" + subTotal + "</td><td style='width:120px;'></td><td></td></tr></table>");
                            freestrbuilder.Append("<table><hr><tr><td style='width:120px;'>Total</td><td  style='width:140px;'>" + FreeTotalAmount + "</td><td style='width:120px;'>" + FreeTaxAmount + "</td><td style='width:120px;'>" + FreeNetAmount + "</td><td>" + FreeCollectedamt + "</td><tr></tr></table>");

                        }
                        else if (Type == "CASHOP")
                        {
                            string FreeType = "";
                            for (int q = 0; q < GetfreeTable.Rows.Count; q++)
                            {
                                FreeType = GetfreeTable.Rows[q]["GroupName"].ToString();
                                DataTable dtfreecollectionip = _collectionReportRepo.GetFreeCollectionReportbyCashandOP(frmdat, todat, FreeType, HospitalId);
                                FreeNetAmount = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_NETTAMOUNT"));
                                FreeTaxAmount = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TAXAMOUNT"));
                                FreeTotalAmount = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                                //  decimal FreeTotalRoundoff = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_ROUNDOFF"));
                                //FreeTotalAmount = FreeTotalAmount + FreeTotalRoundoff;
                                FreeCollectedamt = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CASHRECIVEDAMT"));

                                if (FreeNetAmount != 0)
                                {
                                    Isfreeavailable = "True";
                                    freestrbuilder.Append("<tr><td style='width:120px;'>" + FreeType + "</td><td  style='width:140px;'>" + FreeTotalAmount + "</td><td style='width:120px;'>" + FreeTaxAmount + "</td><td style='width:120px;'>" + FreeNetAmount + "</td><td>" + FreeCollectedamt + "</td></tr>");
                                }


                            }
                            DataTable getwithoutfree = _collectionReportRepo.GetFreeCollectionReportbyCashandOPwithallfree(frmdat, todat, HospitalId);
                            FreeNetAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_NETTAMOUNT"));
                            FreeTaxAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TAXAMOUNT"));
                            FreeTotalAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                            // decimal FreeTotalRoundoff1 = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_ROUNDOFF"));
                            // FreeTotalAmount = FreeTotalAmount + FreeTotalRoundoff1;
                            FreeCollectedamt = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CASHRECIVEDAMT"));
                            //  strHTMLBuilder.Append("<tr><td style='width:120px;'>Collected</td><td  style='width:140px;'>" + Amts + "</td><td style='width:120px;'>" + subTotal + "</td><td style='width:120px;'></td><td></td></tr></table>");
                            freestrbuilder.Append("<table><hr><tr><td style='width:120px;'>Total</td><td  style='width:140px;'>" + FreeTotalAmount + "</td><td style='width:120px;'>" + FreeTaxAmount + "</td><td style='width:120px;'>" + FreeNetAmount + "</td><td>" + FreeCollectedamt + "</td><tr></tr></table>");

                        }
                        else if (Type == "All")
                        {
                            string FreeType = "";
                            for (int q = 0; q < GetfreeTable.Rows.Count; q++)
                            {
                                FreeType = GetfreeTable.Rows[q]["GroupName"].ToString();
                                DataTable dtfreecollectionip = _collectionReportRepo.GetFreeCollectionReportbyIPandOP(frmdat, todat, FreeType, HospitalId);
                                FreeNetAmount = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_NETTAMOUNT"));
                                FreeTaxAmount = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TAXAMOUNT"));
                                FreeTotalAmount = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                                //  decimal FreeTotalRoundoff = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_ROUNDOFF"));
                                //FreeTotalAmount = FreeTotalAmount + FreeTotalRoundoff;
                                FreeCollectedamt = dtfreecollectionip.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CASHRECIVEDAMT"));

                                if (FreeNetAmount != 0)
                                {
                                    Isfreeavailable = "True";
                                    freestrbuilder.Append("<tr><td style='width:120px;'>" + FreeType + "</td><td  style='width:140px;'>" + FreeTotalAmount + "</td><td style='width:120px;'>" + FreeTaxAmount + "</td><td style='width:120px;'>" + FreeNetAmount + "</td><td>" + FreeCollectedamt + "</td></tr>");
                                }


                            }
                            DataTable getwithoutfree = _collectionReportRepo.GetFreeCollectionReportbyIPandOPwithallfree(frmdat, todat, HospitalId);
                            FreeNetAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_NETTAMOUNT"));
                            FreeTaxAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TAXAMOUNT"));
                            FreeTotalAmount = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_TOTAMOUNT"));
                            // decimal FreeTotalRoundoff1 = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_ROUNDOFF"));
                            // FreeTotalAmount = FreeTotalAmount + FreeTotalRoundoff1;
                            FreeCollectedamt = getwithoutfree.AsEnumerable().Sum(c => c.Field<decimal>("PH_CSH_CASHRECIVEDAMT"));
                            //  strHTMLBuilder.Append("<tr><td style='width:120px;'>Collected</td><td  style='width:140px;'>" + Amts + "</td><td style='width:120px;'>" + subTotal + "</td><td style='width:120px;'></td><td></td></tr></table>");
                            freestrbuilder.Append("<table><hr><tr><td style='width:120px;'>Total</td><td  style='width:140px;'>" + FreeTotalAmount + "</td><td style='width:120px;'>" + FreeTaxAmount + "</td><td style='width:120px;'>" + FreeNetAmount + "</td><td>" + FreeCollectedamt + "</td><tr></tr></table>");
                        }

                        if (Isfreeavailable == "True")
                        {
                            strHTMLBuilder.Append(FreeHeader);
                            string Freecontent = freestrbuilder.ToString();
                            strHTMLBuilder.Append(Freecontent);
                        }
                        else
                        {
                            decimal withconcessionNetAmt = subTotal - ConcessionAmt;
                            strHTMLBuilder.Append("<table><hr><tr><td style='width: 50px !important;'></td><td style='width: 20px !important;'></td><td style='width: 20px !important;'></td><td style='width: 50px !important;'></td><td style='width: 78px !important;'>Amount</td><td style='width: 85px !important;'>Tax</td><td style='width: 106px !important;'>Total Amount</td><td style='width: 116px !important;'>Discount</td><td style='width: 116px !important;'>Collected Amount</td></tr>");
                            strHTMLBuilder.Append("<table><hr><tr><td style='width: 50px !important;'>Total</td><td style='width: 20px !important;'></td><td style='width: 20px !important;'></td><td style='width: 50px !important;'></td><td  style='width: 78px !important;'>" + Total + "</td><td style='width: 85px !important;'>" + Tax + "</td><td style='width: 106px !important;'>" + NetTotal + "</td><td style='width: 116px !important;'>" + ConcessionAmt + "</td><td style='width: 116px !important;'>" + subTotal + "</td></tr></table><hr>");
                        }

                        if (Type == "IP")
                        {
                            GetStockDetails = _collectionReportRepo.GetCollectionReturnReportbyIP(frmdat, todat, HospitalId);
                            CashType = "IP";
                        }
                        else if (Type == "CASHOP")
                        {
                            // GetStockDetails = StockTrans.GetCollectionReportbyCashandOP(frmdat, todat);
                            GetStockDetails = _collectionReportRepo.GetCollectionReturnReportbyCashandOP(frmdat, todat, HospitalId);
                            CashType = "CASH/OP";
                        }
                        else if (Type == "All")
                        {
                            GetStockDetails = _collectionReportRepo.GetCollectionReturnReportbyIPandOP(frmdat, todat, HospitalId);
                            CashType = "All";
                        }
                        GetStockDetails = _collectionReportRepo.GetCollectionReturnReportbyIPandOP(frmdat, todat, HospitalId);

                        StringBuilder strHTMLBuilderreturn = new StringBuilder();
                        StringBuilder strHTMLBuilder1return = new StringBuilder();
                        string Headerfilesreturn = "";

                        strHTMLBuilder1return.Append(" <div class='headers'>");
                        strHTMLBuilder1return.Append("<h3 style='text-align: -webkit-center;'>ST.ROCK'S HOSPITAL PHARMACY(CLUNY)</h3>");
                        strHTMLBuilder1return.Append("<h4 style='text-align: -webkit-center;'>Colletion Return Report - " + CashType + "</h4>");
                        strHTMLBuilder1.Append("<h5 style='text-align: -webkit-center;'>FromDate  : " + Frd + "  ToDate  : " + tod + " </h5>");

                        strHTMLBuilder1return.Append("<table  cellpadding='1' cellspacing='1' style='margin-top: 1px;border: 0;' >");
                        strHTMLBuilder1return.Append("<tr >");


                        strHTMLBuilder1return.Append("<td >");
                        strHTMLBuilder1return.Append("<div style='width: 37px !important;'>S.NO</div>");
                        strHTMLBuilder1return.Append("</td>");

                        foreach (DataColumn myColumn in GetStockDetails.Columns)
                        {
                            string HeaderName = "";
                            string S = "";

                            if (myColumn.ColumnName == "PH_RET_BILLNO")
                            {
                                HeaderName = "B.NO";
                                S = "class='bill'  style='width: 63px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_BILLDT")
                            {
                                HeaderName = "BILL DATE";
                                S = "class='bill'  style='width: 90px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_PATID")
                            {
                                HeaderName = "PAT-ID";
                                S = "class='id'  style='width: 83px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_PATNAME")
                            {
                                HeaderName = "NAME";
                                S = "class='name'   style='width: 177px !important;'";

                            }
                            else if (myColumn.ColumnName == "PH_RET_NETTAMOUNT")
                            {
                                HeaderName = "AMOUNT";
                                S = " class='amount' style='width: 80px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_TAXAMOUNT")
                            {
                                HeaderName = "TAX";
                                S = "class='tax'  style='width: 50px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_TAXAMOUNT")
                            {
                                HeaderName = "TAX";
                                S = "class='tax'  style='width: 50px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_CONCESSION")
                            {
                                HeaderName = "DISCOUNT";
                                S = "class='total'   style='width: 123px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_CASHRECIVEDAMT")
                            {
                                HeaderName = "COLLECTED";
                                S = "class='collect' style='width: 152px !important;'";
                            }
                            else if (myColumn.ColumnName == "PH_RET_PROCESSKEY")
                            {
                                HeaderName = "STORE";
                                S = "class='collect' style='width: 152px !important;'";
                            }
                            if (HeaderName != "")
                            {
                                strHTMLBuilder1return.Append("<td >");
                                strHTMLBuilder1return.Append("<div " + S + ">");
                                strHTMLBuilder1return.Append(HeaderName);
                                strHTMLBuilder1return.Append("</div></td>");
                            }
                        }
                        strHTMLBuilder1return.Append("</tr>");
                        strHTMLBuilder1return.Append("</table>");
                        strHTMLBuilder1return.Append(" </div>");
                        Headerfilesreturn = strHTMLBuilder1return.ToString();


                        count = 1;
                        // int chk1 = 0;
                        //  int chk = 0;
                        ss = 0;
                        bool Isfirstchk = true;
                        for (int b = 0; b < GetStockDetails.Rows.Count; b++)
                        {


                            if (ss == 0)
                            {
                                if (chk != 0)
                                {

                                    strHTMLBuilder.Append("</table>");
                                    strHTMLBuilder.Append("</div>");
                                    strHTMLBuilder.Append(" </div> </div> </div>");
                                }


                                strHTMLBuilder.Append(" <div class='section'> <div> <div>");
                                strHTMLBuilder.Append(Headerfilesreturn);
                                strHTMLBuilder.Append(" <div class='content'>");
                                strHTMLBuilder.Append("<table>");

                                chk = 1;

                            }
                            else
                            {

                            }

                            if (Isfirstchk == true)
                            {
                                strHTMLBuilder.Append("<hr><div>Return Amout Details</div>");
                                strHTMLBuilder.Append("<tr ><td ><div style='width: 37px !important;'>S.NO</div></td><td ><div style='width: 63px !important;'>B.NO</div></td>");
                                strHTMLBuilder.Append("<td ><div style='width: 90px !important;'>BILL DATE</div></td><td ><div style='width: 83px !important;'>PAT-ID</div></td>");
                                strHTMLBuilder.Append("<td ><div style='width: 177px !important;'>NAME</div></td><td ><div style='width: 80px !important;'>AMOUNT</div></td>");
                                strHTMLBuilder.Append(@"<td ><div style='width: 50px !important;'>TAX</div></td><td ><div style='width: 123px !important;'>DISCOUNT</div></td><td ><div style='width: 123px !important;'>TOTAMOUNT</div></td>
                                <td ><div style='width: 145px !important;'>COLLECTED</div></td><td><div style='width: 100px !important;'>STORE</div></td>");
                                Isfirstchk = false;
                            }

                            strHTMLBuilder.Append("<tr >");
                            strHTMLBuilder.Append("<td style='width: 37px !important;'>" + count++ + "");
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 60px !important;'>");
                            //string BillNoreturn = GetStockDetails.Rows[b]["PH_RET_BILLNO"].ToString();
                            //strHTMLBuilder.Append(BillNoreturn);
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 90px !important;'>");
                            DateTime BillDatereturn = Convert.ToDateTime(GetStockDetails.Rows[b]["PH_RET_BILLDT"].ToString());
                            string Datetreturn = BillDatereturn.ToString("dd-MM-yyyy");
                            strHTMLBuilder.Append(Datetreturn);
                            strHTMLBuilder.Append("</td>");



                            strHTMLBuilder.Append("<td style='width: 85px !important;'>");
                            string PATIDreturn = GetStockDetails.Rows[b]["PH_RET_PATID"].ToString();
                            strHTMLBuilder.Append(PATIDreturn);
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 180px !important;'>");
                            string PATNAMEreturn = GetStockDetails.Rows[b]["PH_RET_PATNAME"].ToString();
                            strHTMLBuilder.Append(PATNAMEreturn);
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 76px !important;'>");
                            string TOTAMOUNTreturn = GetStockDetails.Rows[b]["PH_RET_TOTAMOUNT"].ToString();
                            // string PH_CSH_ROUNDOFFreturn = GetStockDetails.Rows[b]["PH_RET_ROUNDOFF"].ToString();
                            // double Amtreturn = Convert.ToDouble(TOTAMOUNTreturn) + Convert.ToDouble(PH_CSH_ROUNDOFFreturn);
                            // decimal ssss = Convert.ToDecimal(Amt);
                            strHTMLBuilder.Append(TOTAMOUNTreturn);
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 80px !important;'>");
                            string TAXAMOUNTreturn = GetStockDetails.Rows[b]["PH_RET_TAXAMOUNT"].ToString();
                            strHTMLBuilder.Append(TAXAMOUNTreturn);
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 90px !important;'>");
                            string PH_RET_CONCESSION = GetStockDetails.Rows[b]["PH_RET_CONCESSION"].ToString();
                            strHTMLBuilder.Append(PH_RET_CONCESSION);
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 116px !important;'>");
                            string NETTAMOUNTreturn = GetStockDetails.Rows[b]["PH_RET_NETTAMOUNT"].ToString();
                            strHTMLBuilder.Append(NETTAMOUNTreturn);
                            strHTMLBuilder.Append("</td>");

                            strHTMLBuilder.Append("<td style='width: 145px !important;'>");
                            string CASHRECIVEDAMTreturn = GetStockDetails.Rows[b]["PH_RET_CASHRECIVEDAMT"].ToString();
                            strHTMLBuilder.Append(CASHRECIVEDAMTreturn);
                            strHTMLBuilder.Append("</td>");


                            strHTMLBuilder.Append("<td >");
                            string PH_CSH_PROCESSKEYs = GetStockDetails.Rows[b]["PH_RET_PROCESSKEY"].ToString();
                            if (PH_CSH_PROCESSKEYs == "PHBYSOP" || PH_CSH_PROCESSKEYs == "PHBYORD")
                            {
                                PH_CSH_PROCESSKEYs = "OP";
                            }
                            else if (PH_CSH_PROCESSKEYs == "PHBYSIP" || PH_CSH_PROCESSKEYs == "PHBYIRD")
                            {
                                PH_CSH_PROCESSKEYs = "IP";
                            }
                            strHTMLBuilder.Append(PH_CSH_PROCESSKEYs);
                            strHTMLBuilder.Append("</td>");



                            strHTMLBuilder.Append("</tr>");

                            if (b == GetStockDetails.Rows.Count - 1)
                            {
                                decimal Totalreturn = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_RET_TOTAMOUNT"));
                                //  decimal Total1return = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_RET_ROUNDOFF"));
                                //  decimal Amtsreturn = Convert.ToDecimal(Totalreturn) + Convert.ToDecimal(Total1return);
                                decimal Taxreturn = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_RET_TAXAMOUNT"));
                                decimal subTotalreturn = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_RET_CASHRECIVEDAMT"));
                                decimal NetTotalreturn = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_RET_NETTAMOUNT"));
                                decimal ConcessionRetAmt = GetStockDetails.AsEnumerable().Sum(c => c.Field<decimal>("PH_RET_CONCESSION"));

                                strHTMLBuilder.Append("<table><hr><tr><td style='width: 160px !important;'>Total</td><td style='width: 60px !important;'></td><td style='width: 90px !important;'></td><td style='width: 147px !important;'></td><td  style='width: 78px !important;'>" + Totalreturn + "</td><td style='width: 85px !important;'>" + Taxreturn + "</td><td style='width: 95px !important;'>" + ConcessionRetAmt + "</td><td style='width: 120px !important;'>" + NetTotalreturn + "</td><td style='width: 116px !important;'>" + subTotalreturn + "</td></tr></table>");



                                // decimal Getamt = Reducefreeamt - Amtsreturn;
                                decimal Getamt = Total - Totalreturn;
                                decimal GetTax = Tax - Taxreturn;
                                // decimal GetsubTotal = ReducefreeTotal - subTotalreturn;
                                decimal GetsubTotal = subTotal - subTotalreturn;
                                decimal GetTotal = NetTotal - NetTotalreturn;

                                decimal NetAmtCollected = subTotal - ConcessionAmt;
                                decimal NetAmtReturn = subTotalreturn - ConcessionRetAmt;

                                decimal GrandNetTotal = NetAmtCollected - NetAmtReturn;
                                decimal GrandConcession = ConcessionAmt - ConcessionRetAmt;
                                strHTMLBuilder.Append("<table><hr><tr><td style='width:120px;'></td><td  style='width:140px;'>Amount</td><td style='width:120px;'>Tax</td><td style='width:120px;'>Total Amount</td><td width:120px;>Discount</td><td width:120px;>Collected</td></tr>");
                                strHTMLBuilder.Append("<tr><td style='width:120px;'>Collection Amount</td><td style='width:140px;'>" + Total + "</td><td style='width:120px;'>" + Tax + "</td><td style='width:120px;'>" + NetTotal + "</td><td style='width:120px;'>" + ConcessionAmt + "</td><td style='width:120px;'>" + Math.Round(subTotal) + "</td></tr>");
                                strHTMLBuilder.Append("<tr><td style='width:120px;'>Return Amount</td><td style='width:140px;'>" + Totalreturn + "</td><td style='width:120px;'>" + Taxreturn + "</td><td style='width:120px;'>" + NetTotalreturn + "</td><td style='width:120px;'>" + ConcessionRetAmt + "</td><td style='width:120px;'>" + Math.Round(subTotalreturn) + "</td></tr></table>");
                                strHTMLBuilder.Append("<table><hr><tr><td style='width:120px;'>Total Amount</td><td style='width:140px;'>" + Getamt + "</td><td style='width:120px;'>" + GetTax + "</td><td style='width:120px;'>" + GetTotal + "</td><td style='width:120px;'>" + Math.Round(GrandConcession) + "</td><td style='width:120px;'>" + Math.Round(GetsubTotal) + "</td></tr></table><hr>");


                            }


                            ss++;


                            if (ss == 20)
                            {
                                ss = 0;
                            }
                        }

                    }


                    ss++;


                    if (ss == 20)
                    {
                        ss = 0;
                    }
                }


                //Close tags.  

                strHTMLBuilder.Append("</body>");
                strHTMLBuilder.Append("</html>");

                Htmltext = strHTMLBuilder.ToString();


            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Htmltext;
        }

        //[HttpGet("GetCollectionReportDetails")]
        //public JsonResult GetCollectionReportDetails(string fromDate, string fromTime , string toDate , string toTime ,string Pharmacy)
        //{
        //    List<CollectionReport> lstresult = new  List<CollectionReport>();
        //    List<CollectionReport> Clienttresult = new  List<CollectionReport>();
        //    try
        //    {
        //       var Fromdate =  DateTime.Parse(fromDate);
        //       var Todate =  DateTime.Parse(toDate);
        //       string FromDate = Fromdate.ToString("dd-MM-yyyy");
        //       string ToDate = Todate.ToString("dd-MM-yyyy");
        //       string FromDateTime = string.Format("{0:yyyy-MM-dd}", FromDate) + " " + fromTime;
        //       string ToDateTime = string.Format("{0:yyyy-MM-dd}", toDate) + " " + toTime;
        //        lstresult = _collectionReportRepo.GetCollectionReportDetails(FromDateTime, ToDateTime, Pharmacy);
        //        Clienttresult = _collectionReportRepo.GetClientAddress();


        //    }
        //    catch(Exception ex)
        //    {
        //        _errorlog.WriteErrorLog(ex.ToString());
        //    }
        //    return Json(new { lstresult = lstresult, Clienttresult = Clienttresult });
        //    }
        [HttpGet("GetCollectionReportbyDate")]
        public JsonResult GetCollectionReportbyDate(string FromDate, string ToDate,string StorName)
        {
            DateTime frmdattm = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todatm = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string frmdat = frmdattm.ToString("yyyy-MM-dd 00:00:00");
            string todat = todatm.ToString("yyyy-MM-dd 23:59:59");

            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));

            List<CashBillHeaderInfo> lstCashBill = new List<CashBillHeaderInfo>();
            List<SalesInfo> lstReturn = new List<SalesInfo>();

            lstCashBill = _collectionReportRepo.GetCollectionReportbyDate(frmdat, todat, StorName,HospitalId);
            if (StorName.Equals("All"))
            {
                lstReturn = _collectionReportRepo.GetCollectionReturnReportbyDate(frmdat, todat, HospitalId);
            }
            else
            {
                lstReturn = _collectionReportRepo.GetCollectionReturnReportbyStoreName(frmdat, todat, HospitalId, StorName);
            }
            return Json(new { Header = lstCashBill, Return = lstReturn });
        }
    }

}
