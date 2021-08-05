using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Interface;
using BizLayer.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BizLayer.Domain;
using Microsoft.AspNetCore.Http;
using Emr_web.Common;
using Syncfusion.EJ2.Navigations;
using System.Data;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CommonController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IErrorlog _errorlog;
        private readonly IEmrRepo _emrRepo;
        private readonly ISymptonRepo _symptonRepo;
        private readonly IDiagnosisRepo _diagnosisRepo;
        private readonly IInvestigationRepo _investigationRepo;
        private readonly IDrugRepo _drugRepo;
        public CommonController(IDBConnection iDBConnection, IErrorlog errorlog, IEmrRepo emrRepo, ISymptonRepo symptonRepo, IDiagnosisRepo diagnosisRepo, IInvestigationRepo investigationRepo, IDrugRepo drugRepo)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
            _emrRepo = emrRepo;
            _symptonRepo = symptonRepo;
            _diagnosisRepo = diagnosisRepo;
            _investigationRepo = investigationRepo;
            _drugRepo = drugRepo;
        }
        public IActionResult CommonData()
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<CommonSymptomsMaster> lstCommonSymMaster = _emrRepo.GetAllCommonSymptomMaster();
            ViewBag.CommonSymptom = null;
            List<CommonHealthHabitsMaster> lstCommonHabitsMaster = _emrRepo.GetAllHealthHabitsMaster();
            ViewBag.CommonHealthHabits = null;
            //List<CommonDiagnosisMaster> lstCommonDiagMaster = _emrRepo.GetAllCommonDiagnosisMaster();
            //ViewBag.CommonDiagnosis = lstCommonDiagMaster;
            ViewBag.CommonDiagnosis = null;
            List<CommonInvestigationMaster> lstCommonInvestMaster = _emrRepo.GetAllCommonInvestigationMaster();
            ViewBag.CommonInvestigation = null;
            List<CommonDiseaseMaster> lstCommonDiseaseMaster = _emrRepo.GetAllCommonDiseaseMaster();
            ViewBag.CommonDisease = null;
            List<CommonAllergyMaster> lstCommonAllergyMaster = _emrRepo.GetAllCommonAllergyMaster();
            ViewBag.CommonAllergy = null;
            List<CommonDrugMaster> lstCommonDrugMaster = _emrRepo.GetAllCommonDrug();
            ViewBag.CommonDrug = null;
            List<CommonExamintion> lstCommonExamniation = _emrRepo.GetAllCommonExamintion();
            ViewBag.CommonExamintion = null;
            List<CommonHISInvestigation> lstCommonHISInvestigation = _emrRepo.GetAllCommonHISInvestigation();
            ViewBag.CommonHISInvestigation = lstCommonHISInvestigation;

            return View();
        }
        [HttpPost]
        public JsonResult AddSymptoms([FromBody] CommonSymptomsMaster[] commonSymptomsArray)
        {
            string Result = "SaveResult";
            //int rowAffected = 0;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            if (commonSymptomsArray.Length > 0)
            {
                for (int count = 0; count < commonSymptomsArray.Length; count++)
                {
                    SymptomsMaster master = new SymptomsMaster
                    {
                        Symptoms = commonSymptomsArray[count].CommonSymptoms,
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                    };
                    bool isSuccess = _symptonRepo.CreateNewSymptom(master);
                }
            }
            else
            {
                Result = "Please ";
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult AddHealthHabits([FromBody] CommonHealthHabitsMaster[] commonHealthHabitsArray)
        {
            string Result = "";
            //int rowAffected = 0;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            if (commonHealthHabitsArray.Length > 0)
            {
                for (int count = 0; count < commonHealthHabitsArray.Length; count++)
                {
                    HealthHabit master = new HealthHabit
                    {
                        Habits = commonHealthHabitsArray[count].CommonHealthHabits,
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                    };
                    int HabitSeqID = _emrRepo.CreateNewHealthHabit(master);
                    if (HabitSeqID > 0)
                    {
                        Result = "Created Successfully";
                    }
                    else
                    {
                        Result = "Already Exists";
                    }
                }
            }
            else
            {
                Result = "Please Select Minimum One";
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult AddDiagnosis([FromBody] CommonDiagnosisMaster[] commonDiagnosisArray)
        {
            string Result = "SaveResult";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (commonDiagnosisArray.Length > 0)
                {
                    for (int count = 0; count < commonDiagnosisArray.Length; count++)
                    {
                        DiagnosisMaterCls master = new DiagnosisMaterCls
                        {
                            Diagnosis_Name = commonDiagnosisArray[count].DiagnosisName,
                            ICD10 = null,
                            CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                            IsActive = true
                        };
                        int isSuccess = _diagnosisRepo.CreateNewDiagnosis(master);
                    }
                }
                else
                {
                    Result = "Please ";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult AddInvestigation([FromBody] CommonInvestigationMaster[] commonInvestArray)
        {
            string Result = "SaveResult";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (commonInvestArray.Length > 0)
                {
                    for (int count = 0; count < commonInvestArray.Length; count++)
                    {
                        InvestigationMaster master = new InvestigationMaster
                        {
                            Investigation_Name = commonInvestArray[count].InvestigationName,
                            Investigation_Rate = 0,
                            CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                            Isactive = true
                        };
                        int isSuccess = _investigationRepo.CreateNewInvestigation(master);
                    }
                }
                else
                {
                    Result = "Please ";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult AddDisease([FromBody] CommonDiseaseMaster[] commonDiseaseArray)
        {
            string Result = "SaveResult";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (commonDiseaseArray.Length > 0)
                {
                    for (int count = 0; count < commonDiseaseArray.Length; count++)
                    {
                        Diseases master = new Diseases
                        {
                            DiseasesName = commonDiseaseArray[count].DiseaseName,
                            CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                            IsActive = true
                        };
                        int isSuccess = _emrRepo.CreateNewDiseases(master);
                    }
                }
                else
                {
                    Result = "Please ";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult AddAllergy([FromBody] CommonAllergyMaster[] commonAllergyArray)
        {
            string Result = "SaveResult";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (commonAllergyArray.Length > 0)
                {
                    for (int count = 0; count < commonAllergyArray.Length; count++)

                    {
                        Allergies master = new Allergies
                        {
                            Allergy = commonAllergyArray[count].AllergyName,
                            Createdatetime = timezoneUtility.Gettimezone(Timezoneid),
                            Modifiedatetime = timezoneUtility.Gettimezone(Timezoneid),
                            CreateUser = HttpContext.Session.GetString("Userseqid"),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        };
                        int isSuccess = _emrRepo.CreateNewAllergy(master);
                    }
                }
                else
                {
                    Result = "Please ";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult AddDrugs([FromBody] CommonDrugMaster[] commonDrugArray)
        {
            string Result = "SaveResult";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (commonDrugArray.Length > 0)
                {
                    for (int count = 0; count < commonDrugArray.Length; count++)
                    {
                        DrugMasterInfo drugMasterInfo = new DrugMasterInfo
                        {
                            DrugName = commonDrugArray[count].DrugName,
                            Category = commonDrugArray[count].Category,
                            Uom = commonDrugArray[count].Uom,
                            Gst = commonDrugArray[count].Gst,
                            ScheduleType = commonDrugArray[count].ScheduleType,
                            HSnCode = commonDrugArray[count].HSnCode,
                            Company = commonDrugArray[count].Company,
                            Type = commonDrugArray[count].Type,
                            CreateUser = HttpContext.Session.GetString("Userseqid").ToString(),
                            ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifedUser = HttpContext.Session.GetString("Userseqid").ToString(),
                            IsActive = true,
                            HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                        };
                        long rowaffceted = _drugRepo.CreateNewDrug(drugMasterInfo);
                    }
                }
                else
                {
                    Result = "Please ";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult AddCommonExamintion([FromBody] CommonExamintion[] commonExamintionArray)
        {
            string Result = "SaveResult";
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (commonExamintionArray.Length > 0)
                {
                    for (int count = 0; count < commonExamintionArray.Length; count++)
                    {
                        ExaminationMaster master = new ExaminationMaster
                        {
                            ExaminationName = commonExamintionArray[count].ExaminationName,
                            Option1 = commonExamintionArray[count].Option1,
                            Option2 = commonExamintionArray[count].Option2,
                            Option3 = commonExamintionArray[count].Option3,
                            Option4 = commonExamintionArray[count].Option4,
                            Option5 = commonExamintionArray[count].Option5,
                            Option6 = commonExamintionArray[count].Option6,
                            Option7 = commonExamintionArray[count].Option7,
                            Option8 = commonExamintionArray[count].Option8,
                            Option9 = commonExamintionArray[count].Option9,
                            Option10 = commonExamintionArray[count].Option10,
                            Option11 = commonExamintionArray[count].Option11,
                            Option12 = commonExamintionArray[count].Option12,
                            CreateUser = HttpContext.Session.GetString("Userseqid"),
                            CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            HospitalID = HospitalId
                        };
                        int rowaffected = _emrRepo.CreateNewExamintion(master);
                    }
                }
                else
                {
                    Result = "Please ";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }

        [HttpPost]
        public JsonResult AddCommonHISInvestigation([FromBody] CommonHISInvestigation[] commonExamintionArray)
        {
            string Result = "SaveResult";
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (commonExamintionArray.Length > 0)
                {
                    for (int count = 0; count < commonExamintionArray.Length; count++)
                    {
                        CommonHISInvestigation commonHISInvestigation = new CommonHISInvestigation()
                        {
                            TestName = commonExamintionArray[count].TestName,
                            TestShortCode = commonExamintionArray[count].TestShortCode,
                            TestAliasNames = commonExamintionArray[count].TestAliasNames,
                            DeptType = commonExamintionArray[count].DeptType,
                            Sampletype = commonExamintionArray[count].Sampletype,
                            ContainerType = commonExamintionArray[count].ContainerType,
                            TestType = commonExamintionArray[count].TestType,
                            Format = commonExamintionArray[count].Format,
                            IsParameter = commonExamintionArray[count].IsParameter,
                            IsInterface = commonExamintionArray[count].IsInterface,
                            IsProfile = commonExamintionArray[count].IsProfile,
                            ProfileID = commonExamintionArray[count].ProfileID,
                            IsPackage = commonExamintionArray[count].IsPackage,
                            PackageID = commonExamintionArray[count].PackageID,
                            TestActive = true,
                            CreateDT = timezoneUtility.Gettimezone(Timezoneid),
                            ServiceId = commonExamintionArray[count].ServiceId,
                            MSERVICESCODE = commonExamintionArray[count].MSERVICESCODE,
                            HospitalID = HospitalId,
                            QTY_Enable = commonExamintionArray[count].QTY_Enable,
                            RATE_Enable = commonExamintionArray[count].RATE_Enable,
                        };
                        long rowaffected = _emrRepo.CreateNewTestParameter(commonHISInvestigation);
                        if (rowaffected > 0)
                        {
                            CommonHISInvestigation testparameterrate = new CommonHISInvestigation()
                            {
                                TestId = rowaffected,
                                TestRateCommon = 0,
                                Isactive = true,
                                CreatedUser= Convert.ToInt64(HttpContext.Session.GetString("Userseqid")),
                                MSERVICESCODE = commonExamintionArray[count].MSERVICESCODE,
                                ServiceId = commonExamintionArray[count].ServiceId,
                                HospitalID = HospitalId
                            };
                            int rowaffect = _emrRepo.CreateNewTestParameterRate(testparameterrate);
                        }
                    }
                }
                else
                {
                    Result = "Please ";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
    }
}