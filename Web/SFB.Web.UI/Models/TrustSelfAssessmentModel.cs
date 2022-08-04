using System.Collections.Generic;
using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class TrustSelfAssessmentModel
    {
        public string TrustName { get; set; }
        public long Uid { get; set; }
        public int? CompanyNumber { get; set; }
        public SadCategories CurrentCategory { get; set; }
        
        public List<SelfAssesmentModel> Academies { get; set; }
        
        public List<KeyValuePair<string,string>> Navigation { get; set; }
        
        public List<SelfAssessmentModalModel> ModalMappings { get; set; }

        public TrustSelfAssessmentModel()
        {
            
        }
    }
}