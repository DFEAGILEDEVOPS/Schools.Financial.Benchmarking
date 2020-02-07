﻿using System.Collections.Generic;
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
        public SubOptionsVM<BenchmarkCriteriaRangeVM> SubRangeOptions { get; set; }
        public SubOptionsVM<BenchmarkCriteriaMultipleChoiceVM> SubMultipleChoiceOptions { get; set; }

        public OptionVM(string name, string value, string[] selectedOptions)
        {
            Name = name;
            Value = value;
            Selected = (selectedOptions != null) && selectedOptions.Contains(Value);
        }

        public OptionVM(string name, string value, string[] selectedOptions, SubOptionsVM<BenchmarkCriteriaRangeVM> subRangeOptions): this(name, value, selectedOptions)
        {
            SubRangeOptions = subRangeOptions;
        }

        public OptionVM(string name, string value, string[] selectedOptions, SubOptionsVM<BenchmarkCriteriaMultipleChoiceVM> subMultipleChoiceOptions) : this(name, value, selectedOptions)
        {
            SubMultipleChoiceOptions = subMultipleChoiceOptions;
        }
    }

    public class SubOptionsVM<T>
    {
        public string Name { get; set; }
        public List<T> SubOptions { get; set; }
    }
}