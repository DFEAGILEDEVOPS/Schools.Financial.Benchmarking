using SFB.Web.Common;
using SFB.Web.Common.DataObjects;
using SFB.Web.Domain.Services.DataAccess;
using SFB.Web.UI.Helpers;
using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class TrustViewModel : EstablishmentViewModelBase
    {
        public int Code { get; set; }

        public string MatNo { get; set; }

        public override string Name { get; set; }

        public List<AcademiesContextualDataObject> AcademiesList {get; set;}

        public override string Type => "MAT";

        public override EstablishmentType EstablishmentType => EstablishmentType.MAT;

        public TrustViewModel(string matNo, string name)
        {
            this.MatNo = matNo;
            this.Name = name;            
        }

        public TrustViewModel(string matNo, string name, List<AcademiesContextualDataObject> academiesList, SchoolComparisonListModel comparisonList)
            : this(matNo, name)
        {            
            this.AcademiesList = academiesList;
            base.ComparisonList = comparisonList;            
        }

    }
}