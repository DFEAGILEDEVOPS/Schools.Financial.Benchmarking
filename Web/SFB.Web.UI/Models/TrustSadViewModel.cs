using System.Collections.Generic;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class TrustSadViewModel
    {
        public TrustViewModel TrustViewModel { get; set; }
        
        public TrustSelfAssessmentModel TrustSelfAssessmentData { get; set; }
        
        public SadCategories CurrentCategory { get; set; }
    }
}