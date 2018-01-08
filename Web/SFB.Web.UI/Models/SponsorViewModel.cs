using System;
using System.Collections.Generic;
using SFB.Web.Domain.Helpers.Enums;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class SponsorViewModel : EstablishmentViewModelBase
    {
        public int Code { get; set; }

        public string MatNo { get; set; }

        public override string Name { get; set; }

        public SchoolListViewModel SchoolList {get; set;}

        public override string Type => "MAT";

        public override SchoolFinancialType FinancialType => SchoolFinancialType.Academies;

        public SponsorViewModel(string matNo, string name, SchoolListViewModel schoolList, ComparisonListModel comparisonList)
        {
            this.MatNo = matNo;
            this.Name = name;
            this.SchoolList = schoolList;
            base.ComparisonList = comparisonList;            
        }
    }
}