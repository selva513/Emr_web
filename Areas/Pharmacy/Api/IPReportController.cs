using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using BizLayer.Utilities;
using System.Text.RegularExpressions;
using System.Data;
using BizLayer.Interface;
using System.Globalization;
using PharmacyBizLayer.Repo;
using System.Text;
using System.Diagnostics;
using BizLayer.Domain;
using Emr_web.Common;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/IPReport")]
    [Area("Pharmacy")]
    [ApiController]
    public class IPReportController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IIPStatmentRepo _iPStatmentRepo;
        private IPatientRepo _patientRepo;
        public IPReportController(IDBConnection iDBConnection, IErrorlog errorlog, IIPStatmentRepo iPStatmentRepo, IPatientRepo patientRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _iPStatmentRepo = iPStatmentRepo;
            _patientRepo = patientRepo;
        }
        [HttpGet("GetIPStamentHeaderByAdmintID")]
        public JsonResult GetIPStamentHeaderByAdmintID(string PatinetID, int AdmitID, bool IsBedCharges)
        {
            List<IPStamentHeder> lstResult = new List<IPStamentHeder>();
            List<AdvanceHeader> lstAdv = new List<AdvanceHeader>();
            List<BedHeaderInfo> lstBed = new List<BedHeaderInfo>();
            List<BedHeaderInfo> lstBedInfo = new List<BedHeaderInfo>();
            try
            {
                lstResult = _iPStatmentRepo.GetIPStamentHeaderByAdmintID(PatinetID, AdmitID);
                lstAdv = _iPStatmentRepo.GetAdvanceByAdmitID(AdmitID);
                if (IsBedCharges)
                {
                    lstBed = _iPStatmentRepo.GetBedDeatilsByPatAdmitID(AdmitID);
                    if (lstBed.Count > 0)
                    {
                        decimal TotalBedCharge = 0;
                        decimal TotalNurseCharge = 0;
                        decimal TotalAmount = 0;
                        for (int count = 0; count < lstBed.Count; count++)
                        {
                            int DayCount = lstBed[count].Daycount;
                            decimal BedCharge = DayCount * lstBed[count].HIS_M_Category_Rate;
                            decimal NurseCharge = DayCount * lstBed[count].HIS_M_NursCharge_Rate;
                            TotalBedCharge = TotalBedCharge + BedCharge;
                            TotalNurseCharge = TotalNurseCharge + NurseCharge;
                        }
                        TotalAmount = TotalBedCharge + TotalNurseCharge;
                        BedHeaderInfo bedHeaderInfo = new BedHeaderInfo()
                        {
                            ServiceName = "BED AND NURSING SERVICE",
                            TotalAmount = TotalAmount,
                            AdmitID = AdmitID
                        };
                        lstBedInfo.Add(bedHeaderInfo);
                    }
                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { IPOrder = lstResult, Advance = lstAdv, Bed = lstBedInfo });
        }
        [HttpGet("GetServiceWiseTestAmount")]
        public JsonResult GetServiceWiseTestAmount(string ServiceName, int AdmitID)
        {
            List<IPStamentTestInfo> lstResult = new List<IPStamentTestInfo>();
            try
            {
                lstResult = _iPStatmentRepo.GetServiceWiseTestAmount(ServiceName, AdmitID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { IPOrder = lstResult });
        }
        [HttpGet("GetBedTransDeatilsByAdmitID")]
        public JsonResult GetBedTransDeatilsByAdmitID(int AdmitID)
        {
            List<BedTansDeatils> lstResult = new List<BedTansDeatils>();
            try
            {
                lstResult = _iPStatmentRepo.GetBedTransDeatilsByAdmitID(AdmitID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { IPOrder = lstResult });
        }
        [HttpGet("GetIpStatmentPatient")]
        public JsonResult GetIpStatmentPatient()
        {
            List<IPStatmentPatient> lstResult = new List<IPStatmentPatient>();
            try
            {
                lstResult = _iPStatmentRepo.GetIpStatmentPatient();
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { IPPatient = lstResult });
        }

        #region Habib
        [HttpGet("GetIPStatmentReport")]
        public JsonResult GetIPStatmentReport(int AdmitID, string PatientID, bool IsBedCharges)
        {
            List<BedTansDeatils> lstBedDetails = new List<BedTansDeatils>();
            List<IPStamentHeder> lstServiceDetails = new List<IPStamentHeder>();
            List<IPStamentTestInfo> lstTestDetails = new List<IPStamentTestInfo>();
            List<AdvanceHeader> lstAdv = new List<AdvanceHeader>();
            List<IPStatmentPatient> lstPatient = new List<IPStatmentPatient>();
            StringBuilder sb = new StringBuilder();
            string ServiceName = "";
            DataSet Dslab = new DataSet();
            string LocationId = "1";
            string AdmitDateTime = "";
            string PATIENT_NAME = "";
            string Current_Date = "";
            decimal TotalBedAmount = 0;
            decimal TotalNursingAmount = 0;
            decimal TotalBedCharges = 0;
            decimal TotalServiceAmount = 0;
            decimal TotalAdvanceAmount = 0;
            decimal TotalDebitAmount = 0;
            decimal TotalCreditAmount = 0;
            decimal NetTotal = 0;
            try
            {
                Current_Date = DateTime.Now.ToString("dd/MM/yyyy");
                sb.Append(@"<html>");
                Dslab = _patientRepo.labheaderclass(LocationId);
                if (Dslab.Tables.Count > 0)
                {
                    if (Dslab.Tables[0].Rows.Count > 0)
                    {
                        sb.Append("<head>");
                        sb.Append("<table style='width:95%;'");
                        sb.Append("<tr>");

                        sb.Append("<td style = 'width:65%;border-color: white;'>");
                        sb.Append("<table ");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:left;font-size:20px;;border-color: white;'>" + Dslab.Tables[0].Rows[0]["LIS_LABNAME"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:left;font-size:12px;;border-color:white;'>" + Dslab.Tables[0].Rows[0]["LIS_LAB_ADDRESS"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:left;font-size:12px;;border-color: white;'>" + Dslab.Tables[0].Rows[0]["LIS_LAB_ADDRESS1"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");
                        sb.Append("</td>");

                        sb.Append("<td style = 'width:15%;border-color: white;' >");
                        sb.Append("<table ");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:right;font-size:12px;border-color: white;'>Phone:" + Dslab.Tables[0].Rows[0]["LIS_LABPHONE"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:right;font-size:12px;border-color: white;'>Email:" + Dslab.Tables[0].Rows[0]["LIS_LABEMAIL"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sb.Append("<tr>");
                        sb.Append("<td style='text-align:right;font-size:12px;border-color: white;'>Website:" + Dslab.Tables[0].Rows[0]["Lis_Website"].ToString() + "</td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");
                        sb.Append("</td>");

                        sb.Append("<td style = 'width:20%;border-color: white;'>");
                        sb.Append("<table ");
                        sb.Append("<tr>");
                        sb.Append("<td  style=';border-color: white;'><img src='/images/TOSH_LOGO.jpg' width='90%' style='margin-left: 25px;'></td>");
                        sb.Append("</tr>");
                        sb.Append("</table>");
                        sb.Append("</td>");


                        sb.Append("</tr>");
                        sb.Append("</table>");
                        sb.Append("<style>table, th, td {border: 1px solid lightgrey;border-collapse: collapse;}</style>");
                        sb.Append("</head>");
                    }
                }


                sb.Append(@"<body>");
                sb.Append(@"<hr/>");

                lstPatient = _iPStatmentRepo.GetIpPatientDetails(AdmitID, PatientID);

                if (lstPatient.Count > 0)
                {
                    PATIENT_NAME = lstPatient[0].PATIENT_NAME;
                    AdmitDateTime = lstPatient[0].HIS_ADMITDTTIME;
                    sb.Append(@"<div style='text-align:Center;font-size:20px;'><span>PROVISIONAL BILL</span style='font-size:25px;width: 100%;'></div>");
                    sb.Append(@"<hr/>");
                    sb.Append(@"<table  style='font-size: 14px; width: 100%;'>");
                    sb.Append("<tr><td><b>Patient Name:</b></td><td>" + PATIENT_NAME + "</td><td><b>Patient ID:</b></td><td>" + PatientID + "</td></tr>");
                    sb.Append("<tr><td><b>Admission No :</b></td><td>IP" + AdmitID + "</td><td><b>Admission Date :</b></td><td>" + AdmitDateTime + "</td></tr></table>");

                    //< td style = 'width: 30px !important;' >< b > SNO </ b ></ td >
                    sb.Append(@" <table style='font-family:sans-serif;font-size: 12px;font-weight: 500;width:94%;margin-left: 1%;'>");
                    sb.Append(@"<thead><tr>
                            <td style='width: 170px !important;'><b>DATE</b></td>
                            <td style='width: 350px !important;'><b>PARTICULARS</b></td>
                            <td style='width: 30px !important;text-align:right;'><b>QTY</b></td>
                            <td  style='width: 70px !important;text-align:right;'><b>AMOUNT</b></td>
                        </tr></thead>");
                    sb.Append("</br>");
                    if (IsBedCharges)
                    {
                        lstBedDetails = _iPStatmentRepo.GetBedTransDeatilsByAdmitID(AdmitID);
                        if (lstBedDetails.Count > 0)
                        {
                            sb.Append("<tr><td colspan='7'><b>BED AND NURSING SERVICE</b></td></tr>");
                            for (int bed = 0; bed < lstBedDetails.Count; bed++)
                            {
                                decimal bedamount = (lstBedDetails[bed].Daycount) * (lstBedDetails[bed].HIS_M_Category_Rate);
                                decimal nurseamount = (lstBedDetails[bed].Daycount) * (lstBedDetails[bed].HIS_M_NursCharge_Rate);
                                sb.Append("<tr>");
                                //sb.Append("td style='width: 83px !important;'>" + (bed + 1) + "</td>");
                                sb.Append("<td>" + lstBedDetails[bed].StartDate + " To " + lstBedDetails[bed].EndDate + "</td>");
                                sb.Append("<td> Bed Charges For Bed No " + lstBedDetails[bed].HIS_M_BEDID + "</td>");
                                sb.Append("<td style='text-align:right;' >" + lstBedDetails[bed].Daycount + "</td>");
                                sb.Append("<td style='text-align:right;' >" + bedamount + "</td>");
                                sb.Append("</tr>");

                                if (nurseamount != 0)
                                {
                                    sb.Append("<tr>");
                                    //sb.Append("<tr><td style='width: 83px !important;'>" + (bed + 1) + "</td>");
                                    sb.Append("<td>" + lstBedDetails[bed].StartDate + " To " + lstBedDetails[bed].EndDate + "</td>");
                                    sb.Append("<td> Nursing Charges For Bed No " + lstBedDetails[bed].HIS_M_BEDID + "</td>");
                                    sb.Append("<td style='text-align:right;' >" + lstBedDetails[bed].Daycount + "</td>");
                                    sb.Append("<td style='text-align:right;' >" + nurseamount + "</td>");
                                    sb.Append("</tr>");
                                }
                                TotalBedAmount = TotalBedAmount + bedamount;
                                TotalNursingAmount = TotalNursingAmount + nurseamount;
                            }

                            TotalBedCharges = TotalBedAmount + TotalNursingAmount;
                            sb.Append("<tr>");
                            sb.Append("<td></td>");
                            sb.Append("<td><b>Total</b></td>");
                            sb.Append("<td></td>");
                            sb.Append("<td style='text-align:right;' ><b>" + TotalBedCharges + "</b></td>");
                            sb.Append("</tr>");
                        }
                    }

                    lstServiceDetails = _iPStatmentRepo.GetIPStamentHeaderByAdmintID(PatientID, AdmitID);
                    if (lstServiceDetails.Count > 0)
                    {
                        for (int Serv = 0; Serv < lstServiceDetails.Count; Serv++)
                        {
                            ServiceName = lstServiceDetails[Serv].HIS_M_ServiceName;
                            lstTestDetails = _iPStatmentRepo.GetServiceWiseTestAmount(ServiceName, AdmitID);
                            if (lstTestDetails.Count > 0)
                            {
                                sb.Append("<tr><td colspan='7'><b>" + ServiceName + "</b></td></tr>");
                                for (int test = 0; test < lstTestDetails.Count; test++)
                                {
                                    string LIS_TestName = lstTestDetails[test].LIS_TestName;
                                    if (LIS_TestName == "Total")
                                    {
                                        sb.Append("<tr>");
                                        sb.Append("<td></td>");
                                        sb.Append("<td><b>Total</b></td>");
                                        sb.Append("<td style='text-align:right;' ><b>" + lstTestDetails[test].Qty + "</b></td>");
                                        sb.Append("<td style='text-align:right;' ><b>" + lstTestDetails[test].Amount + "</b></td>");
                                        sb.Append("</tr>");
                                    }
                                    else
                                    {
                                        sb.Append("<tr>");
                                        sb.Append("<td>" + AdmitDateTime + " To " + Current_Date + "</td>");
                                        sb.Append("<td>" + lstTestDetails[test].LIS_TestName + "</td>");
                                        sb.Append("<td style='text-align:right;' >" + lstTestDetails[test].Qty + "</td>");
                                        sb.Append("<td style='text-align:right;' >" + lstTestDetails[test].Amount + "</td>");
                                        sb.Append("</tr>");
                                    }

                                }
                            }

                            TotalServiceAmount = TotalServiceAmount + lstServiceDetails[Serv].Amount;
                        }
                    }

                    sb.Append("<table style='font-size: 16px; width: 96%;margin-top:20px;'>");
                    sb.Append("<tr><td colspan = '6' align='center'><b>From " + AdmitDateTime + " To " + Current_Date + "</b></td></tr>");
                    //sb.Append("<tr>");
                    //sb.Append("<td><b>Particulars</b></td><td style='text-align:right;'><b>Amount</b></td>");
                    //sb.Append("</tr>");

                    lstAdv = _iPStatmentRepo.GetAdvanceByAdmitID(AdmitID);
                    if (lstAdv.Count > 0)
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>Advance</td>");
                        sb.Append("<td style='text-align:right;'>" + lstAdv[0].Amount + "</td>");
                        sb.Append("</tr>");

                        TotalAdvanceAmount = lstAdv[0].Amount;
                    }

                    TotalCreditAmount = TotalAdvanceAmount;
                    TotalDebitAmount = TotalBedAmount + TotalNursingAmount + TotalServiceAmount;

                    sb.Append("<tr>");
                    sb.Append("<td>Total Service Cost</td>");
                    sb.Append("<td style='text-align:right;'>" + TotalDebitAmount + "</td>");
                    sb.Append("</tr>");

                    sb.Append("<tr>");
                    if (TotalCreditAmount > TotalDebitAmount)
                    {
                        sb.Append("<td>Refund Amount</td>");
                        NetTotal = TotalCreditAmount - TotalDebitAmount;
                    }
                    else
                    {
                        sb.Append("<td>Balance Amount</td>");
                        NetTotal = TotalDebitAmount - TotalCreditAmount;
                    }

                    sb.Append("<td style='text-align:right;'>" + NetTotal + "</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");

                    //sb.Append("<tr>");
                    //sb.Append("<td>" + AdmitDateTime + " To " + Current_Date + "</td>");
                    //sb.Append("<td style='text-align:right;'>" + TotalDebitAmount+ "</td>");
                    //sb.Append("<td style='text-align:right;'>" + TotalCreditAmount + "</td>");
                    //sb.Append("<td style='text-align:right;'>" + NetTotal + "</td>");
                    //sb.Append("</tr>");
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return Json(sb.ToString());
        }
        #endregion
    }
}