using System.Collections.Generic;

namespace SFB.Web.Domain.Models
{
    public class BenchmarkChartModel
    {
        private List<int> _incompleteDataIndex;
        public List<BenchmarkChartData> ChartData { get; set; }
        public int BenchmarkSchoolIndex { get; set; }

        public List<int> IncompleteDataIndex
        {
            get { return _incompleteDataIndex; }
            set { _incompleteDataIndex = value; }
        }
    }
}
