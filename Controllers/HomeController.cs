using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Emr_web.Models;
using Syncfusion.EJ2.Navigations;
using BizLayer.Utilities;
using BizLayer.Interface;
using Emr_web.Common;
using System.Data;
using BizLayer.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class HomeController : Controller
    {

        private IDBConnection _IDBConnection;
        private IPatientRepo _patientRepo;
        private IErrorlog _errorlog;
        private IInvestRepo _investRepo;
        private IDiagnosisRepo _diagnosisRepo;
        private IEmrRepo _emrRepo;
        private ISymptonRepo _symptonRepo;
        private IHospitalRepo _hospitalRepo;
        private ISubMenuRepo _subMenuRepo;
        public HomeController(IDBConnection iDBConnection, IPatientRepo patientRepo, IErrorlog errorlog, IInvestRepo investRepo, IDiagnosisRepo diagnosisRepo, IEmrRepo emrRepo, ISymptonRepo symptonRepo, IHospitalRepo hospitalRepo, ISubMenuRepo subMenuRepo)
        {
            _IDBConnection = iDBConnection;
            _patientRepo = patientRepo;
            _errorlog = errorlog;
            _investRepo = investRepo;
            _diagnosisRepo = diagnosisRepo;
            _emrRepo = emrRepo;
            _symptonRepo = symptonRepo;
            _hospitalRepo = hospitalRepo;
            _subMenuRepo = subMenuRepo;
        }
        public IActionResult Index()
        {
            try
            {
                ViewBag.FrommaxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                ViewBag.TomaxDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                int PatientSubmenuid = _subMenuRepo.GetPatientScreenSubmenuid();  // Get My Patient Submenu id from Config Table for save and verify button enable.
                List<Config> myconfig = _patientRepo.GetConfig();
                bool HISConnected = myconfig[0].IsConnectedHIS;
                bool PharmacyConnected = myconfig[0].IsConnectedPharmacy;
                List<Login> logins = myComplexObject;
                long roleid = logins[0].RoleID;
                long userseqid = logins[0].UserSeqid;
                long Hospitalid = logins[0].HospitalID;
                long Clinicid = logins[0].ClinicID;
                long Doctorid = logins[0].DoctorID;
                long Licenceid = logins[0].LicenceID;
                long Roleid = logins[0].RoleID;
                string UserName= Encrypt_Decrypt.Decrypt(logins[0].UserName);
                string Userid = Encrypt_Decrypt.Decrypt(logins[0].Userid);
                string TimezoneID = logins[0].TimezoneID;
                long Mobilenumber = logins[0].MobileNumber;
                HttpContext.Session.SetString("Userid", Userid);
                HttpContext.Session.SetString("UserName", UserName);
                HttpContext.Session.SetString("Userseqid", userseqid.ToString());
                HttpContext.Session.SetString("Clinicid", Clinicid.ToString());
                HttpContext.Session.SetString("Hospitalid", Hospitalid.ToString());
                HttpContext.Session.SetString("Doctorid", Doctorid.ToString());
                HttpContext.Session.SetString("Licenceid", Licenceid.ToString());
                HttpContext.Session.SetString("Roleid", Roleid.ToString());
                HttpContext.Session.SetString("PatientSubmenuid", PatientSubmenuid.ToString());
                HttpContext.Session.SetString("MobileNumber", Mobilenumber.ToString());
                if (TimezoneID != "" && TimezoneID != null)
                    HttpContext.Session.SetString("TimezoneID", TimezoneID.ToString());
                else
                    HttpContext.Session.SetString("TimezoneID", "India Standard Time");
                
                DataTable dtmainmenu = new DataTable();
                DataTable dtsubmenu = new DataTable();
                dtmainmenu = _patientRepo.GetAllMainmenus(roleid);
                if (dtmainmenu.Rows.Count > 0)
                {
                    List<MenuItem> mainMenuItem = new List<MenuItem>();
                    for (int i = 0; i < dtmainmenu.Rows.Count; i++)
                    {
                        MenuItem menuItem = new MenuItem();
                        long Menuid = Convert.ToInt64(dtmainmenu.Rows[i]["M_Menu_id"]);
                        string Menuname = dtmainmenu.Rows[i]["M_Menuname"].ToString();
                        menuItem.Text = Menuname;
                        if (HISConnected == true)
                        {
                            if (Menuname != "Registration")
                            {
                                menuItem.IconCss = "icon-user icon";
                                dtsubmenu = _patientRepo.GetAllsubmenus(Menuid, roleid);
                                if (dtsubmenu.Rows.Count > 0)
                                {
                                    List<MenuItem> submenuitem = new List<MenuItem>();
                                    MenuItem sub = new MenuItem();
                                    for (int j = 0; j < dtsubmenu.Rows.Count; j++)
                                    {
                                        sub = new MenuItem();
                                        long Submenuid = Convert.ToInt64(dtsubmenu.Rows[j]["Submenuid"]);
                                        string Submenuname = dtsubmenu.Rows[j]["Submenuname"].ToString();
                                        string Menulink = dtsubmenu.Rows[j]["Submenulink"].ToString();
                                        sub.Text = Submenuname;
                                        sub.Url = Menulink;
                                        submenuitem.Add(sub);
                                    }
                                    menuItem.Items = submenuitem;
                                    mainMenuItem.Add(menuItem);
                                }
                            }
                        }
                        else
                        {
                            menuItem.IconCss = "icon-user icon";
                            dtsubmenu = _patientRepo.GetAllsubmenus(Menuid, roleid);
                            if (dtsubmenu.Rows.Count > 0)
                            {
                                List<MenuItem> submenuitem = new List<MenuItem>();
                                MenuItem sub = new MenuItem();
                                for (int j = 0; j < dtsubmenu.Rows.Count; j++)
                                {
                                    sub = new MenuItem();
                                    long Submenuid = Convert.ToInt64(dtsubmenu.Rows[j]["Submenuid"]);
                                    string Submenuname = dtsubmenu.Rows[j]["Submenuname"].ToString();
                                    string Menulink = dtsubmenu.Rows[j]["Submenulink"].ToString();
                                    sub.Text = Submenuname;
                                    sub.Url = Menulink;
                                    submenuitem.Add(sub);
                                }
                                menuItem.Items = submenuitem;
                                mainMenuItem.Add(menuItem);
                            }
                        }
                    }
                    ViewBag.mainMenuItems = mainMenuItem;
                    HttpContext.Session.SetObjectAsJsonLsit("MenuList", mainMenuItem.ToArray());

                }
                List<MenuItem> AccountMenuItems = new List<MenuItem>();
                AccountMenuItems.Add(new MenuItem
                {
                    Text = Userid,
                    Items = new List<MenuItem>()
                    {
                        new MenuItem { Text = "Profile" },
                        new MenuItem { Text = "Sign out", Url = "/Login/Login" }

                    }
                });
                ViewBag.AccountMenuItems = AccountMenuItems;
                HttpContext.Session.SetObjectAsJsonLsit("AccountList", AccountMenuItems.ToArray());
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return View();
        }
        [HttpGet]
        public JsonResult GetMenuList()
        {
            List<MenuItem> lstHeader = new List<MenuItem>();
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                List<Login> logins = myComplexObject;
                DataTable dtmainmenu = new DataTable();
                DataTable dtsubmenu = new DataTable();
                long roleid = logins[0].RoleID;
                string Userid = Encrypt_Decrypt.Decrypt(logins[0].Userid);
                ViewBag.Username = Userid;
                //List<Config> myconfig = _patientRepo.GetConfig();
                //bool HISConnected = myconfig[0].IsConnectedHIS;
                //bool PharmacyConnected = myconfig[0].IsConnectedPharmacy;
                bool HISConnected = logins[0].IsConnectedHIS;
                bool PharmacyConnected = logins[0].IsConnectedPharmacy;
                dtmainmenu = _patientRepo.GetAllMainmenus(roleid);
                if (dtmainmenu.Rows.Count > 0)
                {
                    List<MenuItem> mainMenuItem = new List<MenuItem>();
                    for (int i = 0; i < dtmainmenu.Rows.Count; i++)
                    {
                        MenuItem menuItem = new MenuItem();
                        long Menuid = Convert.ToInt64(dtmainmenu.Rows[i]["M_Menu_id"]);
                        string Menuname = dtmainmenu.Rows[i]["M_Menuname"].ToString();
                        string MenuClass = dtmainmenu.Rows[i]["M_Class"].ToString();
                        menuItem.Text = Menuname;
                        menuItem.Url = MenuClass;
                        //if (HISConnected == true)
                        //{
                        //    if (Menuname != "Registration")
                        //    {
                        //        menuItem.IconCss = Userid;
                        //        dtsubmenu = _patientRepo.GetAllsubmenus(Menuid, roleid);
                        //        if (dtsubmenu.Rows.Count > 0)
                        //        {
                        //            List<MenuItem> submenuitem = new List<MenuItem>();
                        //            MenuItem sub = new MenuItem();
                        //            for (int j = 0; j < dtsubmenu.Rows.Count; j++)
                        //            {
                        //                sub = new MenuItem();
                        //                long Submenuid = Convert.ToInt64(dtsubmenu.Rows[j]["Submenuid"]);
                        //                string Submenuname = dtsubmenu.Rows[j]["Submenuname"].ToString();
                        //                string Menulink = dtsubmenu.Rows[j]["Submenulink"].ToString();
                        //                string SubClass = dtsubmenu.Rows[j]["S_Class"].ToString();
                        //                sub.Text = Submenuname;
                        //                sub.Url = Menulink;
                        //                sub.IconCss = SubClass;
                        //                submenuitem.Add(sub);
                        //            }
                        //            menuItem.Items = submenuitem;
                        //            mainMenuItem.Add(menuItem);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                            menuItem.IconCss = Userid;
                            dtsubmenu = _patientRepo.GetAllsubmenus(Menuid, roleid);
                            if (dtsubmenu.Rows.Count > 0)
                            {
                                List<MenuItem> submenuitem = new List<MenuItem>();
                                MenuItem sub = new MenuItem();
                                for (int j = 0; j < dtsubmenu.Rows.Count; j++)
                                {
                                    sub = new MenuItem();
                                    long Submenuid = Convert.ToInt64(dtsubmenu.Rows[j]["Submenuid"]);
                                    string Submenuname = dtsubmenu.Rows[j]["Submenuname"].ToString();
                                    string Menulink = dtsubmenu.Rows[j]["Submenulink"].ToString();
                                    string SubClass = dtsubmenu.Rows[j]["S_Class"].ToString();
                                    sub.Text = Submenuname;
                                    sub.Url = Menulink;
                                    sub.IconCss = SubClass;
                                    submenuitem.Add(sub);
                                }
                                menuItem.Items = submenuitem;
                                mainMenuItem.Add(menuItem);
                            }
                        //}
                    }
                    lstHeader = mainMenuItem;
                }
            }
            catch (Exception)
            {

            }
            return Json(new { MenuHeader = lstHeader });
        }

        [HttpGet]
        public JsonResult GetAllMenuList()
        {
            List<MainMenu> mainMenus = new List<MainMenu>();
            List<SubMenu> subMenus = new List<SubMenu>();
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                List<Login> logins = myComplexObject;
                long roleid = logins[0].RoleID;
                string Userid = Encrypt_Decrypt.Decrypt(logins[0].Userid);
                bool HISConnected = logins[0].IsConnectedHIS;
                mainMenus = _patientRepo.GetAllMainmenusByRole(roleid);
                if (mainMenus.Count > 0)
                {
                    if (HISConnected == true)
                    {
                        if(mainMenus.Any(item => item.M_Menuname == "Registration"))
                        {
                            var itemToRemove = mainMenus.Single(r => r.M_Menuname == "Registration");
                            mainMenus.Remove(itemToRemove);

                        }
                    }

                    for (int i = 0; i < mainMenus.Count; i++)
                    {
                        long Mainmenuid = mainMenus[i].M_Menu_id;
                        subMenus = _patientRepo.GetAllsubmenuByMainMenu(roleid, Mainmenuid);
                        mainMenus[i].subMenus = subMenus;
                    }
                    mainMenus[0].M_CreatedUser = Userid;
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(new { MenuHeader = mainMenus });
        }
        [HttpGet]
        public JsonResult GenderChartData(string DayType)
        {
            PieChart _chart = new PieChart();
            try
            {
                string Fromdate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                string Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                if (DayType == "1")
                {
                    Fromdate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "2")
                {
                    Fromdate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "7")
                {
                    string sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).ToString();
                    sunday = sunday.Substring(0, 2);
                    int weekstart = Convert.ToInt32(sunday);
                    var Today = DateTime.Now.ToString("dd");
                    int todaydate = Convert.ToInt32(Today);
                    int Difference = todaydate - weekstart;
                    Fromdate = DateTime.Now.AddDays(-(Difference)).ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "30")
                {
                    Fromdate = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                List<string> Gender = new List<string>();
                List<decimal> genderdata = new List<decimal>();
                List<string> colors = new List<string>();
                decimal male = _patientRepo.GetMalePatientCount(Fromdate, Todate, Hospitalid);
                decimal female = _patientRepo.GetFeMalePatientCount(Fromdate, Todate, Hospitalid);
                decimal others = _patientRepo.GetOtherPatientCount(Fromdate, Todate, Hospitalid);
                genderdata.Add(male);
                genderdata.Add(female);
                genderdata.Add(others);
                Gender.Add("Male");
                Gender.Add("Female");
                Gender.Add("Others");
                colors.Add("#f56954");
                colors.Add("#00a65a");
                colors.Add("#f39c12");
                _chart.labels = Gender.ToArray();
                _chart.datasets = new List<PieDatasets>();
                List<PieDatasets> _dataSet = new List<PieDatasets>();
                PieDatasets dataSet = new PieDatasets();
                dataSet.data = genderdata.ToArray();
                dataSet.backgroundColor = colors.ToArray();
                _dataSet.Add(dataSet);
                _chart.datasets = _dataSet;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(_chart);
        }
        [HttpGet]
        public JsonResult GenderDiagnosisBarchart(string DayType)
        {
            DiagnosisBarChart _diagnosisBarChart = new DiagnosisBarChart();
            try
            {
                decimal total = 0;
                string Fromdate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                string Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                if (DayType == "1")
                {
                    Fromdate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "2")
                {
                    Fromdate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "7")
                {
                    string sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).ToString();
                    sunday = sunday.Substring(0, 2);
                    int weekstart = Convert.ToInt32(sunday);
                    var Today = DateTime.Now.ToString("dd");
                    int todaydate = Convert.ToInt32(Today);
                    int Difference = todaydate - weekstart;
                    Fromdate = DateTime.Now.AddDays(-(Difference)).ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "30")
                {
                    Fromdate = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                List<string> GenderDiagnosis = new List<string>();
                List<string> Gender = new List<string>();
                List<decimal> genderdata = new List<decimal>();
                List<decimal> TotalCount = new List<decimal>();
                List<DiagnosisBarDatasets> _dataSet = new List<DiagnosisBarDatasets>();
                Gender.Add("Male");
                Gender.Add("Female");
                Gender.Add("Others");
                GenderDiagnosis = _patientRepo.GetGenderwiseDiagnosis(Fromdate, Todate, Hospitalid);
                _diagnosisBarChart.labels = GenderDiagnosis.ToArray();
                _diagnosisBarChart.datasets = new List<DiagnosisBarDatasets>();
                _dataSet = new List<DiagnosisBarDatasets>();
                for (int y = 0; y < GenderDiagnosis.Count(); y++)
                {
                    total = 0;
                    string Diagnosisname = GenderDiagnosis[y].ToString();
                    for (int z = 0; z < Gender.Count(); z++)
                    {
                        string Genderval = Gender[z].ToString();
                        decimal Count = _patientRepo.DiagnosiscountbyGender(Fromdate, Todate, Hospitalid, Diagnosisname, Genderval);
                        total = total + Count;
                    }
                    TotalCount.Add(total);
                }
                for (int j = 0; j < Gender.Count(); j++)
                {
                    genderdata = new List<decimal>();
                    string Genderval = Gender[j].ToString();
                    for (int i = 0; i < GenderDiagnosis.Count(); i++)
                    {
                        string Diagnosisname = GenderDiagnosis[i].ToString();
                        decimal Count = _patientRepo.DiagnosiscountbyGender(Fromdate, Todate, Hospitalid, Diagnosisname, Genderval);
                        genderdata.Add(Count);
                    }
                    if (j == 0)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Male",
                            backgroundColor = "rgba(60,141,188,0.9)",
                            borderColor = "rgba(60,141,188,0.8)",
                            pointRadius = false,
                            pointColor = "#3b8bba",
                            pointStrokeColor = "rgba(60,141,188,1)",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(60,141,188,1)",
                            data = genderdata.ToArray(),
                        });
                    }
                    else if (j == 1)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Female",
                            backgroundColor = "rgba(210, 214, 222, 1)",
                            borderColor = "rgba(210, 214, 222, 1)",
                            pointRadius = false,
                            pointColor = "rgba(210, 214, 222, 1)",
                            pointStrokeColor = "#c1c7d1",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(220,220,220,1)",
                            data = genderdata.ToArray(),
                        });
                    }
                    else if (j == 2)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Others",
                            backgroundColor = "rgba(0, 192, 239, 1)",
                            borderColor = "rgba(0, 192, 239, 1)",
                            pointRadius = false,
                            pointColor = "rgba(0, 192, 239, 1)",
                            pointStrokeColor = "#00c0ef",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(0, 192, 239, 1)",
                            data = genderdata.ToArray(),
                        });
                    }
                }
                _diagnosisBarChart.Total = TotalCount.ToArray();
                _diagnosisBarChart.datasets = _dataSet;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(_diagnosisBarChart);
        }
        [HttpGet]
        public JsonResult DiagnosisBarchart(string DayType)
        {
            DiagnosisBarChart _diagnosisBarChart = new DiagnosisBarChart();
            try
            {
                string Fromdate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                string Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                if (DayType == "1")
                {
                    Fromdate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "2")
                {
                    Fromdate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "7")
                {
                    string sunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).ToString();
                    sunday = sunday.Substring(0, 2);
                    int weekstart = Convert.ToInt32(sunday);
                    var Today = DateTime.Now.ToString("dd");
                    int todaydate = Convert.ToInt32(Today);
                    int Difference = todaydate - weekstart;
                    Fromdate = DateTime.Now.AddDays(-(Difference)).ToString("yyyy-MM-dd 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                else if (DayType == "30")
                {
                    Fromdate = DateTime.Now.ToString("yyyy-MM-01 00:00:00");
                    Todate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
                }
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                List<string> Diagnosis = new List<string>();
                List<string> Gender = new List<string>();
                List<decimal> genderdata = new List<decimal>();
                List<DiagnosisBarDatasets> _dataSet = new List<DiagnosisBarDatasets>();
                Gender.Add("List of Diagnosis");
                Diagnosis = _patientRepo.TopFiveDiagnosis(Fromdate, Todate, Hospitalid);
                _diagnosisBarChart.labels = Diagnosis.ToArray();
                _diagnosisBarChart.datasets = new List<DiagnosisBarDatasets>();
                _dataSet = new List<DiagnosisBarDatasets>();
                for (int j = 0; j < Gender.Count(); j++)
                {
                    genderdata = new List<decimal>();
                    string Genderval = Gender[j].ToString();
                    for (int i = 0; i < Diagnosis.Count(); i++)
                    {
                        string Diagnosisname = Diagnosis[i].ToString();
                        decimal Count = _patientRepo.Diagnosiscount(Fromdate, Todate, Hospitalid, Diagnosisname);
                        genderdata.Add(Count);
                    }
                    if (j == 0)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Diagnosis",
                            backgroundColor = "rgba(243, 156, 18,0.9)",
                            borderColor = "rgba(243, 156, 18,0.8)",
                            pointRadius = false,
                            pointColor = "#f39c12",
                            pointStrokeColor = "rgba(243, 156, 18,1)",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(243, 156, 18,1)",
                            data = genderdata.ToArray(),
                        });
                    }
                }
                _diagnosisBarChart.datasets = _dataSet;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(_diagnosisBarChart);
        }
        [HttpGet]
        public JsonResult GenderSearchChartData(string Fromdate, string Todate)
        {
            PieChart _chart = new PieChart();
            try
            {
                Fromdate = Fromdate + " " + "00:00:00";
                Todate = Todate + " " + "23:59:59";
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                List<string> Gender = new List<string>();
                List<decimal> genderdata = new List<decimal>();
                List<string> colors = new List<string>();
                decimal male = _patientRepo.GetMalePatientCount(Fromdate, Todate, Hospitalid);
                decimal female = _patientRepo.GetFeMalePatientCount(Fromdate, Todate, Hospitalid);
                decimal others = _patientRepo.GetOtherPatientCount(Fromdate, Todate, Hospitalid);
                genderdata.Add(male);
                genderdata.Add(female);
                genderdata.Add(others);
                Gender.Add("Male");
                Gender.Add("Female");
                Gender.Add("Others");
                colors.Add("#f56954");
                colors.Add("#00a65a");
                colors.Add("#f39c12");
                _chart.labels = Gender.ToArray();
                _chart.datasets = new List<PieDatasets>();
                List<PieDatasets> _dataSet = new List<PieDatasets>();
                PieDatasets dataSet = new PieDatasets();
                dataSet.data = genderdata.ToArray();
                dataSet.backgroundColor = colors.ToArray();
                _dataSet.Add(dataSet);
                _chart.datasets = _dataSet;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(_chart);
        }
        [HttpGet]
        public JsonResult DiagnosisSearchBarchart(string Fromdate, string Todate)
        {
            DiagnosisBarChart _diagnosisBarChart = new DiagnosisBarChart();
            try
            {
                Fromdate = Fromdate + " " + "00:00:00";
                Todate = Todate + " " + "23:59:59";
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                List<string> Diagnosis = new List<string>();
                List<string> Gender = new List<string>();
                List<decimal> genderdata = new List<decimal>();
                List<DiagnosisBarDatasets> _dataSet = new List<DiagnosisBarDatasets>();
                Gender.Add("List of Diagnosis");
                Diagnosis = _patientRepo.TopFiveDiagnosis(Fromdate, Todate, Hospitalid);
                _diagnosisBarChart.labels = Diagnosis.ToArray();
                _diagnosisBarChart.datasets = new List<DiagnosisBarDatasets>();
                _dataSet = new List<DiagnosisBarDatasets>();
                for (int j = 0; j < Gender.Count(); j++)
                {
                    genderdata = new List<decimal>();
                    string Genderval = Gender[j].ToString();
                    for (int i = 0; i < Diagnosis.Count(); i++)
                    {
                        string Diagnosisname = Diagnosis[i].ToString();
                        decimal Count = _patientRepo.Diagnosiscount(Fromdate, Todate, Hospitalid, Diagnosisname);
                        genderdata.Add(Count);
                    }
                    if (j == 0)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Diagnosis",
                            backgroundColor = "rgba(243, 156, 18,0.9)",
                            borderColor = "rgba(243, 156, 18,0.8)",
                            pointRadius = false,
                            pointColor = "#f39c12",
                            pointStrokeColor = "rgba(243, 156, 18,1)",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(243, 156, 18,1)",
                            data = genderdata.ToArray(),
                        });
                    }
                }
                _diagnosisBarChart.datasets = _dataSet;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(_diagnosisBarChart);
        }
        [HttpGet]
        public JsonResult GenderSearchDiagnosisBarchart(string Fromdate, string Todate)
        {
            DiagnosisBarChart _diagnosisBarChart = new DiagnosisBarChart();
            try
            {
                decimal total = 0;
                Fromdate = Fromdate + " " + "00:00:00";
                Todate = Todate + " " + "23:59:59";
                long Hospitalid = Convert.ToInt64(HttpContext.Session.GetString("Hospitalid"));
                List<string> GenderDiagnosis = new List<string>();
                List<string> Gender = new List<string>();
                List<decimal> genderdata = new List<decimal>();
                List<decimal> TotalCount = new List<decimal>();
                List<DiagnosisBarDatasets> _dataSet = new List<DiagnosisBarDatasets>();
                Gender.Add("Male");
                Gender.Add("Female");
                Gender.Add("Others");
                GenderDiagnosis = _patientRepo.GetGenderwiseDiagnosis(Fromdate, Todate, Hospitalid);
                _diagnosisBarChart.labels = GenderDiagnosis.ToArray();
                _diagnosisBarChart.datasets = new List<DiagnosisBarDatasets>();
                _dataSet = new List<DiagnosisBarDatasets>();
                for (int y = 0; y < GenderDiagnosis.Count(); y++)
                {
                    total = 0;
                    string Diagnosisname = GenderDiagnosis[y].ToString();
                    for (int z = 0; z < Gender.Count(); z++)
                    {
                        string Genderval = Gender[z].ToString();
                        decimal Count = _patientRepo.DiagnosiscountbyGender(Fromdate, Todate, Hospitalid, Diagnosisname, Genderval);
                        total = total + Count;
                    }
                    TotalCount.Add(total);
                }
                for (int j = 0; j < Gender.Count(); j++)
                {
                    genderdata = new List<decimal>();
                    string Genderval = Gender[j].ToString();
                    for (int i = 0; i < GenderDiagnosis.Count(); i++)
                    {
                        string Diagnosisname = GenderDiagnosis[i].ToString();
                        decimal Count = _patientRepo.DiagnosiscountbyGender(Fromdate, Todate, Hospitalid, Diagnosisname, Genderval);
                        genderdata.Add(Count);
                    }
                    if (j == 0)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Male",
                            backgroundColor = "rgba(60,141,188,0.9)",
                            borderColor = "rgba(60,141,188,0.8)",
                            pointRadius = false,
                            pointColor = "#3b8bba",
                            pointStrokeColor = "rgba(60,141,188,1)",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(60,141,188,1)",
                            data = genderdata.ToArray(),
                        });
                    }
                    else if (j == 1)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Female",
                            backgroundColor = "rgba(210, 214, 222, 1)",
                            borderColor = "rgba(210, 214, 222, 1)",
                            pointRadius = false,
                            pointColor = "rgba(210, 214, 222, 1)",
                            pointStrokeColor = "#c1c7d1",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(220,220,220,1)",
                            data = genderdata.ToArray(),
                        });
                    }
                    else if (j == 2)
                    {
                        _dataSet.Add(new DiagnosisBarDatasets()
                        {
                            label = "Others",
                            backgroundColor = "rgba(0, 192, 239, 1)",
                            borderColor = "rgba(0, 192, 239, 1)",
                            pointRadius = false,
                            pointColor = "rgba(0, 192, 239, 1)",
                            pointStrokeColor = "#00c0ef",
                            pointHighlightFill = "#fff",
                            pointHighlightStroke = "rgba(0, 192, 239, 1)",
                            data = genderdata.ToArray(),
                        });
                    }
                }
                _diagnosisBarChart.Total = TotalCount.ToArray();
                _diagnosisBarChart.datasets = _dataSet;
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(_diagnosisBarChart);
        }

        [HttpGet]
        public JsonResult GetTrailDays()
        {
            string Result = "";
            try
            {
                var myComplexObject = HttpContext.Session.GetObjectFromJsonList<Login>("LoginDetails");
                List<Login> logins = myComplexObject;
                if (logins[0].IsTrailUser)
                {
                    Result = "Trial period " + logins[0].TrailDays + " days";
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            return Json(Result);
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        public IActionResult Print()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult LogOff()
        {
            return RedirectToAction("Login", "Login");
        }
    }
}
