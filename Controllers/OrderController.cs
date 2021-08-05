using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Emr_web.Models;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using Microsoft.AspNetCore.Http;
using Syncfusion.EJ2.Navigations;
using Emr_web.Common;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrderController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IOrderRepo _orderRepo;
        private readonly IInvestRepo _investrepo;
        private readonly IHospitalRepo _hospitalRepo;
        private readonly IClinicRepo _clinicRepo;
        List<OrderPatient> _lstOrd = null;

        public OrderController(IDBConnection dBConnection, IErrorlog errorlog, IOrderRepo orderRepo, IInvestRepo investRepo, IHospitalRepo hospitalRepo, IClinicRepo clinicRepo)
        {
            _IDBConnection = dBConnection;
            _errorlog = errorlog;
            _orderRepo = orderRepo;
            _investrepo = investRepo;
            _clinicRepo = clinicRepo;
            _hospitalRepo = hospitalRepo;
        }
        public IActionResult Order()
        {
            return View();
        }

        public IActionResult Orders()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;
            return View();
        }
        [HttpGet]
        public JsonResult GetTodayPatients(long ClinicId, string Search, int RowCount, long StatusID, string PatType)
        {
            List<OrderPatient> orderPatient = new List<OrderPatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            //string Search = "";
            //int RowCount = 0;
            //long StatusID = 0;
            //string PatientType = "";
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if (Search == null)
                {
                    Search = "";
                }
                if (PatType == "OP")
                {
                    orderPatient = _orderRepo.GetTodayPatients(StartDatetime, EndDatetime, HospitalID, ClinicId, StatusID, Search);
                }
                else if (PatType == "IP")
                {
                    orderPatient = _orderRepo.GetIPPatients(HospitalID, ClinicId, StatusID, Search);
                }
                else if (PatType == "All")
                {
                    orderPatient = _orderRepo.GetTodayPatients(StartDatetime, EndDatetime, HospitalID, ClinicId, StatusID, Search);
                    orderPatient.AddRange(_orderRepo.GetIPPatients(HospitalID, ClinicId, StatusID, Search));
                }

                _lstOrd = orderPatient as List<OrderPatient>;
                _lstOrd = _lstOrd.OrderByDescending(i => i.VisitID).Take(RowCount).ToList();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayPatientsByStatus(long StatusID, long ClinicId)
        {
            List<OrderPatient> orderPatient = new List<OrderPatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayPatientsByStatus(StartDatetime, EndDatetime, HospitalID, StatusID, ClinicId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayPatientsByRowCount(long RowCount, long ClinicId)
        {
            List<OrderPatient> orderPatient = new List<OrderPatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayPatientsByRowCount(StartDatetime, EndDatetime, HospitalID, RowCount, ClinicId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayPatientsBySearch(string Search, long ClinicId)
        {
            List<OrderPatient> orderPatient = new List<OrderPatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            long StatusID = 4;
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if (Search == null || Search == "")
                    orderPatient = _orderRepo.GetTodayPatients(StartDatetime, EndDatetime, HospitalID, ClinicId,StatusID,Search);
                else
                    orderPatient = _orderRepo.GetTodayPatientsBySearch(StartDatetime, EndDatetime, HospitalID, Search, ClinicId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayOrderCancelPatients()
        {
            List<OrderCancel> orderPatient = new List<OrderCancel>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayOrderCancelPatients(StartDatetime, EndDatetime, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayOrderCancelPatientsSearch(string Fromdate, string Todate, string Search, string Days, string PatientType, string RowCount)
        {
            List<OrderCancel> orderPatient = new List<OrderCancel>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (Fromdate != null && Todate != null)
                {
                    string[] FromDatesplit = Fromdate.Split("/");
                    string[] ToDatesplit = Todate.Split("/");
                    Fromdate = FromDatesplit[2] + "-" + FromDatesplit[1] + "-" + FromDatesplit[0] + " " + "00:00:00";
                    Todate = ToDatesplit[2] + "-" + ToDatesplit[1] + "-" + ToDatesplit[0] + " " + "23:59:59";
                }
                if (Fromdate == null && Todate == null)
                {
                    if (Days == "Today")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "2 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonetwodays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "7 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonesevendays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                }
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayOrderCancelPatientsSearch(Fromdate, Todate, Search, Days, PatientType, RowCount, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayOrderReprintPatients()
        {
            List<OrderCancel> orderPatient = new List<OrderCancel>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayOrderReprintPatients(StartDatetime, EndDatetime, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayOrderReprintPatientsSearch(string Fromdate, string Todate, string Search, string Days, string PatientType, string RowCount)
        {
            List<OrderCancel> orderPatient = new List<OrderCancel>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (Fromdate != null && Todate != null)
                {
                    string[] FromDatesplit = Fromdate.Split("/");
                    string[] ToDatesplit = Todate.Split("/");
                    Fromdate = FromDatesplit[2] + "-" + FromDatesplit[1] + "-" + FromDatesplit[0] + " " + "00:00:00";
                    Todate = ToDatesplit[2] + "-" + ToDatesplit[1] + "-" + ToDatesplit[0] + " " + "23:59:59";
                }
                if (Fromdate == null && Todate == null)
                {
                    if (Days == "Today")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "2 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonetwodays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "7 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonesevendays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                }

                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayOrderReprintPatientsSearch(Fromdate, Todate, Search, Days, PatientType, RowCount, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetOrderReprint(long OrderID)
        {
            string PrintResult = "";
            StringBuilder sb = new StringBuilder();
            List<MyPatient> lstresult = new List<MyPatient>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<OrderReprint> lstorder = new List<OrderReprint>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstHospital = _hospitalRepo.GetHospitalDetails(HospitalID);
                lstresult = _orderRepo.GetOrderPatientlist(OrderID, HospitalID);
                lstorder = _orderRepo.GetOrderslist(OrderID, HospitalID);
                var imageString = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(lstHospital[0].HospitalLogo));
                sb.Append("<table style='width:95%;'");
                sb.Append("<tr>");
                sb.Append("<td style = 'width:65%'>");
                sb.Append("<table ");
                sb.Append("<tr>");
                sb.Append("<td style='text-align:left;font-size:20px;'>" + lstHospital[0].HospitalName + "</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='text-align:left;font-size:12px;'>" + lstHospital[0].HospitalAddress + "</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='text-align:left;font-size:12px;'>" + lstHospital[0].HospitalAddress1 + "</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("<td style = 'width:15%;' >");
                sb.Append("<table ");
                sb.Append("<tr>");
                sb.Append("<td style='text-align:right;font-size:12px;'>Phone:" + lstHospital[0].HospitalMobileNo + "</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='text-align:right;font-size:12px;'>Email:</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='text-align:right;font-size:12px;'>Website:</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("<td style = 'width:20%'>");
                sb.Append("<table ");
                sb.Append("<tr>");
                sb.Append("<td  style='border-left-color: white;border-right-color: white;border-top-color: white;'><img src='" + imageString + "' width='90%' style='margin-left: 25px;'></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("  <tr>");
                sb.Append(" <td colspan='3' style='font-weight: bold;border-top: solid 1px black;'></td>");
                sb.Append("  </tr>");
                sb.Append("</table>");

                #region table1

                sb.Append("   <table style='float:left;font-size:10px;width:50%;' class='OrederReprint'>");
                sb.Append(" <tr><td>UHID No</td>");
                sb.Append("    <td>:</td>");
                sb.Append(" <td style='font-size:10px;'>" + lstresult[0].PatientID + "</td> </tr>");
                sb.Append(" <tr><td>Patient Name</td> ");
                sb.Append("    <td>:</td>");
                sb.Append(" <td style='font-size:10px;'>" + lstresult[0].FirstName + "</td> </tr>");
                sb.Append("      <td>Age  </td>");
                sb.Append("      <td>:</td>");
                sb.Append("     <td style='font-size:10px;'>" + lstresult[0].Age + "</td>");
                sb.Append(" </tr>  <tr>");
                sb.Append("      <td>Gender  </td>");
                sb.Append("      <td>:</td>");
                sb.Append("     <td style='font-size:10px;'>" + lstresult[0].Gender + " </td>");
                sb.Append("  </tr>");
                sb.Append("  <tr>");
                sb.Append("      <td>Cell No  </td>");
                sb.Append("      <td>:</td>");
                sb.Append("     <td style='font-size:10px;'>" + lstresult[0].MobileNumber + " </td>");
                sb.Append("  </tr>");
                //if (sefcmp == "Y")
                //{
                //    sb.Append(" <tr><td>Staff ID</td> ");
                //    sb.Append("    <td>:</td>");
                //    sb.Append(" <td style='font-size:10px;'>" + ds.Tables[1].Rows[0]["HIS_StaffCode"] + "</td> </tr>");

                //    sb.Append(" <tr><td>Company</td> ");
                //    sb.Append("    <td>:</td>");
                //    sb.Append(" <td colspan='3' style='font-size:10px;width:250px;'>" + ds.Tables[1].Rows[0]["HIS_PanelName"] + "</td> </tr>");
                //}
                sb.Append(" </table>");
                #endregion

                #region table2

                string font1 = "\"Bar-Code 39\"";
                sb.Append("   <table style='float: right;font-size:10px;width:50%;' class='OrederReprint'>");
                sb.Append("       <tr>");
                sb.Append("        <td colspan='3' style='font: bold; margin-top:-20px; font-size: 16px;font-family:" + font1 + ";'>*" + lstresult[0].PatientID + "*</td> "); //+ ds.Tables[0].Rows[0]["Lis_Patient_Id"].ToString() +
                sb.Append("    </tr>");
                sb.Append("       <tr>");
                sb.Append("         <td>Bill No</td>");
                sb.Append("        <td>:</td>");
                sb.Append("        <td style='font-size:10px;'>" + lstresult[0].BillNo + " </td>");
                sb.Append("    </tr>");
                sb.Append("     <tr>");
                sb.Append("        <td>Bill Date </td>");
                sb.Append("       <td>:</td>");
                sb.Append("     <td style='font-size:10px;'>" + lstresult[0].BillDate + "</td>");
                sb.Append("   </tr>");
                sb.Append("   <tr>");
                sb.Append("    <td>Consultant Name</td>");
                sb.Append("    <td>:</td>");
                sb.Append("   <td style='font-size:10px;'>" + lstresult[0].DoctorName + "</td>");
                sb.Append("  </tr>");
                sb.Append("   <tr>");
                sb.Append("      <td>  </td>");
                sb.Append("      <td></td>");
                sb.Append("     <td style='font-size:10px;'></td>");
                sb.Append("  </tr>");
                //if (sefcmp == "Y")
                //{
                //    sb.Append(" <tr><td>Department</td> ");
                //    sb.Append("    <td>:</td>");
                //    sb.Append(" <td style='font-size:10px;'>" + ds.Tables[1].Rows[0]["HIS_DepartmentCode"] + "</td> </tr>");
                //}

                sb.Append("</table>");
                #endregion

                sb.Append("<table style='width:95%' class='OrederReprint'>");
                sb.Append("<tr>");
                sb.Append("<td colspan='7' style='border-top:1px solid black'></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td style='font-size:10px;'>SNo</td>");
                sb.Append("<td colspan='2' style='font-size:10px;' >SERVICE</td>");
                sb.Append("<td style='font-size:10px;width:40px;text-align: right;' >QTY</td>");
                sb.Append("<td style='font-size:10px;width:60px;text-align: right;' >RATE</td>");
                sb.Append("<td style='font-size:10px;text-align: right;' >DISCOUNT</td>");
                sb.Append("<td style='font-size:10px;text-align: right;' >AMOUNT (Rs)</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(" <td colspan='7' style='border-top:1px solid black'></td>");
                sb.Append("</tr>");
                sb.Append("<tbody>");

                for (int i = 0; i < lstorder.Count; i++)
                {
                    sb.Append("<tr style='height:1px'>   ");
                    sb.Append(" <td style='width:10px;font-size:10px;' >" + lstorder[i].SNO + "</td>   ");
                    sb.Append(" <td colspan='2' style='font-size:10px;'>" + lstorder[i].TestName + "</td>   ");
                    sb.Append(" <td style='width:10px;font-size:10px;text-align: right;'>" + lstorder[i].Ordqty + "</td>   ");
                    sb.Append(" <td style='width:10px;font-size:10px;text-align: right;'>" + lstorder[i].Ordprice + "</td>   ");
                    sb.Append(" <td style='font-size:10px;text-align: right;'>" + lstorder[i].Orddiscount + "</td>   ");
                    sb.Append(" <td style='font-size:10px;text-align: right;' >" + lstorder[i].Ordtotal + "</td>   ");
                    sb.Append("</tr>   ");
                }
                sb.Append("<tr>");
                sb.Append(" <td colspan='7' style='border-top:1px solid black'></td>");
                sb.Append("</tr>");
                sb.Append("<tr>");

                double cashamount = Convert.ToDouble(lstorder[0].CashReceivedAmt);
                double grandtotal = Convert.ToDouble(lstorder[0].Ord_Grandtotal);
                double chequeamount = Convert.ToDouble(lstorder[0].ChequeAmt);
                double cardamount = Convert.ToDouble(lstorder[0].CreditCardAmt) + Convert.ToDouble(lstorder[0].DebitCardAmt);
                double granddiscount = Convert.ToDouble(lstorder[0].Ord_Granddiscount);
                double collectedamount = Convert.ToDouble(lstorder[0].RECIEVEDAMT);
                double pendingamount = Convert.ToDouble(lstorder[0].Currentpendingamount);
                double bankamount = Convert.ToDouble(lstorder[0].ThroughBankAmt);
                double RefundAmt = Convert.ToDouble(lstorder[0].OP_Current_Refund);

                if (cashamount == 0.00)
                {
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                }
                else
                {
                    sb.Append(" <td colspan='2' style='font-weight:bold;font-size:10px;'>Cash  Amt</td>   ");
                    sb.Append(" <td style='font-weight:bold;font-size:10px;'>" + cashamount + ".00/-</td>   ");
                    sb.Append("<td colspan='2'></td>");
                }

                sb.Append(" <td style='font-size:10px;'>Total Bill Amt</td>   ");
                sb.Append(" <td style='font-size:10px;text-align: right;'>" + grandtotal + ".00/-</td>   ");
                sb.Append("</tr>   ");
                sb.Append("<tr>   ");

                if (chequeamount != 0)
                {
                    sb.Append(" <td colspan='2' style='font-weight:bold;font-size:10px;'>Cheque Amt </td>   ");
                    sb.Append(" <td style='font-weight:bold;font-size:10px;'>" + chequeamount + ".00/-</td>   ");
                    sb.Append(" <td colspan='2'></td>   ");
                }
                else
                {
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                }

                sb.Append(" <td style='font-size:10px;'>Discount</td>   ");
                sb.Append(" <td style='font-size:10px;text-align: right;'>" + granddiscount + ".00/-</td>   ");

                sb.Append("</tr>   ");
                sb.Append("<tr>   ");
                if (cardamount != 0.00)
                {
                    sb.Append("<td colspan='2' style='font-weight:bold;font-size:10px;'>Card Amt</td>");
                    sb.Append("<td style='font-weight:bold;font-size:10px;'>" + cardamount + ".00/-</td>");
                    sb.Append("<td colspan='2'></td>");
                }
                else
                {
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                    sb.Append("<td></td>");
                }

                sb.Append("</tr>   ");
                sb.Append("<tr>   ");

                if (bankamount == 0.00)
                {
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                    sb.Append(" <td></td>   ");
                }
                else
                {
                    sb.Append(" <td colspan='2' style='font-weight:bold;font-size:10px;'>Bank Amt </td>   ");
                    sb.Append(" <td style='font-weight:bold;font-size:10px;'>" + bankamount + ".00/-</td>   ");
                    sb.Append(" <td colspan='2'></td>   ");
                }

                sb.Append(" <td style='font-size:10px;'>Collected Amt</td>   ");
                sb.Append(" <td style='font-size:10px;text-align: right;'>" + collectedamount + ".00/-</td>   ");

                sb.Append("</tr>   ");
                sb.Append("<tr>   ");
                sb.Append(" <td colspan='2'></td>   ");
                sb.Append(" <td ></td>   ");
                sb.Append("<td colspan='2'></td>");
                if (pendingamount != 0)
                {
                    sb.Append(" <td style='font-size:10px;'>Pending Amt</td>   ");
                    sb.Append(" <td style='font-size:10px;text-align: right;'>" + pendingamount + ".00/-</td>   ");
                }
                sb.Append("</tr>   ");

                sb.Append("<tr>   ");
                sb.Append(" <td colspan='2'></td>   ");
                sb.Append(" <td ></td>   ");
                sb.Append("<td colspan='2'></td>");
                if (RefundAmt != 0)
                {
                    sb.Append(" <td style='font-size:10px;'>Refund Amt</td>   ");
                    sb.Append(" <td style='font-size:10px;text-align: right;'>" + RefundAmt + ".00/-</td>   ");
                }
                sb.Append("</tr>   ");

                sb.Append("<tr >   ");
                sb.Append("<td></td>");
                sb.Append(" <td style='font-weight:bold;font-size:12px;'>Rupees &nbsp;&nbsp;&nbsp;" + NumberToWords.Amounttowords(collectedamount) + " only </td>   ");
                sb.Append("</tr>   ");




                sb.Append("<tr>   ");
                sb.Append(" <td colspan='7' style='border-top:1px solid black'></td>   ");
                sb.Append("</tr>   ");
                sb.Append(" </tbody>   ");
                sb.Append("</table>   ");

                PrintResult = sb.ToString();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(PrintResult);
        }
        [HttpGet]
        public JsonResult GetTodaDuePatients()
        {
            List<DuePatient> orderPatient = new List<DuePatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayDuePatients(StartDatetime, EndDatetime, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayDuePatientsSearch(string Fromdate, string Todate, string Search, string Days, string PatientType, string RowCount)
        {
            List<DuePatient> orderPatient = new List<DuePatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (Fromdate != null && Todate != null)
                {
                    string[] FromDatesplit = Fromdate.Split("/");
                    string[] ToDatesplit = Todate.Split("/");
                    Fromdate = FromDatesplit[2] + "-" + FromDatesplit[1] + "-" + FromDatesplit[0] + " " + "00:00:00";
                    Todate = ToDatesplit[2] + "-" + ToDatesplit[1] + "-" + ToDatesplit[0] + " " + "23:59:59";
                }
                if (Fromdate == null && Todate == null)
                {
                    if (Days == "Today")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "2 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonetwodays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "7 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonesevendays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                }
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayDuePatientsSearch(Fromdate, Todate, Search, Days, PatientType, RowCount, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayRefundPatients()
        {
            List<RefundPatient> orderPatient = new List<RefundPatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string StartDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string EndDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayRefundPatients(StartDatetime, EndDatetime, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTodayRefundPatientsSearch(string Fromdate, string Todate, string Search, string Days, string PatientType, string RowCount)
        {
            List<RefundPatient> orderPatient = new List<RefundPatient>();
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (Fromdate != null && Todate != null)
                {
                    string[] FromDatesplit = Fromdate.Split("/");
                    string[] ToDatesplit = Todate.Split("/");
                    Fromdate = FromDatesplit[2] + "-" + FromDatesplit[1] + "-" + FromDatesplit[0] + " " + "00:00:00";
                    Todate = ToDatesplit[2] + "-" + ToDatesplit[1] + "-" + ToDatesplit[0] + " " + "23:59:59";
                }
                if (Fromdate == null && Todate == null)
                {
                    if (Days == "Today")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "2 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonetwodays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                    else if (Days == "7 Days")
                    {
                        Fromdate = timezoneUtility.GetDateBytimezonesevendays(Timezoneid) + " " + "00:00:00";
                        Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                    }
                }
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                orderPatient = _orderRepo.GetTodayRefundPatientsSearch(Fromdate, Todate, Search, Days, PatientType, RowCount, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(orderPatient);
        }
        [HttpGet]
        public JsonResult GetTestDetails(long OrderID)
        {
            List<TestBinding> TestDetails = new List<TestBinding>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                TestDetails = _orderRepo.GetTestDetails(HospitalID, OrderID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(TestDetails);
        }

        #region Order Entry Screen Function 
        [HttpGet]
        public JsonResult GetOrderTestByTestID(long TestId)
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<TestOrderBinding> lstResult = new List<TestOrderBinding>();
            try
            {
                lstResult = _orderRepo.GetOrderTestByTestID(TestId, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpPost]
        public JsonResult CreateNewPatOrder([FromBody] PatientTestOrderView patientTestOrder)
        {
            int Status = 0;
            string StausText = "";
            int result = 0;
            long OrderID = 0;
            try
            {
                //decimal DueCollected = patientTestOrder.DueCollected;
                //if (DueCollected != 0)
                //{
                //    patientTestOrder.Ord_nettotal = patientTestOrder.Ord_nettotal - DueCollected;
                //    if (patientTestOrder.CashReceivedAmt != 0)
                //    {
                //        if (patientTestOrder.CashReceivedAmt > DueCollected)
                //        {
                //            patientTestOrder.CashReceivedAmt = patientTestOrder.CashReceivedAmt - DueCollected;
                //        }
                //    }
                //    else if(patientTestOrder.CreditCardAmt!=0)
                //    {
                //        if (patientTestOrder.CreditCardAmt > DueCollected)
                //        {
                //            patientTestOrder.CreditCardAmt = patientTestOrder.CreditCardAmt - DueCollected;
                //        }
                //    }
                //}

                int Departmentid = _investrepo.GetDepartmentid(patientTestOrder.OrderDocCode);
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDateTime = timezoneUtility.Gettimezone(Timezoneid);
                int HospitalId = Convert.ToInt16(HttpContext.Session.GetString("Hospitalid"));
                if (patientTestOrder.OrderID == 0)
                {
                    OrderHeader orderHeader = new OrderHeader()
                    {
                        OrderPatID = patientTestOrder.OrderPatID,
                        VisitID = patientTestOrder.VisitID,
                        OrderType = patientTestOrder.OrderType,
                        OrderDocCode = patientTestOrder.OrderDocCode,
                        IsOrderActive = patientTestOrder.IsOrderActive,
                        Ord_Grandtotal = patientTestOrder.Ord_Grandtotal,
                        Ord_Granddiscount = patientTestOrder.Ord_Granddiscount,
                        Ord_GranddistypeID = patientTestOrder.Ord_GranddistypeID,
                        Ord_nettotal = patientTestOrder.Ord_nettotal,
                        Finyear = patientTestOrder.Finyear,
                        RECIEVEDAMT = patientTestOrder.RECIEVEDAMT,
                        PENDINGTOPAY = patientTestOrder.PENDINGTOPAY,
                        HospitalID = HospitalId,
                        Currentpendingamount = patientTestOrder.Currentpendingamount,
                        IsSentforAuthorise = patientTestOrder.IsSentforAuthorise,
                        AuthoriseUserCode = patientTestOrder.AuthoriseUserCode,
                        OP_RefundAmt = patientTestOrder.OP_RefundAmt,
                        OP_Current_Refund = patientTestOrder.OP_Current_Refund,

                        CreateDt = timezoneUtility.Gettimezone(Timezoneid),
                        CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid")),
                        ModifyDt = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid"))
                    };
                    bool isHeaderSuccess = _orderRepo.CreateNewOrderHeader(orderHeader);
                    if (isHeaderSuccess)
                    {
                        if (patientTestOrder.OrderDetails.Length > 0)
                        {
                            OrderID = orderHeader.OrderID;
                            for (int count = 0; count < patientTestOrder.OrderDetails.Length; count++)
                            {
                                OrderDetails orderDetails = new OrderDetails()
                                {
                                    OrderId = orderHeader.OrderID,
                                    TestID = patientTestOrder.OrderDetails[count].TestID,
                                    DoctorID = patientTestOrder.OrderDocCode,
                                    DepartmentID = Departmentid,
                                    IsOrderListActive = patientTestOrder.OrderDetails[count].IsOrderListActive,
                                    CreatedDt = timezoneUtility.Gettimezone(Timezoneid),
                                    CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid")),
                                    ModifiedDt = timezoneUtility.Gettimezone(Timezoneid),
                                    ModifiedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid")),
                                    ISStat = patientTestOrder.OrderDetails[count].ISStat,
                                    Ordqty = patientTestOrder.OrderDetails[count].Ordqty,
                                    Ordprice = patientTestOrder.OrderDetails[count].Ordprice,
                                    Orddiscount = patientTestOrder.OrderDetails[count].Orddiscount,
                                    Ordtotal = patientTestOrder.OrderDetails[count].Ordtotal,
                                    HospitalID = HospitalId,
                                    PackageID = patientTestOrder.OrderDetails[count].PackageID
                                };
                                result = _orderRepo.CreateNewOrderDeatils(orderDetails);
                            }
                            OrderPayments orderPayments = new OrderPayments()
                            {
                                OrderID = orderHeader.OrderID,
                                CashReceivedAmt = patientTestOrder.CashReceivedAmt,
                                CreditCardAmt = patientTestOrder.CreditCardAmt,
                                DebitCardAmt = patientTestOrder.DebitCardAmt,
                                ThroughBankAmt = patientTestOrder.ThroughBankAmt,
                                ChequeAmt = patientTestOrder.ChequeAmt,
                                CreditCardNumber = patientTestOrder.CreditCardNumber,
                                DebitCardNumber = patientTestOrder.DebitCardNumber,
                                CreditCardTransactionID = patientTestOrder.CreditCardTransactionID,
                                DebitCardTransactionID = patientTestOrder.DebitCardTransactionID,
                                BankName = patientTestOrder.BankName,
                                BankTransactionID = patientTestOrder.BankTransactionID,
                                ChequeNo = patientTestOrder.ChequeNo,
                                IsActive = true,
                            };
                            int Result = _orderRepo.CreateNewOrderPayment(orderPayments);
                            Result = _orderRepo.UpdateAfterOrderCreate(1, 0, orderHeader.VisitID);
                            if (Result > 0)
                            {
                                Status = 1;
                                StausText = "Save Success";
                            }
                        }

                        //if (DueCollected == 0 && patientTestOrder.Currentpendingamount > 0)
                        //{
                        //    DueHeader dueHeader = new DueHeader()
                        //    {
                        //        Due_PatientId = patientTestOrder.OrderPatID,
                        //        Due_OrderID = OrderID,
                        //        Due_PendingAmt = patientTestOrder.Currentpendingamount,
                        //        Due_HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                        //        Due_IsActive = true
                        //    };
                        //    bool IsDueHeaderSuccess = _orderRepo.CreateNewDueHeader(dueHeader);
                        //}
                    }
                    else
                    {
                        Status = 0;
                        StausText = "Logical Error";
                    }
                }
                else
                {
                    OrderHeader orderHeader = new OrderHeader()
                    {
                        OrderID = patientTestOrder.OrderID,
                        OrderPatID = patientTestOrder.OrderPatID,
                        VisitID = patientTestOrder.VisitID,
                        IsOrderActive = true,
                        Ord_Grandtotal = patientTestOrder.Ord_Grandtotal,
                        Ord_Granddiscount = patientTestOrder.Ord_Granddiscount,
                        Ord_GranddistypeID = patientTestOrder.Ord_GranddistypeID,
                        Ord_nettotal = patientTestOrder.Ord_nettotal,
                        Finyear = patientTestOrder.Finyear,
                        RECIEVEDAMT = patientTestOrder.RECIEVEDAMT,
                        PENDINGTOPAY = patientTestOrder.PENDINGTOPAY,
                        HospitalID = HospitalId,
                        Currentpendingamount = patientTestOrder.Currentpendingamount,
                        IsSentforAuthorise = patientTestOrder.IsSentforAuthorise,
                        AuthoriseUserCode = patientTestOrder.AuthoriseUserCode,
                        OP_RefundAmt = patientTestOrder.OP_RefundAmt,
                        OP_Current_Refund = patientTestOrder.OP_Current_Refund,
                        CreateDt = timezoneUtility.Gettimezone(Timezoneid),
                        CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid")),
                        ModifyDt = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid"))
                    };
                    bool isHeaderSuccess = _orderRepo.UpdateOrderHeader(orderHeader);
                    if (isHeaderSuccess)
                    {
                        if (patientTestOrder.OrderDetails.Length > 0)
                        {
                            OrderID = orderHeader.OrderID;
                            for (int count = 0; count < patientTestOrder.OrderDetails.Length; count++)
                            {
                                OrderDetails orderDetails = new OrderDetails()
                                {
                                    OrderId = orderHeader.OrderID,
                                    TestID = patientTestOrder.OrderDetails[count].TestID,
                                    DoctorID = patientTestOrder.OrderDocCode,
                                    DepartmentID = Departmentid,
                                    IsOrderListActive = true,
                                    CreatedDt = timezoneUtility.Gettimezone(Timezoneid),
                                    CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid")),
                                    ModifiedDt = timezoneUtility.Gettimezone(Timezoneid),
                                    ModifiedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid")),
                                    ISStat = patientTestOrder.OrderDetails[count].ISStat,
                                    Ordqty = patientTestOrder.OrderDetails[count].Ordqty,
                                    Ordprice = patientTestOrder.OrderDetails[count].Ordprice,
                                    Orddiscount = patientTestOrder.OrderDetails[count].Orddiscount,
                                    Ordtotal = patientTestOrder.OrderDetails[count].Ordtotal,
                                    HospitalID = HospitalId,
                                    PackageID = patientTestOrder.OrderDetails[count].PackageID
                                };
                                long CheckTestIDExist = _orderRepo.CheckTestIDExist(patientTestOrder.OrderDetails[count].TestID, orderHeader.OrderID);
                                if (CheckTestIDExist == 0)
                                    result = _orderRepo.CreateNewOrderDeatils(orderDetails);
                                else
                                    result = _orderRepo.UpdateOrderDeatils(orderDetails);
                            }
                            OrderPayments orderPayments = new OrderPayments()
                            {
                                OrderID = orderHeader.OrderID,
                                CashReceivedAmt = patientTestOrder.CashReceivedAmt,
                                CreditCardAmt = patientTestOrder.CreditCardAmt,
                                DebitCardAmt = patientTestOrder.DebitCardAmt,
                                ThroughBankAmt = patientTestOrder.ThroughBankAmt,
                                ChequeAmt = patientTestOrder.ChequeAmt,
                                CreditCardNumber = patientTestOrder.CreditCardNumber,
                                DebitCardNumber = patientTestOrder.DebitCardNumber,
                                CreditCardTransactionID = patientTestOrder.CreditCardTransactionID,
                                DebitCardTransactionID = patientTestOrder.DebitCardTransactionID,
                                BankName = patientTestOrder.BankName,
                                BankTransactionID = patientTestOrder.BankTransactionID,
                                ChequeNo = patientTestOrder.ChequeNo,
                                IsActive = true,
                            };
                            int Result = _orderRepo.UpdateOrderPayment(orderPayments);
                            Result = _orderRepo.UpdateAfterOrderCreate(1, 0, orderHeader.VisitID);
                            if (Result > 0)
                            {
                                Status = 1;
                                StausText = "Save Success";
                            }
                        }
                    }
                    else
                    {
                        Status = 0;
                        StausText = "Logical Error";
                    }
                }
            }
            catch (Exception ex)
            {
                Status = 0;
                StausText = "Error";
                _errorlog.WriteErrorLog(ex.InnerException.ToString());
            }
            return Json(new { status = Status, statusText = StausText, orderID = OrderID });
        }
        [HttpGet]
        public JsonResult GetOrderDueRefundAmountByPatID(string PatientId)
        {
            List<PatientCurrentPayments> lstResult = new List<PatientCurrentPayments>();
            try
            {
                lstResult = _orderRepo.GetOrderDueRefundAmountByPatID(PatientId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult GetOrderTestByPackageID(long PakckageID)
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<TestOrderBinding> lstResult = new List<TestOrderBinding>();
            try
            {
                lstResult = _orderRepo.GetOrderTestByPackageID(PakckageID, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        #endregion

        #region Raffi for Due
        [HttpPost]
        public JsonResult CreateNewDue([FromBody] DueHeader patientTestOrder)
        {
            int Status = 0;
            string StausText = "";
            long isHeaderSuccess = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDateTime = timezoneUtility.Gettimezone(Timezoneid);
                int HospitalId = Convert.ToInt16(HttpContext.Session.GetString("Hospitalid"));

                DueHeader dueHeader = new DueHeader()
                {
                    Due_PatientId = patientTestOrder.Due_PatientId,
                    Due_OrderID = patientTestOrder.Due_OrderID,
                    Due_CollectedAmt = patientTestOrder.Due_CollectedAmt,
                    Due_DiscountAmt = patientTestOrder.Due_DiscountAmt,
                    Due_NetAmt = patientTestOrder.Due_NetAmt,
                    Due_PendingAmt = patientTestOrder.Due_PendingAmt,
                    Due_Bycash = patientTestOrder.Due_Bycash,
                    Due_ByCCard = patientTestOrder.Due_ByCCard,
                    Due_ByDCard = patientTestOrder.Due_ByDCard,
                    Due_ByBank = patientTestOrder.Due_ByBank,
                    Due_ByCheque = patientTestOrder.Due_ByCheque,
                    Due_CreditCardNo = patientTestOrder.Due_CreditCardNo,
                    Due_DebitCardNo = patientTestOrder.Due_DebitCardNo,
                    Due_BankTransactionID = patientTestOrder.Due_BankTransactionID,
                    Due_ChequeNo = patientTestOrder.Due_ChequeNo,
                    Due_CollectedBy = Convert.ToInt32(HttpContext.Session.GetString("Userseqid")),
                    Due_CollectedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    Due_HospitalID = HospitalId,
                    Due_IsActive = true
                };
                isHeaderSuccess = _orderRepo.CreateNewDueHeader(dueHeader);
                if (isHeaderSuccess > 0)
                {
                    DueDetail dueDetail = new DueDetail()
                    {
                        Due_HeaderID = isHeaderSuccess,
                        Due_OrderID = patientTestOrder.Due_OrderID,
                        Due_CollectAmt = patientTestOrder.Due_CollectedAmt,
                        Due_IsActive = true
                    };
                    long isDetailSuccess = _orderRepo.CreateNewDueDetail(dueDetail);
                    decimal CurrentPendingAmt = patientTestOrder.Due_PendingAmt;
                    bool IsSuccess = _orderRepo.UpdateOrderPendngAmt(CurrentPendingAmt, patientTestOrder.Due_OrderID);
                }
            }
            catch (Exception ex)
            {
                Status = 0;
                StausText = "Error";
                _errorlog.WriteErrorLog(ex.InnerException.ToString());
            }
            return Json(new { status = Status, statusText = StausText, DueID = isHeaderSuccess });
        }
        #endregion
    }
}