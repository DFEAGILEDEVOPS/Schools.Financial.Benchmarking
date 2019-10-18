using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Models
{
    public class BenchmarkChartModel
    {
        public List<BenchmarkChartData> ChartData { get; set; }
        public int BenchmarkSchoolIndex { get; set; }
        public List<int> IncompleteFinanceDataIndex { get; set; }
        public List<int> IncompleteWorkforceDataIndex { get; set; }
    }
}
