using SFB.Web.Common;
using SFB.Web.Domain.Models;

namespace SFB.Web.UI.Models
{
    public class BenchmarkCriteriaVM : ViewModelBase
    {
        public BenchmarkCriteriaVM() {}

        public BenchmarkCriteria AdvancedCriteria { get; set; }

        public BenchmarkCriteriaVM(BenchmarkCriteria benchmarkCriteria)
        {
            AdvancedCriteria = benchmarkCriteria;
        }
    }
}