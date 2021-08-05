using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emr_web.Models
{
    public class UploadModel
    {
        public string TagName { get; set; }
        public long DescriptionID { get; set; }
        public IFormFile FilePatientDocment { get; set; }
        public virtual string DescriptionName { get; set; }
    }
}
