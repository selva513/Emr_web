using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class ConfigView
    {
        public string Speical_Password { get; set; }
        public bool IsConnectedHIS { get; set; }
        public bool IsReportHeader { get; set; }
        public string HospitalPrefix { get; set; }
        public string LicenceFromEmail { get; set; }
        public string LicenceFromPwd { get; set; }
        public string LicenceToEmail { get; set; }
        public string MessageSubject { get; set; }
        public string ActivationEmailBody { get; set; }
        public bool IsConnectedPharmacy { get; set; }
        public long TrialPeriod { get; set; }
    }
}
