using System.Collections.Generic;
using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Models
{
    public class SelfAssessmentDashboardViewModel
    {
        public List<SelfAssessmentModalModel> ModalMappings { get; set; }
        public SelfAssesmentModel DashboardData { get; set; }

        public SelfAssessmentDashboardViewModel()
        {
            
        }
    }
}