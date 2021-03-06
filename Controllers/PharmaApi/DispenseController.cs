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

namespace Emr_web.Controllers.PharmaApi
{
    [Produces("application/json")]
    [Route("Pharma/Dispense")]
    public class DispenseController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IDrugFreeSearchRepo _drugFreeSearchRepo;
        private readonly ICashBillRepo _cashBillRepo;
        private readonly IBillHoldRepo _billHoldRepo;
        private readonly IPurchaseOrderRepo _purchaseOrderRepo;
        private readonly IClientMasterRepo _clientMasterRepo;
        private readonly ICurrentStockRepo _currentStockRepo;
        public DispenseController(IDBConnection iDBConnection, IErrorlog errorlog, IDrugFreeSearchRepo drugFreeSearchRepo,
            ICashBillRepo cashBillRepo, IBillHoldRepo billHoldRepo, IPurchaseOrderRepo purchaseOrderRepo, IClientMasterRepo clientMasterRepo,ICurrentStockRepo currentStockRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _drugFreeSearchRepo = drugFreeSearchRepo;
            _cashBillRepo = cashBillRepo;
            _billHoldRepo = billHoldRepo;
            _purchaseOrderRepo = purchaseOrderRepo;
            _clientMasterRepo = clientMasterRepo;
            _currentStockRepo = currentStockRepo;
        }
        [HttpGet("GetDrugSearchByFreeText")]
        public JsonResult GetDrugSearchByFreeText(string SearchTearm, string StoreName)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugFreeSearch> lstResult = new List<DrugFreeSearch>();
            if (!string.IsNullOrWhiteSpace(SearchTearm))
            {
                var EmptySearch = SearchTearm.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < EmptySearch.Length; i++)
                {
                    if (i == 0)
                    {
                        SearchTearm = EmptySearch[0];
                    }
                    if (i == 1)
                    {
                        SearchTearm = EmptySearch[1];
                    }
                }
            }
            try
            {
                lstResult = _drugFreeSearchRepo.GetDrugSearchByFreeText(SearchTearm, HospitalID, StoreName);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetCurrentStockByDrugCode")]
        public JsonResult GetCurrentStockByDrugCode(int DrugCode, string StoreName, string GstType)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugInfo> lstResult = new List<DrugInfo>();
            try
            {
                lstResult = _drugFreeSearchRepo.GetCurrentStockByDrugCode(DrugCode, HospitalID, StoreName, GstType);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetCurrentStockByDrugCodeAndBatch")]
        public JsonResult GetCurrentStockByDrugCodeAndBatch(int DrugCode, string StoreName, string Batch, string GstType)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugInfo> lstResult = new List<DrugInfo>();
            try
            {
                lstResult = _drugFreeSearchRepo.GetCurrentStockByDrugCodeAndBatch(DrugCode, HospitalID, StoreName, Batch, GstType);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetPatientIDByFreeSearch")]
        public JsonResult GetPatientIDByFreeSearch(string FreeSearch)
        {
            List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<string> lstResult = new List<string>();
            try
            {
                bool IsConnectedHIS = logins[0].IsConnectedHIS;
                if (IsConnectedHIS)
                    lstResult = _drugFreeSearchRepo.GetPatientIDByFreeSearch(FreeSearch);
                else
                    lstResult = _drugFreeSearchRepo.GetPatientIDBySearchTeram(FreeSearch, HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetPatDeatilsByPatID")]
        public JsonResult GetPatDeatilsByPatID(string PatientID)
        {
            List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<PatientInfo> lstResult = new List<PatientInfo>();
            try
            {
                bool IsConnectedHIS = logins[0].IsConnectedHIS;
                if (IsConnectedHIS)
                    lstResult = _drugFreeSearchRepo.GetPatDeatilsByPatID(PatientID);
                else
                    lstResult = _drugFreeSearchRepo.GetNonHISPatinetDeatilsByPatID(PatientID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpPost("BillSave")]
        public JsonResult BillSave([FromBody] DispanseInfo dispanseInfo)
        {
            List<PatientInfo> lstResult = new List<PatientInfo>();
            List<BillHeader> billHeaders = new List<BillHeader>();
            List<CashBillDeatilsInfo> cashBillDeatilsInfos = new List<CashBillDeatilsInfo>();
            List<ClientDeatilsInfo> lstClient = new List<ClientDeatilsInfo>();
            List<StoreNameInfo> lstWarehouse = new List<StoreNameInfo>();
            long WarehouseId = 0;
            int HIS_Pat_AdmissionID = 0;
            int HIS_AdmitBedID = 0;
            int HIS_CurrentBedID = 0;
            try
            {
                #region Raffi for inserting isfree to true for free dispense 
                bool isfree = false;
                string Group = "";
                if (dispanseInfo.PatinetID == dispanseInfo.Lis_PatFirstName)
                {
                    isfree = true;
                    Group = dispanseInfo.PatinetID;
                }
                #endregion
                decimal ReceiveAmt = dispanseInfo.CashReceivedAmt + dispanseInfo.CreditCardAmt + dispanseInfo.DebitCardAmt + dispanseInfo.ThroughBankAmt + dispanseInfo.ChequeAmt;
                if (ReceiveAmt == 0)
                    dispanseInfo.PendingtoPay = dispanseInfo.NetTotlaAmount;
                CashBillPatInfo cashBillPatInfo = new CashBillPatInfo()
                {
                    BillNo = _cashBillRepo.GetCashBillNo(),
                    BillDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProcessKey = "PHBYSOP",
                    FinYear = "20-21",
                    Paymentmode = dispanseInfo.PaymentMode,
                    PatientID = dispanseInfo.PatinetID,
                    PatientType = dispanseInfo.Lis_Pattype,
                    PatientName = dispanseInfo.Lis_PatFirstName,
                    PatSaluation = "Mr",
                    PatSex = dispanseInfo.Lis_Sex,
                    PhoneNumber = dispanseInfo.Mobile,
                    NetAmount = dispanseInfo.NetTotlaAmount,
                    Roundoff = dispanseInfo.Roundoff,
                    TaxAmount = dispanseInfo.Tax,
                    consession = dispanseInfo.Consession,
                    TotalAmount = dispanseInfo.Amount,
                    CashRecivedAmont = dispanseInfo.CashReceivedAmt + dispanseInfo.CreditCardAmt + dispanseInfo.DebitCardAmt + dispanseInfo.ThroughBankAmt + dispanseInfo.ChequeAmt,
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
                    PatPhone = dispanseInfo.HIS_PHONE,
                    Grandroundtotal = dispanseInfo.CashReceivedAmt,
                    Patage = dispanseInfo.Lis_AgeYear,
                    RefDoctor = dispanseInfo.LIS_RefDRNAME,
                    GroupName = Group,  // Changed By Raffi for Free dispense
                    isFree = isfree,    // Changed By Raffi for Free dispense
                    Relation = dispanseInfo.HIS_RELATIONNAME,
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
                    StoreName = dispanseInfo.StoreName,
                    PH_CSH_Company = dispanseInfo.PH_CSH_Company,
                    PH_CSH_StaffCode = dispanseInfo.PH_CSH_StaffCode,
                    PH_CSH_Department = dispanseInfo.PH_CSH_Department,
                    PH_CSH_IsResponse = dispanseInfo.PH_CSH_IsResponse,
                    PatinetAge=dispanseInfo.Lis_AgeYear,
                };
                int CashBillHeader = _cashBillRepo.CreateNewCashBillHeader(cashBillPatInfo);
                if (CashBillHeader > 0)
                {
                    for (int count = 0; count < dispanseInfo.DrugInfos.Count; count++)
                    {
                        DateTime ExDate = DateTime.Now;
                        DateTime RunningExpiryDate = GetDataformat(dispanseInfo.DrugInfos[count].ExpiryDt, ExDate);
                        CashBillDeatilsInfo cashBillDeatilsInfo = new CashBillDeatilsInfo()
                        {
                            PH_CSHDTL_BILLNO = cashBillPatInfo.BillNo,
                            PH_CSHDTL_DRUGCODE = dispanseInfo.DrugInfos[count].PH_CUR_DRUGCODE,
                            PH_CSHDTL_DRUG_QTY = dispanseInfo.DrugInfos[count].Qty,
                            PH_CSHDTL_DRUGBATCHNO = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_BATCHNO,
                            PH_CSHDTL_DRUGEXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                            PH_CSHDTL_DRUG_AMTEACH = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_BILLINGPRICE,
                            PH_CSHDTL_DRUG_ROWTOTALAMT = dispanseInfo.DrugInfos[count].Amount,
                            PH_CSHDTL_DRUG_CONCESSION_AMT = 0,
                            PH_CSHDTL_DRUG_TAXPERCENT = dispanseInfo.DrugInfos[count].PH_ITEM_DRUG_VAT,
                            PHCSHDTL_DRUG_TAXVALUE = dispanseInfo.DrugInfos[count].Tax,
                            PH_CSHDTL_DRUG_NETTAMT = dispanseInfo.DrugInfos[count].TotalAmount,
                            PH_CSHDTL_DRUG_ISACTIVE = true,
                            PH_CSHDTL_DRUGSTOCK_BEFOREDISPENSE = dispanseInfo.DrugInfos[count].PH_CUR_STOCK
                        };
                        int CashDeatils = _cashBillRepo.CreateNewCashBillDetails(cashBillDeatilsInfo);
                    }
                    int updateConfig = _cashBillRepo.UpdateBillConfig(cashBillPatInfo.BillNo);

                }
                for (int count = 0; count < dispanseInfo.DrugInfos.Count; count++)
                {
                    string storeName = dispanseInfo.StoreName;
                    string Batch = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_BATCHNO;
                    int DrugCode = dispanseInfo.DrugInfos[count].PH_CUR_DRUGCODE;
                    List<CurrentStockTimeStamp> currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                    if (currentStockTimes.Count > 0)
                    {
                        byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                        currentStockTimes = _cashBillRepo.GetTimeStampByTimeStamp(Batch, DrugCode, storeName, LastTimeStamp);
                        if (currentStockTimes.Count > 0)
                        {
                        }
                        else
                        {
                            currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                        }
                        int Qty = dispanseInfo.DrugInfos[count].Qty;
                        int CurrentStock = currentStockTimes[0].PH_CUR_STOCK - Qty;
                        string Remarks = "Dispense";
                        string Reference = cashBillPatInfo.BillNo.ToString();
                        int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                        DateTime ExDate = DateTime.Now;
                        DateTime RunningExpiryDate = GetDataformat(dispanseInfo.DrugInfos[count].ExpiryDt, ExDate);
                        StockMovement stockMovement = new StockMovement
                        {
                            PH_RUN_PROCESSIDKEY = "PHBYORD",
                            PH_RUN_STORENAME = storeName,
                            PH_RUN_DRUGCODE = DrugCode,
                            PH_RUN_STOCK_TRANSACTVALUE = Qty,
                            PH_RUN_STOCK_AFTERTRANSACT = CurrentStock,
                            PH_RUN_STOCK_LEFTOUTINBATCH = currentStockTimes[0].PH_CUR_STOCK,
                            PH_RUN_BATCHNO = Batch,
                            PH_RUN_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                            PH_RUN_DOC_HDRNO = 1,
                            PH_RUN_DOC_DTLNO = 1,
                            PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                            PH_RUN_FINYEAR = "20-21",
                            PH_RUN_TRANSACT_ISACTIVE = true,
                            PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                            PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
                        int StockMovementUpdate = _cashBillRepo.InsertStockMovement(stockMovement);
                    }
                }
                long HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));

                if(dispanseInfo.Lis_Pattype == "IP" && dispanseInfo.PaymentMode != "Self Payment")
                {
                    int HIS_IP_OrderID = _cashBillRepo.GetMaxIPOrderId_Config();
                    List<HIS_Pat_AdmissionMain> hIS_Pat_AdmissionMain = new List<HIS_Pat_AdmissionMain>();
                    hIS_Pat_AdmissionMain = _cashBillRepo.GetIPDetailsByPatientID(dispanseInfo.PatinetID);

                    if (hIS_Pat_AdmissionMain.Count > 0)
                    {
                        HIS_Pat_AdmissionID = hIS_Pat_AdmissionMain[0].HIS_Pat_AdmissionID;
                        HIS_AdmitBedID = hIS_Pat_AdmissionMain[0].HIS_AdmitBedID;
                        HIS_CurrentBedID = hIS_Pat_AdmissionMain[0].HIS_CurrentBedID;
                    }
                    HIS_IP_ORDER hIS_IP_ORDER = new HIS_IP_ORDER()
                    {
                        HIS_IP_ORDERID = HIS_IP_OrderID,
                        HIS_IP_ORDERStartDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        HIS_IP_OrderPatID = dispanseInfo.PatinetID,
                        HIS_IP_ADMISSIONID = HIS_Pat_AdmissionID,
                        HIS_IP_ORDERTYPE = "1",
                        HIS_IP_OrderDt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        HIS_IP_ORDER_PayFlag = false,
                        HIS_IP_FirstAdmitBedID = HIS_AdmitBedID,
                        HIS_IP_CurrentBedID = HIS_CurrentBedID,
                        HIS_IP_OrdersClosed_Status = false,
                        HIS_IP_OrderCreateDttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        HIS_IP_OrderIsActive = true,
                        HIS_IP_LocationId = "1",
                        HIS_IP_Order_DocID = "0",
                        PH_Bill_no = cashBillPatInfo.BillNo
                    };
                    int OrderID = _cashBillRepo.CreateNewHIS_IP_Order(hIS_IP_ORDER);
                    if (OrderID > 0)
                    {
                        int UpdatedOrderID = HIS_IP_OrderID + 1;
                        _cashBillRepo.UpdateIPOrder_Config(UpdatedOrderID);
                        int HIS_IP_ORDER_TestID= _cashBillRepo.GetTestId_PHARM();

                        for (int count = 0; count < dispanseInfo.DrugInfos.Count; count++)
                        {
                            HIS_IP_ORDERDTL hIS_IP_ORDERDTL = new HIS_IP_ORDERDTL()
                            {
                                HIS_IP_PatAdmissionID = HIS_Pat_AdmissionID,
                                HIS_IP_ORDERID = HIS_IP_OrderID,
                                HIS_IP_BedID_whileOrder = HIS_CurrentBedID,
                                HIS_IP_ORDER_TestID = HIS_IP_ORDER_TestID,
                                HIS_IP_Order_Trans_Dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                HIS_IP_ORDER_isStat = false,
                                HIS_IP_OrdQty = dispanseInfo.DrugInfos[count].Qty,
                                HIS_IP_OrdPrice = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_BILLINGPRICE,
                                HIS_IP_Ord_RowTotal = dispanseInfo.DrugInfos[count].TotalAmount,
                                HIS_IP_ORDER_CancelFlag = false,
                                HIS_IP_LocationId = "1",
                                HIS_IP_Order_DocID = 0,
                                HIS_IP_OrderCreatedUserCode = 1,
                                HIS_IP_OrderCreatedDttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                ISPACKAGE = false
                            };
                            _cashBillRepo.CreateNewHIS_IP_OrderDtl(hIS_IP_ORDERDTL);
                        }
                    }
                }

                billHeaders = _cashBillRepo.GetCashBillHeaderByBillNo(cashBillPatInfo.BillNo);
                cashBillDeatilsInfos = _cashBillRepo.GetCashBillDeatilsByBillNo(cashBillPatInfo.BillNo);
                lstWarehouse = _clientMasterRepo.GetWarehouseDetailsByName(dispanseInfo.StoreName, HospitalID);
                if (lstWarehouse.Count > 0)
                {
                    WarehouseId = lstWarehouse[0].HIS_PH_STOREMASTER;
                    lstClient = _purchaseOrderRepo.GetClientDetailsByWarehouseId(WarehouseId, HospitalID);
                }

                if (dispanseInfo.HoldBillId > 0)
                {
                    int result = _billHoldRepo.DeleteHoldBillHeader(dispanseInfo.HoldBillId);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = billHeaders, PrintDeatils = cashBillDeatilsInfos, ClientDetails = lstClient });
        }
        [HttpGet("GetBillReprint")]
        public JsonResult GetBillReprint(long BillNo)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            List<CashBillDeatilsInfo> cashBillDeatilsInfos = new List<CashBillDeatilsInfo>();
            List<ClientDeatilsInfo> lstClient = new List<ClientDeatilsInfo>();
            List<StoreNameInfo> lstWarehouse = new List<StoreNameInfo>();
            long HospitalId = 0;
            long WarehouseId = 0;
            string StoreName = "";
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                billHeaders = _cashBillRepo.GetCashBillHeaderByBillNo(BillNo);
                cashBillDeatilsInfos = _cashBillRepo.GetCashBillDeatilsByBillNo(BillNo);
                if (billHeaders.Count > 0)
                {
                    StoreName = billHeaders[0].StoreName;
                    lstWarehouse = _clientMasterRepo.GetWarehouseDetailsByName(StoreName, HospitalId);
                    if (lstWarehouse.Count > 0)
                    {
                        WarehouseId = lstWarehouse[0].HIS_PH_STOREMASTER;
                        lstClient = _purchaseOrderRepo.GetClientDetailsByWarehouseId(WarehouseId, HospitalId);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = billHeaders, PrintDeatils = cashBillDeatilsInfos, ClientDetails = lstClient });
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
        [HttpGet("GetLast100BillHeader")]
        public JsonResult GetLast100BillHeader(string StoreName)
        {
            List<BillHeader> lstResult = new List<BillHeader>();
            try
            {
                long HosPitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _cashBillRepo.GetLast100BillHeader(HosPitalID, StoreName);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetBillCancel")]
        public JsonResult GetBillCancel(long BillNo)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            List<CashBillDeatilsInfo> cashBillDeatilsInfos = new List<CashBillDeatilsInfo>();
            try
            {
                billHeaders = _cashBillRepo.GetCashBillHeaderByBillNo(BillNo);
                cashBillDeatilsInfos = _cashBillRepo.GetCashBillDeatilsByBillNo(BillNo);
                if (cashBillDeatilsInfos.Count > 0)
                {
                    for (int count = 0; count < cashBillDeatilsInfos.Count; count++)
                    {
                        string Batch = cashBillDeatilsInfos[count].PH_CSHDTL_DRUGBATCHNO;
                        int Qty = Convert.ToInt32(cashBillDeatilsInfos[count].PH_CSHDTL_DRUG_QTY);
                        string ProcessKey = billHeaders[0].PH_CSH_PROCESSKEY;
                        string ExpiryDate = cashBillDeatilsInfos[count].ExpiryDt;
                        string StoreName = billHeaders[0].StoreName;
                        int DrugCode = Convert.ToInt32(cashBillDeatilsInfos[count].PH_CSHDTL_DRUGCODE);
                        List<CurrentStockTimeStamp> currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, StoreName);
                        byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                        currentStockTimes = _cashBillRepo.GetTimeStampByTimeStamp(Batch, DrugCode, StoreName, LastTimeStamp);
                        if (currentStockTimes.Count > 0)
                        {
                        }
                        else
                        {
                            currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, StoreName);
                        }
                        int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty;
                        string Remarks = "BillCancel";
                        string Reference = BillNo.ToString();
                        int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, StoreName, Batch, DrugCode, Remarks, Reference);
                        DateTime ExDate = DateTime.Now;
                        DateTime RunningExpiryDate = GetDataformat(ExpiryDate, ExDate);
                        StockMovement stockMovement = new StockMovement
                        {
                            PH_RUN_PROCESSIDKEY = ProcessKey,
                            PH_RUN_STORENAME = StoreName,
                            PH_RUN_DRUGCODE = DrugCode,
                            PH_RUN_STOCK_TRANSACTVALUE = Qty,
                            PH_RUN_STOCK_AFTERTRANSACT = CurrentStock,
                            PH_RUN_STOCK_LEFTOUTINBATCH = currentStockTimes[0].PH_CUR_STOCK,
                            PH_RUN_BATCHNO = Batch,
                            PH_RUN_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                            PH_RUN_DOC_HDRNO = 1,
                            PH_RUN_DOC_DTLNO = 1,
                            PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                            PH_RUN_FINYEAR = "20-21",
                            PH_RUN_TRANSACT_ISACTIVE = true,
                            PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                            PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        };
                        int StockMovementUpdate = _cashBillRepo.InsertStockMovement(stockMovement);
                    }
                    int BillHeaderCancel = _cashBillRepo.UpdateBillCancel(BillNo);
                    int BillDeatilsCancel = _cashBillRepo.UpdateBillDeatilsCancel(BillNo);
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = billHeaders, PrintDeatils = cashBillDeatilsInfos });
        }
        [HttpGet("GetCashBillHeaderByBillNo")]
        public JsonResult GetCashBillHeaderByBillNo(long BillNumber)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            try
            {
                billHeaders = _cashBillRepo.GetCashBillHeaderByBillNo(BillNumber);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(billHeaders);
        }

        [HttpGet("GetCashBillBySearch")]
        public JsonResult GetCashBillBySearch(string Search)
        {
            List<BillHeader> billHeaders = new List<BillHeader>();
            try
            {
                billHeaders = _cashBillRepo.GetCashBillBySearch(Search);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(billHeaders);
        }
        [HttpGet("GetSearchPatDeatils")]
        public JsonResult GetSearchPatDeatils()
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            List<PatientInfo> lstResult = new List<PatientInfo>();
            List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                bool IsConnectedHIS = logins[0].IsConnectedHIS;

                if (IsConnectedHIS)
                    lstResult = _drugFreeSearchRepo.GetSearchPatDeatils();
                else
                    lstResult = _drugFreeSearchRepo.GetNonConnectedHISPatDeatils(HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetSearchPatDeatilsBySearchTearm")]
        public JsonResult GetSearchPatDeatilsBySearchTearm(string Search)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            List<PatientInfo> lstResult = new List<PatientInfo>();
            List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            try
            {
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string Fromdate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "00:00:00";
                string Todate = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + "23:59:59";
                bool IsConnectedHIS = logins[0].IsConnectedHIS;

                if (IsConnectedHIS)
                    lstResult = _drugFreeSearchRepo.GetSearchPatDeatilsBySearchTearm(Search);
                else
                    lstResult = _drugFreeSearchRepo.GetNonConnectedHISPatDeatilsBySearch(HospitalID, Search);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpPost("BillHoldSave")]
        public JsonResult BillHoldSave([FromBody] DispanseInfo dispanseInfo)
        {
            BillHoldHeaderInfo billHoldHeaderInfo = new BillHoldHeaderInfo()
            {
                PatientID = dispanseInfo.PatinetID,
                Name = dispanseInfo.Lis_PatFirstName,
                Gender = dispanseInfo.Lis_Sex,
                Doctor = dispanseInfo.LIS_RefDRNAME,
                Realtion = dispanseInfo.HIS_RELATIONNAME,
                MobileNo = dispanseInfo.HIS_PHONE,
                HospitalID = HttpContext.Session.GetString("Hospitalid").ToString(),
                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                StoreName = dispanseInfo.StoreName,
                HoldNumber = Common.CommonSetting.generateStudyID(),
                Age = dispanseInfo.Lis_AgeYear+"Y "+dispanseInfo.Lis_AgeMonth+"M "+dispanseInfo.Lis_AgeDays+"D",
                CreateDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            };
            long HeaderSeqID = _billHoldRepo.CreateNewBillHoldHeader(billHoldHeaderInfo);
            if (HeaderSeqID > 0)
            {
                for (int count = 0; count < dispanseInfo.DrugInfos.Count; count++)
                {
                    BillHoldDeatilsInfo billHoldDeatilsInfo = new BillHoldDeatilsInfo()
                    {
                        HoldSeqID = HeaderSeqID,
                        DrugCode = dispanseInfo.DrugInfos[count].PH_CUR_DRUGCODE,
                        Batch = dispanseInfo.DrugInfos[count].PH_CUR_STOCK_BATCHNO,
                        Qty = dispanseInfo.DrugInfos[count].Qty
                    };
                    long Result = _billHoldRepo.CreateNewBillHoldDeatils(billHoldDeatilsInfo);
                }
            }
            return Json(HeaderSeqID);
        }

        [HttpGet("GetHoldBillHeader")]
        public JsonResult GetHoldBillHeader(string StoreName)
        {
            List<BillHoldHeaderInfo> lstResult = new List<BillHoldHeaderInfo>();
            try
            {
                long HosPitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _billHoldRepo.GetHoldBillHeader(HosPitalID, StoreName);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetHoldBillDetailsBySeqID")]
        public JsonResult GetHoldBillDetailsBySeqID(long SeqID)
        {
            List<BillHoldDeatilsInfo> billDEatilsInfo = new List<BillHoldDeatilsInfo>();
            try
            {
                billDEatilsInfo = _billHoldRepo.GetHoldBillDetailsBySeqID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(billDEatilsInfo);
        }

        [HttpPost("CreateStorePinDeatils")]
        public int CreateStorePinDeatils(string StoreName, long StoreCode)
        {
            List<DispensePin> lstResult = new List<DispensePin>();
            int Result = 0;
            int UserId = 0;
            long seqId = 0;


            try
            {
                long userSeqid = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                lstResult = _cashBillRepo.GetStorePinDeatilsByUserId(userSeqid);

                //lstResult = _cashBillRepo.GetStorePinDeatils();
                //for (int i = 0; i < lstResult.Count(); i++)
                //{
                //if (userSeqid == lstResult[i].UserSeqId)
                //{
                //}

                //}
                //if (Result != 1)

                if (lstResult.Count > 0)
                {
                    Result = _cashBillRepo.UpdateStorePinDeatils(userSeqid, StoreName, StoreCode);
                }
                else
                {
                    Result = _cashBillRepo.CreateStorePinDeatils(userSeqid, StoreName, StoreCode);
                }

            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Result;
        }
        [HttpGet("GetStorePinDeatilsByUserId")]
        public List<DispensePin> GetStorePinDeatilsByUserId()
        {
            List<DispensePin> lstResult = new List<DispensePin>();
            try
            {

                long userSeqid = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                lstResult = _cashBillRepo.GetStorePinDeatilsByUserId(userSeqid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;


        }
        [HttpGet("GetPatDeatilsByMobileNo")]
        public JsonResult GetPatDeatilsByMobileNo(long MobileNo)
        {
            List<PatientDeatils> PatientDetails = new List<PatientDeatils>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                PatientDetails = _cashBillRepo.GetPatDeatilsByMobileNo(MobileNo, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(PatientDetails);
        }
        [HttpGet("GetDispenseBillBySearch")]
        public List<BillHeader> GetDispenseBillBySearch(string BillNo)
        {
            List<BillHeader> lstResult = new List<BillHeader>();
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _cashBillRepo.GetDispenseBillBySearch(HospitalId, BillNo);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return lstResult;
        }

        [HttpGet("GetTreatmentHeaderByPatientID")]
        public List<PatTreatmentHeader> GetTreatmentHeaderByPatientID(string PatientID)
        {
            List<PatTreatmentHeader> lstResult = new List<PatTreatmentHeader>();
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _drugFreeSearchRepo.GetTreatmentHeaderByPatientID(PatientID, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return lstResult;
        }
        [HttpGet("GetPatientDrugDeatilsBySeqID")]
        public List<PatientDrugDeatils> GetPatientDrugDeatilsBySeqID(long PHSeqID)
        {
            List<PatientDrugDeatils> lstResult = new List<PatientDrugDeatils>();
            try
            {
                lstResult = _drugFreeSearchRepo.GetPatientDrugDeatilsBySeqID(PHSeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return lstResult;
        }

        #region Habib
        [HttpGet("DeleteHoldBillHeader")]
        public bool DeleteHoldBillHeader(long SeqID)
        {
            try
            {
                int result = _billHoldRepo.DeleteHoldBillHeader(SeqID);
                if (result > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return false;
        }
        #endregion

        #region ABdullah
        [HttpGet("GetDispnesePurchaseExpiry")]
        public string GetDispnesePurchaseExpiry(long WareHouse)
        {
            string retValue = "";
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                retValue = _cashBillRepo.GetDispnesePurchaseExpiry(HospitalId, WareHouse);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return retValue;
        }
        #endregion
        #region Selvendiran
        [HttpGet("GetDispenseBillBySearchAllCondtion")]
        public List<BillHeader> GetDispenseBillBySearchAllCondtion(string SearchTearm)
        {
            List<BillHeader> lstResult = new List<BillHeader>();
            try
            {
                long HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _cashBillRepo.GetDispenseBillBySearchAllCondtion(HospitalId,SearchTearm);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return lstResult;
        }
        [HttpGet("GetHoldBillHeaderSearch")]
        public JsonResult GetHoldBillHeaderSearch(string StoreName,string SearchTearm)
        {
            List<BillHoldHeaderInfo> lstResult = new List<BillHoldHeaderInfo>();
            try
            {
                long HosPitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _billHoldRepo.GetHoldBillHeaderSearch(HosPitalID, StoreName, SearchTearm);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetExpiredDrugByStore")]
        public List<CurrentStockInfo> GetExpiredDrugByStore(string StoreName)
        {
            List<CurrentStockInfo> lstResult = new List<CurrentStockInfo>();
            try
            {
                int  HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _currentStockRepo.GetExpiredDrugByStore(StoreName,HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return lstResult;
        }
        [HttpGet("GetLast100CanceledBills")]
        public JsonResult GetLast100CanceledBills(string StoreName)
        {
            List<BillHeader> lstResult = new List<BillHeader>();
            try
            {
                long HosPitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _cashBillRepo.GetLast100CanceledBills(HosPitalID, StoreName);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetLast100CanceledBillsByDate")]
        public JsonResult GetLast100CanceledBillsByDate(string StoreName,string FromDt,string ToDt)
        {
            DateTime frmdattm = DateTime.ParseExact(FromDt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todatm = DateTime.ParseExact(ToDt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string frmdat = frmdattm.ToString("yyyy-MM-dd 00:00:00");
            string todat = todatm.ToString("yyyy-MM-dd 23:59:59");
            List<BillHeader> lstResult = new List<BillHeader>();
            try
            {
                long HosPitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _cashBillRepo.GetLast100CanceledBillsByDate(HosPitalID, StoreName, frmdat, todat);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        #endregion
    }
}                                                                                                                                                                
