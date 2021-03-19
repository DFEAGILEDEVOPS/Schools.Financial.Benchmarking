using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Helpers.Enums;
using System.Collections.Generic;

namespace SFB.Web.UI.Models
{
    public class FederationViewModel : EstablishmentViewModelBase
    {
        private string _federationName;

        public int? _uid;
        public int? UID
        {
            set { _uid = value; }
            get
            {
                if (_uid.HasValue)
                {
                    return _uid;
                }
                return LatestYearFinancialData?.FinancialDataObjectModel?.FederationUid;
            }
        }

        public override string Name
        {
            get
            {
                return _federationName;
            }
            set
            {
                _federationName = value;
            }
        }

        public override string Type => "Federation";

        public override EstablishmentType EstablishmentType => EstablishmentType.Federation;

        public List<EdubaseDataObject> SchoolsInFederation { get; set; }

        public string OpenDate => LatestYearFinancialData?.OpenDate;

        public decimal? PupilCount => LatestYearFinancialData?.PupilCount;

        public string OverallPhase => LatestYearFinancialData.SchoolOverallPhase;

        public bool? HasNursery => LatestYearFinancialData?.HasNursery;

        public bool? Has6Form => LatestYearFinancialData?.Has6Form;

        public int?  LA => LatestYearFinancialData?.LaNumber;
        public decimal? LowestAge => LatestYearFinancialData?.LowestAgePupils;
        public decimal? HighestAge => LatestYearFinancialData?.HighestAgePupils;


        public FederationViewModel(int uid)
        {
            this.UID = uid;
        }

        public FederationViewModel(int uid, SchoolComparisonListModel comparisonList = null)
        : this(uid)
        {
            base.ComparisonList = comparisonList;
        }
    }
}