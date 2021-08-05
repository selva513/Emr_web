using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Domain;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Emr_web.Common;
using System.Globalization;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Area("Pharmacy")]
    [Route("api/PurchaseApi")]
    [ApiController]
    public class PurchaseApiController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IPurchaseRepo _IPurchaseRepo;
        private readonly IDrugMastersRepo _IDrugMasterRepo;
        private readonly IPurchaseOrderRepo _purchaseOrderRepo;
        public PurchaseApiController(IDBConnection dBConnection, IErrorlog errorlog, IPurchaseRepo purchaseRepo, IDrugMastersRepo drugMasterRepo,IPurchaseOrderRepo purchaseOrderRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _IPurchaseRepo = purchaseRepo;
            _IDrugMasterRepo = drugMasterRepo;
            _purchaseOrderRepo = purchaseOrderRepo;
        }
        [HttpPost("GetDrugDeatilsForpurchaseOrder")]
        public List<DrugMaster> GetDrugDeatilsForpurchaseOrder(long[] DrugPurchase)
         {
            List<DrugMaster> lstResult = new List<DrugMaster>();
            try
            {
                for (long i = 0; i < DrugPurchase.Length; i++)
                {
                    long PH_ITEM_DrugCode = DrugPurchase[i];

                lstResult.AddRange(_IDrugMasterRepo.GetDrugMasterDetailsByDrugCode(PH_ITEM_DrugCode));
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;

         }
        [HttpPost("InsertPurchaseOrder")]
        public long InsertPurchaseOrder(List<PurchaseOrder> purchaseOrders)
        {

            long retvalue = 0;
            long HretValue = 0;

            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                string ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);


                HretValue = _IPurchaseRepo.CreatePurchaseOrderHDR(purchaseOrders[0], CreatedDatetime);
                long PH_ITEM_HEADERID = HretValue;
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                for (int i = 0; i < purchaseOrders.Count(); i++)
                {
                    retvalue = _IPurchaseRepo.InsertPurchaseOrder(purchaseOrders[i], CreatedDatetime, PH_ITEM_HEADERID, Hospitalid);
                }
                


            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return retvalue;
        }
        [HttpGet("GetSupplierNameBySearch")]
        public List<PurchaseOrder> GetSupplierNameBySearch(string Search)
        {
            List<PurchaseOrder> lstResult = new List<PurchaseOrder>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IPurchaseRepo.GetSupplierNameBySearch(Search, Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetSupplierDataBySupplierName")]
        public JsonResult GetSupplierDataBySupplierName(string SupplierName,long PH_ITEM_HEADERID)
        {
            List<PurchaseOrder> PurchaselstResult = new List<PurchaseOrder>();
            List<PurchaseOrder> UpdatelstResult = new List<PurchaseOrder>();
            List<DrugMaster> DruglstResult = new List<DrugMaster>();
            try
            {
                UpdatelstResult = _IPurchaseRepo.GetPurchaseDetailsbyHeaderId(PH_ITEM_HEADERID);
                PurchaselstResult = _IPurchaseRepo.GetSupplierDataBySupplierName(SupplierName, PH_ITEM_HEADERID);
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                DruglstResult = _IDrugMasterRepo.GetDrugMasterDetailsByHospitalID(Hospitalid);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Result = PurchaselstResult , Update = UpdatelstResult,  lstResult = DruglstResult }) ;

        }
        [HttpGet("GetDrugDetailsBySearch")]
        public JsonResult GetDrugDetailsBySearch(string Search)
        {
            List<DrugMaster> DruglstResult = new List<DrugMaster>();
            List<PurchaseOrder> PurchaselstResult = new List<PurchaseOrder>();
            try
            {
                DruglstResult = _IPurchaseRepo.GetDrugDetailsBySearch(Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { lstResult = DruglstResult, Result = PurchaselstResult });
        }
        [HttpGet("GetPurchaseDetailsbyHeaderId")]
        public List<PurchaseOrder> GetPurchaseDetailsbyHeaderId(long PH_ITEM_HEADERID)
        {
            List<PurchaseOrder> lstResult = new List<PurchaseOrder>();

            try
            {

                lstResult = _IPurchaseRepo.GetPurchaseDetailsbyHeaderId(PH_ITEM_HEADERID);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetPurchaseOrderByHospitalId")]
        public List<PurchaseActivities> GetPurchaseOrderByHospitalId()
        {
            List<PurchaseActivities> lstResult = new List<PurchaseActivities>();
            try
            {
                lstResult = _IPurchaseRepo.GetPurchaseOrderByHospitalId();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
       
        #region Selevendiran
        [HttpGet("GetDrugMasterDetailsByHospitalID")]
        public JsonResult GetDrugMasterDetailsByHospitalID()
        {
            
            List<DrugMaster> DruglstResult = new List<DrugMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                DruglstResult = _IDrugMasterRepo.GetDrugMasterDetailsByHospitalID(Hospitalid);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { lstResult = DruglstResult });
        }
        [HttpGet("GetDrugBySearch")]
        public JsonResult GetDrugBySearch(string Search)
        {
            List<DrugMaster> DruglstResult = new List<DrugMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                DruglstResult = _IPurchaseRepo.GetDrugBySearch(Search, Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(DruglstResult);
        }

        [HttpGet("GetPurDrugByDrugCode")]
        public JsonResult GetPurDrugByDrugCode(int DrugCode)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DrugInfo> lstResult = new List<DrugInfo>();
            try
            {
                lstResult = _IPurchaseRepo.GetPurDrugByDrugCode(DrugCode, HospitalID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpPost("SavePurchaseOrder")]
        public JsonResult SavePurchaseOrder([FromBody] PurchaseOrderInfo purchaseOrderInfo)
        {
            List<PurchaseOrderHeader> lstHeader = new List<PurchaseOrderHeader>();
            List<PurchaseOrderDtl> lstDeatils = new List<PurchaseOrderDtl>();
            List<PurchaseOrderText> lstResult = new List<PurchaseOrderText>();
            List<ClientDeatilsInfo> lstClinet = new List<ClientDeatilsInfo>();
            try
            {
                if (purchaseOrderInfo.SeqID <= 0)
                {
                    DateTime SupDate = DateTime.Now;
                    DateTime SupplierInvoiceDate = GetDataformat(purchaseOrderInfo.DeliveryDate, SupDate);
                    if (purchaseOrderInfo.purchaseOrderDtlInfos.Length > 0)
                    {
                        PurchaseOrderHeader purchaseOrderHeader = new PurchaseOrderHeader
                        {
                            PurchaseOrderNumber = CommonSetting.GetPurchaseOrderNumber(),
                            SupplierID = purchaseOrderInfo.SupplierID,
                            TotalAmount = purchaseOrderInfo.TotalAmount,
                            TotalItem = purchaseOrderInfo.purchaseOrderDtlInfos.Length,
                            QuotationNo = purchaseOrderInfo.QuotationNo,
                            DeliveryDate = SupplierInvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            IsAprovedBy = purchaseOrderInfo.IsAprovedBy,
                            CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            WarehouseId = purchaseOrderInfo.WarehouseId,
                            WarehouseName = purchaseOrderInfo.WarehouseName
                        };
                        if (purchaseOrderInfo.IsAprovedBy)
                            purchaseOrderHeader.Status = "Approved";
                        else
                            purchaseOrderHeader.Status = "Not Approved";

                        long HeaderSeqID = _purchaseOrderRepo.CreateNewPurcahseOrder(purchaseOrderHeader);
                        if (HeaderSeqID > 0)
                        {
                            for (int count = 0; count < purchaseOrderInfo.purchaseOrderDtlInfos.Length; count++)
                            {
                                PurchaseOrderDtl purchaseOrderDtl = new PurchaseOrderDtl
                                {
                                    HDTSeqID = HeaderSeqID,
                                    DrugCode = purchaseOrderInfo.purchaseOrderDtlInfos[count].DrugCode,
                                    BrandName = purchaseOrderInfo.purchaseOrderDtlInfos[count].BrandName,
                                    GST = purchaseOrderInfo.purchaseOrderDtlInfos[count].GST,
                                    Cost = purchaseOrderInfo.purchaseOrderDtlInfos[count].Cost,
                                    Qty = purchaseOrderInfo.purchaseOrderDtlInfos[count].Qty,
                                    OrderStripQty = purchaseOrderInfo.purchaseOrderDtlInfos[count].OrderStripQty,
                                    TaxAmount = purchaseOrderInfo.purchaseOrderDtlInfos[count].TaxAmount,
                                    TotalAmount = purchaseOrderInfo.purchaseOrderDtlInfos[count].TotalAmount,
                                    IsMoved = false
                                };
                                long Result = _purchaseOrderRepo.CreateNewPurchaseOrderDtl(purchaseOrderDtl);
                            }
                        }
                        lstHeader = _purchaseOrderRepo.GetPurchaseOrderBySeqID(HeaderSeqID);
                        lstDeatils = _purchaseOrderRepo.GetPurchaseOrderDtlBySeqID(HeaderSeqID);
                        lstResult = _purchaseOrderRepo.GetPurchaseOrderText();
                        lstClinet = _purchaseOrderRepo.GetClientDeatils();
                    }
                }
                else
                {
                    DateTime SupDate = DateTime.Now;
                    DateTime SupplierInvoiceDate = GetDataformat(purchaseOrderInfo.DeliveryDate, SupDate);
                    if (purchaseOrderInfo.purchaseOrderDtlInfos.Length > 0)
                    {
                        PurchaseOrderHeader purchaseOrderHeader = new PurchaseOrderHeader
                        {
                            SeqID = purchaseOrderInfo.SeqID,
                            SupplierID = purchaseOrderInfo.SupplierID,
                            TotalAmount = purchaseOrderInfo.TotalAmount,
                            TotalItem = purchaseOrderInfo.purchaseOrderDtlInfos.Length,
                            QuotationNo = purchaseOrderInfo.QuotationNo,
                            DeliveryDate = SupplierInvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            IsAprovedBy = purchaseOrderInfo.IsAprovedBy,
                            ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            WarehouseId = purchaseOrderInfo.WarehouseId,
                            WarehouseName = purchaseOrderInfo.WarehouseName
                        };
                        if (purchaseOrderInfo.IsAprovedBy)
                            purchaseOrderHeader.Status = "Approved";
                        else
                            purchaseOrderHeader.Status = "Not Approved";
                        long UpdateOrder = _purchaseOrderRepo.UpdatePurchaseOrderBySeqID(purchaseOrderHeader);
                        if (UpdateOrder > 0)
                        {
                            long DeleteResult = _purchaseOrderRepo.DeletePurchaseOrderBySeqID(purchaseOrderInfo.SeqID);
                            if (DeleteResult > 0)
                            {
                                for (int count = 0; count < purchaseOrderInfo.purchaseOrderDtlInfos.Length; count++)
                                {
                                    PurchaseOrderDtl purchaseOrderDtl = new PurchaseOrderDtl
                                    {
                                        HDTSeqID = purchaseOrderInfo.SeqID,
                                        DrugCode = purchaseOrderInfo.purchaseOrderDtlInfos[count].DrugCode,
                                        BrandName = purchaseOrderInfo.purchaseOrderDtlInfos[count].BrandName,
                                        GST = purchaseOrderInfo.purchaseOrderDtlInfos[count].GST,
                                        Cost = purchaseOrderInfo.purchaseOrderDtlInfos[count].Cost,
                                        Qty = purchaseOrderInfo.purchaseOrderDtlInfos[count].Qty,
                                        OrderStripQty = purchaseOrderInfo.purchaseOrderDtlInfos[count].OrderStripQty,
                                        TaxAmount = purchaseOrderInfo.purchaseOrderDtlInfos[count].TaxAmount,
                                        TotalAmount = purchaseOrderInfo.purchaseOrderDtlInfos[count].TotalAmount,
                                        IsMoved = false
                                    };
                                    long Result = _purchaseOrderRepo.CreateNewPurchaseOrderDtl(purchaseOrderDtl);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { PrintHeader = lstHeader, PrintDeatils = lstDeatils, PurchaseText = lstResult, Client = lstClinet });
        }
        [HttpGet("GetPurchaseOrderTop100")]
        public JsonResult GetPurchaseOrderTop100()
        {
            List<PurchaseOrderHeader> listRetHeader = new List<PurchaseOrderHeader>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                listRetHeader = _purchaseOrderRepo.GetPurchaseOrderTop100(Hospitalid);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(listRetHeader);
        }
        [HttpGet("GetSelectedPurchaseOrder")]
        public JsonResult GetSelectedPurchaseOrder(long BillNo)
        {
            List<PurchaseOrderHeader> lstHeader = new List<PurchaseOrderHeader>();
            List<PurchaseOrderDtl> lstDeatils = new List<PurchaseOrderDtl>();
            List<PurchaseOrderText> lstResult = new List<PurchaseOrderText>();
            List<ClientDeatilsInfo> lstClient = new List<ClientDeatilsInfo>();
            long WarehouseId = 0;
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstHeader = _purchaseOrderRepo.GetPurchaseOrderBySeqID(BillNo);
                lstDeatils = _purchaseOrderRepo.GetPurchaseOrderDtlBySeqID(BillNo);
                lstResult = _purchaseOrderRepo.GetPurchaseOrderText();
                //lstClinet = _purchaseOrderRepo.GetClientDeatils();
                if (lstHeader.Count > 0)
                {
                    string UserName = lstHeader[0].UserName;
                    if(UserName!= "")
                    {
                        lstHeader[0].UserName = Encrypt_Decrypt.Decrypt(UserName);
                    }
                    WarehouseId = lstHeader[0].WarehouseId;
                    if (WarehouseId > 0)
                    {
                        lstClient = _purchaseOrderRepo.GetClientDetailsByWarehouseId(WarehouseId, HospitalId);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = lstHeader, PrintDeatils = lstDeatils, PurchaseText = lstResult, Client = lstClient });
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
        [HttpGet("GetPurchaseOrderDrugBySupplierID")]
        public JsonResult GetPurchaseOrderDrugBySupplierID(int SupplierID)
        {
            List<PurchaseOrderDtl> lstDeatils = new List<PurchaseOrderDtl>();
            try
            {
                lstDeatils = _purchaseOrderRepo.GetPurchaseOrderDrugBySupplierID(SupplierID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new {PrintDeatils = lstDeatils});
        }
        [HttpGet("GetPurchaseOrderDrugBySeqID")]
        public JsonResult GetPurchaseOrderDrugBySeqID(long SeqID)
        {
            List<PurchaseOrderHeader> lstHeader = new List<PurchaseOrderHeader>();
            List<PurchaseOrderDtl> lstDeatils = new List<PurchaseOrderDtl>();
            try
            {
                lstHeader = _purchaseOrderRepo.GetPurchaseOrderBySeqID(SeqID);
                lstDeatils = _purchaseOrderRepo.GetPurchaseOrderDrugBySeqID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(new { PrintHeader = lstHeader, PrintDeatils = lstDeatils });
        }
        [HttpGet("GetSupllierPurchaseOrderTop100")]
        public JsonResult GetSupllierPurchaseOrderTop100(long SupplierID)
        {
            List<PurchaseOrderHeader> listRetHeader = new List<PurchaseOrderHeader>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                listRetHeader = _purchaseOrderRepo.GetSupllierPurchaseOrderTop100(SupplierID, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(listRetHeader);
        }

        [HttpGet("GetPolistDrugDeatils")]
        public JsonResult GetPolistDrugDeatils(long SeqID)
        {
            List<PurchaseOrderDtl> lstDeatils = new List<PurchaseOrderDtl>();
            try
            {
                lstDeatils = _purchaseOrderRepo.GetPurchaseOrderDtlBySeqID(SeqID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstDeatils);
        }
        #endregion
        #region ABdullah
        [HttpGet("GetPoNumberDetails")]
        public List<PurchaseOrderHeader> GetPoNumberDetails(int Search, long SupplierId)
        {
            List<PurchaseOrderHeader> lstResult = new List<PurchaseOrderHeader>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _purchaseOrderRepo.GetPoNumberDetails(HospitalId, Search, SupplierId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        #endregion
    }
}
