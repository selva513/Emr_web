using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BizLayer.Domain;
namespace Emr_web.Models
{
    public class TreatmentModel
    {
        public List<PatientTreatmentHeader> ItemHeader { get; set; }
        public List<PatientDrugDetails> ItemDetail { get; set; }
        public List<HospitalMaster> ItemHospital { get; set; }
        public List<MyPatient> itemPatient { get; set; }
        public TreatmentModel treatmentModel(List<PatientTreatmentHeader> lstHeader, List<PatientDrugDetails> lstDeatils, List<HospitalMaster> lstHospital, List<MyPatient> lstPatient)
        {
            TreatmentModel treatmentModel = new TreatmentModel();
            try
            {
                treatmentModel.ItemHeader = lstHeader;
                treatmentModel.ItemDetail = lstDeatils;
                treatmentModel.ItemHospital = lstHospital;
                treatmentModel.itemPatient = lstPatient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return treatmentModel;
        }
    }
}
