using System;
using System.Collections.Generic;
using BizLayer.Domain;

namespace Emr_web.Models
{
    public class InvestigationModel
    {
        public List<PatientInvestHeader> ItemHeader { get; set; }
        public List<PatientInvestDetails> ItemDetail { get; set; }
        public List<HospitalMaster> ItemHospital { get; set; }
        public List<MyPatient> itemPatient { get; set; }
        public InvestigationModel investigationModel(List<PatientInvestHeader> lstHeader, List<PatientInvestDetails> lstDeatils, List<HospitalMaster> lstHospital, List<MyPatient> lstPatient)
        {
            InvestigationModel investigationModel = new InvestigationModel();
            try
            {
                investigationModel.ItemHeader = lstHeader;
                investigationModel.ItemDetail = lstDeatils;
                investigationModel.ItemHospital = lstHospital;
                investigationModel.itemPatient = lstPatient;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return investigationModel;
        }
    }
}
