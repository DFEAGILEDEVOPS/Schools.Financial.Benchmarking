using SFB.Web.Common;
using SFB.Web.Common.DataObjects;
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