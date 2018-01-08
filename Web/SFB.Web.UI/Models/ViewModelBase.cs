using Newtonsoft.Json;

namespace SFB.Web.UI.Models
{
    public class ViewModelBase
    {
        [JsonIgnore]
        public ComparisonListModel ComparisonList;

        [JsonIgnore]
        public int ComparisonListCount => ComparisonList?.BenchmarkSchools?.Count ?? 0;

        [JsonIgnore]
        public string ErrorMessage { get; set; }

        public bool HasError()
        {
            return !string.IsNullOrEmpty(this.ErrorMessage);
        }
    }
}