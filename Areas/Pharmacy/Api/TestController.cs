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
using System.Threading.Tasks;

namespace Emr_web.Areas.Pharmacy.Api
{
    [Produces("application/json")]
    [Area("Pharmacy")]
    [Route("api/TestApi")]
    [ApiController]
    public class TestController : Controller
    {
        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly ISupplierMasterRepo _supplierMasterRepo;
        private readonly ICountryRepo _ICountryRepo;
        private readonly IStateRepo _IStateRepo;
        private readonly ICityRepo _cityRepo;


        public TestController(IDBConnection dBConnection, IErrorlog errorlog, ISupplierMasterRepo supplierMasterRepo, ICountryRepo iCountryRepo, IStateRepo iStateRepo, ICityRepo cityRepo)
        {

            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _supplierMasterRepo = supplierMasterRepo;
            _ICountryRepo = iCountryRepo;
            _IStateRepo = iStateRepo;
            _cityRepo = cityRepo;
        }
        [HttpPost("CreateSupplierMaster")]
        public long CreateSupplierMaster([FromBody] SupplierMaster supplierMaster)
        {
            long retvalue = 0;
            List<SupplierMaster> lstResult = new List<SupplierMaster>();
            List<CountryMaster> CountryResult = new List<CountryMaster>();
            List<StateMaster> StateResult = new List<StateMaster>();
            List<CityMaster> CityResult = new List<CityMaster>();


            try
            {
                supplierMaster.CountryName = supplierMaster.CountryName.Trim().ToUpper();
                string Country = supplierMaster.CountryName;
                CountryResult = _supplierMasterRepo.GetCountryByName(Country);

                if (CountryResult.Count == 0)
                {
                    try
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                            Timezoneid = "India Standard Time";
                        CountryMaster countryMaster = new CountryMaster();
                        countryMaster.CountryName = supplierMaster.CountryName;
                        countryMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.Isactive = true;
                        int Result = _ICountryRepo.CreateNewCountry(countryMaster);
                        supplierMaster.PH_Supplier_CountryId = Result;

                    }
                    catch (Exception ex)
                    {
                        _errorlog.WriteErrorLog(ex.ToString());
                    }
                }
                else
                {
                    long CountrySeqId = CountryResult[0].CountrySeqId;
                    supplierMaster.PH_Supplier_CountryId = CountrySeqId;

                }
                supplierMaster.PH_Supplier_State = supplierMaster.PH_Supplier_State.Trim().ToUpper();
                string State = supplierMaster.PH_Supplier_State;
                StateResult = _supplierMasterRepo.GetStateByName(State);
                if (StateResult.Count == 0)
                {
                    try
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                            Timezoneid = "India Standard Time";
                        StateMaster stateMaster = new StateMaster();
                        stateMaster.StateName = supplierMaster.PH_Supplier_State;
                        stateMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        stateMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        stateMaster.Isactive = true;
                        int result = _IStateRepo.CreateNewState(stateMaster);
                        supplierMaster.PH_Supplier_StateId = result;
                    }

                    catch (Exception ex)
                    {
                        _errorlog.WriteErrorLog(ex.ToString());
                    }
                }
                else
                {
                    long StateSeqID = StateResult[0].StateSeqID;
                    supplierMaster.PH_Supplier_StateId = StateSeqID;

                }

