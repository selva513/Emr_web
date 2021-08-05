using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyBizLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PharmacyBizLayer.Domain;
using BizLayer.Domain;
using System.Globalization;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/OpeningStockApi")]
    [ApiController]

    public class OpeningStockController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IOpeningStockRepo _openingStockRepo;
        private readonly IDrugMastersRepo _drugMastersRepo;
        private readonly ICashBillRepo _cashBillRepo;
        private readonly INewInvoiceRepo _newInvoiceRepo;
        private readonly IInstrumentDIRepo _instrumentDIRepo;

        public OpeningStockController(IDBConnection dBConnection, IErrorlog errorlog, IOpeningStockRepo openingStockRepo,
            IDrugMastersRepo drugMastersRepo, ICashBillRepo cashBillRepo, INewInvoiceRepo newInvoiceRepo,IInstrumentDIRepo instrumentDIRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _openingStockRepo = openingStockRepo;
            _drugMastersRepo = drugMastersRepo;
            _cashBillRepo = cashBillRepo;
            _newInvoiceRepo = newInvoiceRepo;
            _instrumentDIRepo = instrumentDIRepo;
        }

        [HttpGet("GetProductListByCategory")]
        public JsonResult GetProductListByCategory(long CategoryId, string Search, int StoreId)
        {
            List<OpeningStockMaster> lstProduct = new List<OpeningStockMaster>();
            List<DrugMaster> lstDrug = new List<DrugMaster>();
            long HospitalId = 0;
            string Message = "";
            try
            {
                if (String.IsNullOrWhiteSpace(Search))
                {
                    Search = "";
                }
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                //lstDrug = _drugMastersRepo.GetDrugMasterbySearch("", HospitalId);
                lstProduct = _openingStockRepo.GetProductListByCategory(CategoryId, HospitalId, Search, StoreId);
                if (lstProduct.Count == 0)
                {
                    Message = "Converted To OP";
                    switch (CategoryId)
                    {
                        case 1:
                            var druglst = _drugMastersRepo.GetDrugMasterDetailsByHospitalID(HospitalId);
                            if (druglst.Count == 0)
                            {
                                Message = "No Drug";
                            }
                            break;
                        case 2:
                            var SurgDisplst = _drugMastersRepo.GetSurgicalDisposableByHospitalId(HospitalId);
                            if (SurgDisplst.Count == 0)
                            {
                                Message = "No Drug";
                            }

                            break;
                        case 3:
                            var SurgInstlst = _drugMastersRepo.GetSurgicalInstrumentsByHospitalId(HospitalId);
                            if (SurgInstlst.Count == 0)
                            {
                                Message = "No Drug";
                            }
                            break;
                        case 4:
                            var SurgOtherslst = _drugMastersRepo.GetSurgicalOthersByHospitalId(HospitalId);
                            if (SurgOtherslst.Count == 0)
                            {
                                Message = "No Drug";
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return Json(new { lstProduct = lstProduct, Message = Message });
        }

        [HttpPost("SaveOpeningBalance")]
        public JsonResult SaveOpeningBalance([FromBody] OpeningStockMaster openingStockMaster)
        {
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));


                int DrugCode = openingStockMaster.PH_DRUG_CODE;
                string DrugName = openingStockMaster.PH_DRUGBRANDNAME;
                string Batch = openingStockMaster.PH_DRUG_BATCHNO;
                string storeName = openingStockMaster.PH_SUBSTORENAME;
                DateTime ExDate = DateTime.Now;
                DateTime RunningExpiryDate = GetDataformat(openingStockMaster.PH_DRUG_EXPIRYDT, ExDate);
                List<CurrentStockTimeStamp> currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                if (currentStockTimes.Count > 0)
                {
                    //_openingStockRepo.GetOpeningStockByDrug_Batch(DrugCode,Batch);
                    byte[] LastTimeStamp = currentStockTimes[0].LastTimeStamp;
                    currentStockTimes = _cashBillRepo.GetTimeStampByTimeStamp(Batch, DrugCode, storeName, LastTimeStamp);
                    if (currentStockTimes.Count > 0)
                    {
                    }
                    else
                    {
                        currentStockTimes = _cashBillRepo.GetTimeStampByBatch(Batch, DrugCode, storeName);
                    }
                    int Qty = Convert.ToInt32(openingStockMaster.PH_StockQty);
                    int CurrentStock = currentStockTimes[0].PH_CUR_STOCK + Qty;
                    string Remarks = "OpeningStock";
                    string Ref = "";
                    int StockUpdate = _cashBillRepo.UpdateCurrentStockMinus(CurrentStock, storeName, Batch, DrugCode, Remarks, Ref);

                    //DateTime ExDate = DateTime.Now;
                    //DateTime RunningExpiryDate = GetDataformat(openingStockMaster.PH_DRUG_EXPIRYDT, ExDate);
                    //DateTime RunningExpiryDate = DateTime.Now;
                    StockMovement stockMovement = new StockMovement
                    {
                        PH_RUN_PROCESSIDKEY = "Opening Balance",
                        PH_RUN_STORENAME = openingStockMaster.PH_SUBSTORENAME,
                        PH_RUN_DRUGCODE = DrugCode,
                        PH_RUN_STOCK_TRANSACTVALUE = openingStockMaster.PH_StockQty,
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
                    //DateTime ExDate = DateTime.Now;
                    //DateTime RunningExpiryDate = GetDataformat(openingStockMaster.PH_DRUG_EXPIRYDT, ExDate);
                    //DateTime RunningExpiryDate = DateTime.Now;
                    CurrentStockInfo currentStockInfo = new CurrentStockInfo
                    {
                        PH_CUR_SEQID = (int)CurrentStockID,
                        PH_CUR_DRUGCODE = DrugCode,
                        PH_CUR_STOCK_BATCHNO = Batch,
                        PH_CUR_OLDDRUGCODE = "",
                        PH_CUR_DRUGBRANDNAME = DrugName,
                        PH_CUR_OPSEQID = 0,
                        PH_CUR_STOCK = Convert.ToInt32(openingStockMaster.PH_StockQty),
                        PH_CUR_STOCKUOM = openingStockMaster.PH_DRUG_UOM,
                        PH_CUR_STOCK_INLOCK = 0,
                        PH_CUR_STOCK_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        PH_CUR_STOCK_PURCHCOST = openingStockMaster.PH_StockRate,
                        PH_CUR_STOCK_BILLINGPRICE = openingStockMaster.PH_MRP,
                        PH_CUR_STOCKISZERO = false,
                        PH_CUR_STOCK_STORENAME = storeName,
                        PH_CUR_LAST_PROCESSKEY = "OpStock",
                        PH_CUR_LAST_TRANSID = "",
                        PH_CUR_isROWACTIVE = true,
                        PH_CUR_LAST_TRANSDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        remarks = "OpeningStock",
                        Reference="",
                        HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"))
                    };
                    long CurrentsockInsert = _newInvoiceRepo.CreateNewStock(currentStockInfo);
                    if (CurrentsockInsert > 0)
                    {

                        long Result = _newInvoiceRepo.UpdateCurrentStockInConfig(CurrentStockID);
                        int Qty = Convert.ToInt32(openingStockMaster.PH_StockQty);
                        int CurrentStock = Qty;
                        StockMovement stockMovement = new StockMovement
                        {
                            PH_RUN_PROCESSIDKEY = "OpStock",
                            PH_RUN_STORENAME = storeName,
                            PH_RUN_DRUGCODE = DrugCode,
                            PH_RUN_STOCK_TRANSACTVALUE = Qty,
                            PH_RUN_STOCK_AFTERTRANSACT = Qty,
                            PH_RUN_STOCK_LEFTOUTINBATCH = Qty,
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

                OpeningStockMaster openingStock = new OpeningStockMaster()
                {
                    PH_DRUG_CODE = openingStockMaster.PH_DRUG_CODE,
                    PH_DRUGBRANDNAME = openingStockMaster.PH_DRUGBRANDNAME,
                    PH_DRUG_UOM = openingStockMaster.PH_DRUG_UOM,
                    PH_DRUG_BATCHNO = openingStockMaster.PH_DRUG_BATCHNO,
                    PH_DRUG_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                    PH_DRUG_STOREID = openingStockMaster.PH_DRUG_STOREID,
                    PH_Drug_Strength = openingStockMaster.PH_Drug_Strength,
                    PH_StockQty = openingStockMaster.PH_StockQty,
                    PH_StockRate = openingStockMaster.PH_StockRate,
                    PH_StockValue = openingStockMaster.PH_StockValue,
                    PH_MRP = openingStockMaster.PH_MRP,
                    PH_ISMAINSTORE = openingStockMaster.PH_ISMAINSTORE,
                    PH_SUBSTORENAME = openingStockMaster.PH_SUBSTORENAME,
                    PH_DRUG_STOCK_TAKEDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                    PH_DRUG_MOVEMENT_DONE = "",
                    PH_DRUG_MOVEMENT_STARTDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                    PH_DRUG_DAMAGE_QTY = 0,
                    HospitalId = HospitalId,
                    PH_CreatedUser = HttpContext.Session.GetString("Userseqid"),
                    PH_ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    PH_CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PH_ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    PH_IsActive = true
                };

                //if (openingStockMaster.Action == "Insert")
                //{
                openingStockMaster.PH_OPSEQID = _openingStockRepo.CreateNewOpeningBalance(openingStock);
                //}
                //else if (openingStockMaster.Action == "Update")
                //{

                //}

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("SaveOpeningBalance " + ex.ToString());
            }

            return Json("Success");
        }
        [HttpPost("SaveDraftOpeningStock")]
        public JsonResult SaveDraftOpeningStock([FromBody] DraftOpeningStockHdr draftOpStock)
        {
            string result = "";
            long HospitalId = 0;
            string User = "";
            long DraftHdrId = 0;
            int ItemCatId = 0;
            int StoreId = 0;
            int DrugCode = 0;
            long DtlSeqId = 0;
            string BatchNo = "";
            DraftOpeningStockHdr Drafthdr = new DraftOpeningStockHdr();
            DraftOpeningStockDtl Draftdtl = new DraftOpeningStockDtl();
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                User = HttpContext.Session.GetString("Userseqid");
                ItemCatId = draftOpStock.ItemCatId;
                StoreId = draftOpStock.StoreId;

                Drafthdr = _openingStockRepo.GetDraftHeaderByItemCat_StoreId(draftOpStock.ItemCatId, draftOpStock.StoreId);
                if (Drafthdr != null)
                {
                    DraftHdrId = Drafthdr.SeqID;
                }

                if (!(DraftHdrId > 0))
                {
                    draftOpStock.CreatedUser = User;
                    draftOpStock.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    draftOpStock.HospitalId = HospitalId;
                    DraftHdrId = _openingStockRepo.CreateNewDraftOpeningStock(draftOpStock);
                }

                if (DraftHdrId > 0)
                {
                    for (int i = 0; i < draftOpStock.draftOpStockDtl.Length; i++)
                    {
                        BatchNo = draftOpStock.draftOpStockDtl[i].DrugBatchNo;
                        DrugCode = draftOpStock.draftOpStockDtl[i].DrugCode;
                        if (ItemCatId == 1)
                        {
                            Draftdtl = _openingStockRepo.GetDraftDetailsByDrugCodeAndBatch(ItemCatId, StoreId, DrugCode, BatchNo);
                        }
                        else
                        {
                            Draftdtl = _openingStockRepo.GetDraftDetailsByDrugCode(ItemCatId, StoreId, DrugCode);
                        }
                        if (Draftdtl == null)
                        {
                            draftOpStock.draftOpStockDtl[i].HdrSeqId = DraftHdrId;
                            draftOpStock.draftOpStockDtl[i].DrugStoreId = StoreId;
                            draftOpStock.draftOpStockDtl[i].CreatedUser = User;
                            draftOpStock.draftOpStockDtl[i].CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            DtlSeqId = _openingStockRepo.CreateNewDraftOpeningStockDtl(draftOpStock.draftOpStockDtl[i]);
                        }
                        else
                        {
                            draftOpStock.draftOpStockDtl[i].HdrSeqId = DraftHdrId;
                            draftOpStock.draftOpStockDtl[i].DtlSeqId = Draftdtl.DtlSeqId;
                            draftOpStock.draftOpStockDtl[i].DrugStoreId = StoreId;
                            draftOpStock.draftOpStockDtl[i].ModifiedUser = User;
                            draftOpStock.draftOpStockDtl[i].ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            _openingStockRepo.UpdateDraftOpeningStockDtl(draftOpStock.draftOpStockDtl[i]);
                        }

                    }
                    result = "Success";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
                throw;
            }

            return Json(result);
        }

        [HttpGet("GetDraftListByItemCat_Store")]
        public JsonResult GetDraftListByItemCat_Store(int ItemCatId, int StoreId, string Search)
        {
            DraftOpeningStockHdr Header = new DraftOpeningStockHdr();
            List<DraftOpeningStockDtl> lstDetail = new List<DraftOpeningStockDtl>();

            try
            {
                if (Search == null)
                {
                    Search = "";
                }
                lstDetail = _openingStockRepo.GetDraftDetailsByItemCat_StoreId(ItemCatId, StoreId, Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return Json(lstDetail);
        }
        [HttpGet("DeleteDraftByHeaderId")]
        public JsonResult DeleteDraftByHeaderId(long DraftHdrId, long DtlSeqId)
        {
            string result = "";
            long retvalue = 0;
            DraftOpeningStockDtl detail = new DraftOpeningStockDtl();
            try
            {
                detail.HdrSeqId = DraftHdrId;
                detail.DtlSeqId = DtlSeqId;
                detail.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                detail.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                retvalue = _openingStockRepo.DeleteDraftDetailByHeaderId(detail);
                if (retvalue > 0)
                {
                    result = "Success";
                }
                else
                {
                    result = "Unable To Delete";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("DeleteDraftByHeaderId " + ex.ToString());
            }

            return Json(result);
        }
        [HttpPost("ConvertDraftToOpStock")]
        public JsonResult ConvertDraftToOpStock([FromBody] DraftOpeningStockHdr draftOpeningStockHdr)
        {
            string result = "";
            DraftOpeningStockHdr draftHdr = new DraftOpeningStockHdr();
            List<DraftOpeningStockDtl> draftdtl = new List<DraftOpeningStockDtl>();
            int StoreId = 0;
            int ItemCatId = 0;
            string StoreName = "";
            bool IsMainstore = false;

            try
            {
                StoreId = draftOpeningStockHdr.StoreId;
                ItemCatId = draftOpeningStockHdr.ItemCatId;
                draftHdr = _openingStockRepo.GetDraftHeaderByItemCat_StoreId(ItemCatId, StoreId);
                draftdtl = _openingStockRepo.GetDraftDetailsByItemCat_StoreId(ItemCatId, StoreId, "");

                if (draftHdr != null)
                {
                    StoreName = draftHdr.StoreName;
                    if (StoreId == 1)
                    {
                        IsMainstore = true;
                    }

                    if (draftdtl.Count > 0)
                    {
                        for (int dtl = 0; dtl < draftdtl.Count; dtl++)
                        {
                            bool IsSuccess = false;
                            if (ItemCatId == 1)
                            {
                                OpeningStockMaster openingStockMaster = new OpeningStockMaster
                                {
                                    PH_DRUG_CODE = draftdtl[dtl].DrugCode,
                                    PH_DRUGBRANDNAME = draftdtl[dtl].DrugBrandName,
                                    PH_DRUG_BATCHNO = draftdtl[dtl].DrugBatchNo,
                                    PH_DRUG_EXPIRYDT = draftdtl[dtl].DrugExpiryDt,
                                    PH_DRUG_UOM = draftdtl[dtl].DrugUOM,
                                    PH_StockQty = draftdtl[dtl].StockQty,
                                    PH_StockRate = draftdtl[dtl].StockRate,
                                    PH_StockValue = draftdtl[dtl].StockValue,
                                    PH_MRP = draftdtl[dtl].MRP,
                                    PH_DRUG_STOREID = StoreId,
                                    PH_SUBSTORENAME = StoreName,
                                    PH_ISMAINSTORE = IsMainstore,
                                    PH_Drug_Strength = draftdtl[dtl].DrugStrength
                                };

                                JsonResult results =  SaveOpeningBalance(openingStockMaster);
                                if(result.ToString() == "Success")
                                {
                                    IsSuccess = true;
                                }
                            }
                            else if(ItemCatId == 3)
                            {
                                InstrumentOpeningStock openingStockMasters = new InstrumentOpeningStock
                                {
                                    ItemID = draftdtl[dtl].DrugCode,
                                    ItemCatId = ItemCatId,
                                    ItemName = draftdtl[dtl].DrugBrandName,
                                    ExpiryDate = draftdtl[dtl].DrugExpiryDt,
                                    StockQty = draftdtl[dtl].StockQty,
                                    StockRate = draftdtl[dtl].StockRate,
                                    StockValue = draftdtl[dtl].StockValue,
                                    MRP = draftdtl[dtl].MRP,
                                    StoreId = StoreId,
                                    SubstoreName = StoreName,
                                    IsMainstore = IsMainstore
                                };
                                JsonResult result1 = SaveSurgicalOpeningBalance(openingStockMasters);
                                if(result1.Value.ToString() == "Success")
                                {
                                    IsSuccess = true;
                                }
                            }

                            if (IsSuccess)
                            {
                                DeleteDraftByHeaderId(draftdtl[dtl].HdrSeqId, draftdtl[dtl].DtlSeqId);
                            }
                        }
                        DeleteDraftHeaderById(draftHdr.SeqID);
                        result = "Success";
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("ConvertDraftToOpStock " + ex.ToString());
            }

            return Json(result);
        }
        public string DeleteDraftHeaderById(long SeqId)
        {
            string result = "";
            DraftOpeningStockHdr header = new DraftOpeningStockHdr();
            try
            {
                header.SeqID = SeqId;
                header.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                header.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                long retvalue = _openingStockRepo.DeleteDraftHeaderById(header);
                if (retvalue > 0)
                {
                    result = "Success";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return result;
        }

        private JsonResult SaveSurgicalOpeningBalance(InstrumentOpeningStock openingStockMaster)
        {
            long HospitalId = 0;
            DateTime RunningExpiryDate;
            string ExpiryDatestring=null;
            long DrugCode = 0;
            string storeName = "";
            int StockQty = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));


                DrugCode = openingStockMaster.ItemID;
                storeName = openingStockMaster.SubstoreName;
                StockQty = Convert.ToInt32(openingStockMaster.StockQty);
                DateTime ExDate = DateTime.Now;
                if(openingStockMaster.ExpiryDate != null)
                {
                    RunningExpiryDate = GetDataformat(openingStockMaster.ExpiryDate, ExDate);
                    ExpiryDatestring = RunningExpiryDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                List<InstrumentCurrentStockInfo> instrumentCurrentStocks = _instrumentDIRepo.GetStockByItemId_Warehouse(DrugCode, storeName,HospitalId);
                if (instrumentCurrentStocks.Count > 0)
                {
                    long StockSeqId = instrumentCurrentStocks[0].SeqID;
                    int CurrentStock = instrumentCurrentStocks[0].CurrentStockQty + StockQty;
                    string Remarks = "OpeningStock";
                    InstrumentCurrentStockInfo currentStockInfo = new InstrumentCurrentStockInfo
                    {
                        SeqID = StockSeqId,
                        ItemID = DrugCode,
                        CurrentStockQty = Convert.ToInt32(openingStockMaster.StockQty),
                        ExpiryDate = ExpiryDatestring,
                        PurchasePrice = openingStockMaster.StockRate,
                        SellingPrice = openingStockMaster.MRP,
                        WarehouseName = storeName,
                        IsActive = true,
                        ModifiedUser = Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                        ModifiedDttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Remarks = Remarks
                    };
                    
                    int StockUpdate = _instrumentDIRepo.UpdateCurrentStock(currentStockInfo);
                    if (StockUpdate > 0)
                    {
                        //Need to Insert Stock Movement
                    }
                }
                else
                {
                    InstrumentCurrentStockInfo currentStockInfo = new InstrumentCurrentStockInfo
                    {
                        ItemID = DrugCode,
                        CurrentStockQty = StockQty,
                        ExpiryDate = ExpiryDatestring,
                        PurchasePrice = openingStockMaster.StockRate,
                        SellingPrice = openingStockMaster.MRP,
                        WarehouseName = storeName,
                        IsActive = true,
                        CreatedUser = Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                        CreatedDttime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Remarks = "OpeningStock",
                        HospitalID = Convert.ToInt32(HttpContext.Session.GetString("Hospitalid"))
                    };
                    long CurrentsockInsert = _instrumentDIRepo.CreateNewStock(currentStockInfo);
                    if (CurrentsockInsert > 0)
                    {
                        //Need to Insert Stock Movement

                        //StockMovement stockMovement = new StockMovement
                        //{
                        //    PH_RUN_PROCESSIDKEY = "OpStock",
                        //    PH_RUN_STORENAME = storeName,
                        //    PH_RUN_DRUGCODE = DrugCode,
                        //    PH_RUN_STOCK_TRANSACTVALUE = Qty,
                        //    PH_RUN_STOCK_AFTERTRANSACT = Qty,
                        //    PH_RUN_STOCK_LEFTOUTINBATCH = Qty,
                        //    PH_RUN_BATCHNO = Batch,
                        //    PH_RUN_EXPIRYDT = RunningExpiryDate.ToString("yyyy-MM-dd"),
                        //    PH_RUN_DOC_HDRNO = 1,
                        //    PH_RUN_DOC_DTLNO = 1,
                        //    PH_RUN_PROCESSDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                        //    PH_RUN_FINYEAR = "20-21",
                        //    PH_RUN_TRANSACT_ISACTIVE = true,
                        //    PH_RUN_CREATEDUSER = HttpContext.Session.GetString("Userseqid"),
                        //    PH_RUN_CREATEDDTTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        //};
                        //int StockMovementUpdate = _cashBillRepo.InsertStockMovement(stockMovement);
                    }
                }

                InstrumentOpeningStock InstopeningStock = new InstrumentOpeningStock()
                {
                    ItemID = openingStockMaster.ItemID,
                    ItemName = openingStockMaster.ItemName,
                    UOM = openingStockMaster.UOM,
                    ExpiryDate = ExpiryDatestring,
                    StoreId = openingStockMaster.StoreId,
                    SubstoreName = openingStockMaster.SubstoreName,
                    StockQty = openingStockMaster.StockQty,
                    StockRate = openingStockMaster.StockRate,
                    StockValue = openingStockMaster.StockValue,
                    MRP = openingStockMaster.MRP,
                    IsMainstore = openingStockMaster.IsMainstore,
                    Stock_TakeDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Movement_Done = "",
                    Movement_StartDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Damage_Qty = 0,
                    HospitalID = HospitalId,
                    CreatedUser = Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                    CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
               
                openingStockMaster.OPSeqId = _openingStockRepo.CreateNewInstrument_OPBalance(InstopeningStock);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("SaveSurgicalOpeningBalance " + ex.ToString());
            }

            return Json("Success");
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
