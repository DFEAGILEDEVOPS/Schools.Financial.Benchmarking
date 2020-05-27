using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Models
{
    public class BestInClassCharacteristicsViewModel : ViewModelBase
    {
        public SchoolViewModel BenchmarkSchool { get; set; }        
        public BestInClassCriteria BicCriteria { get; set; }

        public BestInClassCharacteristicsViewModel(SchoolViewModel school, BestInClassCriteria bicCriteria)
        {
            this.BenchmarkSchool = school;
            this.BicCriteria = bicCriteria;
        }
    }
}