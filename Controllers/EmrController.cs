using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Emr_web.Models;
using BizLayer.Utilities;
using BizLayer.Interface;
using BizLayer.Domain;
using Microsoft.AspNetCore.Http;
using Syncfusion.EJ2.Navigations;
using Emr_web.Common;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Extensions.Configuration;


namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class EmrController : Controller
    {
        private readonly IDBConnection _IDBConnection;
        private readonly IPatientRepo _patientRepo;
        private readonly IVitlasRepo _vitlasRepo;
        private readonly ISymptonRepo _symptonRepo;
        private readonly IEmrRepo _emrRepo;
        private readonly IInvestRepo _investrepo;
        private readonly IErrorlog _errorlog;
        private readonly IDiagnosisRepo _diagnosisRepo;
        private readonly IHospitalRepo _hospitalRepo;
        private readonly ISettingRepo _settingRepo;
        private readonly IFunctionalRepo _functionalRepo;
        private readonly IDrugRepo _drugRepo;
        private readonly IMDBConnection _mDBConnection;
        private readonly IMediviewRepo _mediviewRepo;
        private readonly IConfiguration _configuration;
        private readonly IClinicRepo _clinicRepo;

        public EmrController(IDBConnection iDBConnection, IPatientRepo patientRepo, IVitlasRepo vitlasRepo, ISymptonRepo symptonRepo, IEmrRepo emrRepo, IInvestRepo investRepo, IErrorlog errorlog, IDiagnosisRepo diagnosisRepo, IHospitalRepo hospitalRepo, ISettingRepo settingRepo, IFunctionalRepo functionalRepo, IDrugRepo drugRepo, IMDBConnection mDBConnection, IMediviewRepo mediviewRepo, IConfiguration configuration, IClinicRepo clinicRepo)
        {
            _IDBConnection = iDBConnection;
            _patientRepo = patientRepo;
            _vitlasRepo = vitlasRepo;
            _symptonRepo = symptonRepo;
            _emrRepo = emrRepo;
            _investrepo = investRepo;
            _errorlog = errorlog;
            _diagnosisRepo = diagnosisRepo;
            _hospitalRepo = hospitalRepo;
            _settingRepo = settingRepo;
            _functionalRepo = functionalRepo;
            _drugRepo = drugRepo;
            _mDBConnection = mDBConnection;
            _mediviewRepo = mediviewRepo;
            _configuration = configuration;
            _clinicRepo = clinicRepo;
        }
       
        public IActionResult EmrView()
        {
            var myComplexObject = HttpContext.Session.GetObjectFromJsonList<MenuItem>("MenuList");
            ViewBag.mainMenuItems = myComplexObject;
            var myComplexObjectaccount = HttpContext.Session.GetObjectFromJsonList<MenuItem>("AccountList");
            ViewBag.AccountMenuItems = myComplexObjectaccount;

            //List<MyPatient> lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
            //string HISVstid = lstresult[0].HISVisitID;
            //if (HISVstid != null && HISVstid != "")
            //    HttpContext.Session.SetString("IsHISPatient", "1");
            //else
            //    HttpContext.Session.SetString("IsHISPatient", "0");
            //string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
            //string IsConnectedHIS = HttpContext.Session.GetString("IsConnectedHIS");
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            //List<Config> myconfig = _patientRepo.GetConfig();
            //bool HISConnected = myconfig[0].IsConnectedHIS;
            //string IsHISPatient = HISConnected.ToString();

            long UserSeqID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
            string IsHISPatient = _patientRepo.GetHISConnected(UserSeqID);
            if (IsHISPatient == "True")
                IsHISPatient = "1";
            else
                IsHISPatient = "0";
            HttpContext.Session.SetString("IsHISPatient", IsHISPatient);
            //List<SymptomView> lstResult = _symptonRepo.GetAllSymptom(HospitalId);
            //ViewBag.Symptom = lstResult;

            //List<HealthHabit> lstHealthHabits = _emrRepo.GetAllHealthHabits(HospitalId);
            //ViewBag.Habits = lstHealthHabits;

            List<InvestigationMaster> lstInvestigation = _investrepo.GetAllInvestigation(IsHISPatient, HospitalId);
            ViewBag.Investigation = lstInvestigation;

            List<DiagnosisMaterCls> lstdiagnosis = _diagnosisRepo.GetAllDiagnosisMaster(HospitalId);
            ViewBag.Diagnosis = lstdiagnosis;

            List<DepartmentMaster> lstDept = _investrepo.GetAllDepartment(IsHISPatient);
            ViewBag.Department = lstDept;

            List<DoctorMaster> lstDoctor = _investrepo.GetAllDoctors(IsHISPatient);
            ViewBag.Doctor = lstDoctor;

            List<Diseases> lstDiseases = _emrRepo.GetAllDiseases(HospitalId);
            ViewBag.Diseases = lstDiseases;

            List<FunctionalStatusMaster> lstFunStatus = _functionalRepo.GetAllFunctionalStatus(HospitalId);
            ViewBag.FunctionalStatus = lstFunStatus;

            ViewBag.minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            return View();
        }
        [HttpGet]
        public JsonResult MultiLineChartData(string PatientID)
        {
            Chart _chart = new Chart
            {
                labels = _vitlasRepo.GetVitalsDateByPatID(PatientID).ToArray(),
                datasets = new List<Datasets>()
            };
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "BP Systolic",
                data = _vitlasRepo.GetBPSystolicByPatID(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(255,99,132,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "BP Diastolic",
                data = _vitlasRepo.GetBPDiastolicByPatID(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(70,191,189,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Temperature",
                data = _vitlasRepo.GetTemperatureByPatID(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(253,180,92,1)" },
                borderWidth = "3"
            });
            _chart.datasets = _dataSet;
            return Json(_chart);
        }
        [HttpGet]
        public JsonResult MultiLineChartDataByLastVisit(string PatientID)
        {
            Chart _chart = new Chart
            {
                labels = _vitlasRepo.GetVitalsDateByLastVisit(PatientID).ToArray(),
                datasets = new List<Datasets>()
            };
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "BP Systolic",
                data = _vitlasRepo.GetBPSystolicByLastVisit(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(255,99,132,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "BP Diastolic",
                data = _vitlasRepo.GetBPDiastolicByLastVisit(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(70,191,189,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Temperature",
                data = _vitlasRepo.GetTemperatureByLastVisit(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(253,180,92,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Blood Sugar",
                data = _vitlasRepo.GetBloodSugarByLastVisit(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(138,253,92,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Respiration",
                data = _vitlasRepo.GetRespirationByLastVisit(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(252,92,253,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Pulse",
                data = _vitlasRepo.GetPulseByLastVisit(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(253,246,92,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Oxygen Saturation",
                data = _vitlasRepo.GetOxygenSaturationByLastVisit(PatientID).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(92,253,192,1)" },
                borderWidth = "3"
            });
            _chart.datasets = _dataSet;
            return Json(_chart);
        }

        [HttpGet]
        public JsonResult MultiLineChartDataByVisitId(string VisitId)
        {
            Chart _chart = new Chart
            {
                labels = _vitlasRepo.GetVitalsDateByVisitId(VisitId).ToArray(),
                datasets = new List<Datasets>()
            };
            List<Datasets> _dataSet = new List<Datasets>();
            _dataSet.Add(new Datasets()
            {
                label = "BP Systolic",
                data = _vitlasRepo.GetBPSystolicByVisitId(VisitId).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(255,99,132,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "BP Diastolic",
                data = _vitlasRepo.GetBPDiastolicByVisitId(VisitId).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(70,191,189,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Temperature",
                data = _vitlasRepo.GetTemperatureByVisitId(VisitId).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(253,180,92,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Blood Sugar",
                data = _vitlasRepo.GetBloodSugarByVisitId(VisitId).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(138,253,92,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Respiration",
                data = _vitlasRepo.GetRespirationByVisitId(VisitId).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(252,92,253,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Pulse",
                data = _vitlasRepo.GetPulseByVisitId(VisitId).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(253,246,92,1)" },
                borderWidth = "3"
            });
            _dataSet.Add(new Datasets()
            {
                label = "Oxygen Saturation",
                data = _vitlasRepo.GetOxygenSaturationByVisitId(VisitId).ToArray(),
                backgroundColor = "transparent",
                borderColor = new string[] { "rgba(92,253,192,1)" },
                borderWidth = "3"
            });
            _chart.datasets = _dataSet;
            return Json(_chart);
        }

        [HttpGet]
        public JsonResult CreateNewSymptom(string SymptomName)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            SymptomView view = new SymptomView();
            SymptomsMaster master = new SymptomsMaster
            {
                Symptoms = SymptomName,
                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
            };
            bool isSuccess = _symptonRepo.CreateNewSymptom(master);
            if (isSuccess)
            {
                view.Symptoms = master.Symptoms;
                view.SymSeqID = master.SymSeqID;
                string EventName = "New Symptom Added";
                CreateEventManagemnt(EventName);
            }
            else
            {
                view.Symptoms = "Already Exists";
            }

            return Json(view);
        }

        [HttpPost]
        public JsonResult CreateNewPickList([FromBody] PickListView[] pickListArray)
        {
            string Result = "SaveResult";
            //int rowAffected = 0;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            if (pickListArray.Length > 0)
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PhSeqId = _symptonRepo.IsPicNameExists(pickListArray[0].PickListName, HospitalID);
                if (PhSeqId > 0)
                {
                    if (pickListArray[0].PicklstClick.Equals("New"))
                    {
                        Result = "Pick list " + pickListArray[0].PickListName + " already exists. Select a new name for creating a new pick list" +
                             " or select Save To feature to add new Symptom";
                    }
                    else
                    {
                        // rowAffected = _symptonRepo.DeleteSymPicDetails(PhSeqId);
                        for (int count = 0; count < pickListArray.Length; count++)
                        {
                            PickListView pickList = pickListArray[count];
                            if (PhSeqId > 0)
                            {
                                PicklistDeatils list = new PicklistDeatils
                                {
                                    symPickID = PhSeqId,
                                    symptomId = pickList.SymptomId,
                                    CreateUser = HttpContext.Session.GetString("Userseqid"),
                                    CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                    ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                                };
                                bool isExists = _symptonRepo.IsSympotomExists(list.symPickID, list.symptomId);
                                if (isExists)
                                {
                                    _symptonRepo.CreateNewPickListDetails(list);
                                    Result = "Save Success";
                                }
                            }
                            Result = "Save Success";
                        }
                        string EventName = "Symptom Pick List Update";
                        CreateEventManagemnt(EventName);
                    }
                }
                else
                {
                    PickListHeader header = new PickListHeader
                    {
                        PickListName = pickListArray[0].PickListName,
                        CreateUser = HttpContext.Session.GetString("Userseqid"),
                        CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                    };
                    bool isSuccess = _symptonRepo.CreateNewPickListHeader(header);
                    if (isSuccess)
                    {
                        for (int count = 0; count < pickListArray.Length; count++)
                        {
                            PickListView pickList = pickListArray[count];
                            PicklistDeatils list = new PicklistDeatils
                            {
                                symPickID = header.SymPSeqID,
                                symptomId = pickList.SymptomId,
                                CreateUser = HttpContext.Session.GetString("Userseqid"),
                                CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                ModifiedUser = HttpContext.Session.GetString("Userseqid")
                            };
                            _symptonRepo.CreateNewPickListDetails(list);
                        }
                        Result = "Save Success";
                        string EventName = "New Symptom Pick List Update";
                        CreateEventManagemnt(EventName);
                    }
                }
            }
            return Json(Result);
        }

        [HttpPost]
        public JsonResult CreateNewPatSym([FromBody] PatSymptomView patSymptom)
        {
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            List<PatientSympotmHeader> lstResult = new List<PatientSympotmHeader>();
            bool isSuccess = false;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);

            PatientSympotmHeader header = new PatientSympotmHeader()
            {
                PatientId = patSymptom.PatientId,
                VisitId = patSymptom.VisitId,
                HistoryNotes = patSymptom.HistoryNotes,
                PainScale = patSymptom.PainScale,
                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                IsVerified = patSymptom.IsVerified,
                HospitalID = HospitalId
            };
            long SeqID = _symptonRepo.IsPatSymVisitIDExists(header.VisitId, CurrentDatetime);
            if (SeqID > 0)
            {

                _symptonRepo.UpdatePatSymHeader(header);
                header.PshSeqID = SeqID;
                isSuccess = true;
                lstResult = _symptonRepo.GetSymptomHeaderByVisit(patSymptom.PatientId, HospitalId);
                string EventName = "";
                if (patSymptom.IsVerified)
                {
                    EventName = "Update and Verify  Presenting Illness for Patient and UHID-" + patSymptom.PatientId;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    EventName = "Update Presenting Illness for Patient and UHID-" + patSymptom.PatientId;
                    CreateEventManagemnt(EventName);
                }
            }
            else
            {
                isSuccess = _symptonRepo.CreateNewPatSymHeader(header);
                string EventName = "Add Presenting Illness for Patient and UHID-" + patSymptom.PatientId;
                CreateEventManagemnt(EventName);
            }
            if (isSuccess)
            {
                if (patSymptom.SymDetails.Count() > 0)
                {
                    _symptonRepo.DeletePatsymDtl(header.PshSeqID);
                    for (int count = 0; count < patSymptom.SymDetails.Count(); count++)
                    {
                        PatientSympotmDetails details = new PatientSympotmDetails()
                        {
                            SymHeaderSeqId = header.PshSeqID,
                            SymptomName = patSymptom.SymDetails[count].SymptomName,
                            duration = patSymptom.SymDetails[count].duration,
                            SymptomId = patSymptom.SymDetails[count].SymptomId,
                            Remarks = patSymptom.SymDetails[count].Remarks
                        };
                        int dtlResult = _symptonRepo.CreateNewPatSymDtl(details);
                    }
                }
                lstResult = _symptonRepo.GetSymptomHeaderByVisit(patSymptom.PatientId, HospitalId);
            }
            //if (patSymptom.FunStatsuDtl.Length > 0)
            //{
            //    PatientFunctionalStatusHeader patientFunctionalStatusHeader = new PatientFunctionalStatusHeader
            //    {
            //        PatientID = patSymptom.PatientId,
            //        VisitID=patSymptom.VisitId,
            //        CreateUser = HttpContext.Session.GetString("Userseqid"),
            //        ModifieDatetime = DateTime.Now,
            //        ModifieUser = HttpContext.Session.GetString("Userseqid"),
            //        IsActive = patSymptom.IsVerified
            //    };
            //    int FunHeaderSeqID = _emrRepo.AddPatFunStatusHeader(patientFunctionalStatusHeader);
            //    if (FunHeaderSeqID > 0)
            //    {
            //        int DeletFundtl = _symptonRepo.DeletePatFunStatusDtl(FunHeaderSeqID);
            //        for(int count = 0; count < patSymptom.FunStatsuDtl.Length; count++)
            //        {
            //            PatientFunctionalStatusDetails functionalStatusDetails = new PatientFunctionalStatusDetails
            //            {
            //                HeaderSeqID = FunHeaderSeqID,
            //                FunctionalStatus = patSymptom.FunStatsuDtl[count].FuncationalStatus,
            //                Comments = patSymptom.FunStatsuDtl[count].Comments
            //            };
            //            int DtlResult = _emrRepo.AddPatFunStatusDtl(functionalStatusDetails);
            //        }
            //    }
            //}
            return Json(new { Header = lstResult });
        }
        [HttpGet]
        public JsonResult GetSymptomByVist(string PatientID)
        {
            List<PatientSympotmHeader> lstHeader = new List<PatientSympotmHeader>();
            List<PatientSympotmDetails> lstDeatils = new List<PatientSympotmDetails>();
            List<PatientFunctionalStatusHeader> lstFunHdr = new List<PatientFunctionalStatusHeader>();
            List<PatientFunctionalStatusDetails> lstFunDtl = new List<PatientFunctionalStatusDetails>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstHeader = _symptonRepo.GetSymptomHeaderByVisit(PatientID, HospitalId);
                lstDeatils = _symptonRepo.GetSymptomDetailsByVisit(PatientID);
                lstFunHdr = _symptonRepo.PatFunStatusHdrByPatID(PatientID);
                lstFunDtl = _symptonRepo.PatFunStatusDtlByPatID(PatientID);
            }
            catch (Exception)
            {

            }
            return Json(new { Header = lstHeader, Details = lstDeatils, FHeader = lstFunHdr, FDetails = lstFunDtl });
        }
        [HttpGet]
        public JsonResult GetSymptomHeader(string PatientID)
        {
            List<PatientSympotmHeader> lstHeader = new List<PatientSympotmHeader>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstHeader = _symptonRepo.GetSymptomHeaderByVisit(PatientID, HospitalId);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader });
        }
        [HttpGet]
        public JsonResult GetSymptomDeatils(long HeaderSeqID)
        {
            List<OldSymptomView> lstResult = new List<OldSymptomView>();
            lstResult = _symptonRepo.GetSymptomViewByID(HeaderSeqID);
            return Json(lstResult);
        }

        [HttpGet]
        public JsonResult GetCurrentComplaint(string PatientID)
        {
            List<OldSymptomView> lstCurrentsym = new List<OldSymptomView>();
            try
            {
                lstCurrentsym = _symptonRepo.GetCurrentComplaint(PatientID);
            }
            catch (Exception)
            {

            }
            return Json(new { Symptom = lstCurrentsym });
        }
        [HttpPost]
        public JsonResult CreateNewPatHistory([FromBody] PatientHistoryView historyView)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            string CurrentDateTime = timezoneUtility.Gettimezone(Timezoneid);

            List<PatientHistoryView> lstResult = new List<PatientHistoryView>();
            long HistroySeqID = _emrRepo.IsUpdateCheck(historyView.VisitID, CurrentDateTime);
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            PatientHistory history = new PatientHistory();

            if (HistroySeqID > 0)
            {
                history.HistorySeqID = HistroySeqID;
                history.PatientID = historyView.PatientID;
                history.VisitID = historyView.VisitID;
                history.SimilarEpisodes = historyView.SimilarEpisodes;
                history.Duration = historyView.Duration;
                history.TreatmentTaken = historyView.TreatmentTaken;
                history.BloodTransfusion = historyView.BloodTransfusion;
                history.ReactionComments = historyView.ReactionComments;
                history.GenitalReproductiveHistory = historyView.GenitalReproductiveHistory;
                history.Gravida = historyView.Gravida;
                history.Para = historyView.Para;
                history.Abortion = historyView.Abortion;
                history.Proterm = historyView.Proterm;
                history.FullTerm = historyView.FullTerm;
                history.LivingChildren = historyView.LivingChildren;
                history.MenstrualHistory = historyView.MenstrualHistory;
                history.MenopauseText = historyView.MenopauseText;
                history.MenopauseText = historyView.MenopauseText;
                if (historyView.DateArray.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(historyView.DateArray[0]))
                        history.LMPDate = Convert.ToDateTime(historyView.DateArray[0]);
                    else
                        history.LMPDate = DateTime.Now;
                }


                history.ASNSAD = historyView.ASNSAD;
                history.COX2 = historyView.COX2;
                history.Insulin = historyView.Insulin;
                history.Anticoagulant = historyView.Anticoagulant;
                history.RelevantHistory = historyView.RelevantHistory;
                history.SurgicalHistory = historyView.SurgicalHistory;
                history.CreateUser = HttpContext.Session.GetString("Userseqid");
                history.Createdatetime = timezoneUtility.Gettimezone(Timezoneid);
                history.Modifiedatetime = timezoneUtility.Gettimezone(Timezoneid);
                history.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                history.IsActive = historyView.IsActive;
               
                int Result = _emrRepo.UpdatePatinetHistory(history);
                CreatePatHaibt(historyView.PatHabit, history.HistorySeqID, history.PatientID, history.VisitID);
                CreatePatAllergies(historyView.Allergies, history.HistorySeqID, history.PatientID, history.VisitID);
                CreatePatMedicalHistory(historyView.PatMedicalHistory, history.HistorySeqID, history.PatientID, history.VisitID);
                CreatePatMedicineHistory(historyView.PatMedicine, history.HistorySeqID, history.PatientID, history.VisitID);
                CreatePatSurgicalHistory(historyView.PatSurgical, history.HistorySeqID, history.PatientID, history.VisitID);
                string EventName = "";
                if (history.IsActive)
                {
                    EventName = "Update and Verify Past History for Patient and UHID-" + history.PatientID;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    EventName = "Update and Save Past History for Patient and UHID-" + history.PatientID;
                    CreateEventManagemnt(EventName);
                }
            }
            else
            {
                history.PatientID = historyView.PatientID;
                history.VisitID = historyView.VisitID;
                history.SimilarEpisodes = historyView.SimilarEpisodes;
                history.Duration = historyView.Duration;
                history.TreatmentTaken = historyView.TreatmentTaken;
                history.BloodTransfusion = historyView.BloodTransfusion;
                history.ReactionComments = historyView.ReactionComments;
                history.GenitalReproductiveHistory = historyView.GenitalReproductiveHistory;
                history.Gravida = historyView.Gravida;
                history.Para = historyView.Para;
                history.Abortion = historyView.Abortion;
                history.Proterm = historyView.Proterm;
                history.FullTerm = historyView.FullTerm;
                history.LivingChildren = historyView.LivingChildren;
                history.MenstrualHistory = historyView.MenstrualHistory;
                history.MenopauseText = historyView.MenopauseText;
                history.MenopauseText = historyView.MenopauseText;
                if (historyView.DateArray.Length > 0)
                {
                    if (!string.IsNullOrWhiteSpace(historyView.DateArray[0]))
                        history.LMPDate = Convert.ToDateTime(historyView.DateArray[0]);
                    else
                        history.LMPDate = DateTime.Now;
                }

                history.ASNSAD = historyView.ASNSAD;
                history.COX2 = historyView.COX2;
                history.Insulin = historyView.Insulin;
                history.Anticoagulant = historyView.Anticoagulant;
                history.RelevantHistory = historyView.RelevantHistory;
                history.SurgicalHistory = historyView.SurgicalHistory;
                history.CreateUser = HttpContext.Session.GetString("Userseqid");
                history.Createdatetime = timezoneUtility.Gettimezone(Timezoneid);
                history.Modifiedatetime = timezoneUtility.Gettimezone(Timezoneid);
                history.ModifiedUser = HttpContext.Session.GetString("Userseqid");
                history.IsActive = historyView.IsActive;
                history.HospitalID = HospitalId;
                int Result = _emrRepo.CreateNewPatinetHistory(history);
                if (Result > 0)
                {
                    CreatePatHaibt(historyView.PatHabit, Result, history.PatientID, history.VisitID);
                    CreatePatAllergies(historyView.Allergies, Result, history.PatientID, history.VisitID);
                    CreatePatMedicalHistory(historyView.PatMedicalHistory, Result, history.PatientID, history.VisitID);
                    CreatePatMedicineHistory(historyView.PatMedicine, Result, history.PatientID, history.VisitID);
                    CreatePatSurgicalHistory(historyView.PatSurgical, Result, history.PatientID, history.VisitID);
                }
                string EventName = "";
                if (history.IsActive)
                {
                    EventName = "Save and Verify Past History for Patient and UHID-" + history.PatientID;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    EventName = "Save Past History for Patient and UHID-" + history.PatientID;
                    CreateEventManagemnt(EventName);
                }
            }
            lstResult = _emrRepo.GetLatestPatientHistoryByPatientID(historyView.PatientID,HospitalId);
            return Json(new
            {
                PatHistroy = lstResult,
            });
        }
        [HttpGet]
        public JsonResult GetLatestPatHistoryByVisitID(string PatientID)
        {
            List<PatientHistoryView> lstResult = new List<PatientHistoryView>();
            List<PatHabitView> lstPatHabit = new List<PatHabitView>();
            List<PatMedicalHistoryView> lstMediclHistory = new List<PatMedicalHistoryView>();
            List<string> lstAllergies = new List<string>();
            List<PatientMedicineView> lstMedicine = new List<PatientMedicineView>();
            List<PatientSurgicalHistory> lstPatSurgical = new List<PatientSurgicalHistory>();
            try
            {
                lstResult = _emrRepo.GetLatestPatHistoryByVisitID(PatientID);
                if (lstResult.Count > 0)
                {
                    long historyID = lstResult[0].HistorySeqID;
                    lstPatHabit = _emrRepo.GetPatinetHabitsByID(historyID);
                    lstAllergies = _emrRepo.GetPatinetAllergiesByHistoryID(historyID);
                    lstMediclHistory = _emrRepo.GetPatMedicalHistoryByID(historyID);
                    lstMedicine = _emrRepo.GetPatCurrentMedicineById(historyID);
                    lstPatSurgical = _emrRepo.GetPatSurgicalHistoryByHistoryID(historyID);
                }
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatHistroy = lstResult,
                Pathabits = lstPatHabit,
                PatAllergy = lstAllergies,
                PatMedicalHstd = lstMediclHistory,
                PatMedicine = lstMedicine,
                PatSurgical = lstPatSurgical
            });
        }
        [HttpGet]
        public JsonResult GetLatestPatientHistoryByPatientID(string PatientID)
        {
            List<PatientHistoryView> lstResult = new List<PatientHistoryView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _emrRepo.GetLatestPatientHistoryByPatientID(PatientID, HospitalId);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatHistroy = lstResult,
            });
        }
        [HttpGet]
        public JsonResult GetPatHistoryByHistoryID(long HistoryID)
        {
            List<PatientHistoryView> lstResult = new List<PatientHistoryView>();
            List<PatHabitView> lstPatHabit = new List<PatHabitView>();
            List<PatMedicalHistoryView> lstMediclHistory = new List<PatMedicalHistoryView>();
            List<string> lstAllergies = new List<string>();
            List<PatientMedicineView> lstMedicine = new List<PatientMedicineView>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            List<PatientSurgicalHistory> lstPatSurgical = new List<PatientSurgicalHistory>();
            List<ClinicMaster> lstClinic = new List<ClinicMaster>();
            try
            {

                long historyID = HistoryID;
                lstResult = _emrRepo.GetPatientHistoryByHistoryID(historyID);
                lstPatHabit = _emrRepo.GetPatinetHabitsByID(historyID);
                lstAllergies = _emrRepo.GetPatinetAllergiesByHistoryID(historyID);
                lstMediclHistory = _emrRepo.GetPatMedicalHistoryByID(historyID);
                lstMedicine = _emrRepo.GetPatCurrentMedicineById(historyID);
                lstPatSurgical = _emrRepo.GetPatSurgicalHistoryByHistoryID(historyID);
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                lstClinic = _clinicRepo.GetClinicDetails(Hospitalid, lstresult[0].ClinicID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatHistroy = lstResult,
                Pathabits = lstPatHabit,
                PatAllergy = lstAllergies,
                PatMedicalHstd = lstMediclHistory,
                PatMedicine = lstMedicine,
                HospitalDtl = lstHospital,
                PatientDtl = lstresult,
                PatSurgical = lstPatSurgical,
                ClinicDtl = lstClinic
            });
        }
        [HttpGet]
        public JsonResult CreateNewHealthHabit(string HabitName)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            HealthHabit habit = new HealthHabit
            {
                Habits = HabitName,
                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
            };
            int HabitSeqID = 0;
            if (!string.IsNullOrWhiteSpace(HabitName))
                HabitSeqID = _emrRepo.CreateNewHealthHabit(habit);
            if (HabitSeqID > 0)
            {
                habit.HealthHabitsSeqID = HabitSeqID;
                habit.Habits = habit.Habits;
                string EventName = "Add New Heath Habit" + habit.Habits;
                CreateEventManagemnt(EventName);
            }
            else
            {
                habit.Habits = "Already Exists";
            }
            return Json(habit);
        }
        [HttpPost]
        public JsonResult CreateNewHabitLsit([FromBody] HabitView[] habitArray)
        {
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            long HdrID = _emrRepo.IsExistsHabitPicHdr(habitArray[0].PickListName, HospitalID);
            string PickLstClick = habitArray[0].HPicklstClick;
            string Result = "";
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            if (HdrID > 0 && PickLstClick.Equals("Old"))
            {
                for (int count = 0; count < habitArray.Length; count++)
                {
                    HealthHabitPickDetails pickDetails = new HealthHabitPickDetails
                    {
                        HabitHeaderSeqID = HdrID,
                        HabitSeqID = habitArray[0].HabitSeqID,
                    };
                    int rowaffetced = _emrRepo.CreateNewHbitPickListDetails(pickDetails);
                }
                string EventName = "Health Habit Pick List Update " + habitArray[0].PickListName;
                CreateEventManagemnt(EventName);
                Result = "Save Success";
            }
            else if (HdrID > 0 && PickLstClick.Equals("New"))
            {
                Result = "Pick list " + habitArray[0].PickListName + " already exists. Select a new name for creating a new pick list" +
                             " or select Save To feature to add new Health Habit";
            }
            else
            {
                if (habitArray.Length > 0)
                {
                    HabitView habit = habitArray[0];
                    HealthHabitPicklistMaster picklistMaster = new HealthHabitPicklistMaster
                    {
                        PickListName = habit.PickListName,
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                    };
                    int HeaderSeqID = _emrRepo.CreateNewHbitPickListHeader(picklistMaster);
                    //int rowaffected = _emrRepo.DeleteHealthHabitsDetails(HeaderSeqID);
                    for (int count = 0; count < habitArray.Length; count++)
                    {
                        habit = habitArray[count];
                        if (HeaderSeqID > 0)
                        {
                            HealthHabitPickDetails pickDetails = new HealthHabitPickDetails
                            {
                                HabitHeaderSeqID = HeaderSeqID,
                                HabitSeqID = habit.HabitSeqID,
                            };
                            int rowaffetced = _emrRepo.CreateNewHbitPickListDetails(pickDetails);
                        }
                    }

                    string EventName = "New Health Habit Pick List Added " + habitArray[0].PickListName;
                    CreateEventManagemnt(EventName);
                    Result = "Save Success";
                }
            }
            return Json(Result);
        }
        [HttpGet]
        public JsonResult GetHabitsByPickListName(string PickListName)
        {
            List<HabitView> lstHabit = new List<HabitView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstHabit = _emrRepo.GetHabitsByPickListName(PickListName, HospitalId);
            }
            catch (Exception)
            {

            }
            return Json(new { Header = lstHabit });

        }
        [HttpGet]
        public JsonResult CreateNewAllergy(string Allergy)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            Allergies allergies = new Allergies
            {
                Allergy = Allergy,
                CreateUser = HttpContext.Session.GetString("Userseqid"),
                Createdatetime = timezoneUtility.Gettimezone(Timezoneid),
                Modifiedatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifiedUser = HttpContext.Session.GetString("Userseqid")
            };
            int Value = _emrRepo.CreateNewAllergy(allergies);
            string EventName = "New Allergy Added " + Allergy;
            CreateEventManagemnt(EventName);
            return Json(Value);
        }
        [HttpGet]
        public JsonResult CreateNewDiseases(string DiseasesName)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            Diseases diseases = new Diseases
            {
                DiseasesName = DiseasesName,
                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                IsActive = true
            };
            int Value = _emrRepo.CreateNewDiseases(diseases);
            if (Value > 0)
            {
                string EventName = "New Diseases Added " + DiseasesName;
                CreateEventManagemnt(EventName);
            }
            else
            {
                diseases.DiseasesName = "Already Exists";
            }
            return Json(diseases);
        }
        [HttpPost]
        public JsonResult CreateNewDiseasesLsit([FromBody] DiseasesView[] diseasesArray)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            string Result = "SaveResult";
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            long DHdr = _emrRepo.IsExistsDiseasesPicHdr(diseasesArray[0].PickListName, HospitalID);
            if (DHdr > 0 && diseasesArray[0].DPickClick.Equals("Old"))
            {
                for (int count = 0; count < diseasesArray.Length; count++)
                {
                    DiseasesPickLstDeatils pickDetails = new DiseasesPickLstDeatils
                    {
                        DiseasesPickID = DHdr,
                        DiseasesSeqId = diseasesArray[count].DiseasesSeqId
                    };
                    int rowaffetcted = _emrRepo.CreateNewDiseasesPickLstDetails(pickDetails);
                }
                string EventName = "Comorbadities Pick List Added " + diseasesArray[0].PickListName;
                CreateEventManagemnt(EventName);
                Result = "Save Success";
            }
            else if (DHdr > 0 && diseasesArray[0].DPickClick.Equals("New"))
            {
                Result = "Pick list " + diseasesArray[0].PickListName + " already exists. Select a new name for creating a new pick list" +
                             " or select Save To feature to add new Diseases";
            }
            else
            {
                if (diseasesArray.Length > 0)
                {
                    for (int count = 0; count < diseasesArray.Length; count++)
                    {
                        DiseasesView diseases = diseasesArray[count];
                        DiseasesPicLstHeader picklistMaster = new DiseasesPicLstHeader
                        {
                            PickListName = diseases.PickListName,
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                            HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))

                        };
                        int HeaderSeqID = _emrRepo.CreateNewDiseasesPickLstHeader(picklistMaster);
                        if (HeaderSeqID > 0)
                        {
                            DiseasesPickLstDeatils pickDetails = new DiseasesPickLstDeatils
                            {
                                DiseasesPickID = HeaderSeqID,
                                DiseasesSeqId = diseases.DiseasesSeqId,
                            };
                            int rowaffetcted = _emrRepo.CreateNewDiseasesPickLstDetails(pickDetails);
                        }
                    }
                    string EventName = "Comorbadities Pick List Update " + diseasesArray[0].PickListName;
                    CreateEventManagemnt(EventName);
                    Result = "Save Success";
                }
            }
            return Json(Result);
        }
        [HttpGet]
        public JsonResult GetDiseasesByPickListName(string PickListName)
        {
            List<DiseasesView> lstResult = new List<DiseasesView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _emrRepo.GetAllDiseasesByPickName(PickListName, HospitalId);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult CreateNewMedicine(string Medicine)
        {
            if (!string.IsNullOrWhiteSpace(Medicine))
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                MedicineMaster medicine = new MedicineMaster
                {
                    Medicine = Medicine,
                    CreatedUser = HttpContext.Session.GetString("Userseqid"),
                    CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid")
                };
                int Value = _emrRepo.CreateNewMedicine(medicine);
                if (Value > 0)
                {
                    string EventName = "Add New Current Medication " + Medicine;
                    CreateEventManagemnt(EventName);
                }

            }
            return Json(Medicine);
        }
        [HttpPost]
        public JsonResult CreateNewDrugPickList([FromBody] DrugPickListView[] drugPickList)
        {
            int rowaffected = 0;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            string Result = "Save Success";
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            long DHdr = _emrRepo.IsExistsDrugPicHdr(drugPickList[0].PickListName, HospitalId);
            if (DHdr > 0 && drugPickList[0].DPickClick.Equals("Old"))
            {
                rowaffected = _emrRepo.DeleteDrugPickListById(DHdr);
                for (int count = 0; count < drugPickList.Length; count++)
                {
                    string[] cat = drugPickList[count].DrugName.Split('.');
                    DrugPickListDetails pickListDetails = new DrugPickListDetails
                    {
                        DrugHdSeqID = DHdr,
                        DrugCode = drugPickList[count].DrugCode,
                        DrugCat = cat[0],
                        DrugName = cat[1],
                        Frequency = drugPickList[count].Frequency,
                        Duration = drugPickList[count].Duration,
                        Remarks = drugPickList[count].Remarks
                    };
                    rowaffected = _emrRepo.CreateNewDrugPickLstDetails(pickListDetails);
                }
            }
            else if (DHdr > 0 && drugPickList[0].DPickClick.Equals("New"))
            {
                Result = "Pick list " + drugPickList[0].PickListName + " already exists. Select a new name for creating a new pick list" +
                             " or select Save To feature to add new Drug";
            }
            else
            {
                if (drugPickList.Length > 0)
                {
                    DrugPickListHeader pickListHeader = new DrugPickListHeader
                    {
                        PickListName = drugPickList[0].PickListName,
                        CreateUser = HttpContext.Session.GetString("Userseqid"),
                        CreateDate = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieDate = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                        HospitalID = HospitalId
                    };
                    int Header = _emrRepo.CreateNewDrugListHeader(pickListHeader);
                    if (Header > 0)
                    {
                        rowaffected = _emrRepo.DeleteDrugPickListById(Header);
                        for (int count = 0; count < drugPickList.Length; count++)
                        {
                            string[] cat = drugPickList[count].DrugName.Split('.');
                            DrugPickListDetails pickListDetails = new DrugPickListDetails
                            {
                                DrugHdSeqID = Header,
                                DrugCode = drugPickList[count].DrugCode,
                                DrugCat = cat[0],
                                DrugName = cat[1],
                                Frequency = drugPickList[count].Frequency,
                                Duration = drugPickList[count].Duration,
                                Remarks = drugPickList[count].Remarks
                            };
                            rowaffected = _emrRepo.CreateNewDrugPickLstDetails(pickListDetails);
                        }
                    }
                }
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult CreateNewPatientTreatment([FromBody] PatTreatmentView patTreatment)
        {
            List<PatTreatmentView> lstResult = new List<PatTreatmentView>();
            int rowaffected = 0;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            string CurrentDatetime = timezoneUtility.Gettimezone(Timezoneid);
            int SeqID = _emrRepo.IsCheckApprovePatTreatment(patTreatment.PatientID, patTreatment.VisitID, CurrentDatetime);
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            if (SeqID > 0)
            {
                UpdatePatTreatmentHeader(patTreatment, SeqID);
                string EventName = "";
                if (patTreatment.IsActive)
                {
                    EventName = "Update and Verify Treatment Plan for Patient and UHID-" + patTreatment.PatientID;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    EventName = "Update Treatment Plan for Patient and UHID-" + patTreatment.PatientID;
                    CreateEventManagemnt(EventName);
                }
            }
            else
            {
                if (patTreatment.DrugPickList.Length != 0 || patTreatment.Notes != "" || patTreatment.Diet != "" || patTreatment.GeneralRecommendation != "" || patTreatment.ProcedureName != null || patTreatment.PrecedureNotes != "")
                {
                    PatientTreatmentHeader treatmentHeader = new PatientTreatmentHeader
                    {
                        PatientID = patTreatment.PatientID,
                        VisitID = patTreatment.VisitID,
                        Notes = patTreatment.Notes,
                        Diet = patTreatment.Diet,
                        GeneralRecommendation = patTreatment.GeneralRecommendation,
                        ProcedureName = patTreatment.ProcedureName,
                        PrecedureNotes = patTreatment.PrecedureNotes,
                        Instruction = patTreatment.Instruction,
                        InstructionNotes = patTreatment.InstructionNotes,
                        CreateUser = HttpContext.Session.GetString("Userseqid"),
                        CreateDate = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieDate = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieUser = HttpContext.Session.GetString("Userseqid"),
                        IsActive = patTreatment.IsActive,
                        HospitalID = HospitalId
                    };
                    if (!string.IsNullOrWhiteSpace(patTreatment.ProcedureDate))
                        treatmentHeader.ProcedureDate = Convert.ToDateTime(patTreatment.ProcedureDate);
                    int TrHeader = _emrRepo.CreateNewPatientTreatmentHeader(treatmentHeader);
                    if (TrHeader > 0)
                    {
                        if (patTreatment.DrugPickList.Length > 0)
                        {
                            for (int count = 0; count < patTreatment.DrugPickList.Length; count++)
                            {
                                string[] cat = patTreatment.DrugPickList[count].DrugName.Split('.');
                                PatientDrugDetails patientDrug = new PatientDrugDetails
                                {
                                    PatDrugHdtID = TrHeader,
                                    DrugCode = patTreatment.DrugPickList[count].DrugCode,
                                    DrugCat = cat[0],
                                    DrugName = cat[1],
                                    Frequency = patTreatment.DrugPickList[count].Frequency,
                                    Duration = patTreatment.DrugPickList[count].Duration,
                                    Remarks = patTreatment.DrugPickList[count].Remarks
                                };
                                rowaffected = _emrRepo.CreateNewPatDrugDtl(patientDrug);
                            }
                            string EventName = "";
                            if (patTreatment.IsActive)
                            {
                                EventName = "Save and Verify Treatment Plan for Patient and UHID-" + patTreatment.PatientID;
                                CreateEventManagemnt(EventName);
                            }
                            else
                            {
                                EventName = "Save Treatment Plan for Patient and UHID-" + patTreatment.PatientID;
                                CreateEventManagemnt(EventName);
                            }
                        }
                    }
                    SeqID = TrHeader;
                }

            }
            if (patTreatment.DiagDetails.Length > 0)
            {
                PatDiagnosisView patDiagnosisView = new PatDiagnosisView()
                {
                    PatientId = patTreatment.PatientID,
                    VisitId = patTreatment.VisitID,
                    IsVerified = patTreatment.IsActive,
                    Comments = patTreatment.Notes,
                    DignosisType = patTreatment.DignosisType,
                    DiagDetails = patTreatment.DiagDetails,
                    TreatmentHdrSeqID = SeqID,
                    HospitalID=HospitalId
                };
                CreateNewPatDiagnosis(patDiagnosisView);
            }
            lstResult = _emrRepo.GetAllTreatmentByPatientID(patTreatment.PatientID, HospitalId);
            return Json(new
            {
                PatTreatment = lstResult

            });
        }
        [HttpGet]
        public JsonResult GetDrugPickByPickListName(string PickListName)
        {
            List<DrugPickListView> lstResult = new List<DrugPickListView>();
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            try
            {
                lstResult = _emrRepo.GetDrugPickLstByName(PickListName, HospitalId);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult CreateNewProcedure(string ProcedureName)
        {
            ProcedureMaster procedure = new ProcedureMaster
            {
                ProcedureName = ProcedureName,
                CreateUser = HttpContext.Session.GetString("Userseqid")
            };
            int Result = _emrRepo.CreateNewProcedure(procedure);
            string EventName = "Add New Procedure " + ProcedureName;
            CreateEventManagemnt(EventName);
            return Json(procedure);
        }
        [HttpGet]
        public JsonResult CreateNewInstruction(string Instruction)
        {
            InstructionMaster instruction = new InstructionMaster
            {
                Instruction = Instruction,
                CreateUser = HttpContext.Session.GetString("Userseqid")
            };
            int Result = _emrRepo.CreateNewInstruction(instruction);
            string EventName = "Add New Instruction " + Instruction;
            CreateEventManagemnt(EventName);
            return Json(instruction);
        }

        [HttpGet]
        public JsonResult GetLatestPatTreatment(string PatientId)
        {
            List<PatTreatmentView> lstResult = new List<PatTreatmentView>();
            List<DrugPickListView> lstDrugLsit = new List<DrugPickListView>();
            List<TreatmentDiagnostic> lstdDiagnostics = new List<TreatmentDiagnostic>();
            try
            {
                lstResult = _emrRepo.GetLastPatientTreatmentHeader(PatientId);
                long seqID = lstResult[0].SeqID;
                if (seqID > 0)
                {
                    lstDrugLsit = _emrRepo.GetPatDrugDetailsByID(seqID);
                    lstdDiagnostics = _diagnosisRepo.GetTreatmentDignostic(seqID);
                }
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatTreatment = lstResult,
                PatDrug = lstDrugLsit,
                PatDiang = lstdDiagnostics
            });
        }
        [HttpGet]
        public JsonResult GetAllTreatmentByPatientID(string PatientID)
        {
            List<PatTreatmentView> lstResult = new List<PatTreatmentView>();
            List<DrugPickListView> lstDrugLsit = new List<DrugPickListView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _emrRepo.GetAllTreatmentByPatientID(PatientID, HospitalId);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatTreatment = lstResult
            });
        }
        [HttpGet]
        public JsonResult GetPatDrugDetailsByID(long SeqID)
        {
            List<DrugPickListView> lstDrugLsit = new List<DrugPickListView>();
            List<TreatmentDiagnostic> lstdDiagnostics = new List<TreatmentDiagnostic>();
            try
            {
                lstDrugLsit = _emrRepo.GetPatDrugDetailsByID(SeqID);
                lstdDiagnostics = _diagnosisRepo.GetTreatmentDignostic(SeqID);

            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatDrug = lstDrugLsit,
                PatDiang = lstdDiagnostics
            });
        }
        [HttpPost]
        public JsonResult CreateNewExamintion([FromBody] ExaminationView examination)
        {
            int rowaffected = 0;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            ExaminationMaster master = new ExaminationMaster
            {
                ExaminationName = examination.ExaminationName,
                Option1 = examination.Option1,
                Option2 = examination.Option2,
                Option3 = examination.Option3,
                Option4 = examination.Option4,
                Option5 = examination.Option5,
                Option6 = examination.Option6,
                Option7 = examination.Option7,
                Option8 = examination.Option8,
                Option9 = examination.Option9,
                Option10 = examination.Option10,
                Option11 = examination.Option11,
                Option12 = examination.Option12,
                CreateUser = HttpContext.Session.GetString("Userseqid"),
                CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                HospitalID = HospitalId
            };
            rowaffected = _emrRepo.CreateNewExamintion(master);
            string EventName = "Add New Examination " + examination.ExaminationName;
            CreateEventManagemnt(EventName);
            return Json(examination);
        }
        [HttpPost]
        public JsonResult UpdateExamintionByName([FromBody] ExaminationView examination)
        {
            int rowaffected = 0;
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            ExaminationMaster master = new ExaminationMaster
            {
                ExaminationName = examination.ExaminationName,
                Option1 = examination.Option1,
                Option2 = examination.Option2,
                Option3 = examination.Option3,
                Option4 = examination.Option4,
                Option5 = examination.Option5,
                Option6 = examination.Option6,
                Option7 = examination.Option7,
                Option8 = examination.Option8,
                Option9 = examination.Option9,
                Option10 = examination.Option10,
                Option11 = examination.Option11,
                Option12 = examination.Option12,
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                HospitalID = HospitalId
            };
            rowaffected = _emrRepo.UpdateExamintionByName(master);
            rowaffected = _emrRepo.UpdatePicklistExamintionByName(master);
            string EventName = "Update Examination " + examination.ExaminationName;
            CreateEventManagemnt(EventName);
            return Json(examination);
        }
        [HttpGet]
        public JsonResult GetExamintionByName(string Examintion)
        {
            List<ExaminationView> lstResult = new List<ExaminationView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _emrRepo.GetExamintionByName(Examintion, HospitalId);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatExam = lstResult
            });
        }
        [HttpPost]
        public JsonResult CreateExamintionPickList([FromBody] ExaminationPickLstView examinationPick)
        {
            string Result = "Save Success";
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            long EHdr = _emrRepo.IsExistsExmainationPicHdr(examinationPick.PickListName, HospitalID);
            if (EHdr > 0 && examinationPick.EPickClick.Equals("Old"))
            {
                if (examinationPick.GetExamination.Length > 0)
                {
                    int rowaffected = _emrRepo.DeleteExamintionPickLstDtl(EHdr);
                    for (int count = 0; count < examinationPick.GetExamination.Length; count++)
                    {
                        ExamintionPickLstDtl pickLstDtl = new ExamintionPickLstDtl
                        {
                            PickHtdSeqID = EHdr,
                            ExaminationName = examinationPick.GetExamination[count].ExaminationName,
                            Option1 = examinationPick.GetExamination[count].Option1,
                            Option2 = examinationPick.GetExamination[count].Option2,
                            Option3 = examinationPick.GetExamination[count].Option3,
                            Option4 = examinationPick.GetExamination[count].Option4,
                            Option5 = examinationPick.GetExamination[count].Option5,
                            Option6 = examinationPick.GetExamination[count].Option6,
                            Option7 = examinationPick.GetExamination[count].Option7,
                            Option8 = examinationPick.GetExamination[count].Option8,
                            Option9 = examinationPick.GetExamination[count].Option9,
                            Option10 = examinationPick.GetExamination[count].Option10,
                            Option11 = examinationPick.GetExamination[count].Option11,
                            Option12 = examinationPick.GetExamination[count].Option12
                        };
                        rowaffected = _emrRepo.CreateNewExamintionPickLstDtl(pickLstDtl);
                    }
                    string EventName = "Update Examination Pick List" + examinationPick.PickListName;
                    CreateEventManagemnt(EventName);
                }
            }
            else if (EHdr > 0 && examinationPick.EPickClick.Equals("New"))
            {
                Result = "Pick list " + examinationPick.PickListName + " already exists. Select a new name for creating a new pick list" +
                            " or select Save To feature to add new Examination";
            }
            else
            {
                ExaminationPickLstHeader examPickHeader = new ExaminationPickLstHeader
                {
                    PickListName = examinationPick.PickListName,
                    CreateUser = HttpContext.Session.GetString("Userseqid"),
                    CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifieUser = HttpContext.Session.GetString("Userseqid"),
                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                };
                int HeaderSeqID = _emrRepo.CreateNewExamnitionPickLstHeader(examPickHeader);
                if (HeaderSeqID > 0)
                {
                    if (examinationPick.GetExamination.Length > 0)
                    {
                        int rowaffected = _emrRepo.DeleteExamintionPickLstDtl(HeaderSeqID);
                        for (int count = 0; count < examinationPick.GetExamination.Length; count++)
                        {
                            ExamintionPickLstDtl pickLstDtl = new ExamintionPickLstDtl
                            {
                                PickHtdSeqID = HeaderSeqID,
                                ExaminationName = examinationPick.GetExamination[count].ExaminationName,
                                Option1 = examinationPick.GetExamination[count].Option1,
                                Option2 = examinationPick.GetExamination[count].Option2,
                                Option3 = examinationPick.GetExamination[count].Option3,
                                Option4 = examinationPick.GetExamination[count].Option4,
                                Option5 = examinationPick.GetExamination[count].Option5,
                                Option6 = examinationPick.GetExamination[count].Option6,
                                Option7 = examinationPick.GetExamination[count].Option7,
                                Option8 = examinationPick.GetExamination[count].Option8,
                                Option9 = examinationPick.GetExamination[count].Option9,
                                Option10 = examinationPick.GetExamination[count].Option10,
                                Option11 = examinationPick.GetExamination[count].Option11,
                                Option12 = examinationPick.GetExamination[count].Option12
                            };
                            rowaffected = _emrRepo.CreateNewExamintionPickLstDtl(pickLstDtl);
                        }
                        string EventName = "Add New Examination Pick List" + examinationPick.PickListName;
                        CreateEventManagemnt(EventName);
                    }
                }
            }

            return Json(Result);
        }
        [HttpGet]
        public JsonResult GetPickLstByPickName(string PickListName)
        {
            List<ExaminationView> lstResult = new List<ExaminationView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                lstResult = _emrRepo.GetPickLstByPickName(PickListName, HospitalId);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatExam = lstResult
            });
        }
        [HttpPost]
        public JsonResult CreateNewPatientExamintion([FromBody] PatientExamintionView examintionView)
        {
            int rowaffected = 0;
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            string CurrentDateTime = timezoneUtility.Gettimezone(Timezoneid);
            int SeqID = _emrRepo.IsCheckApprovePatExamintion(examintionView.PatientID, examintionView.VisitID, CurrentDateTime);
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            if (SeqID > 0)
            {
                UpdatePatExamintion(examintionView, SeqID);
                string EventName = "";
                if (examintionView.IsActive)
                {
                    EventName = "Update and Verify Examination for Patient and UHID-" + examintionView.PatientID;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    EventName = "Update Examination for Patient and UHID-" + examintionView.PatientID;
                    CreateEventManagemnt(EventName);
                }
            }
            else
            {
                PatientExaminationHeader examinationHeader = new PatientExaminationHeader
                {
                    PatientID = examintionView.PatientID,
                    VisitID = examintionView.VisitID,
                    CreateUser = HttpContext.Session.GetString("Userseqid"),
                    Comments = examintionView.Comments,
                    SpecifiComments = examintionView.SpecifiComments,
                    IsActive = examintionView.IsActive,
                    CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    HospitalID = HospitalId
                };
                int HeaderSeqID = _emrRepo.CreateNewPatientExamintionHrd(examinationHeader);
                if (HeaderSeqID > 0)
                {
                    if (examintionView.GetExamination.Length > 0)
                    {
                        rowaffected = _emrRepo.DeletePatExamintionDetails(SeqID);
                        for (int count = 0; count < examintionView.GetExamination.Length; count++)
                        {
                            PatientExamintionDetails examintionDetails = new PatientExamintionDetails
                            {
                                PatExamSeqID = HeaderSeqID,
                                ExamintionName = examintionView.GetExamination[count].ExaminationName,
                                Option1 = examintionView.GetExamination[count].Option1,
                                Option2 = examintionView.GetExamination[count].Option2,
                                Option3 = examintionView.GetExamination[count].Option3,
                                Option4 = examintionView.GetExamination[count].Option4,
                                Option5 = examintionView.GetExamination[count].Option5,
                                Option6 = examintionView.GetExamination[count].Option6,
                                Option7 = examintionView.GetExamination[count].Option7,
                                Option8 = examintionView.GetExamination[count].Option8,
                                Option9 = examintionView.GetExamination[count].Option9,
                                Option10 = examintionView.GetExamination[count].Option10,
                                Option11 = examintionView.GetExamination[count].Option11,
                                Option12 = examintionView.GetExamination[count].Option12,
                                OptChk1 = examintionView.GetExamination[count].OptChk1,
                                OptChk2 = examintionView.GetExamination[count].OptChk2,
                                OptChk3 = examintionView.GetExamination[count].OptChk3,
                                OptChk4 = examintionView.GetExamination[count].OptChk4,
                                OptChk5 = examintionView.GetExamination[count].OptChk5,
                                OptChk6 = examintionView.GetExamination[count].OptChk6,
                                OptChk7 = examintionView.GetExamination[count].OptChk7,
                                OptChk8 = examintionView.GetExamination[count].OptChk8,
                                OptChk9 = examintionView.GetExamination[count].OptChk9,
                                OptChk10 = examintionView.GetExamination[count].OptChk10,
                                OptChk11 = examintionView.GetExamination[count].OptChk11,
                                OptChk12 = examintionView.GetExamination[count].OptChk12,
                                Notes = examintionView.GetExamination[count].Notes
                            };
                            rowaffected = _emrRepo.CreateNewPatExaminationDtl(examintionDetails);
                            string EventName = "";
                            if (examintionView.IsActive)
                            {
                                EventName = "Save and Verify Examination for Patient and UHID-" + examintionView.PatientID;
                                CreateEventManagemnt(EventName);
                            }
                            else
                            {
                                EventName = "Save Examination for Patient and UHID-" + examintionView.PatientID;
                                CreateEventManagemnt(EventName);
                            }
                        }
                    }
                }
            }
            return Json(examintionView);
        }
        [HttpGet]
        public JsonResult GetLatestPatientExamintion(string PatientId)
        {
            List<PatientExamintionView> lstResult = new List<PatientExamintionView>();
            List<ExaminationView> lstExamView = new List<ExaminationView>();
            long seqId = 0;
            try
            {
                lstResult = _emrRepo.GetLastPatExamHdr(PatientId);
                if (lstResult.Count > 0)
                    seqId = lstResult[0].SeqID;
                lstExamView = _emrRepo.GetLastPatinetExamintion(seqId);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatExam = lstResult,
                ExamView = lstExamView
            });
        }
        [HttpGet]
        public JsonResult GetAllPatientExamination(string PatientID)
        {
            List<PatientExamintionView> lstResult = new List<PatientExamintionView>();
            try
            {
                lstResult = _emrRepo.GetAllPatientExamination(PatientID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatExam = lstResult
            });
        }
        [HttpGet]
        public JsonResult GetLatestPatientExamintionView(long seqID)
        {
            List<ExaminationView> lstExamView = new List<ExaminationView>();
            try
            {
                lstExamView = _emrRepo.GetLastPatinetExamintion(seqID);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                ExamView = lstExamView
            });
        }
        #region Raffi for Investigation
        [HttpPost]
        public JsonResult CreateNewInvestPickList([FromBody] InvestPickHeadView[] pickListArray)
        {
            string Result = "SaveResult";
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            try
            {
                if (pickListArray.Length > 0)
                {
                    for (int count = 0; count < pickListArray.Length; count++)
                    {
                        InvestPickHeadView pickList = pickListArray[count];
                        long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                        long PhSeqId = _investrepo.IsPicNameExists(pickList.PicklistName, Hospitalid);
                        if (PhSeqId > 0)
                        {
                            InvestPickHeader list = new InvestPickHeader
                            {
                                PicklistSeqid = PhSeqId,
                                InvestigationSeqid = pickList.InvestigationSeqid,
                                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                                CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                            };
                            bool isExists = _investrepo.IsInvestigationExists(list.PicklistSeqid, list.InvestigationSeqid);
                            if (!isExists)
                                _investrepo.CreateNewPickListDetails(list);
                        }
                        else
                        {
                            InvestPickHeader header = new InvestPickHeader
                            {
                                PicklistName = pickList.PicklistName,
                                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                                CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                            };
                            bool isSuccess = _investrepo.CreateNewPickListHeader(header);
                            if (isSuccess)
                            {
                                InvestPickHeader list = new InvestPickHeader
                                {
                                    PicklistSeqid = header.PicklistSeqid,
                                    InvestigationSeqid = pickList.InvestigationSeqid,
                                    CreatedUser = HttpContext.Session.GetString("Userseqid"),
                                    CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                    ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                                };
                                bool isExists = _investrepo.IsInvestigationExists(list.PicklistSeqid, list.InvestigationSeqid);
                                if (!isExists)
                                    _investrepo.CreateNewPickListDetails(list);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult CreateNewHISPickList([FromBody] HISPackageView[] pickListArray)
        {
            string Result = "SaveResult";
            try
            {
                long HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                if (pickListArray.Length > 0)
                {
                    decimal TotalAamt = 0;
                    for (int j = 0; j < pickListArray.Length; j++)
                    {
                        decimal TestAmt = pickListArray[j].HIS_TestAmt;
                        TotalAamt = TotalAamt + TestAmt;
                    }
                    for (int count = 0; count < pickListArray.Length; count++)
                    {
                        HISPackageView pickList = pickListArray[count];
                        long PhSeqId = _investrepo.IsHISPicNameExists(pickList.HIS_PACKAGENAME, HospitalID);
                        if (PhSeqId > 0)
                        {
                            HISPackageHeader list = new HISPackageHeader
                            {
                                HIS_PackageID = PhSeqId,
                                HIS_PACKAGENAME = pickList.HIS_PACKAGENAME,
                                HIS_PACKAGEALIASNAME = pickList.HIS_PACKAGENAME,
                                HIS_PACKAGEACTIVE = true,
                                HIS_PACKAGETYPE = "General",
                                HIS_PACKAGETOTALAMT = TotalAamt,
                                HIS_PACKAGEDISC_AMOUNT = 0,
                                HIS_TestID = pickList.HIS_TestID,
                                HIS_TestAmt = pickList.HIS_TestAmt
                            };
                            bool isExists = _investrepo.IsHISInvestigationExists(list.HIS_PackageID, list.HIS_TestID);
                            if (!isExists)
                                _investrepo.CreateNewHISPickListDetails(list);
                        }
                        else
                        {
                            HISPackageHeader header = new HISPackageHeader
                            {
                                HIS_PackageID = PhSeqId,
                                HIS_PACKAGENAME = pickList.HIS_PACKAGENAME,
                                HIS_PACKAGEALIASNAME = pickList.HIS_PACKAGENAME,
                                HIS_PACKAGEACTIVE = true,
                                HIS_PACKAGETYPE = "General",
                                HIS_PACKAGETOTALAMT = TotalAamt,
                                HIS_PACKAGEDISC_AMOUNT = 0,
                                HIS_TestID = pickList.HIS_TestID,
                                HIS_TestAmt = pickList.HIS_TestAmt
                            };
                            long isSuccess = _investrepo.CreateNewHISPickListHeader(header, HospitalID);
                            if (isSuccess > 0)
                            {
                                HISPackageHeader list = new HISPackageHeader
                                {
                                    HIS_PackageID = isSuccess,
                                    HIS_PACKAGENAME = pickList.HIS_PACKAGENAME,
                                    HIS_PACKAGEALIASNAME = pickList.HIS_PACKAGENAME,
                                    HIS_PACKAGEACTIVE = true,
                                    HIS_PACKAGETYPE = "General",
                                    HIS_PACKAGETOTALAMT = TotalAamt,
                                    HIS_PACKAGEDISC_AMOUNT = 0,
                                    HIS_TestID = pickList.HIS_TestID,
                                    HIS_TestAmt = pickList.HIS_TestAmt
                                };
                                bool isExists = _investrepo.IsHISInvestigationExists(isSuccess, list.HIS_TestID);
                                if (!isExists)
                                    _investrepo.CreateNewHISPickListDetails(list);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpGet]
        public JsonResult CreateNewInvestigation(string InvestName)
        {
            InvestigationMasterView view = new InvestigationMasterView();
            try
            {
                string IsConnectedHIS = HttpContext.Session.GetString("IsConnectedHIS");
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                InvestigationMaster master = new InvestigationMaster
                {
                    Investigation_Name = InvestName,
                    Investigation_Rate = 0,
                    CreatedUser = HttpContext.Session.GetString("Userseqid"),
                    CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                };
                bool isSuccess = _investrepo.CreateNewInvestigation(master, IsHISPatient);
                if (isSuccess)
                {
                    view.Investigation_Name = master.Investigation_Name;
                    view.Investigation_Seqid = master.Investigation_Seqid;
                }
                else
                {
                    view.Investigation_Name = "Already Exists";
                    view.Investigation_Seqid = master.Investigation_Seqid;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(view);
        }
        [HttpPost]
        public JsonResult GetPatDetails([FromBody] MyPatient patdet)
        {
            var myComplexObject = new List<MyPatient>();
            List<string> lstAllergies = new List<string>();
            string Allergies = "";
            try
            {
                myComplexObject = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                lstAllergies = _emrRepo.GetPatinetAllergiesByPatient(myComplexObject[0].PatientID);
                if (lstAllergies.Count > 0)
                    Allergies = string.Join(",", lstAllergies.ToArray());
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                Pat = myComplexObject,
                Allergy = Allergies
            });
        }
        [HttpPost]
        public JsonResult CreateNewInvVisit([FromBody] PatientView data)
        {
            string Result = "SaveResult";
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                int lett = 0;
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(data.PatientID, HospitalId);
                int doctorid = Convert.ToInt32(data.DoctorID);
                int Departmentid = _investrepo.GetDepartmentid(doctorid);
                string Visitid = CommonSetting.generateStudyID();
                string Age = data.Age;
                string[] Agesplit = data.Age.Split("/");
                string Ageyear = Agesplit[0];
                if (Ageyear.Contains("Y"))
                    Ageyear = Ageyear.Replace("Y", "");
                string AgeMonth = Agesplit[1];
                if (AgeMonth.Contains("M"))
                    AgeMonth = AgeMonth.Replace("M", "");
                string AgeDays = Agesplit[2];
                if (AgeDays.Contains("D"))
                    AgeDays = AgeDays.Replace("D", "");
                if (data.LetterBody != "")
                {
                    Letter letterlst = new Letter
                    {
                        PatientId = PatSeqid,
                        VisitId = Visitid,
                        LetterBody = data.LetterBody,
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid")
                    };
                    lett = _investrepo.CreateNewLetter(letterlst);
                    if (lett != 0)
                    {
                        PatientRegistration list = new PatientRegistration
                        {
                            PatSeqID = PatSeqid,
                            VisitID = Visitid,
                            AgeYear = Convert.ToInt32(Ageyear),
                            AgeMonth = Convert.ToInt32(AgeMonth),
                            AgeDay = Convert.ToInt32(AgeDays),
                            DoctorID = data.DoctorID,
                            DeptID = Departmentid,
                            Status = "Checked-In",
                            CreatedUser = HttpContext.Session.GetString("Userseqid"),
                            CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                            ModifiedUser = HttpContext.Session.GetString("Userseqid")
                        };
                        int NewVisit = _investrepo.CreateNewVisit(list);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        [HttpGet]
        public JsonResult GetProfiletests(string Patid, int Profileid, int Packageid, long Investid)
        {
            List<HISProfile> lstDresult = new List<HISProfile>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(Patid, HospitalId);
                long Invheadseqid = _investrepo.GetInvHeaderSeqid(PatSeqid);
                List<int> GetTotalorderid = _investrepo.GetOrderids(Invheadseqid);
                if (GetTotalorderid.Count > 0)
                {
                    for (int i = 0; i < GetTotalorderid.Count; i++)
                    {
                        int Orderid = Convert.ToInt32(GetTotalorderid[i]);
                        lstDresult = _investrepo.GetProfilelist(Orderid, Profileid, Packageid, Investid);
                        if (lstDresult != null)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                Profilelist = lstDresult
            });
        }
        [HttpPost]
        public JsonResult CreateNewPatInv([FromBody] PatInvestigationView patInvestigation)
        {
            List<PatientInvestHeader> lstResult = new List<PatientInvestHeader>();
            bool isSuccess = false;
            long SeqID = 0;
            int TestInActiveCount = 0;
            bool HISOrderInsert = false;
            int updateresult = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                List<MyPatient> lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                string HISVstid = lstresult[0].HISVisitID;
                int HISDocid = lstresult[0].HISDocID;
                int HISDeptid = lstresult[0].HISDeptID;
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                decimal TotalAmt = 0;
                decimal TotVisitamt = 0;
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(patInvestigation.Patientid, HospitalId);
                PatientInvestDetails[] patientInvestDetails = patInvestigation.InvDetails;
                if (patInvestigation.IsVerified)
                {
                    if (patientInvestDetails.Length > 0)
                    {
                        for (int i = 0; i < patientInvestDetails.Length; i++)
                        {
                            decimal InvestigationTotal = 0;
                            long Packageid = patientInvestDetails[i].InvestigationSeqid;
                            bool TestActiveCheck = _investrepo.IsInvestigationActive(Packageid);
                            if (TestActiveCheck == true)
                            {
                                bool HIS_Package = patientInvestDetails[i].HIS_PackageActive;
                                if (HIS_Package == true)
                                    InvestigationTotal = _investrepo.GetPackageHeaderAmt(Packageid);
                                else
                                    InvestigationTotal = patientInvestDetails[i].InvestigationTotal;

                                TotalAmt = TotalAmt + InvestigationTotal;
                            }
                            else
                                TestInActiveCount += 1;
                        }
                    }
                }

                TotVisitamt = TotalAmt;
                SeqID = _investrepo.IsPatInvVisitIDExists(patInvestigation.Visitid);
                if (patInvestigation.IsVerified)
                {
                    if (SeqID > 0)
                    {
                        decimal TotalordAmt = _investrepo.GetTotalOrderAmt(patInvestigation.Visitid);
                        TotalAmt = TotalAmt + TotalordAmt;
                    }
                }

                if (TestInActiveCount == patientInvestDetails.Length)
                    HISOrderInsert = false;
                else
                    HISOrderInsert = true;

                PatientInvestHeader lstinvest = new PatientInvestHeader()
                {
                    Patientid = PatSeqid.ToString(),
                    Visitid = patInvestigation.Visitid,
                    GrandTotal = TotalAmt,
                    GrandDiscount = 0,
                    NetTotal = TotalAmt,
                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                    ClinicID = Convert.ToInt64(HttpContext.Session.GetString("Clinicid")),
                    InvestigationNotes = patInvestigation.InvestigationNotes,
                    Departmentid = patInvestigation.Departmentid,
                    DepartmentRefNotes = patInvestigation.DepartmentRefNotes,
                    Doctorid = patInvestigation.Doctorid,
                    DoctorRefNotes = patInvestigation.DoctorRefNotes,
                    CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    CreatedUser = HttpContext.Session.GetString("Userseqid"),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    IsActive = true,
                    IsVerified = patInvestigation.IsVerified,
                    TotalVisitAmt = TotVisitamt
                };
                if (SeqID > 0)
                {
                    _investrepo.UpdatePatInvHeader(lstinvest, IsHISPatient, HISVstid, HISDocid, HISDeptid, patInvestigation.Patientid, HISOrderInsert);
                    lstinvest.Seqid = SeqID;
                    isSuccess = true;
                }
                else
                    isSuccess = _investrepo.CreateNewPatInvHeader(lstinvest, IsHISPatient, HISVstid, HISDocid, HISDeptid, patInvestigation.Patientid, HISOrderInsert);
                if (isSuccess)
                {
                    if (patInvestigation.InvDetails.Count() > 0)
                    {
                        if (patInvestigation.IsVerified == false)
                            _investrepo.DeletePatInvDtl(lstinvest.Seqid);
                        for (int count = 0; count < patInvestigation.InvDetails.Count(); count++)
                        {
                            int Orderlistid = count + 1;
                            PatientInvestDetails details = new PatientInvestDetails()
                            {
                                PatInvestHeaderSeqid = lstinvest.Seqid,
                                InvestigationSeqid = patInvestigation.InvDetails[count].InvestigationSeqid,
                                InvestigationName = patInvestigation.InvDetails[count].InvestigationName,
                                InvestigationTotal = patInvestigation.InvDetails[count].InvestigationTotal,
                                InvestigationDiscount = 0,
                                InvestigationNetAmt = patInvestigation.InvDetails[count].InvestigationTotal,
                                CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                                CreatedUser = HttpContext.Session.GetString("Userseqid"),
                                ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                                IsActive = true,
                                IsVerified = patInvestigation.IsVerified,
                                Visitid = patInvestigation.Visitid,
                                HIS_PackageActive = patInvestigation.InvDetails[count].HIS_PackageActive,
                                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                            };
                            int dtlResult = _investrepo.CreateNewPatInvDtl(details, IsHISPatient, HISVstid, HISDocid, HISDeptid, patInvestigation.Patientid, Orderlistid, patInvestigation.IsVerified);

                            if (count == 0)
                                updateresult = dtlResult;
                        }
                        if (updateresult == 1)
                        {
                            DataTable GetConfig = _investrepo.GetHISConfigDetails();
                            int Orderid = Convert.ToInt32(GetConfig.Rows[0]["orderid"]) + 1;
                            int Invoiceid = Convert.ToInt32(GetConfig.Rows[0]["Invoiceid"]) + 1;
                            bool updateconfig = _investrepo.UpdateConfigDetails(Orderid, Invoiceid);
                        }
                    }
                    lstResult = _investrepo.GetInvestigationByPatid(PatSeqid);
                    //lstResult = _investrepo.GetInvestigationViewByID(lstinvest.Seqid, IsHISPatient);
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                Header = lstResult,
            });
        }
        [HttpPost]
        public JsonResult GetLastInvestigation([FromBody] string Patientid)
        {
            List<OldInvestigationView> lstResult = new List<OldInvestigationView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(Patientid, HospitalId);
                long PatHeadSeqid = _investrepo.GetInvHeaderSeqid(PatSeqid);
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                lstResult = _investrepo.GetInvestigationViewByID(PatHeadSeqid, IsHISPatient);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult GetInvestigationDetailsByHid(long HeaderSeqID)
        {
            List<OldInvestigationView> lstResult = new List<OldInvestigationView>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                lstResult = _investrepo.GetInvestigationViewByID(HeaderSeqID, IsHISPatient);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult GetLatestPatInvestigation(string PatientId)
        {
            List<PatientInvestView> lstResult = new List<PatientInvestView>();
            List<PatientInvestDetails> lstInvestlist = new List<PatientInvestDetails>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(PatientId, HospitalId);
                lstResult = _investrepo.GetLastPatientInvestHeader(PatSeqid.ToString());
                long seqID = lstResult[0].Seqid;
                if (seqID > 0)
                    lstInvestlist = _investrepo.GetPatInvestDetailsByID(seqID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                PatInvestHeader = lstResult,
                PatInvestDetail = lstInvestlist,
            });
        }
        #endregion
        #region Raffi for Diagnosis
        [HttpGet]
        public JsonResult CreateNewDiagnosis(string DiagnosisName)
        {
            DiagnosisMasterView view = new DiagnosisMasterView();
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                DiagnosisMaterCls master = new DiagnosisMaterCls
                {
                    Diagnosis_Name = DiagnosisName,
                    CreatedUser = HttpContext.Session.GetString("Userseqid"),
                    CreatedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
                };
                bool isSuccess = _diagnosisRepo.CreateNewDiagnosisMaster(master);
                if (isSuccess)
                {
                    view.Diagnosis_Name = master.Diagnosis_Name;
                    view.Diagnosis_Seqid = master.Diagnosis_Seqid;
                    string EventName = "Add New Diagnosis " + master.Diagnosis_Name;
                    CreateEventManagemnt(EventName);
                }
                else
                {
                    view.Diagnosis_Name = "Already Exists";
                    view.Diagnosis_Seqid = master.Diagnosis_Seqid;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(view);
        }
        [HttpPost]
        public JsonResult CreateNewPatDiagnosis([FromBody] PatDiagnosisView patDiagnosisView)
        {
            List<PatientDiagnosisHeader> lstHeader = new List<PatientDiagnosisHeader>();
            List<OldDiagnosisView> lstResult = new List<OldDiagnosisView>();
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                List<MyPatient> lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(patDiagnosisView.PatientId, HospitalId);
                PatientDiagnosisDetail[] patientDiagnosisDetails = patDiagnosisView.DiagDetails;
                PatientDiagnosisHeader lstDiag = new PatientDiagnosisHeader()
                {
                    PatientId = PatSeqid.ToString(),
                    VisitId = patDiagnosisView.VisitId,
                    HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid")),
                    ClinicID = Convert.ToInt64(HttpContext.Session.GetString("Clinicid")),
                    Comments = patDiagnosisView.Comments,
                    CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    CreatedUser = HttpContext.Session.GetString("Userseqid"),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    IsActive = true,
                    IsVerified = patDiagnosisView.IsVerified,
                    DignosisType = patDiagnosisView.DignosisType,
                    TreatmentHdrSeqID = patDiagnosisView.TreatmentHdrSeqID,
                };
                long SeqID = _diagnosisRepo.IsPatDiagVisitIDExists(patDiagnosisView.VisitId);
                if (SeqID > 0)
                {
                    _diagnosisRepo.UpdatePatDiagHeader(lstDiag);
                    lstDiag.Seqid = SeqID;
                    string EventName = "";
                    if (patDiagnosisView.IsActive)
                    {
                        EventName = "Update and Verify Diagnosis for Patient and UHID-" + patDiagnosisView.PatientId;
                        CreateEventManagemnt(EventName);
                    }
                    else
                    {
                        EventName = "Update Diagnosis for Patient and UHID-" + patDiagnosisView.PatientId;
                        CreateEventManagemnt(EventName);
                    }
                }
                else
                {
                    lstDiag.Seqid = _diagnosisRepo.CreateNewPatDiagHeader(lstDiag);
                }
                if (lstDiag.Seqid > 0)
                {
                    if (patDiagnosisView.DiagDetails.Count() > 0)
                    {
                        _diagnosisRepo.DeletePatDiagnosisDtl(lstDiag.Seqid);
                        for (int count = 0; count < patDiagnosisView.DiagDetails.Count(); count++)
                        {
                            PatientDiagnosisDetail details = new PatientDiagnosisDetail()
                            {
                                HeaderSeqid = lstDiag.Seqid,
                                DiagnosisId = patDiagnosisView.DiagDetails[count].DiagnosisId,
                                DiagnosisName = patDiagnosisView.DiagDetails[count].DiagnosisName,
                                IsActive = true,
                                Remarks = patDiagnosisView.DiagDetails[count].Remarks
                            };
                            int dtlResult = _diagnosisRepo.CreateNewPatDiagnosisDtl(details);
                        }
                        string EventName = "";
                        if (patDiagnosisView.IsActive)
                        {
                            EventName = "Update and Verify Diagnosis for Patient and UHID-" + patDiagnosisView.PatientId;
                            CreateEventManagemnt(EventName);
                        }
                        else
                        {
                            EventName = "Update Diagnosis for Patient and UHID-" + patDiagnosisView.PatientId;
                            CreateEventManagemnt(EventName);
                        }
                    }
                    // lstResult = _diagnosisRepo.GetDiagnosisViewByID(lstDiag.Seqid);
                }
                lstHeader = _diagnosisRepo.GetDiagnosisByPatid(PatSeqid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader });
        }
        [HttpPost]
        public JsonResult GetOldDiagnosis([FromBody] string Patientid)
        {
            List<OldDiagnosisView> lstResult = new List<OldDiagnosisView>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(Patientid, HospitalId);
                long PatHeadSeqid = _diagnosisRepo.GetDiagnosisHeaderSeqid(PatSeqid);
                lstResult = _diagnosisRepo.GetDiagnosisViewByID(PatHeadSeqid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(lstResult);
        }
        #endregion
        #region Raffi for More button in Diagnosis
        [HttpGet]
        public JsonResult GetDiagnosisByPatient(string PatID)
        {
            List<PatientDiagnosisHeader> lstHeader = new List<PatientDiagnosisHeader>();
            List<PatientDiagnosisDetail> lstDeatils = new List<PatientDiagnosisDetail>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(PatID, HospitalId);
                lstHeader = _diagnosisRepo.GetDiagnosisByPatid(PatSeqid);
                lstDeatils = _diagnosisRepo.GetDiagDetailsByPatid(PatSeqid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Details = lstDeatils });
        }
        [HttpGet]
        public JsonResult GetDiagnosisHeaderByPatient(string PatID)
        {
            List<PatientDiagnosisHeader> lstHeader = new List<PatientDiagnosisHeader>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(PatID, HospitalId);
                lstHeader = _diagnosisRepo.GetDiagnosisByPatid(PatSeqid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader });
        }
        [HttpGet]
        public JsonResult GetDiagnosisDeatilsByPatient(long HeaderSeqID)
        {
            List<PatientDiagnosisDetail> lstDeatils = new List<PatientDiagnosisDetail>();
            try
            {
                lstDeatils = _diagnosisRepo.GetDiagDetailsByDigHdrID(HeaderSeqID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Details = lstDeatils });
        }
        #endregion
        #region Raffi for More button in Investigation
        [HttpGet]
        public JsonResult GetInvestigationByPatient(string PatID)
        {
            List<PatientInvestHeader> lstHeader = new List<PatientInvestHeader>();
            List<PatientInvestDetails> lstDeatils = new List<PatientInvestDetails>();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                long PatSeqid = _investrepo.GetPatSeqid(PatID, HospitalId);
                lstHeader = _investrepo.GetInvestigationByPatid(PatSeqid);
                lstDeatils = _investrepo.GetInvestDetailsByPatid(PatSeqid, IsHISPatient);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Details = lstDeatils });
        }
        #endregion
        #region Raffi for Investigation Print
        [HttpGet]
        public JsonResult GetInvestigationByVisit(string Visitid)
        {
            List<PatientInvestHeader> lstHeader = new List<PatientInvestHeader>();
            List<PatientInvestDetails> lstDeatils = new List<PatientInvestDetails>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHeader = _investrepo.GetInvestigationByVisitid(Visitid);
                if (lstHeader.Count > 0)
                {
                    lstDeatils = _investrepo.GetInvestDetailsByVisitid(Visitid, IsHISPatient);
                    lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                    lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                }

            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Details = lstDeatils, HospitalDtl = lstHospital, PatientDtl = lstresult });
        }
        [HttpGet]
        public JsonResult GetProfileDetails(int Profileid, int HISOrderid)
        {
            List<PatientInvestDetails> lstDeatils = new List<PatientInvestDetails>();
            try
            {
                lstDeatils = _investrepo.GetProfileDetailsPrint(Profileid, HISOrderid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { ProfileDetails = lstDeatils });
        }
        [HttpGet]
        public JsonResult GetPackageDetails(int Packageid, int HISOrderid)
        {
            List<PatientInvestDetails> lstDeatils = new List<PatientInvestDetails>();
            try
            {
                lstDeatils = _investrepo.GetPackageDetailsPrint(Packageid, HISOrderid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { PackageDetails = lstDeatils });
        }
        #endregion
        #region Raffi for Diagnosis Print
        [HttpGet]
        public JsonResult GetDiagnosisByVisit(string Visitid)
        {
            List<PatientDiagnosisHeader> lstHeader = new List<PatientDiagnosisHeader>();
            List<PatientDiagnosisDetail> lstDeatils = new List<PatientDiagnosisDetail>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHeader = _diagnosisRepo.GetDiagnosisByVisit(Visitid);
                lstDeatils = _diagnosisRepo.GetDiagDetailsByVisit(Visitid);
                lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Details = lstDeatils, HospitalDtl = lstHospital, PatientDtl = lstresult });
        }
        #endregion
        #region Raffi for Treatment Report
        [HttpGet]
        public JsonResult GetTreatmentByVisit(string Visitid)
        {
            List<PatientTreatmentHeader> lstHeader = new List<PatientTreatmentHeader>();
            List<PatientDrugDetails> lstDeatils = new List<PatientDrugDetails>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                lstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Details = lstDeatils, HospitalDtl = lstHospital, PatientDtl = lstresult });
        }
        #endregion
        #region Raffi for Compliant Report
        [HttpGet]
        public JsonResult GetCompliantByVisit(string Visitid)
        {
            List<PatientSympotmHeader> lstHeader = new List<PatientSympotmHeader>();
            List<PatientSympotmDetails> lstDeatils = new List<PatientSympotmDetails>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHeader = _symptonRepo.GetSymptomHeaderByVisit(Visitid, Hospitalid);
                lstDeatils = _symptonRepo.GetSymptomDetailsByVisit(Visitid);
                lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Details = lstDeatils, HospitalDtl = lstHospital, PatientDtl = lstresult });
        }
        #endregion
        [HttpGet]
        public JsonResult GetVerifyDetails()
        {
            bool IsSuccess = false;
            try
            {
                int PatientSubmenu = Convert.ToInt32(HttpContext.Session.GetString("PatientSubmenuid"));
                long Roleid = Convert.ToInt64(HttpContext.Session.GetString("Roleid"));
                IsSuccess = _emrRepo.GetVerifyDetail(PatientSubmenu, Roleid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(IsSuccess);
        }
        #region Raffi for All Visit Report
        [HttpGet]
        public JsonResult GetAllReportByVisit(string Visitid)
        {
            List<PatientSympotmHeader> ComplstHeader = new List<PatientSympotmHeader>();
            List<PatientSympotmDetails> ComplstDeatils = new List<PatientSympotmDetails>();
            List<PatientTreatmentHeader> TreatlstHeader = new List<PatientTreatmentHeader>();
            List<PatientDrugDetails> TreatlstDeatils = new List<PatientDrugDetails>();
            List<PatientDiagnosisHeader> DiaglstHeader = new List<PatientDiagnosisHeader>();
            List<PatientDiagnosisDetail> DiaglstDeatils = new List<PatientDiagnosisDetail>();
            List<PatientInvestHeader> InvestlstHeader = new List<PatientInvestHeader>();
            List<PatientInvestDetails> InvestlstDeatils = new List<PatientInvestDetails>();
            List<PatientExaminationHeader> ExamlstHeader = new List<PatientExaminationHeader>();
            List<PatientExamintionDetails> ExamlstDetails = new List<PatientExamintionDetails>();
            List<VitalsView> VitallstResult = new List<VitalsView>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<ClinicMaster> lstClinic = new List<ClinicMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            List<string> lstAllergies = new List<string>();

            string Allergies = "";
            List<PatientHistoryView> lstResult = new List<PatientHistoryView>();
            List<PatHabitView> lstPatHabit = new List<PatHabitView>();
            List<PatMedicalHistoryView> lstMediclHistory = new List<PatMedicalHistoryView>();
            List<PatientMedicineView> lstMedicine = new List<PatientMedicineView>();
            List<PatientSurgicalHistory> lstPatSurgical = new List<PatientSurgicalHistory>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<ReportView> visitreport = HttpContext.Session.GetObjectFromJsonList<ReportView>("VisitConfigDetails");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                string PatientID = lstresult[0].PatientID;
                lstAllergies = _emrRepo.GetPatinetAllergiesByPatient(PatientID);
                if (lstAllergies.Count > 0)
                    Allergies = string.Join(",", lstAllergies.ToArray());
                lstClinic = _clinicRepo.GetClinicDetails(Hospitalid, lstresult[0].ClinicID);
                if (visitreport != null)
                {
                    if (visitreport.Count > 0)
                    {
                        for (int i = 0; i < visitreport.Count; i++)
                        {
                            string Reportname = visitreport[i].Report_Name;
                            bool IsActive = Convert.ToBoolean(visitreport[i].IsActive);
                            if (Reportname == "Vitals")
                            {
                                if (IsActive == true)
                                    VitallstResult = _vitlasRepo.GetReportVitalsbyVisitID(Visitid);
                            }
                            else if (Reportname == "Presenting Illness")
                            {
                                if (IsActive == true)
                                {
                                    ComplstHeader = _symptonRepo.GetReportSymptomHeaderByVist(Visitid);
                                    ComplstDeatils = _symptonRepo.GetReportSymptomDeatilByVist(Visitid);
                                }
                            }
                            else if (Reportname == "Past History")
                            {
                                if (IsActive == true)
                                {
                                    lstResult = _emrRepo.GetNewReportPatHistoryByPatVisitID(PatientID, Visitid);
                                    if (lstResult.Count > 0)
                                    {
                                        long historyID = lstResult[0].HistorySeqID;
                                        lstPatHabit = _emrRepo.GetPatinetHabitsByID(historyID);
                                        lstMediclHistory = _emrRepo.GetPatMedicalHistoryByID(historyID);//comor
                                        lstMedicine = _emrRepo.GetPatCurrentMedicineById(historyID);
                                        lstPatSurgical = _emrRepo.GetPatSurgicalHistoryByHistoryID(historyID);
                                    }
                                }
                            }
                            else if (Reportname == "Examination")
                            {
                                if (IsActive == true)
                                {
                                    ExamlstHeader = _emrRepo.GetExamHeaderByVisitid(Visitid);
                                    ExamlstDetails = _emrRepo.GetExamDetailsByVisitid(Visitid);
                                }
                            }
                            else if (Reportname == "Diagnosis and Treatment Plan")
                            {
                                if (IsActive == true)
                                {
                                    DiaglstHeader = _diagnosisRepo.GetDiagnosisByVisit(Visitid);
                                    DiaglstDeatils = _diagnosisRepo.GetDiagDetailsByVisit(Visitid);
                                    TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                                    TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                                }
                            }
                            else if (Reportname == "Investigation")
                            {
                                if (IsActive == true)
                                {
                                    InvestlstHeader = _investrepo.GetInvestigationByVisitid(Visitid);
                                    InvestlstDeatils = _investrepo.GetInvestDetailsByVisitid(Visitid, IsHISPatient);
                                }
                            }
                            //else if (Reportname == "Treatment Plan")
                            //{
                            //    if (IsActive == true)
                            //    {
                            //        TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                            //        TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                            //    }
                            //}
                        }
                    }
                }
                else
                {
                    DataTable GetConfig = _settingRepo.GetAllReportConfig();
                    if (GetConfig.Rows.Count > 0)
                    {
                        for (int i = 0; i < GetConfig.Rows.Count; i++)
                        {
                            string Reportname = GetConfig.Rows[i]["Report_Name"].ToString();
                            bool IsActive = Convert.ToBoolean(GetConfig.Rows[i]["IsActive"]);
                            if (Reportname == "Vitals")
                            {
                                if (IsActive == true)
                                    VitallstResult = _vitlasRepo.GetReportVitalsbyVisitID(Visitid);
                            }
                            else if (Reportname == "Presenting Illness")
                            {
                                if (IsActive == true)
                                {
                                    ComplstHeader = _symptonRepo.GetReportSymptomHeaderByVist(Visitid);
                                    ComplstDeatils = _symptonRepo.GetReportSymptomDeatilByVist(Visitid);
                                }
                            }
                            else if (Reportname == "Past History")
                            {
                                if (IsActive == true)
                                {
                                    lstResult = _emrRepo.GetNewReportPatHistoryByPatVisitID(PatientID, Visitid);
                                    if (lstResult.Count > 0)
                                    {
                                        long historyID = lstResult[0].HistorySeqID;
                                        lstPatHabit = _emrRepo.GetPatinetHabitsByID(historyID);
                                        lstMediclHistory = _emrRepo.GetPatMedicalHistoryByID(historyID);//comor
                                        lstMedicine = _emrRepo.GetPatCurrentMedicineById(historyID);
                                        lstPatSurgical = _emrRepo.GetPatSurgicalHistoryByHistoryID(historyID);
                                    }
                                }
                            }
                            else if (Reportname == "Examination")
                            {
                                if (IsActive == true)
                                {
                                    ExamlstHeader = _emrRepo.GetExamHeaderByVisitid(Visitid);
                                    ExamlstDetails = _emrRepo.GetExamDetailsByVisitid(Visitid);
                                }
                            }
                            else if (Reportname == "Diagnosis and Treatment Plan")
                            {
                                if (IsActive == true)
                                {
                                    DiaglstHeader = _diagnosisRepo.GetDiagnosisByVisit(Visitid);
                                    DiaglstDeatils = _diagnosisRepo.GetDiagDetailsByVisit(Visitid);
                                    TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                                    TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                                }
                            }
                            else if (Reportname == "Investigation")
                            {
                                if (IsActive == true)
                                {
                                    InvestlstHeader = _investrepo.GetInvestigationByVisitid(Visitid);
                                    InvestlstDeatils = _investrepo.GetInvestDetailsByVisitid(Visitid, IsHISPatient);
                                }
                            }
                            //else if (Reportname == "Treatment Plan")
                            //{
                            //    if (IsActive == true)
                            //    {
                            //        TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                            //        TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                CompHeader = ComplstHeader,
                CompDetails = ComplstDeatils,
                TreatHeader = TreatlstHeader,
                TreatDtl = TreatlstDeatils,
                DiagHeader = DiaglstHeader,
                DiagDtl = DiaglstDeatils,
                InvestHeader = InvestlstHeader,
                InvestDtl = InvestlstDeatils,
                ExamHeader = ExamlstHeader,
                ExamDtl = ExamlstDetails,
                Vitals = VitallstResult,
                HospitalDtl = lstHospital,
                PatientDtl = lstresult,
                Allergy = Allergies,
                ClinicDtl = lstClinic,
                PatHistroy = lstResult,
                Pathabits = lstPatHabit,
                PatMedicalHstd = lstMediclHistory,
                PatMedicine = lstMedicine,
                PatSurgical = lstPatSurgical
            });
        }
        #endregion
        #region Raffi for All Report
        [HttpPost]
        public JsonResult SetVisitConfigDetails([FromBody] ReportView[] ReportArray)
        {
            string Result = "SaveResult";
            if (ReportArray.Length > 0)
            {
                HttpContext.Session.SetObjectAsJsonLsit("VisitConfigDetails", ReportArray);
            }
            return Json(Result);
        }
        [HttpPost]
        public JsonResult SetAllVisitConfigDetails([FromBody] ReportView[] ReportArray)
        {
            string Result = "SaveResult";
            if (ReportArray.Length > 0)
            {
                HttpContext.Session.SetObjectAsJsonLsit("AllVisitConfigDetails", ReportArray);
            }
            return Json(Result);
        }
        [HttpGet]
        public JsonResult GetAllReport(string Patid)
        {
            List<PatientRegistration> lstHeader = new List<PatientRegistration>();
            try
            {
                lstHeader = _emrRepo.GetAllVisitByPatid(Patid);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                Header = lstHeader,
            });
        }
        [HttpPost]
        public JsonResult GetReportByVisitid([FromBody] string VisitArray)
        {
            string Visitid = VisitArray;
            List<PatientSympotmHeader> ComplstHeader = new List<PatientSympotmHeader>();
            List<PatientSympotmDetails> ComplstDeatils = new List<PatientSympotmDetails>();
            List<PatientTreatmentHeader> TreatlstHeader = new List<PatientTreatmentHeader>();
            List<PatientDrugDetails> TreatlstDeatils = new List<PatientDrugDetails>();
            List<PatientDiagnosisHeader> DiaglstHeader = new List<PatientDiagnosisHeader>();
            List<PatientDiagnosisDetail> DiaglstDeatils = new List<PatientDiagnosisDetail>();
            List<PatientInvestHeader> InvestlstHeader = new List<PatientInvestHeader>();
            List<PatientInvestDetails> InvestlstDeatils = new List<PatientInvestDetails>();
            List<PatientExaminationHeader> ExamlstHeader = new List<PatientExaminationHeader>();
            List<PatientExamintionDetails> ExamlstDetails = new List<PatientExamintionDetails>();
            List<VitalsView> VitallstResult = new List<VitalsView>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            List<ClinicMaster> lstClinic = new List<ClinicMaster>();
            List<string> lstAllergies = new List<string>();
            string Allergies = "";
            List<PatientHistoryView> lstResult = new List<PatientHistoryView>();
            List<PatHabitView> lstPatHabit = new List<PatHabitView>();
            List<PatMedicalHistoryView> lstMediclHistory = new List<PatMedicalHistoryView>();
            List<PatientMedicineView> lstMedicine = new List<PatientMedicineView>();
            List<PatientSurgicalHistory> lstPatSurgical = new List<PatientSurgicalHistory>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                List<ReportView> visitreport = HttpContext.Session.GetObjectFromJsonList<ReportView>("AllVisitConfigDetails");
                string PatientID = lstresult[0].PatientID;
                lstAllergies = _emrRepo.GetPatinetAllergiesByPatient(PatientID);
                if (lstAllergies.Count > 0)
                    Allergies = string.Join(",", lstAllergies.ToArray());
                lstClinic = _clinicRepo.GetClinicDetails(Hospitalid, lstresult[0].ClinicID);
                if (visitreport != null)
                {
                    if (visitreport.Count > 0)
                    {
                        for (int i = 0; i < visitreport.Count; i++)
                        {
                            string Reportname = visitreport[i].Report_Name;
                            bool IsActive = Convert.ToBoolean(visitreport[i].AllReport_IsActive);
                            if (Reportname == "Vitals")
                            {
                                if (IsActive == true)
                                    VitallstResult = _vitlasRepo.GetReportVitalsbyVisitID(Visitid);
                            }
                            else if (Reportname == "Presenting Illness")
                            {
                                if (IsActive == true)
                                {
                                    ComplstHeader = _symptonRepo.GetReportSymptomHeaderByVist(Visitid);
                                    ComplstDeatils = _symptonRepo.GetReportSymptomDeatilByVist(Visitid);
                                }
                            }
                            else if (Reportname == "Past History")
                            {
                                if (IsActive == true)
                                {
                                    lstResult = _emrRepo.GetNewReportPatHistoryByPatVisitID(PatientID, Visitid);
                                    if (lstResult.Count > 0)
                                    {
                                        long historyID = lstResult[0].HistorySeqID;
                                        lstPatHabit = _emrRepo.GetPatinetHabitsByID(historyID);
                                        lstMediclHistory = _emrRepo.GetPatMedicalHistoryByID(historyID);//comor
                                        lstMedicine = _emrRepo.GetPatCurrentMedicineById(historyID);
                                        lstPatSurgical = _emrRepo.GetPatSurgicalHistoryByHistoryID(historyID);
                                    }
                                }
                            }
                            else if (Reportname == "Examination")
                            {
                                if (IsActive == true)
                                {
                                    ExamlstHeader = _emrRepo.GetExamHeaderByVisitid(Visitid);
                                    ExamlstDetails = _emrRepo.GetExamDetailsByVisitid(Visitid);
                                }
                            }
                            else if (Reportname == "Diagnosis & Treatment Plan")
                            {
                                if (IsActive == true)
                                {
                                    DiaglstHeader = _diagnosisRepo.GetDiagnosisByVisit(Visitid);
                                    DiaglstDeatils = _diagnosisRepo.GetDiagDetailsByVisit(Visitid);
                                    TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                                    TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                                }
                            }
                            else if (Reportname == "Investigation")
                            {
                                if (IsActive == true)
                                {
                                    InvestlstHeader = _investrepo.GetInvestigationByVisitid(Visitid);
                                    InvestlstDeatils = _investrepo.GetInvestDetailsByVisitid(Visitid, IsHISPatient);
                                }
                            }
                            //else if (Reportname == "Treatment Plan")
                            //{
                            //    if (IsActive == true)
                            //    {
                            //        TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                            //        TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                            //    }
                            //}
                        }
                    }
                }
                else
                {
                    DataTable GetConfig = _settingRepo.GetAllReportConfig();

                    if (GetConfig.Rows.Count > 0)
                    {
                        for (int i = 0; i < GetConfig.Rows.Count; i++)
                        {
                            string Reportname = GetConfig.Rows[i]["Report_Name"].ToString();
                            bool IsActive = Convert.ToBoolean(GetConfig.Rows[i]["AllReport_IsActive"]);
                            if (Reportname == "Vitals")
                            {
                                if (IsActive == true)
                                    VitallstResult = _vitlasRepo.GetReportVitalsbyVisitID(Visitid);
                            }
                            else if (Reportname == "Presenting Illness")
                            {
                                if (IsActive == true)
                                {
                                    ComplstHeader = _symptonRepo.GetReportSymptomHeaderByVist(Visitid);
                                    ComplstDeatils = _symptonRepo.GetReportSymptomDeatilByVist(Visitid);
                                }
                            }
                            else if (Reportname == "Past History")
                            {
                                if (IsActive == true)
                                {
                                    lstResult = _emrRepo.GetNewReportPatHistoryByPatVisitID(PatientID, Visitid);
                                    if (lstResult.Count > 0)
                                    {
                                        long historyID = lstResult[0].HistorySeqID;
                                        lstPatHabit = _emrRepo.GetPatinetHabitsByID(historyID);
                                        lstMediclHistory = _emrRepo.GetPatMedicalHistoryByID(historyID);//comor
                                        lstMedicine = _emrRepo.GetPatCurrentMedicineById(historyID);
                                        lstPatSurgical = _emrRepo.GetPatSurgicalHistoryByHistoryID(historyID);
                                    }
                                }
                            }
                            else if (Reportname == "Examination")
                            {
                                if (IsActive == true)
                                {
                                    ExamlstHeader = _emrRepo.GetExamHeaderByVisitid(Visitid);
                                    ExamlstDetails = _emrRepo.GetExamDetailsByVisitid(Visitid);
                                }
                            }
                            else if (Reportname == "Diagnosis & Treatment Plan")
                            {
                                if (IsActive == true)
                                {
                                    DiaglstHeader = _diagnosisRepo.GetDiagnosisByVisit(Visitid);
                                    DiaglstDeatils = _diagnosisRepo.GetDiagDetailsByVisit(Visitid);
                                    TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                                    TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                                }
                            }
                            else if (Reportname == "Investigation")
                            {
                                if (IsActive == true)
                                {
                                    InvestlstHeader = _investrepo.GetInvestigationByVisitid(Visitid);
                                    InvestlstDeatils = _investrepo.GetInvestDetailsByVisitid(Visitid, IsHISPatient);
                                }
                            }
                            //else if (Reportname == "Treatment Plan")
                            //{
                            //    if (IsActive == true)
                            //    {
                            //        TreatlstHeader = _emrRepo.GetTreatmentByVisit(Visitid);
                            //        TreatlstDeatils = _emrRepo.GetTreatmentDetailsByVisit(Visitid);
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new
            {
                CompHeader = ComplstHeader,
                CompDetails = ComplstDeatils,
                TreatHeader = TreatlstHeader,
                TreatDtl = TreatlstDeatils,
                DiagHeader = DiaglstHeader,
                DiagDtl = DiaglstDeatils,
                InvestHeader = InvestlstHeader,
                InvestDtl = InvestlstDeatils,
                ExamHeader = ExamlstHeader,
                ExamDtl = ExamlstDetails,
                Vitals = VitallstResult,
                HospitalDtl = lstHospital,
                PatientDtl = lstresult,
                Allergy = Allergies,
                ClinicDtl = lstClinic,
                PatHistroy = lstResult,
                Pathabits = lstPatHabit,
                PatMedicalHstd = lstMediclHistory,
                PatMedicine = lstMedicine,
                PatSurgical = lstPatSurgical
            });
        }
        #endregion
        private void CreatePatHaibt(PatHabitView[] patHabit, long HistorySeqId, string PatId, string VisitID)
        {
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                if (patHabit.Length > 0)
                {
                    PatHealthHabitHeader habitHeader = new PatHealthHabitHeader
                    {
                        HistorySeqID = HistorySeqId,
                        PatientId = PatId,
                        VisitId = VisitID,
                        CreatedUser = HttpContext.Session.GetString("Userseqid"),
                        CreateDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                        ModifiedUser = HttpContext.Session.GetString("Userseqid")
                    };
                    long HeaderID = _emrRepo.CreateNewPatHabitHeader(habitHeader);
                    int DeleteHabit = _emrRepo.DeletePatHbitByID(HeaderID);
                    for (int count = 0; count < patHabit.Length; count++)
                    {
                        PatHabitDetails patHabitDetails = new PatHabitDetails
                        {
                            PatHabitHdrSeqID = HeaderID,
                            HabitName = patHabit[count].HabitName,
                            HabitSeqID = patHabit[count].HabitSeqID,
                            Comments = patHabit[count].Comments
                        };
                        _emrRepo.CreateNewPatHabitDetails(patHabitDetails);
                    }
                }
            }
            catch
            {

            }
        }
        private void CreatePatAllergies(string[] AllergisArray, long HistorySeqId, string PatId, string VisitID)
        {
            try
            {
                int DeleteAllergy = _emrRepo.DeleteAllergiesByID(HistorySeqId);
                if (AllergisArray.Length > 0)
                {

                    for (int count = 0; count < AllergisArray.Length; count++)
                    {
                        PatientAllergies allergies = new PatientAllergies
                        {
                            PatientID = PatId,
                            VisitID = VisitID,
                            HistoryID = HistorySeqId,
                            Allergy = AllergisArray[count],
                            CreateUser = HttpContext.Session.GetString("Userseqid"),
                        };
                        int Result = _emrRepo.CreatePatientAllergy(allergies);
                    }
                }
            }
            catch
            {

            }
        }
        private void CreatePatMedicalHistory(PatMedicalHistoryView[] patMedical, long HistorySeqId, string PatId, string VisitID)
        {
            int rowaffected = 0;
            try
            {
                if (patMedical.Length > 0)
                {
                    rowaffected = _emrRepo.DeletePatMedicalHistoryByID(HistorySeqId);
                    for (int count = 0; count < patMedical.Length; count++)
                    {
                        PatientMedicalHistory medicalHistory = new PatientMedicalHistory
                        {
                            PatinetID = PatId,
                            VisitID = VisitID,
                            HistoryID = HistorySeqId,
                            MedicalHistory = patMedical[count].MedicalHistory,
                            Duration = patMedical[count].Duration,
                            Remark = patMedical[count].Remark,
                            CreateUser = HttpContext.Session.GetString("Userseqid")
                        };
                        rowaffected = _emrRepo.CreateNewPatMedicalHistory(medicalHistory);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void CreatePatMedicineHistory(PatientMedicineView[] patMedicine, long HistorySeqId, string PatId, string VisitID)
        {
            int rowaffected = 0;
            try
            {
                if (patMedicine.Length > 0)
                {
                    rowaffected = _emrRepo.DeletePatMedicine(HistorySeqId);
                    for (int count = 0; count < patMedicine.Length; count++)
                    {
                        PatCurrentMedicine patCurrent = new PatCurrentMedicine
                        {
                            PatientID = PatId,
                            VisitID = VisitID,
                            HistoryID = HistorySeqId,
                            Medicine = patMedicine[count].Medicine,
                            Duration = patMedicine[count].Duration,
                            Remark = patMedicine[count].Remark,
                            CreateUser = HttpContext.Session.GetString("Userseqid")
                        };
                        rowaffected = _emrRepo.CreateNewPatMedicine(patCurrent);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void CreatePatSurgicalHistory(PatientSurgicalHistory[] patSurgical, long HistorySeqId, string PatId, string VisitID)
        {
            int rowaffected = 0;
            try
            {
                if (patSurgical.Length > 0)
                {
                    rowaffected = _emrRepo.DeletePatSurgicalHistory(HistorySeqId);
                    for (int count = 0; count < patSurgical.Length; count++)
                    {
                        PatientSurgicalHistory patCurrent = new PatientSurgicalHistory
                        {
                            PatientID = PatId,
                            VisitID = VisitID,
                            HistoryID = HistorySeqId,
                            SurgicalHistory = patSurgical[count].SurgicalHistory,
                            Comments = patSurgical[count].Comments,
                            CreateUser = HttpContext.Session.GetString("Userseqid")
                        };
                        rowaffected = _emrRepo.CreateNewPatSurgicalHistory(patCurrent);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void UpdatePatTreatmentHeader(PatTreatmentView patTreatment, long SeqID)
        {
            int rowaffected = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                PatientTreatmentHeader treatmentHeader = new PatientTreatmentHeader
                {
                    SeqID = SeqID,
                    PatientID = patTreatment.PatientID,
                    VisitID = patTreatment.VisitID,
                    Notes = patTreatment.Notes,
                    Diet = patTreatment.Diet,
                    GeneralRecommendation = patTreatment.GeneralRecommendation,
                    ProcedureName = patTreatment.ProcedureName,
                    PrecedureNotes = patTreatment.PrecedureNotes,
                    Instruction = patTreatment.Instruction,
                    InstructionNotes = patTreatment.InstructionNotes,
                    CreateUser = HttpContext.Session.GetString("Userseqid"),
                    ModifieDate = timezoneUtility.Gettimezone(Timezoneid),
                    ModifieUser = HttpContext.Session.GetString("Userseqid"),
                    IsActive = patTreatment.IsActive
                };
                if (!string.IsNullOrWhiteSpace(patTreatment.ProcedureDate))
                    treatmentHeader.ProcedureDate = Convert.ToDateTime(patTreatment.ProcedureDate);
                rowaffected = _emrRepo.UpdatePatientTreatmentHeader(treatmentHeader);
                if (patTreatment.DrugPickList.Length > 0)
                {
                    rowaffected = _emrRepo.DeletePatDrugDetailsByID(SeqID);
                    for (int count = 0; count < patTreatment.DrugPickList.Length; count++)
                    {
                        string[] cat = patTreatment.DrugPickList[count].DrugName.Split('.');
                        PatientDrugDetails patientDrug = new PatientDrugDetails
                        {
                            PatDrugHdtID = SeqID,
                            DrugCode = patTreatment.DrugPickList[count].DrugCode,
                            DrugCat = cat[0],
                            DrugName = cat[1],
                            Frequency = patTreatment.DrugPickList[count].Frequency,
                            Duration = patTreatment.DrugPickList[count].Duration,
                            Remarks = patTreatment.DrugPickList[count].Remarks
                        };
                        rowaffected = _emrRepo.CreateNewPatDrugDtl(patientDrug);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdatePatExamintion(PatientExamintionView patientExamintion, long SeqID)
        {
            int rowaffected = 0;
            try
            {
                TimezoneUtility timezoneUtility = new TimezoneUtility();
                string Timezoneid = HttpContext.Session.GetString("TimezoneID");
                if (Timezoneid == "" || Timezoneid == null)
                    Timezoneid = "India Standard Time";
                PatientExaminationHeader examinationHeader = new PatientExaminationHeader
                {
                    Comments = patientExamintion.Comments,
                    SpecifiComments = patientExamintion.SpecifiComments,
                    IsActive = patientExamintion.IsActive,
                    ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                    ModifiedUser = HttpContext.Session.GetString("Userseqid"),
                    SeqID = SeqID
                };
                if (SeqID > 0)
                    rowaffected = _emrRepo.UpdatePatientExamintionHeader(examinationHeader);
                if (patientExamintion.GetExamination.Length > 0)
                {
                    rowaffected = _emrRepo.DeletePatExamintionDetails(SeqID);
                    for (int count = 0; count < patientExamintion.GetExamination.Length; count++)
                    {
                        PatientExamintionDetails examintionDetails = new PatientExamintionDetails
                        {
                            PatExamSeqID = SeqID,
                            ExamintionName = patientExamintion.GetExamination[count].ExaminationName,
                            Option1 = patientExamintion.GetExamination[count].Option1,
                            Option2 = patientExamintion.GetExamination[count].Option2,
                            Option3 = patientExamintion.GetExamination[count].Option3,
                            Option4 = patientExamintion.GetExamination[count].Option4,
                            Option5 = patientExamintion.GetExamination[count].Option5,
                            Option6 = patientExamintion.GetExamination[count].Option6,
                            Option7 = patientExamintion.GetExamination[count].Option7,
                            Option8 = patientExamintion.GetExamination[count].Option8,
                            Option9 = patientExamintion.GetExamination[count].Option9,
                            Option10 = patientExamintion.GetExamination[count].Option10,
                            Option11 = patientExamintion.GetExamination[count].Option11,
                            Option12 = patientExamintion.GetExamination[count].Option12,
                            OptChk1 = patientExamintion.GetExamination[count].OptChk1,
                            OptChk2 = patientExamintion.GetExamination[count].OptChk2,
                            OptChk3 = patientExamintion.GetExamination[count].OptChk3,
                            OptChk4 = patientExamintion.GetExamination[count].OptChk4,
                            OptChk5 = patientExamintion.GetExamination[count].OptChk5,
                            OptChk6 = patientExamintion.GetExamination[count].OptChk6,
                            OptChk7 = patientExamintion.GetExamination[count].OptChk7,
                            OptChk8 = patientExamintion.GetExamination[count].OptChk8,
                            OptChk9 = patientExamintion.GetExamination[count].OptChk9,
                            OptChk10 = patientExamintion.GetExamination[count].OptChk10,
                            OptChk11 = patientExamintion.GetExamination[count].OptChk11,
                            OptChk12 = patientExamintion.GetExamination[count].OptChk12,
                            Notes = patientExamintion.GetExamination[count].Notes
                        };
                        rowaffected = _emrRepo.CreateNewPatExaminationDtl(examintionDetails);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        [HttpGet]
        public JsonResult GetTreatmentByID(long SeqID)
        {
            List<PatientTreatmentHeader> lstHeader = new List<PatientTreatmentHeader>();
            List<DrugPickListView> lstDeatils = new List<DrugPickListView>();
            List<HospitalMaster> lstHospital = new List<HospitalMaster>();
            List<MyPatient> lstresult = new List<MyPatient>();
            List<TreatmentDiagnostic> lstdDiagnostics = new List<TreatmentDiagnostic>();
            List<ClinicMaster> lstClinic = new List<ClinicMaster>();
            List<PatientAllergies> lstAllery = new List<PatientAllergies>();
            try
            {
                string IsHISPatient = HttpContext.Session.GetString("IsHISPatient");
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                lstHeader = _emrRepo.GetTreatmentByID(SeqID);
                lstDeatils = _emrRepo.GetPatDrugDetailsByID(SeqID);
                lstdDiagnostics = _diagnosisRepo.GetTreatmentDignostic(SeqID);
                lstHospital = _hospitalRepo.GetHospitalDetails(Hospitalid);
                lstresult = HttpContext.Session.GetObjectFromJsonList<MyPatient>("Patientlist");
                //lstAllery = _emrRepo.GetAllergies(Visitid);
                lstClinic = _clinicRepo.GetClinicDetails(Hospitalid, lstresult[0].ClinicID);
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { Header = lstHeader, Details = lstDeatils, HospitalDtl = lstHospital, PatientDtl = lstresult, PatDiang = lstdDiagnostics, ClinicDtl = lstClinic });
        }
        [HttpGet]
        public JsonResult GetDosageByText(string FreeText)
        {
            List<string> lstResult = new List<string>();
            try
            {
                lstResult = _emrRepo.SelectFrequecyByText(FreeText);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult FrequencyChange(string Frequency)
        {
            List<string> lstResult = new List<string>();
            try
            {
                int Result = _emrRepo.AddNewFrequecy(Frequency);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult GetRemarksByText(string FreeText)
        {
            List<string> lstResult = new List<string>();
            try
            {
                lstResult = _emrRepo.SelctRemarksByText(FreeText);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult RemarksChange(string Remarks)
        {
            List<string> lstResult = new List<string>();
            try
            {
                int Result = _emrRepo.AddNewRemarks(Remarks);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult GetDurationByText(string FreeText)
        {
            List<string> lstResult = new List<string>();
            try
            {
                lstResult = _emrRepo.SelectDurationByText(FreeText);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult DurationChange(string Duration)
        {
            List<string> lstResult = new List<string>();
            try
            {
                int Result = _emrRepo.AddNewDrugDuration(Duration);
            }
            catch (Exception)
            {
            }
            return Json(lstResult);
        }
        [HttpGet]
        public JsonResult CreateNewSurgicalHistory(string SurgicalName)
        {
            if (!string.IsNullOrWhiteSpace(SurgicalName))
            {
                List<Login> logins = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                long Hospitalid = logins[0].HospitalID;
                SurgicalHistoryInfo surgicalHistoryInfo = new SurgicalHistoryInfo
                {
                    SurgicalHistory = SurgicalName,
                    HospitalID = Hospitalid,
                    CreateUser = HttpContext.Session.GetString("Userseqid"),

                };
                int Value = _emrRepo.CreateNewSurgicalHistory(surgicalHistoryInfo);
                if (Value > 0)
                {
                    string EventName = "Add New Surgical History " + SurgicalName;
                    CreateEventManagemnt(EventName);
                }
            }
            return Json(SurgicalName);
        }
        [HttpGet]
        public JsonResult UpdateExamaintionNotes(string Examination, string Notes)
        {
            string Result = "Save Success";
            long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            int rowAffected = _emrRepo.UpdateExamNotes(Examination, Notes, HospitalId);
            rowAffected = _emrRepo.UpdatePickLstExamNotes(Examination, Notes);
            return Json(Result);
        }
        [HttpPost]
        public JsonResult CreateNewDrug([FromBody] CommonDrugMaster drugMaster)
        {
            TimezoneUtility timezoneUtility = new TimezoneUtility();
            string Timezoneid = HttpContext.Session.GetString("TimezoneID");
            if (Timezoneid == "" || Timezoneid == null)
                Timezoneid = "India Standard Time";
            DrugMasterInfo drugMasterInfo = new DrugMasterInfo
            {
                DrugName = drugMaster.DrugName,
                Category = drugMaster.Category,
                Uom = drugMaster.Uom,
                CreateUser = HttpContext.Session.GetString("Userseqid").ToString(),
                ModifieDatetime = timezoneUtility.Gettimezone(Timezoneid),
                ModifedUser = HttpContext.Session.GetString("Userseqid").ToString(),
                IsActive = true,
                HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"))
            };
            long rowaffceted = _drugRepo.CreateNewDrug(drugMasterInfo);
            string EventName = "Add New Drugs " + drugMaster.DrugName;
            CreateEventManagemnt(EventName);
            return Json(drugMaster);
        }
        [HttpGet]
        public JsonResult SendMediview(string PatientID)
        {
            string Result = "";
            DataTable dtResult = new DataTable();
            dtResult = _patientRepo.GetPatientByID(PatientID);
            PatientView patientView = new PatientView();
            if (dtResult.Rows.Count > 0)
            {
                for (int count = 0; count < dtResult.Rows.Count; count++)
                {
                    patientView.PatSeqID = Convert.ToInt64(dtResult.Rows[count]["PatSeqID"]);
                    patientView.PatientID = Convert.ToString(dtResult.Rows[count]["PatientID"]);
                    patientView.FirstName = Convert.ToString(dtResult.Rows[count]["FirstName"]);
                    patientView.SecondName = Convert.ToString(dtResult.Rows[count]["SecondName"]);
                    patientView.Gender = Convert.ToString(dtResult.Rows[count]["Gender"]);
                    patientView.BirthDate = Convert.ToString(dtResult.Rows[count]["BirthDate"]);
                    patientView.PhoneNumber = Convert.ToString(dtResult.Rows[count]["PhoneNumber"]);
                    patientView.MobileNumber = Convert.ToInt64(dtResult.Rows[count]["MobileNumber"]);
                    patientView.Email = Convert.ToString(dtResult.Rows[count]["Email"]);
                    patientView.RelationName = Convert.ToString(dtResult.Rows[count]["RelationName"]);
                    patientView.RelationType = Convert.ToString(dtResult.Rows[count]["RelationType"]);
                    patientView.PatientAddress1 = Convert.ToString(dtResult.Rows[count]["PatientAddress1"]);
                    patientView.PatientAddress2 = Convert.ToString(dtResult.Rows[count]["PatientAddress2"]);
                    patientView.City = Convert.ToInt32(dtResult.Rows[count]["City"]);
                    patientView.State = Convert.ToInt32(dtResult.Rows[count]["State"]);
                    patientView.Country = Convert.ToInt32(dtResult.Rows[count]["Country"]);
                    patientView.DeptID = Convert.ToInt32(dtResult.Rows[count]["DeptID"]);
                    patientView.DoctorID = Convert.ToInt32(dtResult.Rows[count]["DoctorID"]);
                    patientView.ClinicID = Convert.ToInt64(dtResult.Rows[count]["ClinicID"]);
                    patientView.CityCode = dtResult.Rows[count]["CityCode"].ToString();
                    patientView.CountryCode = dtResult.Rows[count]["CountryCode"].ToString();
                    if (!string.IsNullOrWhiteSpace(patientView.BirthDate))
                    {
                        DateTime dob = Convert.ToDateTime(patientView.BirthDate);
                        Age age = new Age();
                        age = CommonSetting.GetAge(dob, DateTime.Today);
                        patientView.AgeYear = age.years;
                        patientView.AgeMonth = age.months;
                        patientView.AgeDay = age.days;
                    }
                    else
                    {
                        patientView.AgeYear = Convert.ToInt32(dtResult.Rows[count]["AgeYear"]);
                        patientView.AgeMonth = Convert.ToInt32(dtResult.Rows[count]["AgeMonth"]);
                        patientView.AgeDay = Convert.ToInt32(dtResult.Rows[count]["AgeDay"]);
                    }
                }
                if (!_mediviewRepo.IsPatientExists(patientView.PatientID))
                {
                    if (!string.IsNullOrWhiteSpace(patientView.BirthDate))
                    {
                        DateTime dob = Convert.ToDateTime(patientView.BirthDate);
                    }
                    MediviewPatientInfo mediviewPatientInfo = new MediviewPatientInfo();
                    mediviewPatientInfo.PatientID = patientView.PatientID;
                    mediviewPatientInfo.PatientIDIssuer = "";
                    mediviewPatientInfo.Name = patientView.FirstName + " " + patientView.SecondName;
                    mediviewPatientInfo.Sex = patientView.Gender;
                    if (!string.IsNullOrWhiteSpace(patientView.BirthDate))
                    {
                        DateTime dob = Convert.ToDateTime(patientView.BirthDate);
                        mediviewPatientInfo.BirthDate = dob;
                    }
                    mediviewPatientInfo.MotherName = "";
                    mediviewPatientInfo.OtherPatientID = "";
                    mediviewPatientInfo.IsEmergency = false;
                    mediviewPatientInfo.RegisteredDateTime = DateTime.Now;
                    mediviewPatientInfo.PatientAddress1 = patientView.PatientAddress1;
                    mediviewPatientInfo.PatientAddress2 = patientView.PatientAddress2;
                    mediviewPatientInfo.City = "";
                    mediviewPatientInfo.Country = "";
                    mediviewPatientInfo.EthnicName = "";
                    mediviewPatientInfo.PatientPhoneNumber = patientView.PhoneNumber;
                    mediviewPatientInfo.PatientComment = "";
                    mediviewPatientInfo.Custom1 = "";
                    mediviewPatientInfo.Custom2 = "";
                    mediviewPatientInfo.ZipCode = "";
                    mediviewPatientInfo.IsActive = true;
                    mediviewPatientInfo.Email = patientView.Email;
                    int patSeqID = _mediviewRepo.CreateNewPatReg(mediviewPatientInfo);
                    if (patSeqID > 0)
                    {
                        MediViewStudyInfo mediViewStudyInfo = new MediViewStudyInfo();
                        mediViewStudyInfo.PatSeqID = patSeqID;
                        mediViewStudyInfo.StudyDescription = "";
                        mediViewStudyInfo.StudyDateTime = DateTime.Now;
                        mediViewStudyInfo.AccessionNumber = "";
                        mediViewStudyInfo.PatientAge = patientView.AgeYear + "Y" + patientView.AgeMonth + "M" + patientView.AgeDay + "D";
                        mediViewStudyInfo.PatientSize = "";
                        mediViewStudyInfo.PatientWeight = "";
                        mediViewStudyInfo.ReferringPhysician = "";
                        mediViewStudyInfo.AdmittingDiagnosesDescription = "";
                        mediViewStudyInfo.PatientHistory = "";
                        mediViewStudyInfo.IsEmergency = false;
                        mediViewStudyInfo.IsPreRegistered = false;
                        mediViewStudyInfo.IsLocked = false;
                        mediViewStudyInfo.IsClosed = false;
                        mediViewStudyInfo.ProcedureName = "";
                        mediViewStudyInfo.Custom1 = "";
                        mediViewStudyInfo.Custom2 = "";
                        mediViewStudyInfo.IsActive = true;
                        int StudySeqID = _mediviewRepo.CreateNewStudy(mediViewStudyInfo);
                        if (StudySeqID > 0)
                        {
                            MediViewSeriesInfo mediViewSeriesInfo = new MediViewSeriesInfo();
                            mediViewSeriesInfo.StudySeqID = StudySeqID;
                            mediViewSeriesInfo.SeriesInstanceUID = mediViewStudyInfo.StudyInstanceUID;
                            mediViewSeriesInfo.SeriesNumber = "";
                            mediViewSeriesInfo.Modality = "OT";
                            mediViewSeriesInfo.SeriesDateTime = DateTime.Now;
                            mediViewSeriesInfo.SeriesDescription = "";
                            mediViewSeriesInfo.PerformingPhysician = "";
                            mediViewSeriesInfo.ProtocolName = "";
                            mediViewSeriesInfo.BodyPartExamined = "";
                            mediViewSeriesInfo.PatientPosition = "";
                            mediViewSeriesInfo.OperatorName = "";
                            mediViewSeriesInfo.InstrumentName = "";
                            mediViewSeriesInfo.Custom1 = "";
                            mediViewSeriesInfo.Custom2 = "";
                            mediViewSeriesInfo.IsActive = true;
                            int rowaffected = _mediviewRepo.CreateNewSeries(mediViewSeriesInfo);
                            Result = "Save success";
                        }
                    }
                }
                else
                {
                    int patseqID = _mediviewRepo.GetPatinetSeqIDByID(patientView.PatientID);
                    if (patseqID > 0)
                    {
                        MediViewStudyInfo mediViewStudyInfo = new MediViewStudyInfo();
                        mediViewStudyInfo.PatSeqID = patseqID;
                        mediViewStudyInfo.StudyDescription = "";
                        mediViewStudyInfo.StudyDateTime = DateTime.Now;
                        mediViewStudyInfo.AccessionNumber = "";
                        mediViewStudyInfo.PatientAge = patientView.AgeYear + "Y" + patientView.AgeMonth + "M" + patientView.AgeDay + "D";
                        mediViewStudyInfo.PatientSize = "";
                        mediViewStudyInfo.PatientWeight = "";
                        mediViewStudyInfo.ReferringPhysician = "";
                        mediViewStudyInfo.AdmittingDiagnosesDescription = "";
                        mediViewStudyInfo.PatientHistory = "";
                        mediViewStudyInfo.IsEmergency = false;
                        mediViewStudyInfo.IsPreRegistered = false;
                        mediViewStudyInfo.IsLocked = false;
                        mediViewStudyInfo.IsClosed = false;
                        mediViewStudyInfo.ProcedureName = "";
                        mediViewStudyInfo.Custom1 = "";
                        mediViewStudyInfo.Custom2 = "";
                        mediViewStudyInfo.IsActive = true;
                        int StudySeqID = _mediviewRepo.CreateNewStudy(mediViewStudyInfo);
                        if (StudySeqID > 0)
                        {
                            MediViewSeriesInfo mediViewSeriesInfo = new MediViewSeriesInfo();
                            mediViewSeriesInfo.StudySeqID = StudySeqID;
                            mediViewSeriesInfo.SeriesInstanceUID = mediViewStudyInfo.StudyInstanceUID;
                            mediViewSeriesInfo.SeriesNumber = "";
                            mediViewSeriesInfo.Modality = "OT";
                            mediViewSeriesInfo.SeriesDateTime = DateTime.Now;
                            mediViewSeriesInfo.SeriesDescription = "";
                            mediViewSeriesInfo.PerformingPhysician = "";
                            mediViewSeriesInfo.ProtocolName = "";
                            mediViewSeriesInfo.BodyPartExamined = "";
                            mediViewSeriesInfo.PatientPosition = "";
                            mediViewSeriesInfo.OperatorName = "";
                            mediViewSeriesInfo.InstrumentName = "";
                            mediViewSeriesInfo.Custom1 = "";
                            mediViewSeriesInfo.Custom2 = "";
                            mediViewSeriesInfo.IsActive = true;
                            int rowaffected = _mediviewRepo.CreateNewSeries(mediViewSeriesInfo);
                            Result = "Save success";
                        }
                    }
                }
            }
            return Json(Result);
        }
        [HttpGet]
        public JsonResult GetMediviewReports(string PatientID)
        {
            List<MediviewReport> ReportPath = new List<MediviewReport>();
            DataTable dtResult = new DataTable();
            dtResult = _mediviewRepo.GetReportsByPatientID(PatientID);
            string PortNumber = (_configuration.GetConnectionString("PortNumber")).ToString();
            if (dtResult.Rows.Count > 0)
            {
                for (int count = 0; count < dtResult.Rows.Count; count++)
                {
                    MediviewReport report = new MediviewReport();
                    string ImagePath = dtResult.Rows[count]["ImagePath"].ToString();
                    string[] arr = ImagePath.Split("Data");
                    ImagePath = PortNumber + "Data" + arr[1];
                    report.ImagePath = ImagePath;
                    report.ReportDate = dtResult.Rows[count]["ReportDate"].ToString();
                    ReportPath.Add(report);
                }
            }
            return Json(ReportPath);
        }
        [HttpGet]
        public JsonResult GetConnectedMediview()
        {
            bool IsSuccess = false;
            try
            {
                IsSuccess = _drugRepo.GetConnectedMediview();
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(IsSuccess);
        }
        //Create Event Method
        private void CreateEventManagemnt(string EventName)
        {
            EventManagemntInfo eventManagemntInfo = new EventManagemntInfo();
            eventManagemntInfo.EventName = EventName;
            eventManagemntInfo.UserID = Convert.ToInt64(HttpContext.Session.GetString("Userseqid"));
            eventManagemntInfo.HospitalID = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
            _emrRepo.NewEventCreateion(eventManagemntInfo);
        }
        //Validation for vitals
        [HttpGet]
        public JsonResult CheckVitlasAlreadyExists(string PatientID, string VisitID)
        {
            CheckVerify checkVerify = new CheckVerify();
            try
            {
                //string Status = "";
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                int Result = _emrRepo.IsAlreadyVitalsExists(PatientID, VisitID);
                int NotVerifyResult = _emrRepo.IsAlreadyNotverifyVitals(PatientID, VisitID);

                checkVerify.verified = Result;
                checkVerify.notverified = NotVerifyResult;
                //if (Result > 0)
                //    Status = "Already Exists";
                //else
                //    Status = "Success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(checkVerify);
        }
        //Validation for Presting illness
        [HttpGet]
        public JsonResult IsAlreadyComplaintsExists(string PatientID, string VisitID)
        {
            CheckVerify checkVerify = new CheckVerify();
            try
            {
                //string Status = "";
                int Result = _emrRepo.IsAlreadyComplaintsExists(PatientID, VisitID);
                int NotVerifyResult = _emrRepo.IsAlreadyNotVerifyComplaints(PatientID, VisitID);

                checkVerify.verified = Result;
                checkVerify.notverified = NotVerifyResult;
                //if (Result > 0)
                //    Status = "Already Exists";
                //else
                //    Status = "Success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(checkVerify);
        }
        //Validation for Patient History
        [HttpGet]
        public JsonResult IsAlreadyHistoryExists(string PatientID, string VisitID)
        {
            CheckVerify checkVerify = new CheckVerify();
            try
            {
                //string Status = "";
                int Result = _emrRepo.IsAlreadyHistoryExists(PatientID, VisitID);
                int NotVerifyResult = _emrRepo.IsAlreadyNotVerifyHistory(PatientID, VisitID);
                checkVerify.verified = Result;
                checkVerify.notverified = NotVerifyResult;
                //if (Result > 0)
                //    Status = "Already Exists";
                //else
                //    Status = "Success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(checkVerify);
        }
        //Validation for Patient Examination
        [HttpGet]
        public JsonResult IsAlreadyExaminationExists(string PatientID, string VisitID)
        {
            CheckVerify checkVerify = new CheckVerify();
            try
            {
                //string Status = "";
                int Result = _emrRepo.IsAlreadyExaminationExists(PatientID, VisitID);
                int NotVerifyResult = _emrRepo.IsAlreadyNotVerifyExam(PatientID, VisitID);
                checkVerify.verified = Result;
                checkVerify.notverified = NotVerifyResult;
                //if (Result > 0)
                //    Status = "Already Exists";
                //else
                //    Status = "Success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(checkVerify);
        }
        //Validation for Patient Diagnosis
        [HttpGet]
        public JsonResult IsAlreadyDiagnosisExists(string PatientID, string VisitID)
        {
            string Status = "";
            int Result = _emrRepo.IsAlreadyDiagnosisExists(PatientID, VisitID);
            if (Result > 0)
                Status = "Already Exists";
            else
                Status = "Success";

            return Json(Status);
        }
        //Validation for Patient Treatment
        [HttpGet]
        public JsonResult IsAlreadyTreatmentExists(string PatientID, string VisitID)
        {
            CheckVerify checkVerify = new CheckVerify();
            try
            {
                //string Status = "";
                int Result = _emrRepo.IsAlreadyTreatmentExists(PatientID, VisitID);
                int NotVerifyResult = _emrRepo.IsAlreadyNotVerifyTreatement(PatientID, VisitID);
                checkVerify.verified = Result;
                checkVerify.notverified = NotVerifyResult;
                //if (Result > 0)
                //    Status = "Already Exists";
                //else
                //    Status = "Success";
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(checkVerify);
        }
        [HttpGet]
        public JsonResult IsAlreadyInvExists(string PatientID, string VisitID)
        {
            CheckVerify checkVerify = new CheckVerify();
            try
            {
                long HospitalId = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                long PatSeqid = _investrepo.GetPatSeqid(PatientID, HospitalId);
                int Result = _investrepo.IsAlreadyInvestExists(PatSeqid, VisitID);
                int NotVerifyResult = _investrepo.IsAlreadyNotVerifyInvest(PatSeqid, VisitID);
                checkVerify.verified = Result;
                checkVerify.notverified = NotVerifyResult;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(checkVerify);
        }
        [HttpGet]
        public JsonResult GetHistoryLastData(string PatientID, string Visitid)
        {
            List<PatientHistoryView> lstResult = new List<PatientHistoryView>();
            List<PatHabitView> lstPatHabit = new List<PatHabitView>();
            List<PatMedicalHistoryView> lstMediclHistory = new List<PatMedicalHistoryView>();
            List<string> lstAllergies = new List<string>();
            List<PatientMedicineView> lstMedicine = new List<PatientMedicineView>();
            List<PatientSurgicalHistory> lstPatSurgical = new List<PatientSurgicalHistory>();
            try
            {
                lstResult = _emrRepo.GetLatestPatHistoryByPatVisitID(PatientID, Visitid);
                if (lstResult.Count > 0)
                {
                    long historyID = lstResult[0].HistorySeqID;
                    lstPatHabit = _emrRepo.GetPatinetHabitsByID(historyID);
                    lstAllergies = _emrRepo.GetPatinetAllergiesByHistoryID(historyID);
                    lstMediclHistory = _emrRepo.GetPatMedicalHistoryByID(historyID);
                    lstMedicine = _emrRepo.GetPatCurrentMedicineById(historyID);
                    lstPatSurgical = _emrRepo.GetPatSurgicalHistoryByHistoryID(historyID);
                }
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatHistroy = lstResult,
                Pathabits = lstPatHabit,
                PatAllergy = lstAllergies,
                PatMedicalHstd = lstMediclHistory,
                PatMedicine = lstMedicine,
                PatSurgical = lstPatSurgical
            });
        }
        [HttpGet]
        public JsonResult GetExamLastData(string PatientId, string Visitid)
        {
            List<PatientExamintionView> lstResult = new List<PatientExamintionView>();
            List<ExaminationView> lstExamView = new List<ExaminationView>();
            long seqId = 0;
            try
            {
                lstResult = _emrRepo.GetLastPatExamHdrNotVerify(PatientId, Visitid);
                if (lstResult.Count > 0)
                    seqId = lstResult[0].SeqID;
                lstExamView = _emrRepo.GetLastPatinetExamintion(seqId);
            }
            catch (Exception)
            {
            }
            return Json(new
            {
                PatExam = lstResult,
                ExamView = lstExamView
            });
        }
    }
    public class CheckVerify
    {
        public int verified { get; set; }
        public int notverified { get; set; }
    }
}