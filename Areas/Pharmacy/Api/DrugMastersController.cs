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
using System.Text;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Route("api/DrugMastersApi")]
    [ApiController]
    public class DrugMastersController : Controller
    {

        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IDrugMastersRepo _IDrugMasterRepo;
        private readonly ICashBillRepo _cashBillRepo;
        public DrugMastersController(IDBConnection dBConnection, IErrorlog errorlog, IDrugMastersRepo drugMaster, ICashBillRepo cashBillRepo)
        {

            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _IDrugMasterRepo = drugMaster;
            _cashBillRepo = cashBillRepo;
        }
        [HttpGet("Getdata")]
        public string Getdata()
        {
            return "sucess";
        }


        [HttpGet("GetDrugsUombySearch")]
        public List<DrugUom> GetDrugsUombySearch(string Search)
        {
            List<DrugUom> lstResult = new List<DrugUom>();
            try
            {
                //long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetDrugUomBySearch(Search);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetGenericNamesbySearch")]
        public List<GenericName> GetGenericNamesbySearch(string Search)
        {
            List<GenericName> lstResult = new List<GenericName>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetGenericNameBySearch(Search);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetDrugUoms")]
        public List<DrugUom> GetDrugUoms()
        {
            List<DrugUom> lstResult = new List<DrugUom>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetDrugUoms();

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetGenericNameByHospitalId")]
        public List<GenericName> GetGenericNameByHospitalId()
        {
            List<GenericName> lstResult = new List<GenericName>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetGenericNameByHospitalId();

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetDrugCategoryByHospitalId")]
        public List<DrugCategory> GetDrugCategoryByHospitalId()
        {
            List<DrugCategory> lstResult = new List<DrugCategory>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetDrugCategoryByHospitalId(Hospitalid);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetSubstancesNameBySearch")]
        public List<SubstanceMaster> GetSubstancesNameBySearch()
        {
            List<SubstanceMaster> lstResult = new List<SubstanceMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetSubstanceNameBySearch();

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetSubstanceDetailsBySearch")]
        public List<SubstanceMaster> GetSubstanceDetailsBySearch(long SubstanceSeqId)
        {
            List<SubstanceMaster> lstResult = new List<SubstanceMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetSubstanceDetailsBySearch(SubstanceSeqId);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpPost("CreateDrugMasters")]
        public long CraeteDrugMaster(DrugMaster drugMaster)
        {
            List<DrugUom> lstResult = new List<DrugUom>();
            List<GenericName> GenericResult = new List<GenericName>();
            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<DrugCategory> CategoryResult = new List<DrugCategory>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugForm> formResult = new List<DrugForm>();
            bool IsSucess = false;
            long retValue = 0;
            string Name;
            long Hospitalid;

            try
            {
                Name = drugMaster.PH_ITEM_PACKAGE.Trim().ToUpper();
                lstResult = _IDrugMasterRepo.GetDrugUomsByName(Name);
                try
                {
                    if (lstResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_PACKAGE != "")
                        {
                            DrugUom DrugUom = new DrugUom();
                            DrugUom.PH_UOMDESC = drugMaster.PH_ITEM_PACKAGE.Trim().ToUpper();
                            DrugUom.PH_ISUOMACTIVE = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                        }


                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_ITEM_DRUG_GENERIC.Trim().ToUpper();
                GenericResult = _IDrugMasterRepo.GetGenericNameByName(Name);
                try
                {
                    if (GenericResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_DRUG_GENERIC != "")
                        {
                            GenericName genericName = new GenericName();
                            genericName.PH_DRUG_GENERICNAME = drugMaster.PH_ITEM_DRUG_GENERIC.Trim().ToUpper();
                            genericName.PH_DRUG_GENERIC_ISACTIVE = true;

                            int retResult = _IDrugMasterRepo.CreateGenericName(genericName);
                        }


                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_ITEM_MANUFACTURENAME.Trim().ToUpper();
                ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
                if(ManufactureResult.Count == 1)
                {
                    drugMaster.PH_ITEM_MANUFACTURE = ManufactureResult[0].PH_ManufactureID;
                }
               
                try
                {
                    if (ManufactureResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_MANUFACTURENAME != "")
                        {
                            ManufactureMaster ManufactureMaster = new ManufactureMaster();
                            ManufactureMaster.PH_ManufactureName = drugMaster.PH_ITEM_MANUFACTURENAME.Trim().ToUpper();
                            ManufactureMaster.PH_MANF_Isactive = true;

                            long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                            drugMaster.PH_ITEM_MANUFACTURE = retManufacture;
                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_ITEM_DRUG_CATEGORY.Trim().ToUpper();
                CategoryResult = _IDrugMasterRepo.GetDrugMasterDetailsByName(Name);
                if (CategoryResult.Count == 1)
                {
                    drugMaster.PH_ITEM_DRUG_CatID = CategoryResult[0].PH_ItemCatID;
                }

                try
                {
                    if (CategoryResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_DRUG_CATEGORY != "")
                        {
                            DrugCategory drugCategory = new DrugCategory();
                            drugCategory.PH_ItemCatName = drugMaster.PH_ITEM_DRUG_CATEGORY.Trim().ToUpper();
                            drugCategory.PH_ItemCatIsActive = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retCategory = _IDrugMasterRepo.SaveDrugCategory(drugCategory, Hospitalid);

                            drugMaster.PH_ITEM_DRUG_CatID = retCategory;
                        }

                    }

                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }


                Name = drugMaster.PH_ITEM_RACK.Trim().ToUpper();
                rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);


                try
                {
                    if (rackResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_RACK != "")
                        {
                            RackMaster rackMaster = new RackMaster();
                            rackMaster.PH_Rack_Name = drugMaster.PH_ITEM_RACK.Trim().ToUpper();
                            rackMaster.PH_Rack_IsActive = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                            drugMaster.PH_ITEM_RACK = drugMaster.PH_ITEM_RACK.ToUpper();


                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_DrugFormName.Trim().ToUpper();
                formResult = _IDrugMasterRepo.GetDrugFormDetailsByName(Name);
                if (formResult.Count == 1)
                {
                    drugMaster.PH_ITEM_DRUG_FORMId = formResult[0].PH_DrugFormSeqid;
                }
                

                try
                {
                    if (formResult.Count == 0)
                    {
                        if (drugMaster.PH_DrugFormName != "")
                        {
                            DrugForm drugForm = new DrugForm();
                            drugForm.PH_DrugFormName = drugMaster.PH_DrugFormName.Trim().ToUpper();
                            drugForm.PH_IsActive = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retrack = _IDrugMasterRepo.CreateDrugForm(drugForm, Hospitalid);
                            drugMaster.PH_ITEM_DRUG_FORMId = retrack;

                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                drugMaster.PH_ITEM_DRUGNAME_BRAND = drugMaster.PH_ITEM_DRUGNAME_BRAND.Trim().ToUpper();
                drugMaster.PH_ITEM_DRUG_GENERIC = drugMaster.PH_ITEM_DRUG_GENERIC.Trim().ToUpper();
                drugMaster.PH_ITEM_PACKAGE = drugMaster.PH_ITEM_PACKAGE.Trim().ToUpper();
                drugMaster.PH_ITEM_HSNCODE = drugMaster.PH_ITEM_HSNCODE.Trim().ToUpper();
                drugMaster.PH_ITEM_DRUG_CATEGORY = drugMaster.PH_ITEM_DRUG_CATEGORY.Trim().ToUpper();
                drugMaster.PH_ITEM_RACK = drugMaster.PH_ITEM_RACK.Trim().ToUpper();
                drugMaster.PH_ITEM_STRENGTH = drugMaster.PH_ITEM_STRENGTH.Trim().ToUpper();
                drugMaster.PH_ITEM_SCHEDULETYPE = drugMaster.PH_ITEM_SCHEDULETYPE.Trim().ToUpper();


                Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retValue = _IDrugMasterRepo.CreateDrugMaster(drugMaster, Hospitalid);

                if (retValue > 0)
                    for (var j = 0; j < drugMaster.substanceMappings.Count(); j++)
                    {
                        IsSucess = true;
                        SubstanceMapping SubstanceMapping = new SubstanceMapping();
                        SubstanceMapping.DrugSeqId = retValue;
                        SubstanceMapping.FormulaSeqId = drugMaster.substanceMappings[j].FormulaSeqId;
                        _IDrugMasterRepo.CreateFormulaMapping(SubstanceMapping);
                    }

            }


            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retValue;
        }

        public List<DrugUom> GetDrugUomsByName(string Name)
        {
            List<DrugUom> lstResult = new List<DrugUom>();
            try
            {

                lstResult = _IDrugMasterRepo.GetDrugUomsByName(Name);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        public List<GenericName> GetGenericNameByName(string Name)
        {
            List<GenericName> lstResult = new List<GenericName>();
            try
            {

                lstResult = _IDrugMasterRepo.GetGenericNameByName(Name);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }

        [HttpGet("ManufactureSearch")]
        public List<ManufactureMaster> ManufactureSearch(string Search)
        {
            List<ManufactureMaster> lstResult = new List<ManufactureMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetManufactureBySearch(Search);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpPost("CreatesubstanceFormulas")]
        public int CreatesubstanceFormula([FromBody] SubstanceMaster substanceMaster)
        {
            int retResult = 0;
            try
            {

                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retResult = _IDrugMasterRepo.CreatesubstanceFormula(substanceMaster);
            }
            catch (Exception ex)
            {

                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retResult;
        }
        [HttpGet("GetDrugMasterByHospitalID")]
        public List<DrugMaster> GetDrugMasterByHospitalID()
        {
            List<DrugMaster> lstResult = new List<DrugMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetDrugMasterDetailsByHospitalID(Hospitalid);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetDrugMastersDetailsByDrugCode")]
        public List<DrugMaster> GetDrugMastersDetailsByDrugCode(long PH_ITEM_DrugCode)
        {
            List<DrugMaster> lstResult = new List<DrugMaster>();
            List<SubstanceMapping> SubResult = new List<SubstanceMapping>();



            try
            {

                lstResult = _IDrugMasterRepo.GetDrugMasterDetailsByDrugCode(PH_ITEM_DrugCode);
                SubResult = _IDrugMasterRepo.GetSubstanceMappingByCodeId(PH_ITEM_DrugCode);
                lstResult[0].substanceMappings = SubResult.ToArray();





            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpPost("UpdateDrugsMasters")]
        public long UpdateDrugsMasters([FromBody] DrugMaster drugMaster)
        {
            bool IsSucess = false;
            long retValue = 0;

            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<DrugUom> lstResult = new List<DrugUom>();
            List<GenericName> GenericResult = new List<GenericName>();
            List<DrugCategory> CategoryResult = new List<DrugCategory>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugForm> formResult = new List<DrugForm>();
            string Name;
            long Hospitalid;

            try
            {

                Name = drugMaster.PH_ITEM_PACKAGE.Trim().ToUpper();
                lstResult = _IDrugMasterRepo.GetDrugUomsByName(Name);
                try
                {
                    if (lstResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_PACKAGE != "")
                        {
                            DrugUom DrugUom = new DrugUom();
                            DrugUom.PH_UOMDESC = drugMaster.PH_ITEM_PACKAGE.Trim().ToUpper();
                            DrugUom.PH_ISUOMACTIVE = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                        }


                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_ITEM_DRUG_GENERIC.Trim().ToUpper();
                GenericResult = _IDrugMasterRepo.GetGenericNameByName(Name);
                try
                {
                    if (GenericResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_DRUG_GENERIC != "")
                        {
                            GenericName genericName = new GenericName();
                            genericName.PH_DRUG_GENERICNAME = drugMaster.PH_ITEM_DRUG_GENERIC.Trim().ToUpper();
                            genericName.PH_DRUG_GENERIC_ISACTIVE = true;

                            int retResult = _IDrugMasterRepo.CreateGenericName(genericName);
                        }


                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_ITEM_MANUFACTURENAME.Trim().ToUpper();
                ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
                if(ManufactureResult.Count == 1)
                {
                    drugMaster.PH_ITEM_MANUFACTURE = ManufactureResult[0].PH_ManufactureID;
                }
              
                try
                {
                    if (ManufactureResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_MANUFACTURENAME != "")
                        {
                            ManufactureMaster ManufactureMaster = new ManufactureMaster();
                            ManufactureMaster.PH_ManufactureName = drugMaster.PH_ITEM_MANUFACTURENAME.Trim().ToUpper();
                            ManufactureMaster.PH_MANF_Isactive = true;

                            long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                            drugMaster.PH_ITEM_MANUFACTURE = retManufacture;
                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_ITEM_DRUG_CATEGORY.Trim().ToUpper();
                CategoryResult = _IDrugMasterRepo.GetDrugMasterDetailsByName(Name);
                if(CategoryResult.Count == 1)
                {
                    drugMaster.PH_ITEM_DRUG_CatID = CategoryResult[0].PH_ItemCatID;
                }
              
                try
                {
                    if (CategoryResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_DRUG_CATEGORY != "")
                        {
                            DrugCategory drugCategory = new DrugCategory();
                            drugCategory.PH_ItemCatName = drugMaster.PH_ITEM_DRUG_CATEGORY.Trim().ToUpper();
                            drugCategory.PH_ItemCatIsActive = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retCategory = _IDrugMasterRepo.SaveDrugCategory(drugCategory, Hospitalid);

                            drugMaster.PH_ITEM_DRUG_CatID = retCategory;
                        }

                    }

                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }


                Name = drugMaster.PH_ITEM_RACK.Trim().ToUpper();
                rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);

                try
                {
                    if (rackResult.Count == 0)
                    {
                        if (drugMaster.PH_ITEM_RACK != "")
                        {
                            RackMaster rackMaster = new RackMaster();
                            rackMaster.PH_Rack_Name = drugMaster.PH_ITEM_RACK.Trim().ToUpper();
                            rackMaster.PH_Rack_IsActive = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                            drugMaster.PH_ITEM_RACK = drugMaster.PH_ITEM_RACK.ToUpper();


                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                Name = drugMaster.PH_DrugFormName.Trim().ToUpper();
                formResult = _IDrugMasterRepo.GetDrugFormDetailsByName(Name);
                if(formResult.Count == 1)
                {
                    drugMaster.PH_ITEM_DRUG_FORMId = formResult[0].PH_DrugFormSeqid;
                }
               

                try
                {
                    if (formResult.Count == 0)
                    {
                        if (drugMaster.PH_DrugFormName != "")
                        {
                            DrugForm drugForm = new DrugForm();
                            drugForm.PH_DrugFormName = drugMaster.PH_DrugFormName.Trim().ToUpper();
                            drugForm.PH_IsActive = true;
                            Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retrack = _IDrugMasterRepo.CreateDrugForm(drugForm, Hospitalid);
                            drugMaster.PH_ITEM_DRUG_FORMId = retrack;

                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                try
                {
                    drugMaster.PH_ITEM_DRUGNAME_BRAND = drugMaster.PH_ITEM_DRUGNAME_BRAND.Trim().ToUpper();
                    drugMaster.PH_ITEM_DRUG_GENERIC = drugMaster.PH_ITEM_DRUG_GENERIC.Trim().ToUpper();
                    drugMaster.PH_ITEM_PACKAGE = drugMaster.PH_ITEM_PACKAGE.Trim().ToUpper();
                    drugMaster.PH_ITEM_HSNCODE = drugMaster.PH_ITEM_HSNCODE.Trim().ToUpper();
                    drugMaster.PH_ITEM_DRUG_CATEGORY = drugMaster.PH_ITEM_DRUG_CATEGORY.Trim().ToUpper();
                    drugMaster.PH_ITEM_RACK = drugMaster.PH_ITEM_RACK.Trim().ToUpper();
                    drugMaster.PH_ITEM_STRENGTH = drugMaster.PH_ITEM_STRENGTH.Trim().ToUpper();
                    drugMaster.PH_ITEM_SCHEDULETYPE = drugMaster.PH_ITEM_SCHEDULETYPE.Trim().ToUpper();

                    retValue = _IDrugMasterRepo.UpdateDrugMaster(drugMaster);
                    if (drugMaster.substanceMappings.Length > 0)
                    {
                        List<SubstanceMapping> Lists = _IDrugMasterRepo.GetSubstanceMappingByCodeId(drugMaster.PH_ITEM_DrugCode);

                        foreach (SubstanceMapping i in drugMaster.substanceMappings)
                        {
                            bool IsExists = false;
                            long FormulaSeqId = i.UserId;
                            //if (OldList.All(useq => useq.UserId.Equals(UserID)))
                            //{
                            //    IsExists = true;
                            //}
                            //else
                            //{

                            //}
                            for (int j = 0; j < Lists.Count(); j++)
                            {
                                if (Lists[j].FormulaSeqId == FormulaSeqId)
                                {
                                    IsExists = true;
                                    Lists[j].IsActive = true;
                                }


                            }
                            if (!IsExists)

                            {

                                IsSucess = true;
                                SubstanceMapping SubstanceMapping = new SubstanceMapping();
                                SubstanceMapping.DrugSeqId = drugMaster.PH_ITEM_DrugCode;
                                SubstanceMapping.FormulaSeqId = FormulaSeqId;
                                long retVaue = _IDrugMasterRepo.CreateFormulaMapping(SubstanceMapping);


                            }
                        }

                        for (int j = 0; j < Lists.Count(); j++)
                        {
                            int h = j;
                            if (Lists[j].IsActive == false)
                            {
                                _IDrugMasterRepo.DeletesubstanceFormula(Lists[j].DrugSeqId, Lists[j].FormulaSeqId);
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return retValue;
        }
        [HttpPost("DeleteDrugMasterByDrugsCode")]
        public long DeleteDrugMasterByDrugsCode(long PH_ITEM_DrugCode)
        {
            long retResult = 0;
            try
            {

                retResult = _IDrugMasterRepo.DeleteDrugMasterByDrugCode(PH_ITEM_DrugCode);
            }
            catch (Exception ex)
            {

                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retResult;
        }
        //[HttpGet("GetDrugMasterbySearch")]
        //public List<DrugMaster> GetDrugMasterbySearch(string Search)
        //{
        //    List<DrugMaster> lstResult = new List<DrugMaster>();

        //    try
        //    {
        //        lstResult = _IDrugMasterRepo.GetDrugMasterbySearch(Search);

        //    }
        //    catch (Exception ex)
        //    {
        //        _errorlog.WriteErrorLog(ex.ToString());
        //    }
        //    return lstResult;
        //}
        [HttpPost("CreateSurgicalDisposable")]
        public int CreateSurgicalDisposable(SurgicalDisposable surgicalDisposable)
        {
            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugUom> lstResult = new List<DrugUom>();
            string Name;
            int retvalue = 0;


            try
            {
                surgicalDisposable.PH_ManufactureName = surgicalDisposable.PH_ManufactureName.Trim().ToUpper();
                Name = surgicalDisposable.PH_ManufactureName;
                ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
                if (ManufactureResult.Count == 0)
                {
                    if (surgicalDisposable.PH_ManufactureName != "")
                    {
                        ManufactureMaster ManufactureMaster = new ManufactureMaster();
                        ManufactureMaster.PH_ManufactureName = surgicalDisposable.PH_ManufactureName.Trim().ToUpper();
                        ManufactureMaster.PH_MANF_Isactive = true;

                        long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                        surgicalDisposable.PH_Manufacture = retManufacture;
                    }

                
                }
                surgicalDisposable.PH_Rack = surgicalDisposable.PH_Rack.Trim().ToUpper();
                Name = surgicalDisposable.PH_Rack;
                rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);

                try
                {
                    if (rackResult.Count == 0)
                    {
                        if (surgicalDisposable.PH_Rack != "")
                        {
                            RackMaster rackMaster = new RackMaster();
                            rackMaster.PH_Rack_Name = surgicalDisposable.PH_Rack.Trim().ToUpper();
                            rackMaster.PH_Rack_IsActive = true;
                            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                            surgicalDisposable.PH_Rack = surgicalDisposable.PH_Rack.ToUpper();

                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                surgicalDisposable.PH_Package = surgicalDisposable.PH_Package.Trim().ToUpper();
                Name = surgicalDisposable.PH_Package;
                lstResult = _IDrugMasterRepo.GetDrugUomsByName(Name);
                try
                {
                    if (lstResult.Count == 0)
                    {
                        if (surgicalDisposable.PH_Package != "")
                        {
                            DrugUom DrugUom = new DrugUom();
                            DrugUom.PH_UOMDESC = surgicalDisposable.PH_Package.Trim().ToUpper();
                            DrugUom.PH_ISUOMACTIVE = true;
                            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                        }


                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CreatedDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
                string CurDatetime = timezoneUtility.Gettimezone(Timezoneid);
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));

                retvalue = _IDrugMasterRepo.CreateSurgicalDisposable(surgicalDisposable, CreatedDatetime, HospitalID);


            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return retvalue;
        }
        [HttpGet("GetSurgicalDisposableByHospitalId")]

        public List<SurgicalDisposable> GetSurgicalDisposableByHospitalId()
        {
            List<SurgicalDisposable> lstResult = new List<SurgicalDisposable>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetSurgicalDisposableByHospitalId(HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpPost("CreateSurgicalInstruments")]
        public int CreateSurgicalInstruments(SurgicalInstruments surgicalInstruments)
        {
            int retvalue = 0;
            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugUom> Result = new List<DrugUom>();
            string Name;

            try
            {
                surgicalInstruments.PH_ManufactureName = surgicalInstruments.PH_ManufactureName.Trim().ToUpper();
                Name = surgicalInstruments.PH_ManufactureName;
                ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
                if (ManufactureResult.Count == 0)
                {
                    if (surgicalInstruments.PH_ManufactureName != "")
                    {
                        ManufactureMaster ManufactureMaster = new ManufactureMaster();
                        ManufactureMaster.PH_ManufactureName = surgicalInstruments.PH_ManufactureName.Trim().ToUpper();
                        ManufactureMaster.PH_MANF_Isactive = true;

                        long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                        surgicalInstruments.PH_Manufacture = retManufacture;
                    }

                }
                surgicalInstruments.PH_Rack = surgicalInstruments.PH_Rack.Trim().ToUpper();
                Name = surgicalInstruments.PH_Rack;
                rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);

                try
                {
                    if (rackResult.Count == 0)
                    {
                        if (surgicalInstruments.PH_Rack != "")
                        {
                            RackMaster rackMaster = new RackMaster();
                            rackMaster.PH_Rack_Name = surgicalInstruments.PH_Rack.Trim().ToUpper();
                            rackMaster.PH_Rack_IsActive = true;
                            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                            surgicalInstruments.PH_Rack = surgicalInstruments.PH_Rack.ToUpper();

                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                surgicalInstruments.PH_Package = surgicalInstruments.PH_Package.Trim().ToUpper();
                Name = surgicalInstruments.PH_Package;
                Result = _IDrugMasterRepo.GetDrugUomsByName(Name);
                try
                {
                    if (Result.Count == 0)
                    {
                        if (surgicalInstruments.PH_Package != "")
                        {
                            DrugUom DrugUom = new DrugUom();
                            DrugUom.PH_UOMDESC = surgicalInstruments.PH_Package.Trim().ToUpper();
                            DrugUom.PH_ISUOMACTIVE = true;
                            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                        }


                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CreatedDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
                string CurDatetime = timezoneUtility.Gettimezone(Timezoneid);
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retvalue = _IDrugMasterRepo.CreateSurgicalInstruments(surgicalInstruments, CreatedDatetime, HospitalID);

            }

            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return retvalue;

        }
        [HttpGet("GetSurgicalInstrumentsByHospitalId")]

        public List<SurgicalInstruments> GetSurgicalInstrumentsByHospitalId()
        {
            List<SurgicalInstruments> lstResult = new List<SurgicalInstruments>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetSurgicalInstrumentsByHospitalId(HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpPost("CreateSurgicalOthers")]
        public int CreateSurgicalOthers(SurgicalOthers surgicalOthers)
        {
            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugUom> Result = new List<DrugUom>();
            int retvalue = 0;
            string Name;
            try
            {
                surgicalOthers.PH_ManufactureName = surgicalOthers.PH_ManufactureName.Trim().ToUpper();
                Name = surgicalOthers.PH_ManufactureName;
                ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
                if (ManufactureResult.Count == 0)
                {
                    if (surgicalOthers.PH_ManufactureName != "")
                    {
                        ManufactureMaster ManufactureMaster = new ManufactureMaster();
                        ManufactureMaster.PH_ManufactureName = surgicalOthers.PH_ManufactureName.Trim().ToUpper();
                        ManufactureMaster.PH_MANF_Isactive = true;

                        long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                        surgicalOthers.PH_Manufacture = retManufacture;
                    }

                }
                surgicalOthers.PH_Rack = surgicalOthers.PH_Rack.Trim().ToUpper();
                Name = surgicalOthers.PH_Rack;
                rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);

                try
                {
                    if (rackResult.Count == 0)
                    {
                        if (surgicalOthers.PH_Rack != "")
                        {
                            RackMaster rackMaster = new RackMaster();
                            rackMaster.PH_Rack_Name = surgicalOthers.PH_Rack.Trim().ToUpper();
                            rackMaster.PH_Rack_IsActive = true;
                            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                            surgicalOthers.PH_Rack = surgicalOthers.PH_Rack.ToUpper();

                        }

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                surgicalOthers.PH_Package = surgicalOthers.PH_Package.Trim().ToUpper();
                Name = surgicalOthers.PH_Package;
                Result = _IDrugMasterRepo.GetDrugUomsByName(Name);
                try
                {
                    if (Result.Count == 0)
                    {
                        if (surgicalOthers.PH_Package != "")
                        {
                            DrugUom DrugUom = new DrugUom();
                            DrugUom.PH_UOMDESC = surgicalOthers.PH_Package.Trim().ToUpper();
                            DrugUom.PH_ISUOMACTIVE = true;
                            long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                        }


                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }


                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CreatedDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
                string CurDatetime = timezoneUtility.Gettimezone(Timezoneid);
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retvalue = _IDrugMasterRepo.CreateSurgicalOthers(surgicalOthers, CreatedDatetime, HospitalID);



            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return retvalue;
        }
        [HttpGet("GetSurgicalOthersByHospitalId")]

        public List<SurgicalOthers> GetSurgicalOthersByHospitalId()
        {
            List<SurgicalOthers> lstResult = new List<SurgicalOthers>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetSurgicalOthersByHospitalId(HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetDisposableBySeqId")]

        public List<SurgicalDisposable> GetDisposableBySeqId(long SeqId)
        {
            List<SurgicalDisposable> lstResult = new List<SurgicalDisposable>();
            try
            {
                lstResult = _IDrugMasterRepo.GetDisposableBySeqId(SeqId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetInstrumentsBySeqId")]

        public List<SurgicalInstruments> GetInstrumentsBySeqId(long SeqId)
        {
            List<SurgicalInstruments> lstResult = new List<SurgicalInstruments>();
            try
            {
                lstResult = _IDrugMasterRepo.GetInstrumentsBySeqId(SeqId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetOthersBySeqId")]

        public List<SurgicalOthers> GetOthersBySeqId(long SeqId)
        {
            List<SurgicalOthers> lstResult = new List<SurgicalOthers>();
            try
            {
                lstResult = _IDrugMasterRepo.GetOthersBySeqId(SeqId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }

        [HttpPost("DeleteSurgicalDisposableBySeqId")]

        public int DeleteSurgicalDisposableBySeqId(long SeqId)
        {
            int i = 0;

            try
            {
                i = _IDrugMasterRepo.DeleteSurgicalDisposableBySeqId(SeqId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return i;
        }
        [HttpPost("DeleteSurgicalOthersBySeqId")]

        public int DeleteSurgicalOthersBySeqId(long SeqId)
        {
            int i = 0;

            try
            {
                i = _IDrugMasterRepo.DeleteSurgicalOthersBySeqId(SeqId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return i;
        }
        [HttpPost("DeleteSurgicalInstrumentsBySeqId")]

        public int DeleteSurgicalInstrumentsBySeqId(long SeqId)
        {
            int i = 0;

            try
            {
                i = _IDrugMasterRepo.DeleteSurgicalInstrumentsBySeqId(SeqId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return i;
        }
        [HttpPost("UpdateSurgicalDisposable")]
        public int UpdateSurgicalDisposable([FromBody] SurgicalDisposable surgicalDisposable)
        {
            int retvalue = 0;
            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugUom> Result = new List<DrugUom>();
            string Name;
            surgicalDisposable.PH_ManufactureName = surgicalDisposable.PH_ManufactureName.Trim().ToUpper();
            Name = surgicalDisposable.PH_ManufactureName;
            ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
            try
            {
                if (ManufactureResult.Count == 0)
                {
                    if (surgicalDisposable.PH_ManufactureName != "")
                    {
                        ManufactureMaster ManufactureMaster = new ManufactureMaster();
                        ManufactureMaster.PH_ManufactureName = surgicalDisposable.PH_ManufactureName.Trim().ToUpper();
                        ManufactureMaster.PH_MANF_Isactive = true;

                        long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                        surgicalDisposable.PH_Manufacture = retManufacture;
                    }

                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            surgicalDisposable.PH_Rack = surgicalDisposable.PH_Rack.Trim().ToUpper();
            Name = surgicalDisposable.PH_Rack;
            rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);

            try
            {
                if (rackResult.Count == 0)
                {
                    if (surgicalDisposable.PH_Rack != "")
                    {
                        RackMaster rackMaster = new RackMaster();
                        rackMaster.PH_Rack_Name = surgicalDisposable.PH_Rack.Trim().ToUpper();
                        rackMaster.PH_Rack_IsActive = true;
                        long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                        surgicalDisposable.PH_Rack = surgicalDisposable.PH_Rack.ToUpper();

                    }

                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            surgicalDisposable.PH_Package = surgicalDisposable.PH_Package.Trim().ToUpper();
            Name = surgicalDisposable.PH_Package;
            Result = _IDrugMasterRepo.GetDrugUomsByName(Name);
            try
            {
                if (Result.Count == 0)
                {
                    if (surgicalDisposable.PH_Package != "")
                    {
                        DrugUom DrugUom = new DrugUom();
                        DrugUom.PH_UOMDESC = surgicalDisposable.PH_Package.Trim().ToUpper();
                        DrugUom.PH_ISUOMACTIVE = true;
                        long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                    }


                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string modifiedDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
                retvalue = _IDrugMasterRepo.UpdateSurgicalDisposable(surgicalDisposable, modifiedDatetime);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return retvalue;
        }
        [HttpPost("UpdateSurgicalInstruments")]
        public int UpdateSurgicalInstruments([FromBody] SurgicalInstruments surgicalInstruments)

        {
            int retvalue = 0;
            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugUom>Result = new List<DrugUom>();
            string Name;
            surgicalInstruments.PH_ManufactureName = surgicalInstruments.PH_ManufactureName.Trim().ToUpper();
            Name = surgicalInstruments.PH_ManufactureName;
            ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
            try
            {
                if (ManufactureResult.Count == 0)
                {
                    if (surgicalInstruments.PH_ManufactureName != "")
                    {
                        ManufactureMaster ManufactureMaster = new ManufactureMaster();
                        ManufactureMaster.PH_ManufactureName = surgicalInstruments.PH_ManufactureName.Trim().ToUpper();
                        ManufactureMaster.PH_MANF_Isactive = true;

                        long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                        surgicalInstruments.PH_Manufacture = retManufacture;
                    }

                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            surgicalInstruments.PH_Rack = surgicalInstruments.PH_Rack.Trim().ToUpper();
            Name = surgicalInstruments.PH_Rack;
            rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);

            try
            {
                if (rackResult.Count == 0)
                {
                    if (surgicalInstruments.PH_Rack != "")
                    {
                        RackMaster rackMaster = new RackMaster();
                        rackMaster.PH_Rack_Name = surgicalInstruments.PH_Rack.Trim().ToUpper();
                        rackMaster.PH_Rack_IsActive = true;
                        long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                        surgicalInstruments.PH_Rack = surgicalInstruments.PH_Rack.ToUpper();

                    }

                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            surgicalInstruments.PH_Package = surgicalInstruments.PH_Package.Trim().ToUpper();
            Name = surgicalInstruments.PH_Package;
            Result = _IDrugMasterRepo.GetDrugUomsByName(Name);
            try
            {
                if (Result.Count == 0)
                {
                    if (surgicalInstruments.PH_Package != "")
                    {
                        DrugUom DrugUom = new DrugUom();
                        DrugUom.PH_UOMDESC = surgicalInstruments.PH_Package.Trim().ToUpper();
                        DrugUom.PH_ISUOMACTIVE = true;
                        long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                    }


                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string modifiedDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
                retvalue = _IDrugMasterRepo.UpdateSurgicalInstruments(surgicalInstruments, modifiedDatetime);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return retvalue;
        }
        [HttpPost("UpdateSurgicalOthers")]
        public int UpdateSurgicalOthers([FromBody] SurgicalOthers surgicalOthers)

        {
            int retvalue = 0;
            List<ManufactureMaster> ManufactureResult = new List<ManufactureMaster>();
            List<RackMaster> rackResult = new List<RackMaster>();
            List<DrugUom> Result = new List<DrugUom>();
            string Name;
            surgicalOthers.PH_ManufactureName = surgicalOthers.PH_ManufactureName.Trim().ToUpper();
            Name = surgicalOthers.PH_ManufactureName;
            ManufactureResult = _IDrugMasterRepo.GetManufactureByName(Name);
            try
            {
                if (ManufactureResult.Count == 0)
                {
                    if (surgicalOthers.PH_ManufactureName != "")
                    {
                        ManufactureMaster ManufactureMaster = new ManufactureMaster();
                        ManufactureMaster.PH_ManufactureName = surgicalOthers.PH_ManufactureName.Trim().ToUpper();
                        ManufactureMaster.PH_MANF_Isactive = true;

                        long retManufacture = _IDrugMasterRepo.CreateManufactureMaster(ManufactureMaster);

                        surgicalOthers.PH_Manufacture = retManufacture;
                    }

                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            surgicalOthers.PH_Rack = surgicalOthers.PH_Rack.Trim().ToUpper();
            Name = surgicalOthers.PH_Rack;
            rackResult = _IDrugMasterRepo.GetRackMasterDetailsByName(Name);

            try
            {
                if (rackResult.Count == 0)
                {
                    if (surgicalOthers.PH_Rack != "")
                    {
                        RackMaster rackMaster = new RackMaster();
                        rackMaster.PH_Rack_Name = surgicalOthers.PH_Rack.Trim().ToUpper();
                        rackMaster.PH_Rack_IsActive = true;
                        long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        long retrack = _IDrugMasterRepo.CreateRackMaster(rackMaster, Hospitalid);
                        surgicalOthers.PH_Rack = surgicalOthers.PH_Rack.ToUpper();

                    }

                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            surgicalOthers.PH_Package = surgicalOthers.PH_Package.Trim().ToUpper();
            Name = surgicalOthers.PH_Package;
            Result = _IDrugMasterRepo.GetDrugUomsByName(Name);
            try
            {
                if (Result.Count == 0)
                {
                    if (surgicalOthers.PH_Package != "")
                    {
                        DrugUom DrugUom = new DrugUom();
                        DrugUom.PH_UOMDESC = surgicalOthers.PH_Package.Trim().ToUpper();
                        DrugUom.PH_ISUOMACTIVE = true;
                        long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        int retResult = _IDrugMasterRepo.CreateDrugUom(DrugUom, Hospitalid);
                    }


                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string modifiedDatetime = timezoneUtility.GetDateBytimezone(Timezoneid) + " " + timezoneUtility.GettimeBytimezone(Timezoneid);
                retvalue = _IDrugMasterRepo.UpdateSurgicalOthers(surgicalOthers, modifiedDatetime);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return retvalue;
        }
        [HttpGet("GetItemMasterBySearch")]
        public JsonResult GetItemMasterBySearch(string ItemMaster, string Search)
        {
            List<DrugMaster> lstResult = new List<DrugMaster>();
            List<SurgicalDisposable> lstResultDis = new List<SurgicalDisposable>();
            List<SurgicalInstruments> lstResultIns = new List<SurgicalInstruments>();
            List<SurgicalOthers> lstResultOthrs = new List<SurgicalOthers>();
            string Search1 = "";
            string Search2 = "";

            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                //EmptySearch = "Empty string ";
                if (Search != null)
                {

                    var itemSearch = Search.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < itemSearch.Length; i++)
                    {
                        if (i == 0)
                        {
                            Search = itemSearch[0];
                        }
                        if (i == 1)
                        {
                            Search1 = itemSearch[1];
                        }
                        // if (i == 2)
                        //{
                        //    Search2 = itemSearch[2];
                        //}
                    }

                }
                else

                {


                    Search = "";
                    Search1 = "";
                }
                if (ItemMaster == "Medicines")
                {
                    lstResult = _IDrugMasterRepo.GetDrugMasterBySearch(Search, Search1, ItemMaster, HospitalId);
                    return Json(lstResult);
                }
                if (ItemMaster == "SurgicalDisposable")
                {
                    lstResultDis = _IDrugMasterRepo.GetSDisposableBySearch(Search, Search1, ItemMaster, HospitalId);
                    return Json(lstResultDis);
                }
                if (ItemMaster == "SurgicalInstruments")
                {
                    lstResultIns = _IDrugMasterRepo.GetSInstrumentsBySearch(Search, Search1, ItemMaster, HospitalId);
                    return Json(lstResultIns);
                }
                if (ItemMaster == "Others")
                {
                    lstResultOthrs = _IDrugMasterRepo.GetSOthersBySearch(Search, Search1, ItemMaster, HospitalId);
                    return Json(lstResultOthrs);
                }

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpGet("GetDrugCategoriesBySearch")]
        public List<DrugCategory> GetDrugCategoriesBySearch(string Search)
        {
            List<DrugCategory> lstResult = new List<DrugCategory>();

            try
            {
                lstResult = _IDrugMasterRepo.GetDrugCategoryBySearch(Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());


            }
            return lstResult;


        }
        [HttpGet("GetRackMasterDetailsBySearch")]
        public List<RackMaster> GetRackMasterDetailsBySearch(string Search)
        {
            List<RackMaster> lstResult = new List<RackMaster>();

            try
            {
                lstResult = _IDrugMasterRepo.GetRackMasterDetailsBySearch(Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());


            }
            return lstResult;
        }
        [HttpGet("GetDrugFormDetailsBySearch")]
        public List<DrugForm> GetDrugFormDetailsBySearch(string Search)
        {
            List<DrugForm> lstResult = new List<DrugForm>();

            try
            {
                lstResult = _IDrugMasterRepo.GetDrugFormDetailsBySearch(Search);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());


            }
            return lstResult;
        }
        [HttpGet("GetExistDrugData")]
        public List<DrugMaster> GetExistDrugData(string DrugName , string drugCategory,string Strength,int DrugCode)
        {
            List<DrugMaster> lstResult = new List<DrugMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _IDrugMasterRepo.GetExistDrugData(DrugName, drugCategory, Strength, Hospitalid,DrugCode);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());


            }
            return lstResult;
        }

        #region Habib
        [HttpGet("GetItemCategoryList")]
        public List<ItemCategoryMaster> GetItemCategoryList()
        {
            List<ItemCategoryMaster> lstItemCategory = new List<ItemCategoryMaster>();
            try
            {
                lstItemCategory = _IDrugMasterRepo.GetItemCategoryList();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("GetItemCategoryList " + ex.ToString());
            }

            return lstItemCategory;
        }
        [HttpGet("GetScheduleTypeReportData")]
        public List<CashBillPatInfo> GetScheduleTypeReportData(string ScheduleType, string FromDate, string ToDate)
        {
            List<CashBillPatInfo> lstSchedule = new List<CashBillPatInfo>();
            string FromDateTime = "";
            string ToDateTime = "";
            try
            {
                FromDateTime = FromDate + " 00:00:00";
                ToDateTime = ToDate + " 23:59:59";
                lstSchedule = _cashBillRepo.GetScheduleTypeReportData(ScheduleType, FromDateTime, ToDateTime);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("GetScheduleTypeReportData " + ex.ToString());
            }

            return lstSchedule;
        }
        [HttpGet("GetScheduleTypeList")]
        public List<ScheduleTypeMaster> GetScheduleTypeList()
        {
            List<ScheduleTypeMaster> lstScheduleType = new List<ScheduleTypeMaster>();
            long HospitalId = 0;
            try
            {
                HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstScheduleType = _IDrugMasterRepo.GetScheduleTypeList(HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("GetScheduleTypeList " + ex.ToString());
            }

            return lstScheduleType;
        }
        [HttpGet("GetScheduleTypeHtmlPrint")]
        public string GetScheduleTypeHtmlPrint(string ScheduleType, string FromDate, string ToDate)
        {
            List<CashBillPatInfo> lstSchedule = new List<CashBillPatInfo>();
            string FromDateTime = "";
            string ToDateTime = "";
            string Html = "";
            StringBuilder strHTMLBuilder = new StringBuilder();
            string Headerfiles = "";

            try
            {
                FromDateTime = FromDate + " 00:00:00";
                ToDateTime = ToDate + " 23:59:59";
                lstSchedule = _cashBillRepo.GetScheduleTypeReportData(ScheduleType, FromDateTime, ToDateTime);
                if (lstSchedule.Count > 0)
                {

                    strHTMLBuilder.Append("<html>");
                    strHTMLBuilder.Append("<head>");
                    strHTMLBuilder.Append(" <style> .section { display: table; width: 100%;  } .section > div {display: table-cell; overflow: hidden; } .section ~ .section > div:before { content: '';  display: block;  margin-bottom: -10.5em; / inverse of header height /; } .section > div > div { page-break-inside: avoid; display: inline-block; width: 100%; vertical-align: top;  } .headers { height: 10.5em; / header must have a fixed height /; }.content {/ this rule set just adds space between sections and is not required / margin-bottom: 1.25em; } </style>");
                    strHTMLBuilder.Append("</head>");
                    strHTMLBuilder.Append("<body>");

                    for (int dtl = 0; dtl < lstSchedule.Count(); dtl++)
                    {

                    }

                    strHTMLBuilder.Append("</body>");

                    Html = strHTMLBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return Html;
        }
        #endregion

    }
}
