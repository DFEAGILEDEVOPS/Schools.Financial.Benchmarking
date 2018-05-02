using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.UI.Models
{
    public class BenchmarkCriteriaMultipleChoiceVM
    {
        public string Question { get; set; }
        public string HomeSchoolValue { get; set; }
        public string HomeSchoolName { get; set; }
        public string ElementName { get; set; }
        public List<OptionVM> Options { get; set; }

        public BenchmarkCriteriaMultipleChoiceVM(string question, string homeSchoolValue, string homeSchoolName, string elementName, List<OptionVM> options)
        {
            Question = question;
            HomeSchoolValue = homeSchoolValue;
            HomeSchoolName = homeSchoolName;
            ElementName = elementName;
            Options = options;
        }
    }

    public class OptionVM
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Selected { get; }

        public OptionVM(string name, string value, string[] selectedOptions)
        {
            Name = name;
            Value = value;
            Selected = (selectedOptions != null) && selectedOptions.Contains(Value);
        }
    }
}