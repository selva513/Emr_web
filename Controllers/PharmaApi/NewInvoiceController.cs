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

namespace Emr_web.Controllers.PharmaApi
{
    [Produces("application/json")]
    [Route("Pharma/Invoice")]
    [ApiController]
    public class NewInvoiceController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IDrugFreeSearchRepo _drugFreeSearchRepo;
        private readonly INewInvoiceRepo _newInvoiceRepo;
        private readonly ICashBillRepo _cashBillRepo;
        public NewInvoiceController(IDBConnection iDBConnection, IErrorlog errorlog, IDrugFreeSearchRepo drugFreeSearchRepo, INewInvoiceRepo newInvoiceRepo, ICashBillRepo cashBillRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _drugFreeSearchRepo = drugFreeSearchRepo;
            _newInvoiceRepo = newInvoiceRepo;
            _cashBillRepo = cashBillRepo;
        }
        [HttpGet("GetDrugFromDrugMaster")]
        public JsonResult GetDrugFromDrugMaster(string SearchTearm)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugFreeSearch> lstResult = new List<DrugFreeSearch>();
            try
            {
                lstResult = _drugFreeSearchRepo.GetDrugFromDrugMaster(SearchTearm, HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetAllSupplierMaster")]
        public JsonResult GetAllSupplierMaster()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SupplierInfo> lstResult = new List<SupplierInfo>();
            try
            {
                lstResult = _newInvoiceRepo.GetAllSupplierMaster(HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }

        [HttpGet("GetStoreName")]
        public JsonResult GetStoreName()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<StoreNameInfo> lstResult = new List<StoreNameInfo>();
            try
            {
                lstResult = _newInvoiceRepo.GetStoreName(HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpPost("DraftSave")]
        public JsonResult DraftSave([FromBody] DraftInfo draftInfo)
        {
            DateTime ExDate = DateTime.Now;
            DateTime RunningExpiryDate = GetDataformat(draftInfo.ExpiryDate, ExDate);
            DateTime SupDate = DateTime.Now;
            DateTime SupplierInvoiceDate = GetDataformat(draftInfo.SupplierInvoiceDate, SupDate);
            List<DraftInfoHeader> lstHeader = new List<DraftInfoHeader>();
            List<DraftDeatils> lstDeatils = new List<DraftDeatils>();
            List<DraftInvoiceSummary> lstSummary = new List<DraftInvoiceSummary>();
            List<InvoiceTotalSum> lstTotal = new List<InvoiceTotalSum>();
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"));
                long HeaderSeqID = _newInvoiceRepo.IsExitsSupplierInvoiceNum(draftInfo.SupplierInvoiceNumber,HospitalId);
                if (HeaderSeqID > 0)
                {
                    long draftSeqID = _newInvoiceRepo.IsExitsDrugByInvoice(HeaderSeqID, draftInfo.DrugCode,draftInfo.Batch);
                    if (draftSeqID > 0)
                    {
                        long DelSeqID = _newInvoiceRepo.DeleteDraftDtlByDrugCode(draftSeqID,draftInfo.DrugCode,draftInfo.Batch);
                        DraftDeatils draftDeatils = new DraftDeatils
                        {
                            DraftSeqID = HeaderSeqID,
                            DrugName = draftInfo.DrugName,
                            DrugCode = draftInfo.DrugCode,
                            Batch = draftInfo.Batch,
                            ExpiryDate = RunningExpiryDate.ToString("yyyy-MM-dd"),
                            Qty = draftInfo.Qty,
                            FreeQty = draftInfo.FreeQty,
                            Discount = draftInfo.Discount,
                            Rate = draftInfo.Rate,
                            MRP = draftInfo.MRP,
                            Tax = draftInfo.TaxAmount,
                            TaxPrecentage = draftInfo.Tax,
                            Amount = draftInfo.NetAmount - draftInfo.TaxAmount,
                            NetAmount = draftInfo.NetAmount
                        };
                        long dtlSeqID = _newInvoiceRepo.CreateNewDraftDeatils(draftDeatils);
                    }
                    else
                    {
                        DraftDeatils draftDeatils = new DraftDeatils
                        {
                            DraftSeqID = HeaderSeqID,
                            DrugName = draftInfo.DrugName,
                            DrugCode = draftInfo.DrugCode,
                            Batch = draftInfo.Batch,
                            ExpiryDate = RunningExpiryDate.ToString("yyyy-MM-dd"),
                            Qty = draftInfo.Qty,
                            FreeQty = draftInfo.FreeQty,
                            Discount = draftInfo.Discount,
                            Rate = draftInfo.Rate,
                            MRP = draftInfo.MRP,
                            Tax = draftInfo.TaxAmount,
                            TaxPrecentage = draftInfo.Tax,
                            Amount = draftInfo.NetAmount - draftInfo.TaxAmount,
                            NetAmount = draftInfo.NetAmount
                        };
                        long dtlSeqID = _newInvoiceRepo.CreateNewDraftDeatils(draftDeatils);
                    }
                }
                else
                {
                    DraftInfoHeader draftInfoHeader = new DraftInfoHeader
                    {
                        SupplierInvoiceNumber = draftInfo.SupplierInvoiceNumber,
                        SupplierID = draftInfo.SupplierID,
                        SupplierInvoiceDate = SupplierInvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        InVoiceTaxType = draftInfo.InVoiceTaxType,
                        InvoiceType = draftInfo.InvoiceType,
                        Amount = 0,
                        Tax = 0,
                        NetAmount = 0,
                        TaxPrecentage = 0,
                        Discount = 0,
                        WareHouse = draftInfo.WareHouse,
                        CreatedDatime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"))
                    };
                    HeaderSeqID = _newInvoiceRepo.CreateNewDraftHeader(draftInfoHeader);
                    if (HeaderSeqID > 0)
                    {
                        DraftDeatils draftDeatils = new DraftDeatils
                        {
                            DraftSeqID = HeaderSeqID,
                            DrugName = draftInfo.DrugName,
                            DrugCode = draftInfo.DrugCode,
                            Batch = draftInfo.Batch,
                            ExpiryDate = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Qty = draftInfo.Qty,
                            FreeQty = draftInfo.FreeQty,
                            Discount = draftInfo.Discount,
                            Rate = draftInfo.Rate,
                            MRP = draftInfo.MRP,
                            Tax = draftInfo.TaxAmount,
                            TaxPrecentage = draftInfo.Tax,
                            Amount = draftInfo.NetAmount - draftInfo.TaxAmount,
                            NetAmount = draftInfo.NetAmount
                        };
                        long dtlSeqID = _newInvoiceRepo.CreateNewDraftDeatils(draftDeatils);
                    }
                }
                lstHeader = _newInvoiceRepo.GetDraftBillSeqID(HeaderSeqID);
                lstDeatils = _newInvoiceRepo.GetDraftDeatailsBy(HeaderSeqID);
                lstSummary = _newInvoiceRepo.GetInvoiceBillGroupByTax(HeaderSeqID);
                lstTotal = _newInvoiceRepo.GetInvoiceTotalSumBySeID(HeaderSeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Header = lstHeader, Deatils = lstDeatils, Summary = lstSummary, Total = lstTotal });
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
        [HttpGet("GetDrugByDrugCode")]
        public JsonResult GetDrugByDrugCode(int DrugCode)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugMaster> lstResult = new List<DrugMaster>();
            try
            {
                lstResult = _newInvoiceRepo.GetDrugByDrugCode(DrugCode, HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetTop50DraftBill")]
        public JsonResult GetTop50DraftBill()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DraftInfoHeader> lstResult = new List<DraftInfoHeader>();
            try
            {
                lstResult = _newInvoiceRepo.GetTop50DraftBill(HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetSelectedDraftBySeqID")]
        public JsonResult GetSelectedDraftBySeqID(long SeqID)
        {
            List<DraftInfoHeader> lstHeader = new List<DraftInfoHeader>();
            List<DraftDeatils> lstDeatils = new List<DraftDeatils>();
            List<DraftInvoiceSummary> lstSummary = new List<DraftInvoiceSummary>();
            List<InvoiceTotalSum> lstTotal = new List<InvoiceTotalSum>();
            try
            {
                lstHeader = _newInvoiceRepo.GetDraftBillSeqID(SeqID);
                lstDeatils = _newInvoiceRepo.GetDraftDeatailsBy(SeqID);
                lstSummary = _newInvoiceRepo.GetInvoiceBillGroupByTax(SeqID);
                lstTotal = _newInvoiceRepo.GetInvoiceTotalSumBySeID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Header = lstHeader, Deatils = lstDeatils, Summary = lstSummary, Total = lstTotal });
        }
        [HttpGet("DeleteDraftDtlBySeqID")]
        public JsonResult DeleteDraftDtlBySeqID(long SeqID)
        {
            long Result = 0;
            try
            {
                Result = _newInvoiceRepo.DeleteDraftDtlBySeqID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(Result);
        }
        [HttpPost("InvoiceSave")]
        public JsonResult InvoiceSave([FromBody] InvoiceInfo invoiceInfo)
        {
            string StatusText = "";
            string storeName = invoiceInfo.WareHouse;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            long SupplierSeqID = _newInvoiceRepo.IsSupplierInvoiceExist(invoiceInfo.SupplierInvoiceNumber,HospitalId);
            if (SupplierSeqID <= 0)
            {
                try
                {
                    if (invoiceInfo.DraftDeatils.Length > 0)
                    {
                        for (int count = 0; count < invoiceInfo.DraftDeatils.Length; count++)
                        {
                            int DrugCode = invoiceInfo.DraftDeatils[count].DrugCode;
                            string Batch = invoiceInfo.DraftDeatils[count].Batch;
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
                                int Qty = invoiceInfo.DraftDeatils[count].Qty;
                                int FreeQty= invoiceInfo.DraftDeatils[count].FreeQty;
                                int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty + FreeQty;
                                string Remarks = "DirectInvoice";
                                string Reference = invoiceInfo.SupplierInvoiceNumber;
                                int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                StockMovement stockMovement = new StockMovement
                                {
                                    PH_RUN_PROCESSIDKEY = "Invoice",
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
                            else
                            {
                                long CurrentStockID = _newInvoiceRepo.GetCurrentStockSeqID();
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                CurrentStockInfo currentStockInfo = new CurrentStockInfo
                                {
                                    PH_CUR_SEQID = (int)CurrentStockID,
                                    PH_CUR_DRUGCODE = invoiceInfo.DraftDeatils[count].DrugCode,
                                    PH_CUR_STOCK_BATCHNO = invoiceInfo.DraftDeatils[count].Batch,
                                    PH_CUR_OLDDRUGCODE = "",
                                    PH_CUR_DRUGBRANDNAME = invoiceInfo.DraftDeatils[count].DrugName,
                                    PH_CUR_OPSEQID = 0,
                                    PH_CUR_STOCK = invoiceInfo.DraftDeatils[count].Qty + invoiceInfo.DraftDeatils[count].FreeQty,
                                    PH_CUR_STOCKUOM = "",
                                    PH_CUR_STOCK_INLOCK = 0,
                                    PH_CUR_STOCK_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                    PH_CUR_STOCK_PURCHCOST = invoiceInfo.DraftDeatils[count].Rate,
                                    PH_CUR_STOCK_BILLINGPRICE = invoiceInfo.DraftDeatils[count].MRP,
                                    PH_CUR_STOCKISZERO = false,
                                    PH_CUR_STOCK_STORENAME = storeName,
                                    PH_CUR_LAST_PROCESSKEY = "Invoice",
                                    PH_CUR_LAST_TRANSID = "",
                                    PH_CUR_isROWACTIVE = true,
                                    PH_CUR_LAST_TRANSDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    remarks = "DirectInvoice",
                                    HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                                    Reference= invoiceInfo.SupplierInvoiceNumber
                                };
                                long CurrentsockInsert = _newInvoiceRepo.CreateNewStock(currentStockInfo);
                                if (CurrentsockInsert > 0)
                                {

                                    long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                                    int Qty = invoiceInfo.DraftDeatils[count].Qty;
                                    int CurrentStock = Qty;
                                    StockMovement stockMovement = new StockMovement
                                    {
                                        PH_RUN_PROCESSIDKEY = "PHBYORD",
                                        PH_RUN_STORENAME = storeName,
                                        PH_RUN_DRUGCODE = DrugCode,
                                        PH_RUN_STOCK_TRANSACTVALUE = invoiceInfo.DraftDeatils[count].Qty,
                                        PH_RUN_STOCK_AFTERTRANSACT = invoiceInfo.DraftDeatils[count].Qty,
                                        PH_RUN_STOCK_LEFTOUTINBATCH = invoiceInfo.DraftDeatils[count].Qty,
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
                        }
                        DateTime SupDate = DateTime.Now;
                        DateTime SupplierInvoiceDate = GetDataformat(invoiceInfo.SupplierInvoiceDate, SupDate);
                        InvoiceHeader invoiceHeader = new InvoiceHeader
                        {
                            PH_IN_DOCNO = invoiceInfo.SupplierInvoiceNumber,
                            PH_IN_SUPID = invoiceInfo.SupplierID,
                            PH_IN_POREFNO = "",
                            PH_IN_UniqueKey = "",
                            PH_IN_ENTRYDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                            PH_IN_ITEMCOUNT = invoiceInfo.DraftDeatils.Length,
                            PH_IN_ISTOINVOICEDONE = true,
                            PH_IN_INVOiCENO = 0,
                            PH_IN_SUPP_INVNO = invoiceInfo.SupplierInvoiceNumber,
                            PH_IN_INV_CONVERTDT = DateTime.Now.ToString("yyyy-MM-dd"),
                            PH_IN_TOTAL_AMOUNT = invoiceInfo.NetAmount,
                            PH_IN_CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            PH_IN_ModifiedBy = HttpContext.Session.GetString("Userseqid"),
                            PH_IN_ModifiedDTtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            PH_INReceivedBy = invoiceInfo.WareHouse,
                            PH_INWareHouse = invoiceInfo.WareHouse,
                            Amount = invoiceInfo.Amount,
                            Tax = invoiceInfo.Tax,
                            NetAmount = invoiceInfo.NetAmount,
                            HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid")),
                            DisType = invoiceInfo.DisType,
                            DiscountValue = invoiceInfo.DiscountValue,
                            DiscountAmt = invoiceInfo.Discount,
                            InvoiceType = invoiceInfo.InvoiceType,
                        
                            InvoiceGSTType = invoiceInfo.InVoiceTaxType
                        };
                        long InvoiceHeaderSeqID = _newInvoiceRepo.CreateNewInvoiceHeader(invoiceHeader);
                        if (InvoiceHeaderSeqID > 0)
                        {
                            for (int count = 0; count < invoiceInfo.DraftDeatils.Length; count++)
                            {
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(invoiceInfo.DraftDeatils[count].ExpiryDate, ExDate);
                                InvoiceDeatils invoiceDeatils = new InvoiceDeatils
                                {
                                    PH_IN_SEQID = InvoiceHeaderSeqID,
                                    PH_INDTL_DOCNO = "",
                                    PH_INDTL_DRUGCODE = invoiceInfo.DraftDeatils[count].DrugCode,
                                    PH_INDTL_DRUGUOM = "",
                                    PH_INDTL_DRUGBATCHNO = invoiceInfo.DraftDeatils[count].Batch,
                                    PH_INDTL_DRUGEXPIRY = RunningExpiryDate.ToString("yyyy-MM-dd"),
                                    PH_INDTL_RECVDQTY = invoiceInfo.DraftDeatils[count].Qty,
                                    PH_INDTL_BONUSQTY = invoiceInfo.DraftDeatils[count].FreeQty,
                                    PH_INDTL_RATEEACH = invoiceInfo.DraftDeatils[count].Rate,
                                    PH_INDTL_AMOUNT = invoiceInfo.DraftDeatils[count].NetAmount,
                                    PH_INDTL_MOVEDTO_CSTOCK = true,
                                    PH_INDTL_POBYREF = "",
                                    PH_INDTL_ISACTIVE = true,
                                    GST = invoiceInfo.DraftDeatils[count].TaxPrecentage,
                                    PurchaseCost = invoiceInfo.DraftDeatils[count].Rate,
                                    BillCost = invoiceInfo.DraftDeatils[count].MRP,
                                    PurAmount = invoiceInfo.DraftDeatils[count].Amount,
                                    PurTaxAmount = invoiceInfo.DraftDeatils[count].Tax,
                                    PurNetAmount = invoiceInfo.DraftDeatils[count].NetAmount
                                };
                                long InvoiceDtlSeqID = _newInvoiceRepo.NewInvoiceDeatils(invoiceDeatils);
                            }
                        }
                        string inVoiceNumber = invoiceInfo.SupplierInvoiceNumber;
                        long DraftHeaderId = invoiceInfo.DraftHeaderId;
                        if(DraftHeaderId > 0)
                        {
                            long DraftDtlSeqID = _newInvoiceRepo.DeleteDraftHeaderDetail(DraftHeaderId);
                        }
                        else
                        {
                             DraftHeaderId = _newInvoiceRepo.GetDraftHeaderInvoiceNumber(inVoiceNumber);
                             long DraftDtlSeqID = _newInvoiceRepo.DeleteDraftHeaderDetail(DraftHeaderId);
                        }
                        StatusText = "Save Success";
                    }
                }
                catch (Exception ex)
                {
                    string ErrorMsg = ex.ToString();
                    StatusText = "Logical Error";
                    long DeletSuppInvoice = _newInvoiceRepo.DeleteSupplierInvoice(invoiceInfo.SupplierInvoiceNumber);
                }
            }
            else
            {
                StatusText = "Supplier Invoice Number Exists";
            }
            return Json(StatusText);
        }

        [HttpGet("GetTop50Invoice")]
        public JsonResult GetTop50Invoice()
        {
            List<InvoiceHeader> lstResult = new List<InvoiceHeader>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _newInvoiceRepo.GetTop50Invoice(HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetInvoiceDrugDeatils")]
        public JsonResult GetInvoiceDrugDeatils(long SeqID)
        {
            List<InvoiceDeatils> lstResult = new List<InvoiceDeatils>();
            List<InvoiceDtlSummary> lstDtlSummary = new List<InvoiceDtlSummary>();
            try
            {
                lstResult = _newInvoiceRepo.GetInvoiceDrugDeatils(SeqID);
                lstDtlSummary = _newInvoiceRepo.GetInvoiceDeatilsSummaryBySeqID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { Deatils = lstResult, Summary = lstDtlSummary });
        }

        [HttpGet("DeleteInvoice")]
        public JsonResult DeleteInvoice(long SeqID, string StoreName, string SupplierInvoiceNo)
        {
            string StatusText = "";
            string storeName = StoreName;
            List<InvoiceDeatils> lstResult = new List<InvoiceDeatils>();
            try
            {
                lstResult = _newInvoiceRepo.GetInvoiceDrugDeatils(SeqID);
                if (lstResult.Count > 0)
                {
                    for (int count = 0; count < lstResult.Count; count++)
                    {
                        int DrugCode = lstResult[count].PH_INDTL_DRUGCODE;
                        string Batch = lstResult[count].PH_INDTL_DRUGBATCHNO;
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
                            int Qty = lstResult[count].PH_INDTL_RECVDQTY + lstResult[count].PH_INDTL_BONUSQTY;
                            int CurrentStock = currentStockTimes[0].PH_CUR_STOCK - Qty;
                            string Remarks = "Cancel DirectInvoice";
                            string Reference = SupplierInvoiceNo;
                            int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                            DateTime ExDate = DateTime.Now;
                            DateTime RunningExpiryDate = GetDataformat(lstResult[count].PH_INDTL_DRUGEXPIRY, ExDate);
                            StockMovement stockMovement = new StockMovement
                            {
                                PH_RUN_PROCESSIDKEY = "INCancel",
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
                }
                long DeletSuppInvoice = _newInvoiceRepo.DeleteSupplierInvoice(SupplierInvoiceNo);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(StatusText);
        }
        [HttpGet("GetSupplierTop50Invoice")]
        public JsonResult GetSupplierTop50Invoice(long SupplierID)
        {
            List<InvoiceHeader> lstResult = new List<InvoiceHeader>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _newInvoiceRepo.GetSupplierTop50Invoice(SupplierID, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        #region Habib
        [HttpGet("GetInvoiceDetailsById")]
        public JsonResult GetInvoiceDetailsById(long SeqID)
        {
            List<InvoiceHeader> lstHdrResult = new List<InvoiceHeader>();
            List<InvoiceDeatils> lstDtlResult = new List<InvoiceDeatils>();
            List<InvoiceDtlSummary> lstDtlSummary = new List<InvoiceDtlSummary>();
            try
            {
                lstHdrResult = _newInvoiceRepo.GetInvoiceHeaderBySeqID(SeqID);
                lstDtlResult = _newInvoiceRepo.GetInvoiceDrugDeatils(SeqID);
                lstDtlSummary = _newInvoiceRepo.GetInvoiceDeatilsSummaryBySeqID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { lstHdr = lstHdrResult, lstDtl = lstDtlResult, DtlSummary = lstDtlSummary });
        }

        [HttpGet("InvoiceBySupplierId")]
        public JsonResult InvoiceBySupplierId(long SeqID)
        {
            List<InvoiceHeader> lstInvoice = new List<InvoiceHeader>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstInvoice = _newInvoiceRepo.GetInvoiceBySuppSeqID(SeqID, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("InvoiceBySupplierId " + ex.ToString());
            }
            return Json(lstInvoice);
        }

        [HttpPost("InvoiceReturnSave")]
        public JsonResult InvoiceReturnSave([FromBody] InvoiceReturnHeader invoiceInfo)
        {
            string StatusText = "";
            string storeName = invoiceInfo.Ph_Ret_StoreName;
            string UserName = HttpContext.Session.GetString("UserName");
            try
            {
                if (invoiceInfo.InvoiceReturnDetails.Length > 0)
                {
                    for (int count = 0; count < invoiceInfo.InvoiceReturnDetails.Length; count++)
                    {
                        string Status = invoiceInfo.InvoiceReturnDetails[count].IsChanged;
                        if (Status.Equals("true"))
                        {
                            int DrugCode = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_DrugCode;
                            string Batch = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_Batch;
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
                                int Qty = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_Qty;
                                int CurrentStock = currentStockTimes[0].PH_CUR_STOCK - Qty;
                                string Remarks = "Invoice Return";
                                string Reference = invoiceInfo.Ph_Ret_SeqID.ToString();
                                int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Reference);
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_ExpiryDate, ExDate);
                                StockMovement stockMovement = new StockMovement
                                {
                                    PH_RUN_PROCESSIDKEY = "Invoice",
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
                    }
                    DateTime SupDate = DateTime.Now;
                    DateTime SupplierInvoiceDate = GetDataformat(invoiceInfo.Ph_Ret_DateTime, SupDate);
                    InvoiceReturnHeader invoiceRetHeader = new InvoiceReturnHeader
                    {
                        Ph_Ret_InvoiceNo = invoiceInfo.Ph_Ret_InvoiceNo,
                        Ph_Ret_SupplierID = invoiceInfo.Ph_Ret_SupplierID,
                        Ph_Ret_Processkey = "",
                        Ph_Ret_StoreName = invoiceInfo.Ph_Ret_StoreName,
                        Ph_Ret_UserName = UserName,
                        Ph_Ret_Comments = invoiceInfo.Ph_Ret_Comments,
                        Ph_Stock_ReturnType = invoiceInfo.Ph_Stock_ReturnType,
                        Ph_Ret_DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Ph_Ret_CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Ph_Ret_CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        Ph_Ret_IsActive = true,
                        HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                        Ph_Ret_Amount = invoiceInfo.InvoiceReturnDetails.Where(x => x.IsChanged == "true").Select(x => x.Ph_Ret_RowAmt).Sum(),
                        Ph_Ret_Tax= invoiceInfo.InvoiceReturnDetails.Where(x => x.IsChanged == "true").Select(x => x.Ph_Ret_RowTax).Sum(),
                        Ph_Ret_NetAmt= invoiceInfo.InvoiceReturnDetails.Where(x => x.IsChanged == "true").Select(x => x.Ph_Ret_RowNetAmt).Sum()
                    };
                    long InvoiceHeaderSeqID = _newInvoiceRepo.CreateNewInvoiceReturnHeader(invoiceRetHeader);
                    if (InvoiceHeaderSeqID > 0)
                    {
                        for (int count = 0; count < invoiceInfo.InvoiceReturnDetails.Length; count++)
                        {
                            string Status = invoiceInfo.InvoiceReturnDetails[count].IsChanged;
                            if (Status.Equals("true"))
                            {
                                DateTime ExDate = DateTime.Now;
                                DateTime RunningExpiryDate = GetDataformat(invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_ExpiryDate, ExDate);
                                InvoiceReturnDetails invoiceRetDetails = new InvoiceReturnDetails
                                {
                                    Ph_Ret_SeqID = InvoiceHeaderSeqID,
                                    Ph_Ret_DrugCode = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_DrugCode,
                                    Ph_Ret_BrandName = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_BrandName,
                                    Ph_Ret_Batch = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_Batch,
                                    Ph_Ret_Cat_Name = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_Cat_Name,
                                    Ph_Ret_ExpiryDate = RunningExpiryDate.ToString("yyyy-MM-dd"),
                                    Ph_Ret_Qty = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_Qty,
                                    Ph_Ret_CreatedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Ph_Ret_EachRate = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_EachRate,
                                    Ph_Ret_RowAmt = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_RowAmt,
                                    Ph_Ret_RowTax = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_RowTax,
                                    Ph_Ret_RowNetAmt = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_RowNetAmt,
                                    Ph_Ret_ReminQty = invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_ReminQty,
                                    Ph_Ret_GST= invoiceInfo.InvoiceReturnDetails[count].Ph_Ret_GST
                                };
                                long InvoiceDtlSeqID = _newInvoiceRepo.CreateNewInvoiceReturnDeatils(invoiceRetDetails);
                            }
                        }
                    }
                    StatusText = "Save Success";
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
                StatusText = "Logical Error";
                //long DeletSuppInvoice = _newInvoiceRepo.DeleteSupplierInvoice(invoiceInfo.Ph_Ret_InvoiceNo);
            }

            return Json(StatusText);
        }
        #endregion
        [HttpGet("UpdatePackQtyINDrugMaster")]
        public JsonResult UpdatePackQtyINDrugMaster(int DrugCode,int PackQty)
        {
            string Result = "";
            try
            {
                int UpateResult = _cashBillRepo.UpdatePackQtyINDrugMaster(DrugCode, PackQty);
                if (UpateResult > 0)
                    Result = "Save Success";
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(Result);
        }
        [HttpGet("UpdateGSTDrugMasterByCode")]
        public JsonResult UpdateGSTDrugMasterByCode(int DrugCode, decimal GST)
        {
            string Result = "";
            try
            {
                int UpateResult = _cashBillRepo.UpdateGSTDrugMasterByCode(DrugCode, GST);
                if (UpateResult > 0)
                    Result = "Save Success";
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(Result);
        }
        #region Abdullah
        [HttpGet("GetStateDeatilsForGst")]
        public int GetStateDeatilsForGst(int SupplierId)
        {
            int Result = 0;
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                Result = _newInvoiceRepo.GetStateDeatilsForGst(SupplierId, HospitalId);
               
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Result;
        }
        [HttpGet("GetInvoiceNumberDetailsBySearch")]
        public List<InvoiceHeader> GetInvoiceNumberDetailsBySearch(string Search, long SupplierId)
        {
            List<InvoiceHeader> lstResult = new List<InvoiceHeader>();
            try
            {
                if (Search == null)
                {
                    Search = "";
                }
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _newInvoiceRepo.GetInvoiceNumberDetailsBySearch(HospitalId, Search, SupplierId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return lstResult;
        }
        [HttpGet("GetPurchaseExpiryDays")]
        public string GetPurchaseExpiryDays( long wareHouse)
        {
            string retValue = "";
            try
            {
                
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retValue = _newInvoiceRepo.GetPurchaseExpiryDays(HospitalId, wareHouse);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return retValue;
        }
        #endregion
    }
}