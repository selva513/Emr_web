using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using System.Text.RegularExpressions;
using System.Data;
using Emr_web.Common;

namespace Emr_web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/EmrApi")]
    public class EmrApiController : Controller
    {
        private IDBConnection _IDBConnection;
        private IPatientRepo _patientRepo;
        private IVitlasRepo _vitlasRepo;
        private ISymptonRepo _symptonRepo;
        private IEmrRepo _emrRepo;
        private IInvestigationRepo _investigationRepo;
        private IErrorlog _errorlog;
        private IInvestRepo _investRepo;
        private IDiagnosisRepo _diagnosisRepo;
        private IDrugRepo _drugRepo;
        public EmrApiController(IDBConnection iDBConnection, IPatientRepo patientRepo, IVitlasRepo vitlasRepo, ISymptonRepo symptonRepo, IEmrRepo emrRepo, IInvestigationRepo investigationRepo, IErrorlog errorlog, IInvestRepo investRepo, IDiagnosisRepo diagnosisRepo, IDrugRepo drugRepo)
        {
            _IDBConnection = iDBConnection;
            _patientRepo = patientRepo;
            _vitlasRepo = vitlasRepo;
            _symptonRepo = symptonRepo;
            _emrRepo = emrRepo;
            _investigationRepo = investigationRepo;
            _errorlog = errorlog;
            _investRepo = investRepo;
            _diagnosisRepo = diagnosisRepo;
            _drugRepo = drugRepo;
        }
        [HttpPost("VitalSave")]
        public JsonResult VitalSave([FromBody] VitalsView vitalsView)
        {
            string Result = "success";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDateTime = timezoneUtility.Gettimezone(Timezoneid);
                long VitalSeqID = _vitlasRepo.IsVerfiedVitals(vitalsView.VisitId, CurrentDateTime);
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if (VitalSeqID > 0)
                {
                    VitalsDetails details = new VitalsDetails
                    {
                        VitalSeqId = VitalSeqID,
                        Weight = vitalsView.Weight,
                        Height = vitalsView.Height,
                        BPSystolic = vitalsView.BPSystolic,
                        BPDiastolic = vitalsView.BPDiastolic,
                        Pulse = vitalsView.Pulse,
                        Respiration = vitalsView.Respiration,
                        Temperature = vitalsView.Temperature,
                        OxygenSaturation = vitalsView.OxygenSaturation,
                        BMI = vitalsView.BMI,
                        BloodSugar = vitalsView.BloodSugar,
                        Notes = vitalsView.Notes,
                        ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        IsVerfied = false
                    };
                    int count = _vitlasRepo.UpdateVitlas(details);
                    if (count > 0)
                    {
                        Result = "Update Success";
                        EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
                        eventManagemntInfo.EventName = "Vitals Update for Patient and UHID-" + vitalsView.PatientID;
                        eventManagemntInfo.UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                        eventManagemntInfo.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        _emrRepo.NewEventCreateion(eventManagemntInfo);
                    }

                }
                else
                {
                    VitalsHeader header = new VitalsHeader
                    {
                        VisitId = vitalsView.VisitId,
                        PatientID = vitalsView.PatientID,
                        ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                        DoctorId = 3,
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        CreatedDate= timezoneUtility.Gettimezone(Timezoneid),
                        HospitalID= Hospitalid
                    };
                    if (_vitlasRepo.CreateNewVitalsHeader(header))
                    {
                        VitalsDetails details = new VitalsDetails
                        {
                            VitalSeqId = header.VitlasSeqId,
                            Weight = vitalsView.Weight,
                            Height = vitalsView.Height,
                            BPSystolic = vitalsView.BPSystolic,
                            BPDiastolic = vitalsView.BPDiastolic,
                            Pulse = vitalsView.Pulse,
                            Respiration = vitalsView.Respiration,
                            Temperature = vitalsView.Temperature,
                            OxygenSaturation = vitalsView.OxygenSaturation,
                            BMI = vitalsView.BMI,
                            BloodSugar = vitalsView.BloodSugar,
                            Notes = vitalsView.Notes,
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            CreatedDate = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            IsVerfied = false
                        };
                        int count = _vitlasRepo.CreateNewVitals(details);
                        if (count > 0)
                        {
                            Result = "Save Success";
                            EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
                            eventManagemntInfo.EventName = "Vitals Save for Patinet and UHID-" + vitalsView.PatientID;
                            eventManagemntInfo.UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                            eventManagemntInfo.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            _emrRepo.NewEventCreateion(eventManagemntInfo);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
                Result = "Error";
            }
            return Json(new
            {
                state = 0,
                msg = Result
            });
        }

        [HttpPost("VitalVerifiy")]
        public JsonResult VitalVerifiy([FromBody] VitalsView vitalsView)
        {
            string Result = "success";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);
                long VitalSeqID = _vitlasRepo.IsVerfiedVitals(vitalsView.VisitId, CurrentDatetime);
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if (VitalSeqID > 0)
                {
                    VitalsDetails details = new VitalsDetails
                    {
                        VitalSeqId = VitalSeqID,
                        Weight = vitalsView.Weight,
                        Height = vitalsView.Height,
                        BPSystolic = vitalsView.BPSystolic,
                        BPDiastolic = vitalsView.BPDiastolic,
                        Pulse = vitalsView.Pulse,
                        Respiration = vitalsView.Respiration,
                        Temperature = vitalsView.Temperature,
                        OxygenSaturation = vitalsView.OxygenSaturation,
                        BMI = vitalsView.BMI,
                        BloodSugar = vitalsView.BloodSugar,
                        Notes = vitalsView.Notes,
                        ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        IsVerfied = true
                    };
                    int count = _vitlasRepo.UpdateVitlas(details);
                    if (count > 0)
                    {
                        Result = "Update Success";
                        EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
                        eventManagemntInfo.EventName = "Vitals Verify for Patient and UHID-" + vitalsView.PatientID;
                        eventManagemntInfo.UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                        eventManagemntInfo.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        _emrRepo.NewEventCreateion(eventManagemntInfo);
                    }
                }
                else
                {
                    VitalsHeader header = new VitalsHeader
                    {
                        VisitId = vitalsView.VisitId,
                        PatientID = vitalsView.PatientID,
                        CreatedDate= timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                        DoctorId = 3,
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = Hospitalid
                    };
                    if (_vitlasRepo.CreateNewVitalsHeader(header))
                    {
                        VitalsDetails details = new VitalsDetails
                        {
                            VitalSeqId = header.VitlasSeqId,
                            Weight = vitalsView.Weight,
                            Height = vitalsView.Height,
                            BPSystolic = vitalsView.BPSystolic,
                            BPDiastolic = vitalsView.BPDiastolic,
                            Pulse = vitalsView.Pulse,
                            Respiration = vitalsView.Respiration,
                            Temperature = vitalsView.Temperature,
                            OxygenSaturation = vitalsView.OxygenSaturation,
                            BMI = vitalsView.BMI,
                            BloodSugar = vitalsView.BloodSugar,
                            Notes = vitalsView.Notes,
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            CreatedDate = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedDate = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            IsVerfied = true
                        };
                        int count = _vitlasRepo.CreateNewVitals(details);
                        if (count > 0)
                        {
                            Result = "Save Success";
                            EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
                            eventManagemntInfo.EventName = "Vitals Verify for Patient and UHID-" + vitalsView.PatientID;
                            eventManagemntInfo.UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
                            eventManagemntInfo.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                            _emrRepo.NewEventCreateion(eventManagemntInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
                Result = "Error";
            }
            return Json(new
            {
                state = 0,
                msg = Result
            });
        }
        [HttpGet("GetVitlasByVisitId")]
        public JsonResult GetVitlasByVisitId(string PatientID)
        {
            List<VitalsView> lstResult = new List<VitalsView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _vitlasRepo.GetVitalsByVisitID(PatientID, HospitalId);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetAllVitlasByVisitId")]
        public JsonResult GetAllVitlasByVisitId(string PatientID)
        {
            List<VitalsView> lstResult = new List<VitalsView>();
            try
            {
                lstResult = _vitlasRepo.GetAllVitalsByVisitID(PatientID);
            }
            catch (Exception ex)
            {
                string ErrorMsg = ex.ToString();
            }
            return Json(lstResult);
        }
        [HttpGet("GetPickListHeader")]
        public List<PickListView> GetPickListHeader()
        {
            List<PickListView> lstResult = new List<PickListView>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _symptonRepo.GetPickListHeaderSearch(EnteredName, HospitalId);
            return lstResult;
        }
        [HttpGet("GetDefaultPick")]
        public List<SymptomView> GetDefaultPick(string DefaultValue)
        {
            List<SymptomView> lstResult = new List<SymptomView>();
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _symptonRepo.GetAllSymptomByDefault(DefaultValue, HospitalId);
            return lstResult;
        }
        [HttpGet("GetDefaultInvestPick")]
        public List<InvestigationMaster> GetDefaultInvestPick(string DefaultValue)
        {
            List<InvestigationMaster> lstResult = new List<InvestigationMaster>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _investRepo.GetAllInvestigationByDefault(DefaultValue, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetHabitPickListHeader")]
        public List<HabitView> GetHabitPickListHeader()
        {
            List<HabitView> lstResult = new List<HabitView>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _emrRepo.GetHabitPickListHeaderSearch(EnteredName, HospitalId);
            return lstResult;
        }
        [HttpGet("GetAllergySearch")]
        public List<Allergies> GetAllergySearch()
        {
            List<Allergies> lstResult = new List<Allergies>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            lstResult = _emrRepo.GetAllergiesSearch(EnteredName);
            return lstResult;
        }
        [HttpGet("GetDiseasesPikHeaderSearch")]
        public List<DiseasesPicLstHeader> GetDiseasesPikHeaderSearch()
        {
            List<DiseasesPicLstHeader> lstResult = new List<DiseasesPicLstHeader>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _emrRepo.GetDiseasesPickListHeaderSearch(EnteredName, HospitalId);
            return lstResult;
        }
        [HttpGet("GetMedicineSearch")]
        public List<MedicineMaster> GetMedicineSearch()
        {
            List<MedicineMaster> lstResult = new List<MedicineMaster>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            lstResult = _emrRepo.GetMedicineSearch(EnteredName);
            return lstResult;
        }
        [HttpGet("GetPhmacyFreeTextDrug")]
        public List<DrugMasterInfo> GetPhmacyFreeTextDrug()
        {
            List<DrugMasterInfo> lstResult = new List<DrugMasterInfo>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            //bool IsConnectedPharmacy = _drugRepo.GetConnectedPharmacy();
            bool IsConnectedPharmacy = logins[0].IsConnectedPharmacy;
            if (IsConnectedPharmacy)
                lstResult = _emrRepo.GetPhmacyFreeTextDrug(EnteredName,HospitalId);
            else
                lstResult = _drugRepo.GetMasterDrugBySearch(EnteredName, HospitalId);
            return lstResult;
        }
        [HttpGet("GetDrugPicklistFreeText")]
        public List<DrugPickListHeader> GetDrugPicklistFreeText()
        {
            List<DrugPickListHeader> lstResult = new List<DrugPickListHeader>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _emrRepo.GetDrugPicklistFreeText(EnteredName, HospitalId);
            return lstResult;
        }
        [HttpGet("GetProcedureBySearch")]
        public List<ProcedureMaster> GetProcedureBySearch()
        {
            List<ProcedureMaster> lstResult = new List<ProcedureMaster>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            lstResult = _emrRepo.GetProcedureBySearch(EnteredName);
            return lstResult;
        }
        [HttpGet("GetInstructionBySearch")]
        public List<string> GetInstructionBySearch()
        {
            List<string> lstResult = new List<string>();
            string EnteredName = GetEnteredData();
            lstResult = _emrRepo.GetInstructionBySearch(EnteredName);
            return lstResult;
        }
        [HttpGet("GetExamintionBySearch")]
        public List<ExaminationMaster> GetExamintionBySearch()
        {
            List<ExaminationMaster> lstResult = new List<ExaminationMaster>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _emrRepo.GetExamintionBySearch(EnteredName, HospitalId);
            return lstResult;
        }
        [HttpGet("GetExamintionPickNameBySearch")]
        public List<ExaminationPickLstHeader> GetExamintionPickNameBySearch()
        {
            List<ExaminationPickLstHeader> lstResult = new List<ExaminationPickLstHeader>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            lstResult = _emrRepo.GetExamintionPickNameBySearch(EnteredName, HospitalId);
            return lstResult;
        }
        public string GetEnteredData()
        {
            string Data = "";
            try
            {
                var query = Request.Query;
                string filter = query["$filter"];
                Match matchString = Regex.Match(filter, @"'(.*)',tolower");
                string[] seperators = { "(", ")", ",", "'", "'" };
                string[] split = matchString.Value.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                string QueryString = split[0];
                Data = QueryString;
            }
            catch (Exception ex)
            {

            }
            return Data;
        }
        [HttpGet("SearchCategoryByText")]
        public List<string> SearchCategoryByText()
        {
            List<string> lstResult = new List<string>();
            string EnteredName = GetEnteredData();
            lstResult = _drugRepo.SearchCategoryByText(EnteredName);
            return lstResult;
        }
        [HttpGet("SearchUomByText")]
        public List<string> SearchUomByText()
        {
            List<string> lstResult = new List<string>();
            string EnteredName = GetEnteredData();
            lstResult = _drugRepo.SearchUomByText(EnteredName);
            return lstResult;
        }
        [HttpGet("ScheduleTypeByText")]
        public List<string> ScheduleTypeByText()
        {
            List<string> lstResult = new List<string>();
            string EnteredName = GetEnteredData();
            lstResult = _drugRepo.ScheduleTypeByText(EnteredName);
            return lstResult;
        }
        [HttpGet("CompanyByText")]
        public List<string> CompanyByText()
        {
            List<string> lstResult = new List<string>();
            string EnteredName = GetEnteredData();
            lstResult = _drugRepo.CompanyByText(EnteredName);
            return lstResult;
        }
        [HttpGet("TypeByText")]
        public List<string> TypeByText()
        {
            List<string> lstResult = new List<string>();
            string EnteredName = GetEnteredData();
            lstResult = _drugRepo.TypeByText(EnteredName);
            return lstResult;
        }
        [HttpGet("SelectFunPickLstByText")]
        public List<string> SelectFunPickLstByText()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<string> lstResult = new List<string>();
            string EnteredName = GetEnteredData();
            lstResult = _emrRepo.SelectFunPickLstByText(EnteredName, HospitalID);
            return lstResult;
        }
        [HttpGet("SurgicalHistoryhBySearch")]
        public List<SurgicalHistoryInfo> SurgicalHistoryhBySearch()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
            List<Login> logins = myComplexObject;
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SurgicalHistoryInfo> lstResult = new List<SurgicalHistoryInfo>();
            string EnteredName = GetEnteredData();
            if (EnteredName == "tolower")
                EnteredName = null;
            lstResult = _emrRepo.SurgicalHistoryhBySearch(EnteredName, HospitalID);
            return lstResult;
        }
        [HttpGet("GetAllSymptombyHospitalID")]
        public List<SymptomView> GetAllSymptombyHospitalID()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<SymptomView> lstResult = new List<SymptomView>();
            string EnteredName = GetEnteredData();
            if (!string.IsNullOrWhiteSpace(EnteredName) && !EnteredName.Equals("tolower"))
            {
                lstResult = _symptonRepo.GetSymptomByFilltering(HospitalID, EnteredName);
            }
            else
                lstResult = _symptonRepo.GetAllSymptom(HospitalID);
            return lstResult;
        }
        [HttpGet("GetAllHealthHabitsBySearchTearm")]
        public List<HealthHabit> GetAllHealthHabitsBySearchTearm()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<HealthHabit> lstResult = new List<HealthHabit>();
            string EnteredName = GetEnteredData();
            if (!string.IsNullOrWhiteSpace(EnteredName) && !EnteredName.Equals("tolower"))
            {
                lstResult = _emrRepo.GetAllHealthHabitsBySearchTearm(HospitalID, EnteredName);
            }
            else
                lstResult = _emrRepo.GetAllHealthHabits(HospitalID);
            return lstResult;
        }
        [HttpGet("GetAllDiseasesBySearch")]
        public List<Diseases> GetAllDiseasesBySearch()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<Diseases> lstResult = new List<Diseases>();
            string EnteredName = GetEnteredData();
            if (!string.IsNullOrWhiteSpace(EnteredName) && !EnteredName.Equals("tolower"))
            {
                lstResult = _emrRepo.GetAllDiseasesBySearch(HospitalID, EnteredName);
            }
            else
                lstResult = _emrRepo.GetAllDiseases(HospitalID);
            return lstResult;
        }
        [HttpGet("GetDiagnosisByFilltering")]
        public List<DiagnosisMaterCls> GetDiagnosisByFilltering()
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<DiagnosisMaterCls> lstResult = new List<DiagnosisMaterCls>();
            string EnteredName = GetEnteredData();
            if (!string.IsNullOrWhiteSpace(EnteredName) && !EnteredName.Equals("tolower"))
            {
                lstResult = _diagnosisRepo.GetDiagnosisByFilltering(HospitalID, EnteredName);
            }
            else
                lstResult = _diagnosisRepo.GetAllDiagnosisMaster(HospitalID);
            return lstResult;
        }
        #region Raffi
        [HttpGet("GetDefaultHISInvestPick")]
        public List<InvestigationMaster> GetDefaultHISInvestPick(string DefaultValue)
        {
            List<InvestigationMaster> lstResult = new List<InvestigationMaster>();
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _investRepo.GetHISInvestigationByDefault(DefaultValue, HospitalID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetInvestPickListHeader")]
        public List<InvestPickHeader> GetInvestPickListHeader()
        {
            List<InvestPickHeader> lstResult = new List<InvestPickHeader>();
            try
            {
                string IsConnectedHIS = HttpContext.Session.GetString("IsHISPatient");
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                string EnteredName = GetEnteredData();
                if (EnteredName == "tolower")
                    EnteredName = null;
                lstResult = _investigationRepo.GetInvestPickListHeaderSearch(EnteredName, IsConnectedHIS, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("CheckISConnect")]
        public string CheckISConnect()
        {
            string Success = "";
            try
            {
                string IsConnectedHIS = HttpContext.Session.GetString("IsConnectedHIS");
                Success = IsConnectedHIS;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("GetInvestRate")]
        public string GetInvestRate(long Investid)
        {
            string Success = "";
            try
            {
                string IsConnectedHIS = HttpContext.Session.GetString("IsConnectedHIS");
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                string Rate = _investRepo.GetRates(Investid, IsHISPatient);
                Success = Rate;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Success;
        }
        [HttpGet("GetDocDepartment")]
        public List<string> GetDocDepartment()
        {
            List<string> lstResult = new List<string>();
            try
            {
                string IsConnectedHIS = HttpContext.Session.GetString("IsConnectedHIS");
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                string EnteredName = GetEnteredData();
                lstResult = _investRepo.GetDocDepartmentSearch(EnteredName, IsHISPatient);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        [HttpGet("GetRefDrSearch")]
        public List<string> GetRefDrSearch()
        {
            List<string> lstResult = new List<string>();
            try
            {
                string IsConnectedHIS = HttpContext.Session.GetString("IsConnectedHIS");
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                string EnteredName = GetEnteredData();
                lstResult = _investRepo.GetRefDocSearch(EnteredName, IsHISPatient);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return lstResult;
        }
        #endregion
        #region Diagnosis
        [HttpGet("GetLastData")]
        public string GetLastData(string Patientid, string Visitid)
        {
            DataSet ds = new DataSet();
            try
            {
                if (ModelState.IsValid)
                {
                    long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    long PatSeqid = _investRepo.GetPatSeqid(Patientid,HospitalId);
                    ds = _diagnosisRepo.GetLastDiagnosis(PatSeqid, Visitid);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        #endregion
        #region Get Investigation Save Data
        [HttpGet("GetInvLastData")]
        public string GetInvLastData(string Patientid, string Visitid)
        {
            DataSet ds = new DataSet();
            try
            {
                if (ModelState.IsValid)
                {
                    long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                    long PatSeqid = _investRepo.GetPatSeqid(Patientid,HospitalId);
                    ds = _investRepo.GetOldInvestigation(PatSeqid, Visitid);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        #endregion
        #region Get Vitals Not verify Data
        [HttpGet("GetVitalsLastData")]
        public string GetVitalsLastData(string Patientid, string Visitid)
        {
            DataSet ds = new DataSet();
            try
            {
                ds = _vitlasRepo.GetLastNotVerifyVitals(Patientid, Visitid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        #endregion
        [HttpGet("GetOldTreatmentDiagnosis")]
        public string GetOldTreatmentDiagnosis(string Patientid, string Visitid)
        {
            DataSet ds = new DataSet();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investRepo.GetPatSeqid(Patientid,HospitalId);
                ds = _emrRepo.GetOldTreatmentDiagnosis(Patientid, Visitid, PatSeqid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
        [HttpGet("GetComplaintsLastData")]
        public string GetComplaintsLastData(string Patientid, string Visitid)
        {
            DataSet ds = new DataSet();
            try
            {
                ds = _symptonRepo.GetComplaintsLastData(Patientid, Visitid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return ds.GetXml();
        }
    }
}