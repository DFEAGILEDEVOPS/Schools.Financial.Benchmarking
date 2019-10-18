using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Models
{
    public class SimpleCharacteristicsViewModel : ViewModelBase
    {
        public SchoolViewModel BenchmarkSchool { get; set; }
        public SimpleCriteria SimpleCriteria { get; set; }

        public SimpleCharacteristicsViewModel(SchoolViewModel school, SimpleCriteria simpleCriteria)
        {
            this.BenchmarkSchool = school;
            this.SimpleCriteria = simpleCriteria;
        }
    }
}