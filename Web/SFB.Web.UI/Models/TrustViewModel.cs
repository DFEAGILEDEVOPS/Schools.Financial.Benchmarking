using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SFB.Web.UI.Models
{
    public class TrustViewModel : EstablishmentViewModelBase
    {
        private string _trustName;

        public int Code { get; set; }

        public int CompanyNo { get; set; }

        public override string Name {
            get {
                if(AcademiesList?.Count > 0)
                {
                    return AcademiesList.FirstOrDefault().TrustName;
                }
                else if (LatestYearFinancialData?.FinancialDataObjectModel != null)
                {
                    return LatestYearFinancialData.FinancialDataObjectModel.TrustOrCompanyName;
                }
                return _trustName;
            }
            set
            {
                _trustName = value;
            }
        }

        public List<AcademiesContextualDataObject> AcademiesList {get; set;}

        public override string Type => "MAT";

        public override EstablishmentType EstablishmentType => EstablishmentType.MAT;

        public int AcademiesContextualCount { get; set; }

        public TrustViewModel(int companyNo)
        {
            this.CompanyNo = companyNo;
        }

        public TrustViewModel(int companyNo, List<AcademiesContextualDataObject> academiesList, SchoolComparisonListModel comparisonList = null)
            : this(companyNo)
        {            
            this.AcademiesList = academiesList;
            base.ComparisonList = comparisonList;            
        }

    }
}