using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
namespace Emr_web.Models
{
    public class ComplaintModel
    {
        public List<PatientSympotmHeader> ItemHeader { get; set; }
        public List<PatientSympotmDetails> ItemDetail { get; set; }
        public List<HospitalMaster> ItemHospital { get; set; }
        public List<MyPatient> itemPatient { get; set; }
        public ComplaintModel complaintModel(List<PatientSympotmHeader> lstHeader, List<PatientSympotmDetails> lstDeatils, List<HospitalMaster> lstHospital, List<MyPatient> lstPatient)
        {
            ComplaintModel complaintModel = new ComplaintModel();
            try
            {
                complaintModel.ItemHeader = lstHeader;
                complaintModel.ItemDetail = lstDeatils;
                complaintModel.ItemHospital = lstHospital;
                complaintModel.itemPatient = lstPatient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return complaintModel;
        }
    }
}
