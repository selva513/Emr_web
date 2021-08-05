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

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/SurgicalDispense")]
    [ApiController]
    public class SurgicalDispenseController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ISurgicalInstrumentRepo _surgicalInstrumentRepo;
        private readonly IInstrumentDIRepo _instrumentDIRepo;
        public SurgicalDispenseController(IDBConnection iDBConnection, IErrorlog errorlog,
            ISurgicalInstrumentRepo surgicalInstrumentRepo, IInstrumentDIRepo instrumentDIRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _surgicalInstrumentRepo = surgicalInstrumentRepo;
            _instrumentDIRepo = instrumentDIRepo;
        }
        [HttpGet("GetSurgicalInstrumentsSearch")]
        public JsonResult GetSurgicalInstrumentsSearch(string SearchTearm)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SurgicalInstruments> lstResult = new List<SurgicalInstruments>();
            try
            {
                lstResult = _surgicalInstrumentRepo.GetSurgicalInstrumentsSearch(SearchTearm, HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetSurgicalCurrentStock")]
        public JsonResult GetSurgicalCurrentStock(string SearchTearm,string Wahrehous)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SurgicalCurrentStockInfo> lstResult = new List<SurgicalCurrentStockInfo>();
            try
            {
                lstResult = _surgicalInstrumentRepo.GetSurgicalCurrentStock(SearchTearm, HospitalID,Wahrehous);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetSurgicalCurrentStockByItemCode")]
        public JsonResult GetSurgicalCurrentStockByItemCode(long ItemCode, string Wahrehous)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SurgicalCurrentStockInfo> lstResult = new List<SurgicalCurrentStockInfo>();
            try
            {
                lstResult = _surgicalInstrumentRepo.GetSurgicalCurrentStockByItemCode(ItemCode, HospitalID, Wahrehous);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetSurgicalCurrentStockByItemName")]
        public JsonResult GetSurgicalCurrentStockByItemName(string ItemName, string Wahrehous)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SurgicalCurrentStockInfo> lstResult = new List<SurgicalCurrentStockInfo>();
            try
            {
                lstResult = _surgicalInstrumentRepo.GetSurgicalCurrentStockByItemName(ItemName, HospitalID, Wahrehous);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpPost("BillSave")]
        public JsonResult BillSave([FromBody] InstrumentDispenseInfo dispanseInfo)
        {
            List<InstrumentBillHeader> billHeaders = new List<InstrumentBillHeader>();
            List<InstrumentCashBillDtlInfo> cashBillDeatilsInfos = new List<InstrumentCashBillDtlInfo>();
            try
            {
                for (int count = 0; count < dispanseInfo.DrugInfos.Count; count++)
                {
                    string storeName = dispanseInfo.StoreName;
                    string Batch = dispanseInfo.DrugInfos[count].PH_CUR_MODELNAME;
                    int DrugCode = dispanseInfo.DrugInfos[count].PH_CUR_DRUGCODE;
                    string Modelno = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_MODLNO;
                    
                    List<CurrentStockTimeStamp> currentStockTimes = _instrumentDIRepo.GetTimeStampByModel(Batch, DrugCode, storeName);
                    if (currentStockTimes.Count > 0)
                    {
                        byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                        currentStockTimes = _instrumentDIRepo.GetTimeStampByTimeStamp(Batch, DrugCode, storeName, LastTimeStamp);
                        if (currentStockTimes.Count > 0)
                        {
                        }
                        else
                        {
                            currentStockTimes = _instrumentDIRepo.GetTimeStampByModel(Batch, DrugCode, storeName);
                        }
                        int Qty = dispanseInfo.DrugInfos[count].Qty;
                        int CurrentStock = currentStockTimes[0].PH_CUR_STOCK - Qty;
                        string Remarks = "Dispense";
                        int StockUpdate = _instrumentDIRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks);
                        //DateTime ExDate = DateTime.Now;
                        //DateTime RunningExpiryDate = GetDataformat(dispanseInfo.DrugInfos[count].ExpiryDt, ExDate);
                        InstrumentStockMovements stockMovement = new InstrumentStockMovements
                        {
                            PH_RUN_PROCESSIDKEY = "Invoice",
                            PH_RUN_STORENAME = storeName,
                            PH_RUN_INSTRUMENTCODE = DrugCode,
                            PH_RUN_STOCK_TRANSACTVALUE = Qty,
                            PH_RUN_STOCK_AFTERTRANSACT = CurrentStock,
                            PH_RUN_STOCK_LEFTOUTINBATCH = currentStockTimes[0].PH_CUR_STOCK,
                            PH_RUN_MODELNAME = Batch,
                            PH_RUN_MODELNO = Modelno,
                            PH_RUN_COMPANYNAME = "",
                            PH_RUN_DOC_HDRNO = 1,
                            PH_RUN_DOC_DTLNO = 1,
                            PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                            PH_RUN_FINYEAR = "20-21",
                            PH_RUN_TRANSACT_ISACTIVE = true,
                            PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                            PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
                        int StockMovementUpdate = _instrumentDIRepo.InsertStockMovement(stockMovement);
                    }
                }
                CashBillCustInfo cashBillPatInfo = new CashBillCustInfo()
                {
                    BillNo = _surgicalInstrumentRepo.GetCashBillNo(),
                    BillDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProcessKey = "PHBYSOP",
                    FinYear = "20-21",
                    Paymentmode = "Cash",
                    CustomerName = dispanseInfo.CustomerName,
                    Address = dispanseInfo.Address,
                    City = dispanseInfo.City,
                    Country = dispanseInfo.Country,
                    NetAmount = dispanseInfo.NetTotlaAmount,
                    Roundoff = dispanseInfo.Roundoff,
                    TaxAmount = dispanseInfo.Tax,
                    consession = dispanseInfo.Consession,
                    TotalAmount = dispanseInfo.Amount,
                    CashRecivedAmont = dispanseInfo.CashReceivedAmt,
                    CardRecivedAmount = dispanseInfo.CreditCardAmt + dispanseInfo.DebitCardAmt + dispanseInfo.ThroughBankAmt + dispanseInfo.ChequeAmt,
                    pendingtoPay = dispanseInfo.PendingtoPay,
                    CashAdvanceAdjusted = 0,
                    CashAdvanceVoucherNo = 0,
                    CurrentPendingAmount = dispanseInfo.PendingtoPay,
                    IsActive = true,
                    CreatedUser = HttpContext.Session.GetString("Userseqid"),
                    PRECRIPTIONID = "",
                    RefoundAmount = 0,
                    CurrentRefoundAmount = 0,
                    StroeRoom = dispanseInfo.StoreName,
                    Grandroundtotal = dispanseInfo.CashReceivedAmt,
                    GroupName = "",  // Changed By Raffi for Free dispense
                    isFree = false,    // Changed By Raffi for Free dispense
                    TidNumber = "",
                    Staffcode = "",
                    CompanyName = "",
                    IsClaim = false,
                    Department = "",
                    isResponse = false,
                    orderid = "",
                    claimip = "",
                    tempprocesskey = "",
                    Chequeno = "",
                    Bankname = "",
                    Chequedate = "",
                    Urnno = "",
                    fournum = "",
                    DiscountType = dispanseInfo.DiscountType,
                    DiscountRate = dispanseInfo.DiscountRate,
                    CashAmt = dispanseInfo.CashReceivedAmt,
                    CreditCardAmt = dispanseInfo.CreditCardAmt,
                    DebitCardAmt = dispanseInfo.DebitCardAmt,
                    ThroughBankAmt = dispanseInfo.ThroughBankAmt,
                    ChequeAmt = dispanseInfo.ChequeAmt,
                    CreditCardNumber = dispanseInfo.CreditCardNumber,
                    DebitCardNumber = dispanseInfo.DebitCardNumber,
                    BankRefNum = dispanseInfo.BankRefNum,
                    HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                    StoreName = dispanseInfo.StoreName
                };
                int CashBillHeader = _surgicalInstrumentRepo.CreateNewCashBillHeader(cashBillPatInfo);
                if (CashBillHeader > 0)
                {
                    for (int count = 0; count < dispanseInfo.DrugInfos.Count; count++)
                    {
                        //DateTime ExDate = DateTime.Now;
                        //DateTime RunningExpiryDate = GetDataformat(dispanseInfo.DrugInfos[count].ExpiryDt, ExDate);
                        CashBillCustDtlInfo cashBillDeatilsInfo = new CashBillCustDtlInfo()
                        {
                            PH_CSHDTL_BILLNO = cashBillPatInfo.BillNo,
                            PH_CSHDTL_DRUGCODE = dispanseInfo.DrugInfos[count].PH_CUR_DRUGCODE,
                            PH_CSHDTL_DRUG_QTY = dispanseInfo.DrugInfos[count].Qty,
                            PH_CSHDTL_MODELNAME = dispanseInfo.DrugInfos[count].PH_CUR_MODELNAME,
                            PH_CSHDTL_MODELNO = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_MODLNO,
                            PH_CSHDTL_DRUG_AMTEACH = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_BILLINGPRICE,
                            PH_CSHDTL_DRUG_ROWTOTALAMT = dispanseInfo.DrugInfos[count].Amount,
                            PH_CSHDTL_DRUG_CONCESSION_AMT = 0,
                            PH_CSHDTL_DRUG_TAXPERCENT = dispanseInfo.DrugInfos[count].PH_ITEM_DRUG_VAT,
                            PHCSHDTL_DRUG_TAXVALUE = dispanseInfo.DrugInfos[count].Tax,
                            PH_CSHDTL_DRUG_NETTAMT = dispanseInfo.DrugInfos[count].TotalAmount,
                            PH_CSHDTL_DRUG_ISACTIVE = true,
                            PH_CSHDTL_DRUGSTOCK_BEFOREDISPENSE = dispanseInfo.DrugInfos[count].PH_CUR_STOCK
                        };
                        int CashDeatils = _surgicalInstrumentRepo.CreateNewCashBillDetails(cashBillDeatilsInfo);
                    }
                    //int updateConfig = _cashBillRepo.UpdateBillConfig(cashBillPatInfo.BillNo);
                    billHeaders = _surgicalInstrumentRepo.GetCashBillHeaderByBillNo(cashBillPatInfo.BillNo);
                    cashBillDeatilsInfos = _surgicalInstrumentRepo.GetCashBillDeatilsByBillNo(cashBillPatInfo.BillNo);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = billHeaders, PrintDeatils = cashBillDeatilsInfos });
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