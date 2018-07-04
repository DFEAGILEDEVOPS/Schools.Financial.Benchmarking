using SFB.Web.Common;
using SFB.Web.Domain.Models;
using System.Reflection;
using System.Text;

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

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (PropertyInfo pi in AdvancedCriteria.GetType().GetProperties())
            {
                var name = pi.Name;
                var value = pi.GetValue(AdvancedCriteria);
                if (value != null)
                {
                    sb.Append($"{name}:{value},");
                }
            }

            return sb.ToString();
        }
    }
}