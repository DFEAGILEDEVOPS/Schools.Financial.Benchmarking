using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.UI.Models
{
    public class ViewModelListBase<T> where T:DynamicViewModelBase
    {
        public ComparisonListModel SchoolComparisonList;

        public int ComparisonListCount
        {
            get
            {
                return this.SchoolComparisonList.BenchmarkSchools.Count;
            }
        }

        public Pagination Pagination { get; set; }

        public List<T> ModelList { get; set; }

        public string OrderBy { get; set; }

        public Filter[] Filters { get; set; }

        public string FilterSelectionState { get; set; }

        public bool BmSchoolInList()
        {
            return SchoolComparisonList.BenchmarkSchools.Any(bs => bs.Urn == SchoolComparisonList.HomeSchoolUrn);
        }
    }
}