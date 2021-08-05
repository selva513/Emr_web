using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
namespace Emr_web.Models
{
    public class DiagnosisModel
    {
        public List<PatientDiagnosisHeader> ItemHeader { get; set; }
        public List<PatientDiagnosisDetail> ItemDetail { get; set; }
        public List<HospitalMaster> ItemHospital { get; set; }
        public List<MyPatient> itemPatient { get; set; }
        public DiagnosisModel diagnosisModel (List<PatientDiagnosisHeader> lstHeader, List<PatientDiagnosisDetail> lstDeatils, List<HospitalMaster> lstHospital, List<MyPatient> lstPatient)
        {
            DiagnosisModel diagnosisModel = new DiagnosisModel();
            try
            {
                diagnosisModel.ItemHeader = lstHeader;
                diagnosisModel.ItemDetail = lstDeatils;
                diagnosisModel.ItemHospital = lstHospital;
                diagnosisModel.itemPatient = lstPatient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return diagnosisModel;
        }
    }
}
