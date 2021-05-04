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
        public override long Id => UID.GetValueOrDefault();

        public long? _uid;
        public long? UID
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

        public long[] FederationMembersURNs => ContextData.FederationMembers;

        public string OpenDate
        {
            get
            {
                try
                {
                    return DateTime.Parse(ContextData.OpenDate, CultureInfo.CurrentCulture, DateTimeStyles.None).ToLongDateString();
                }
                catch
                {
                    return null;
                }
            }
        }

        public string OverallPhase => ContextData.OverallPhase;

        public string Phase => ContextData.PhaseOfEducation;

        public override float TotalPupils => ContextData.NumberOfPupils.GetValueOrDefault();

        public string HasNursery => ContextData.NurseryProvision == "Has Nursery Classes" ? "Yes" : "No";

        public string Has6Form => ContextData.OfficialSixthForm == "Has a sixth form" ? "Yes" : "No";

        public new int?  La => ContextData?.LACode;

        public string AgeRange => this.LowestAge == null ? null : $"{this.LowestAge} to {this.HighestAge}";

        public List<EdubaseDataObject> SchoolsInFederation { get; set; }

        private decimal? LowestAge => ContextData?.StatutoryLowAge;

        private decimal? HighestAge => ContextData?.StatutoryHighAge;

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

        public FederationViewModel(long uid)
        {
            this.UID = uid;
        }

        public FederationViewModel(EdubaseDataObject contextDataModel)
        {
            this.ContextData = contextDataModel;
        }

        public FederationViewModel(long uid, SchoolComparisonListModel comparisonList = null)
        : this(uid)
        {
            base.ComparisonList = comparisonList;
        }

        public FederationViewModel(EdubaseDataObject schoolContextDataModel, SchoolComparisonListModel comparisonList) : this(schoolContextDataModel)
        {
            base.ComparisonList = comparisonList;
        }

    }
}