                supplierMaster.CityName = supplierMaster.CityName.Trim().ToUpper();
                string City = supplierMaster.CityName;
                CityResult = _supplierMasterRepo.GetCityByName(City);
                if (CityResult.Count == 0)
                {
                    try
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                        {
                            Timezoneid = "India Standard Time";
                        }
                        CityMaster cityMaster = new CityMaster();
                        cityMaster.CityName = supplierMaster.CityName;
                        cityMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        cityMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                        cityMaster.Isactive = true;
                        int result = _cityRepo.CreateNewCity(cityMaster);
                        supplierMaster.PH_Supplier_CityId = result;


                    }
                    catch (Exception ex)
                    {
                        _errorlog.WriteErrorLog(ex.ToString());
                    }

                }
                else
                {
                    long CitySeqID = CityResult[0].CitySeqID;
                    supplierMaster.PH_Supplier_CityId = CitySeqID;

                }
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                supplierMaster.PH_SupplierName = supplierMaster.PH_SupplierName.Trim().ToUpper();
                retvalue = _supplierMasterRepo.CreateSupplierMaster(supplierMaster, Hospitalid);
                //supplierMaster.PH_SupplierID = retvalue;
                //long value = _supplierMasterRepo.CreateContactDetails(supplierMaster);


            }

            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retvalue;
        }
        [HttpGet("GetSupplierDetails")]
        public List<SupplierMaster> GetSupplierDetails()
        {
            List<SupplierMaster> lstResult = new List<SupplierMaster>();
            try
            {
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _supplierMasterRepo.GetSupplierDetails(Hospitalid);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetSupplierDetailsBySeqId")]
        public List<SupplierMaster> GetSupplierDetailsBySeqId(long SupplierId)
        {
            List<SupplierMaster> lstResult = new List<SupplierMaster>();
            try
            {

                lstResult = _supplierMasterRepo.GetSupplierDetailsBySeqId(SupplierId);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpPost("UpdateSupplierMaster")]
        public long UpdateSupplierMaster([FromBody] SupplierMaster supplierMaster)
        {
            long retvalue = 0;

            List<SupplierMaster> lstResult = new List<SupplierMaster>();
            List<CountryMaster> CountryResult = new List<CountryMaster>();
            List<StateMaster> StateResult = new List<StateMaster>();
            List<CityMaster> CityResult = new List<CityMaster>();


            try
            {
                supplierMaster.CountryName = supplierMaster.CountryName.Trim().ToUpper();
                string Country = supplierMaster.CountryName;
                CountryResult = _supplierMasterRepo.GetCountryByName(Country);

                if (CountryResult.Count == 0)
                {
                    try
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                            Timezoneid = "India Standard Time";
                        CountryMaster countryMaster = new CountryMaster();
                        countryMaster.CountryName = supplierMaster.CountryName;
                        countryMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.Isactive = true;
                        int Result = _ICountryRepo.CreateNewCountry(countryMaster);
                        supplierMaster.PH_Supplier_CountryId = Result;

                    }
                    catch (Exception ex)
                    {
                        _errorlog.WriteErrorLog(ex.ToString());
                    }
                }
                else
                {
                    long CountrySeqId = CountryResult[0].CountrySeqId;
                    supplierMaster.PH_Supplier_CountryId = CountrySeqId;

                }
                supplierMaster.PH_Supplier_State = supplierMaster.PH_Supplier_State.Trim().ToUpper();
                string State = supplierMaster.PH_Supplier_State;
                StateResult = _supplierMasterRepo.GetStateByName(State);
                if (StateResult.Count == 0)
                {
                    try
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                            Timezoneid = "India Standard Time";
                        StateMaster stateMaster = new StateMaster();
                        stateMaster.StateName = supplierMaster.PH_Supplier_State;
                        stateMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        stateMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        stateMaster.Isactive = true;
                        int result = _IStateRepo.CreateNewState(stateMaster);
                        supplierMaster.PH_Supplier_StateId = result;
                    }

                    catch (Exception ex)
                    {
                        _errorlog.WriteErrorLog(ex.ToString());
                    }
                }
                else
                {
                    long StateSeqID = StateResult[0].StateSeqID;
                    supplierMaster.PH_Supplier_StateId = StateSeqID;

                }

                supplierMaster.CityName = supplierMaster.CityName.Trim().ToUpper();
                string City = supplierMaster.CityName;
                CityResult = _supplierMasterRepo.GetCityByName(City);
                if (CityResult.Count == 0)
                {
                    try
                    {
                        TimezoneUtility timezoneUtility = new TimezoneUtility();
                        string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                        if (Timezoneid == "" || Timezoneid == null)
                        {
                            Timezoneid = "India Standard Time";
                        }

                        CityMaster cityMaster = new CityMaster();
                        cityMaster.CityName = supplierMaster.CityName;
                        cityMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        cityMaster.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                        cityMaster.Isactive = true;
                        int result = _cityRepo.CreateNewCity(cityMaster);
                        supplierMaster.PH_Supplier_CityId = result;

                    }
                    catch (Exception ex)
                    {
                        _errorlog.WriteErrorLog(ex.ToString());
                    }

                }
                else
                {
                    long CitySeqID = CityResult[0].CitySeqID;
                    supplierMaster.PH_Supplier_CityId = CitySeqID;

                }
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                supplierMaster.PH_SupplierName = supplierMaster.PH_SupplierName.Trim().ToUpper();
                retvalue = _supplierMasterRepo.UpdateSupplierMaster(supplierMaster, Hospitalid);

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retvalue;
        }
        [HttpPost("CreateContactDetails")]
        public long CreateContactDetails(SupplierMaster supplierMaster)
        {
            long retvalue = 0;
            try
            {
                retvalue = _supplierMasterRepo.CreateContactDetails(supplierMaster);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return retvalue;
        }
        [HttpGet("GetSupplierContactDetails")]
        public List<SupplierMaster> GetSupplierContactDetails(long SupplierId)
        {
            List<SupplierMaster> lstresult = new List<SupplierMaster>();

            try
            {
                lstresult = _supplierMasterRepo.GetSupplierContactDetails(SupplierId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return lstresult;
        }
        [HttpGet("GetSupplierContactDetailsById")]
        public List<SupplierMaster> GetSupplierContactDetailsById(long PH_ContactDetailId)
        {
            List<SupplierMaster> lstresult = new List<SupplierMaster>();

            try
            {
                lstresult = _supplierMasterRepo.GetSupplierContactDetailsById(PH_ContactDetailId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return lstresult;
        }
        [HttpPost("UpdateContactDetails")]
        public long UpdateContactDetails(SupplierMaster supplierMaster)
        {
            long retvalue = 0;
            try
            {
                retvalue = _supplierMasterRepo.UpdateContactDetails(supplierMaster);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return retvalue;
        }
        [HttpPost("DeletSupplierDetail")]

        public int DeletSupplierDetail(long PH_SupplierID)
        {
            int retValue = 0;
            try
            {
                retValue = _supplierMasterRepo.DeletSupplierDetail(PH_SupplierID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return retValue;
        }
        [HttpPost("DeletContactDetail")]
        public int DeletContactDetail(long PH_ContactDetailId)
        {
            int retValue = 0;
            try
            {
                retValue = _supplierMasterRepo.DeletContactDetail(PH_ContactDetailId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }

            return retValue;
        }
        [HttpGet("SearchSupplierBySupplierName")]
        public List<SupplierMaster> SearchSupplierBySupplierName(string Search)
        {
            List<SupplierMaster> lstresult = new List<SupplierMaster>();

            try
            {
                if (Search == null)
                {
                    Search = "";
                }
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstresult = _supplierMasterRepo.SearchSupplierBySupplierName(Search, Hospitalid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());

            }
            return lstresult;
        }

        #region Habib
        [HttpGet("IsSupplierExists")]
        public bool IsSupplierExists(string SupplierName, string CityName, string GstNo, string Address1,string Address2,long SupplierId)
        {
            bool result = false;
            List<SupplierMaster> lstSupplier = new List<SupplierMaster>();
            try
            {
                if (Address1 == null)
                {
                    Address1 = "";
                }
                if (Address2 == null)
                {
                    Address2 = "";
                }
                lstSupplier = _supplierMasterRepo.IsSupplierExists(SupplierName, CityName, GstNo,Address1,Address2,SupplierId);
                if (lstSupplier.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog("IsSupplierExists " + ex.ToString());
            }

            return result;
        }
        #endregion

    }
}
