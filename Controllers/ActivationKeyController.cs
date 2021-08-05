using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
using BizLayer.Interface;
using BizLayer.Utilities;
using Emr_web.Common;
using Emr_web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Syncfusion.EJ2.Navigations;

namespace Emr_web.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ActivationKeyController : Controller
    {
        private IDBConnection _IDBConnection;
        private IErrorlog _errorlog;
       
        public ActivationKeyController(IDBConnection iDBConnection, IErrorlog errorlog)
        {
            _IDBConnection = iDBConnection;
            _errorlog = errorlog;
        }
        public IActionResult ActivationKey()
        {
            return View();
        }
    }
}