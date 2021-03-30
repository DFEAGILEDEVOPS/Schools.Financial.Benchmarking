using SFB.Web.ApplicationCore.Entities;
using SFB.Web.ApplicationCore.Helpers.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SFB.Web.UI.Models
{
    public class FederationViewModel : EstablishmentViewModelBase
    {
        public int Id => UID.GetValueOrDefault();

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
                return ContextData.FederationUid;
            }
        }

        private string _federationName;
        public override string Name
        {
            set { _federationName = value; }
            get
            {
                if (!string.IsNullOrEmpty(_federationName))
                {
                    return _federationName;
                }
                return ContextData.FederationName;
            }
        }

        public override string Type => "Federation";

        public override EstablishmentType EstablishmentType => EstablishmentType.Federation;

        public int[] FederationMembersURNs => ContextData.FederationMembers;

        public string OpenDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(LatestYearFinancialData.OpenDate, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public decimal? PupilCount => LatestYearFinancialData?.PupilCount;

        public string OverallPhase => LatestYearFinancialData.SchoolOverallPhase;

        public string HasNursery => (bool)LatestYearFinancialData?.HasNursery ? "Yes" : "No";

        public string Has6Form => (bool)LatestYearFinancialData?.Has6Form ? "Yes" : "No";

        public int?  La => LatestYearFinancialData?.LaNumber;
        public string LaName { get; set; }
        public string AgeRange => this.LowestAge == null ? null : $"{this.LowestAge} to {this.HighestAge}";
        public List<EdubaseDataObject> SchoolsInFederation { get; set; }
        private decimal? LowestAge => LatestYearFinancialData?.LowestAgePupils;
        private decimal? HighestAge => LatestYearFinancialData?.HighestAgePupils;

        public bool IsInComparisonList
        {
            get
            {
                if (ComparisonList == null)
                {
                    return false;
                }
                return base.ComparisonList.BenchmarkSchools.Any(s => s.Urn == UID.ToString());
            }
        }

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