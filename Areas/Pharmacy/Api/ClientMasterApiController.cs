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
    [Route("api/ClientMaster")]
    [ApiController]
    public class ClientMasterApiController : Controller
    {

        private readonly IDBConnection _dBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IClientMasterRepo _clientMasterRepo;
        private ICityRepo _cityRepo;
        private IStateRepo _stateRepo;
        private ICountryRepo _CountryRepo;
        public ClientMasterApiController(IDBConnection dBConnection, IErrorlog errorlog, IClientMasterRepo clientMasterRepo, ICityRepo cityRepo
            , IStateRepo stateRepo, ICountryRepo countryRepo)
        {
            _dBConnection = dBConnection;
            _errorlog = errorlog;
            _clientMasterRepo = clientMasterRepo;
            _cityRepo = cityRepo;
            _stateRepo = stateRepo;
            _CountryRepo = countryRepo;
        }
        [HttpPost("CreateClientMaster")]
        public long CreateClientMaster([FromBody] ClientMaster clientMaster)
        {
            List<CityMaster> cityResult = new List<CityMaster>();
            List<CountryMaster> countryResult = new List<CountryMaster>();
            List<StateMaster> stateResult = new List<StateMaster>();

            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";

            long retValue = 0;
            string CityName = "";
            string CountryName = "";
            bool Isactive = false;
            try
            {
                try
                {
                    if (clientMaster.CountryName != "")
                    {
                        CountryName = clientMaster.CountryName;
                        countryResult = _CountryRepo.GetCountryByName(CountryName);
                    }
                    if(countryResult.Count() > 0)
                    {
                        clientMaster.CountryID = countryResult[0].CountrySeqId;
                    }

                        if (countryResult.Count() == 0 && clientMaster.CountryID == 0 && clientMaster.CountryName != "")
                    {
                       
                        CountryMaster countryMaster = new CountryMaster();
                        countryMaster.CountryName = clientMaster.CountryName.Trim().ToUpper();
                        countryMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        countryMaster.Isactive = Isactive = true;
                        retValue = _CountryRepo.CreateNewCountry(countryMaster);
                        clientMaster.CountryID = retValue;
                    }

                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                try
                {
                    if(clientMaster.StateName != "" )
                    {
                     string  StateName = clientMaster.StateName;
                      stateResult = _stateRepo.GetStateByName(StateName);
                        
                    }
                    if(stateResult.Count() > 0)
                    {
                        clientMaster.StateID = stateResult[0].StateSeqID;
                    }

                    if (stateResult.Count() == 0 && clientMaster.StateID == 0 && clientMaster.StateName != "" )
                    {
                        StateMaster stateMaster = new StateMaster();
                        stateMaster.StateName = clientMaster.StateName.Trim().ToUpper();
                        stateMaster.CountrySeqID = clientMaster.CountryID;
                        stateMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        stateMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        stateMaster.Isactive = Isactive = true;
                        retValue = _stateRepo.CreateNewState(stateMaster);
                        clientMaster.StateID = retValue;

                    }
                }
                catch(Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                try
                {
                    if (clientMaster.CityName != "")
                    {
                        CityName = clientMaster.CityName;
                        cityResult = _cityRepo.GetCityByName(CityName);
                        

                    }
                    if(cityResult.Count() > 0)
                    {
                        clientMaster.CityID = cityResult[0].CitySeqID;
                    }

                    if (cityResult.Count() == 0 && clientMaster.CityID == 0 && clientMaster.CityName != "")
                    {
                        CityMaster cityMaster = new CityMaster();
                        cityMaster.CityName = clientMaster.CityName.Trim().ToUpper();
                        cityMaster.CountrySeqID = clientMaster.CountryID;
                        cityMaster.StateSeqID = clientMaster.StateID;
                        cityMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        cityMaster.Isactive = Isactive = true;
                        retValue = _cityRepo.CreateNewCity(cityMaster);
                        clientMaster.CityID = retValue;
                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }


                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retValue = _clientMasterRepo.CreateClientMaster(clientMaster, HospitalId);
                if (retValue > 0)
                {
                    for (var j = 0; j < clientMaster.warehouseMappings.Count(); j++)
                    {
                        ClientWarehouseMapping clientWarehouseMapping = new ClientWarehouseMapping();
                        clientWarehouseMapping.ClientID = retValue;
                        clientWarehouseMapping.WarehouseID = clientMaster.warehouseMappings[j].WarehouseID;
                        clientWarehouseMapping.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        clientWarehouseMapping.IsActive = true;
                        _clientMasterRepo.CreateWarehouseMapping(clientWarehouseMapping);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retValue;
        }
        [HttpPost("UpdateClientMaster")]
        public int UpdateClientMaster([FromBody] ClientMaster clientMaster)
        {
            List<CityMaster> cityResult = new List<CityMaster>();
            List<CountryMaster> countryResult = new List<CountryMaster>();
            List<StateMaster> stateResult = new List<StateMaster>();

            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";

          
            string CityName = "";
            string CountryName = "";
            bool Isactive = false;
            int retValue = 0;
            try
            {
                try
                {
                    if (clientMaster.CountryName != "")
                    {
                        CountryName = clientMaster.CountryName;
                        countryResult = _CountryRepo.GetCountryByName(CountryName);
                    }
                    if (countryResult.Count() > 0)
                    {
                        clientMaster.CountryID = countryResult[0].CountrySeqId;
                    }

                    if (countryResult.Count() == 0 && clientMaster.CountryID == 0 && clientMaster.CountryName != "")
                    {

                        CountryMaster countryMaster = new CountryMaster();
                        countryMaster.CountryName = clientMaster.CountryName.Trim().ToUpper();
                        countryMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        countryMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        countryMaster.Isactive = Isactive = true;
                        retValue = _CountryRepo.CreateNewCountry(countryMaster);
                        clientMaster.CountryID = retValue;
                    }

                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                try
                {
                    if (clientMaster.StateName != "")
                    {
                        string StateName = clientMaster.StateName;
                        stateResult = _stateRepo.GetStateByName(StateName);

                    }
                    if (stateResult.Count() > 0)
                    {
                        clientMaster.StateID = stateResult[0].StateSeqID;
                    }

                    if (stateResult.Count() == 0 && clientMaster.StateID == 0 && clientMaster.StateName != "")
                    {
                        StateMaster stateMaster = new StateMaster();
                        stateMaster.StateName = clientMaster.StateName.Trim().ToUpper();
                        stateMaster.CountrySeqID = clientMaster.CountryID;
                        stateMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        stateMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        stateMaster.Isactive = Isactive = true;
                        retValue = _stateRepo.CreateNewState(stateMaster);
                        clientMaster.StateID = retValue;

                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                try
                {
                    if (clientMaster.CityName != "")
                    {
                        CityName = clientMaster.CityName;
                        cityResult = _cityRepo.GetCityByName(CityName);


                    }
                    if (cityResult.Count() > 0)
                    {
                        clientMaster.CityID = cityResult[0].CitySeqID;
                    }

                    if (cityResult.Count() == 0 && clientMaster.CityID == 0 && clientMaster.CityName != "")
                    {
                        CityMaster cityMaster = new CityMaster();
                        cityMaster.CityName = clientMaster.CityName.Trim().ToUpper();
                        cityMaster.CountrySeqID = clientMaster.CountryID;
                        cityMaster.StateSeqID = clientMaster.StateID;
                        cityMaster.CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid);
                        cityMaster.CreatedUser = HttpContext.Session.GetString("Userseqid");
                        cityMaster.Isactive = Isactive = true;
                        retValue = _cityRepo.CreateNewCity(cityMaster);
                        clientMaster.CityID = retValue;
                    }
                }
                catch (Exception ex)
                {
                    _errorlog.WriteErrorLog(ex.ToString());
                }
                retValue = _clientMasterRepo.UpdateClientMaster(clientMaster);
                if (clientMaster.warehouseMappings.Length > 0)
                {
                    List<ClientWarehouseMapping> Lists = _clientMasterRepo.GetWarehouseMappingByCodeId(clientMaster.SeqID);

                    foreach (ClientWarehouseMapping i in clientMaster.warehouseMappings)
                    {
                        bool IsExists = false;
                        long WarehouseSeqId = i.WarehouseID;
                        
                        for (int j = 0; j < Lists.Count(); j++)
                        {
                            if (Lists[j].WarehouseID == WarehouseSeqId)
                            {
                                IsExists = true;
                                Lists[j].IsActive = true;
                            }
                        }
                        if (!IsExists)
                        {
                            ClientWarehouseMapping clientWarehouseMapping = new ClientWarehouseMapping();
                            clientWarehouseMapping.ClientID = clientMaster.SeqID;
                            clientWarehouseMapping.WarehouseID = WarehouseSeqId;
                            clientWarehouseMapping.CreatedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            clientWarehouseMapping.IsActive = true;
                            _clientMasterRepo.CreateWarehouseMapping(clientWarehouseMapping);
                        }
                    }
                    for (int j = 0; j < Lists.Count(); j++)
                    {
                        int h = j;
                        if (Lists[j].IsActive == false)
                        {
                            _clientMasterRepo.DeleteWarehouseFormula(Lists[j].SeqID, Lists[j].WarehouseID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retValue;
        }
        [HttpGet("GetClientDetailsByHospitalId")]
        public List<ClientMaster> GetClientDetailsByHospitalId()
        {
            List<ClientMaster> lstResult = new List<ClientMaster>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _clientMasterRepo.GetClientDetailsByHospitalId(HospitalId);


            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }

        [HttpGet("GetClientDetailsBySeqId")]
        public List<ClientMaster> GetClientDetailsBySeqId(long SeqId)
        {
            List<ClientMaster> lstResult = new List<ClientMaster>();
            List<ClientWarehouseMapping> SubResult = new List<ClientWarehouseMapping>();
            try
            {
                lstResult = _clientMasterRepo.GetClientDetailsBySeqId(SeqId);
                SubResult = _clientMasterRepo.GetSubstanceMappingByCodeId(SeqId);
                lstResult[0].warehouseMappings = SubResult.ToArray();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetCity")]
        public async Task<IEnumerable<CityMaster>> GetCity()
        {
            List<CityMaster> lstResult = new List<CityMaster>();
            try
            {
                var lstcity = await _cityRepo.GetAllcity();
                if (lstcity != null)
                {
                    lstResult = lstcity as List<CityMaster>;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetState")]
        public async Task<IEnumerable<StateMaster>> GetState()
        {
            List<StateMaster> lstResult = new List<StateMaster>();
            try
            {
                var lst = await _stateRepo.GetAllstate();
                if (lst != null)
                {
                    lstResult = lst as List<StateMaster>;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetCountry")]
        public async Task<IEnumerable<CountryMaster>> GetCountry()
        {
            List<CountryMaster> lstResult = new List<CountryMaster>();
            try
            {
                var lstCountry = await _CountryRepo.GetAllCountry();
                if (lstCountry != null)
                {
                    lstResult = lstCountry as List<CountryMaster>;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        
        [HttpGet("GetStateCountryByCityID")]
        public List<CityMaster> GetStateCountryByCityID(long CityID)
        {
            List<CityMaster> lstResult = new List<CityMaster>();
            try
            {
                lstResult = _clientMasterRepo.GetStateCountryByCityID(CityID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
    
        [HttpGet("GetCountryByStateID")]
        public List<StateMaster> GetCountryByStateID(long StateID)
        {
            List<StateMaster> lstResult = new List<StateMaster>();
            try
            {
                lstResult = _clientMasterRepo.GetCountryByStateID(StateID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpPost("CreateWarehouseMaster")]
        public int CreateWarehouseMaster([FromBody] StoreNameInfo storeNameInfo)
        {
            int retValue = 0;
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                retValue = _clientMasterRepo.CreateWarehouseMaster(storeNameInfo, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retValue;
        }
        [HttpPost("UpdateWarehouseMaster")]
        public int UpdateWarehouseMaster([FromBody] StoreNameInfo storeNameInfo)
        {
            int retValue = 0;
            try
            {
                retValue = _clientMasterRepo.UpdateWarehouseMaster(storeNameInfo);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return retValue;
        }
        [HttpGet("GetCityDetailsByCityName")]
        public List<CityMaster> GetCityDetailsByCityName(string CityName)
        {
            List<CityMaster> lstResult = new List<CityMaster>();
            try
            {
                lstResult = _cityRepo.GetCityByName(CityName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetStateDetailsByStateName")]
        public List<StateMaster> GetStateDetailsByStateName(string StateName)
        {
            List<StateMaster> lstResult = new List<StateMaster>();
            try
            {
                lstResult = _stateRepo.GetStateByName(StateName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetCountryDetailsByCountryName")]
        public List<CountryMaster> GetCountryDetailsByCountryName(string CountryName)
        {
            List<CountryMaster> lstResult = new List<CountryMaster>();
            try
            {
                lstResult = _CountryRepo.GetCountryByName(CountryName);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
    }
}
