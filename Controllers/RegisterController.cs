using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Emr_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Emr_web.Controllers
{
    public class RegisterController : Controller
    {
        private IDBConnection _IDBConnection;
        private IRegisterRepo _registerRepo;
        private IErrorlog _errorlog;
        private ILoginRepo _loginrepo;
        private IHospitalRepo _hospitalRepo;
        private ILicenceRepo _licenceRepo;
        public RegisterController(IDBConnection iDBConnection, IRegisterRepo registerRepo, IErrorlog errorlog, ILoginRepo loginRepo, IHospitalRepo hospitalRepo,ILicenceRepo licenceRepo)
        {
            _IDBConnection = iDBConnection;
            _registerRepo = registerRepo;
            _errorlog = errorlog;
            _loginrepo = loginRepo;
            _hospitalRepo = hospitalRepo;
            _licenceRepo = licenceRepo;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public JsonResult NewRegisterInsert([FromBody] RegistrationView data)
        {
            bool isSuccess = false;
            try
            {
                if (ModelState.IsValid)
                {
                    int value = 0;
                    string rdnvalue = "";
                    Random rnd = new Random();
                    value = rnd.Next(10000, 99999);
                    string HospitalPrefix = _licenceRepo.GetHospitalPrefix();
                    rdnvalue = HospitalPrefix + value.ToString();
                    bool value_ok = false;
                    while (!value_ok)
                    {
                        string CheckRdnexist = _licenceRepo.RandomvalExist(rdnvalue);
                        if (CheckRdnexist == null && CheckRdnexist == "")
                        {
                            value = rnd.Next(10000, 99999);
                            rdnvalue = HospitalPrefix + value.ToString();
                        }
                        else
                            value_ok = true;
                    }
                    long Hospitalid = _registerRepo.GetHospitalID(data.HospitalName);
                    if (Hospitalid == 0)
                    {
                        HospitalMaster hospitalMaster = new HospitalMaster();
                        hospitalMaster.Hospital_Uniqueno = rdnvalue;
                        hospitalMaster.HospitalName = data.HospitalName;
                        hospitalMaster.HospitalMobileNo = null;
                        hospitalMaster.HospitalLandlineNo = null;
                        hospitalMaster.HospitalLandlineNo1 = null;
                        hospitalMaster.HospitalAddress = null;
                        hospitalMaster.HospitalAddress1 = null;
                        hospitalMaster.HospitalAddress2 = null;
                        hospitalMaster.City = null;
                        hospitalMaster.Country = null;
                        hospitalMaster.Pin = null;
                        hospitalMaster.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        hospitalMaster.CreatedUser = "";
                        hospitalMaster.ModifiedUser = "";
                        hospitalMaster.Isactive = true;
                        Hospitalid = _hospitalRepo.InsertHospital(hospitalMaster);
                    }
                    Registration registration = new Registration();
                    registration.AdminUsername = data.AdminUsername;
                    registration.AdminUserid = data.AdminUserid;
                    registration.AdminPassword = data.AdminPassword;
                    registration.AdminEmailid = data.AdminEmailid;
                    registration.ActivationKey = data.ActivationKey;
                    registration.ContactNumber = data.ContactNumber;
                    registration.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    registration.CreatedUser = "";
                    registration.ModifiedUser = "";
                    registration.IsActive = true;
                    registration.Hospitalid = Hospitalid;
                    isSuccess = _registerRepo.CreateNewRegister(registration);
                    if (isSuccess == true)
                    {
                        Login login = new Login();
                        login.UserName = data.AdminUsername;
                        login.Userid = data.AdminUserid;
                        login.Password = data.AdminPassword;
                        login.RoleID = 1;
                        login.DepartmentID = 0;
                        login.SpecialityID = 0;
                        login.UserType = 0;
                        login.HospitalID = Hospitalid;
                        login.ClinicID = 0;
                        login.ForgotPwdEmail = null;
                        login.ActivationKey = null;
                        login.ModifiedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        login.CreatedUser = "";
                        login.ModifiedUser = "";
                        login.IsActive = true;
                        login.IsPrimaryUser = false;
                        login.LicenceID = 0;
                        isSuccess = _loginrepo.InsertNewUser(login);
                    }
                }
            }
            catch (Exception ex)
            {
                _errorlog.WriteErrorLog(ex.ToString());
            }
            finally { }
            return Json(isSuccess);
        }
    }
}