using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Models
{
    public class SelfAssessmentModalModel
    {
        public int Id { get; set; }
        public string AssessmentArea { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public SadAssesmentAreaModel AssessmentAreaModel { get; set; }
    }
}