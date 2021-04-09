using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.UI.Models
{
    public class SimpleCharacteristicsViewModel : ViewModelBase
    {
        public EstablishmentViewModelBase BenchmarkSchool { get; set; }
        public SimpleCriteria SimpleCriteria { get; set; }

        public SimpleCharacteristicsViewModel(EstablishmentViewModelBase school, SimpleCriteria simpleCriteria)
        {
            this.BenchmarkSchool = school;
            this.SimpleCriteria = simpleCriteria;
        }
    }
}