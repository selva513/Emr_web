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
using System.Net;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/DueCollection")]
    [ApiController]
    public class DueAPIController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ICashBillRepo _cashBillRepo;
        private readonly IDueCollectionRepo _dueCollectionRepo;
        public DueAPIController(IDBConnection iDBConnection, IErrorlog errorlog,ICashBillRepo cashBillRepo,IDueCollectionRepo dueCollectionRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _cashBillRepo = cashBillRepo;
            _dueCollectionRepo = dueCollectionRepo;
        }
        [HttpGet("GetDuePatientsByHospital")]
        public JsonResult GetDuePatientsByHospital()
        {
            List<DuePatient> lstResult = new List<DuePatient>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _dueCollectionRepo.GetDuePatientsByHospital(HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetDueBillByPatinetID")]
        public JsonResult GetDueBillByPatinetID(string PatientID,string PaymentMode)
        {
            List<DueBills> lstResult = new List<DueBills>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if(PaymentMode== "IP Payment")
                    lstResult = _dueCollectionRepo.GetDueBillByPatinetIDByPaymentMode(PatientID, HospitalId);
                else
                    lstResult = _dueCollectionRepo.GetDueBillByPatinetID(PatientID, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpPost("SaveDueCollection")]
        public JsonResult SaveDueCollection([FromBody] SelectDueCollectionInfo dueCollectionInfo)
        {
            string Result = "Save Suucess";
            if (dueCollectionInfo.BillArray.Length > 0)
            {
                for(int count = 0; count < dueCollectionInfo.BillArray.Length; count++)
                {
                    if (dueCollectionInfo.DueAmount == 0)
                    {
                        break;
                    }
                    int BillNo= dueCollectionInfo.BillArray[count].PH_CSH_BILLNO;
                    decimal PendingAmt = dueCollectionInfo.BillArray[count].PH_CSH_PENDINGTOPAY;
                    decimal NetAmt= dueCollectionInfo.BillArray[count].PH_CSH_NETTAMOUNT;
                    decimal CashAmt = NetAmt - PendingAmt;
                    decimal Currentdue = dueCollectionInfo.DueAmount - PendingAmt;
                    if (Currentdue < 0)
                    {
                        CashAmt = NetAmt - PendingAmt + dueCollectionInfo.DueAmount;
                        dueCollectionInfo.DueAmount = Math.Abs(Currentdue);
                        int UpdateResult = _dueCollectionRepo.UpdateDueCollection(BillNo, CashAmt, dueCollectionInfo.DueAmount);
                        DueCollectionInfo collectionInfo = new DueCollectionInfo()
                        {
                            BillNo= BillNo,
                            DueAmount=PendingAmt,
                            DueCollectedAmt= PendingAmt - dueCollectionInfo.DueAmount,
                            CreatedDate=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            HospitalID= Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                            CreatedUser= HttpContext.Session.GetString("Userseqid").ToString(),
                            TotalAmount=NetAmt,
                            CashReceivedAmt=dueCollectionInfo.CashReceivedAmt,
                            CreditCardAmt=dueCollectionInfo.CreditCardAmt,
                            DebitCardAmt=dueCollectionInfo.DebitCardAmt,
                            ThroughBankAmt=dueCollectionInfo.ThroughBankAmt,
                            ChequeAmt=dueCollectionInfo.ChequeAmt,
                            CreditCardNumber=dueCollectionInfo.CreditCardNumber,
                            DebitCardNumber=dueCollectionInfo.DebitCardNumber,
                            BankRefNum=dueCollectionInfo.BankRefNum,
                            ChequeNo=dueCollectionInfo.ChequeNo
                        };
                        int SaveResult = _dueCollectionRepo.CreateDueCollection(collectionInfo);
                        if (UpdateResult > 0 && SaveResult > 0)
                            Result = "Save Suucess";
                        break;
                    }
                    else
                    {
                        CashAmt = CashAmt + PendingAmt;
                        Currentdue = dueCollectionInfo.DueAmount - PendingAmt;
                        dueCollectionInfo.DueAmount = Math.Abs(Currentdue);
                        int UpdateResult = _dueCollectionRepo.UpdateDueCollection(BillNo, CashAmt, 0);
                        DueCollectionInfo collectionInfo = new DueCollectionInfo()
                        {
                            BillNo = BillNo,
                            DueAmount = PendingAmt,
                            DueCollectedAmt = PendingAmt,
                            CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                            CreatedUser = HttpContext.Session.GetString("Userseqid").ToString(),
                            TotalAmount = NetAmt,
                            CashReceivedAmt = dueCollectionInfo.CashReceivedAmt,
                            CreditCardAmt = dueCollectionInfo.CreditCardAmt,
                            DebitCardAmt = dueCollectionInfo.DebitCardAmt,
                            ThroughBankAmt = dueCollectionInfo.ThroughBankAmt,
                            ChequeAmt = dueCollectionInfo.ChequeAmt,
                            CreditCardNumber = dueCollectionInfo.CreditCardNumber,
                            DebitCardNumber = dueCollectionInfo.DebitCardNumber,
                            BankRefNum = dueCollectionInfo.BankRefNum,
                            ChequeNo = dueCollectionInfo.ChequeNo
                        };
                        int SaveResult = _dueCollectionRepo.CreateDueCollection(collectionInfo);
                        if (UpdateResult > 0 && SaveResult > 0)
                            Result = "Save Suucess";
                    }
                }
            }
            return Json(Result);
        }
        [HttpGet("GetDueCollectionByDate")]
        public JsonResult GetDueCollectionByDate(string StartDt,string EndDt)
        {
            List<DueCollectionReport> lstResult = new List<DueCollectionReport>();
            try
            {
                DateTime frmdattm = DateTime.ParseExact(StartDt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime todatm = DateTime.ParseExact(EndDt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string frmdat = frmdattm.ToString("yyyy-MM-dd 00:00:00");
                string todat = todatm.ToString("yyyy-MM-dd 23:59:59");
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _dueCollectionRepo.GetDueCollectionByDate(frmdat, todat, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Header = lstResult});
        }
    }
    public class SelectDueCollectionInfo
    {
        public decimal DueAmount { get; set; }
        public decimal CashReceivedAmt { get; set; }
        public decimal CreditCardAmt { get; set; }
        public decimal DebitCardAmt { get; set; }
        public decimal ThroughBankAmt { get; set; }
        public decimal ChequeAmt { get; set; }
        public string CreditCardNumber { get; set; }
        public string DebitCardNumber { get; set; }
        public string ChequeNo { get; set; }
        public string BankRefNum { get; set; }
        public DueBills[] BillArray { get; set; }
    }
}