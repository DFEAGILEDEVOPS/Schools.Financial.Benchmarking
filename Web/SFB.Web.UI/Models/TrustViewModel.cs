﻿using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class TrustViewModel : EstablishmentViewModelBase
    {        
        public int Code { get; set; }

        public int CompanyNo { get; set; }

        public override string Name { get; set; }

        public List<AcademiesContextualDataObject> AcademiesList {get; set;}

        public override string Type => "MAT";

        public override EstablishmentType EstablishmentType => EstablishmentType.MAT;

        public int AcademiesContextualCount { get; set; }

        public TrustViewModel(int companyNo, string name)
        {
            this.CompanyNo = companyNo;
            this.Name = name;            
        }

        public TrustViewModel(int companyNo, string name, List<AcademiesContextualDataObject> academiesList, SchoolComparisonListModel comparisonList = null)
            : this(companyNo, name)
        {            
            this.AcademiesList = academiesList;
            base.ComparisonList = comparisonList;            
        }

    }
}