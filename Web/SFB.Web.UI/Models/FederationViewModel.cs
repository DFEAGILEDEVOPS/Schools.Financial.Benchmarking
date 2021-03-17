using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;
using SFB.Web.ApplicationCore.Models;

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
                if (AcademiesInFinanceList?.Count > 0)
                {
                    return AcademiesInFinanceList.FirstOrDefault().TrustName;
                }
                else if (LatestYearFinancialData?.FinancialDataObjectModel != null)
                {
                    return LatestYearFinancialData.FinancialDataObjectModel.TrustOrCompanyName;
                }
                return _federationName;
            }
            set
            {
                _federationName = value;
            }
        }

        public List<AcademySummaryDataObject> AcademiesInFinanceList { get; set; }

        public List<EdubaseDataObject> AcademiesInContextList { get; set; }

        public override string Type => "MAT";

        public override EstablishmentType EstablishmentType => EstablishmentType.MAT;

        public TrustHistoryModel TrustHistory { get; internal set; }

        public TrustViewModel(int companyNo)
        {
            this.CompanyNo = companyNo;
        }

        public TrustViewModel(int companyNo, string matName)
        {
            this.CompanyNo = companyNo;
            this.Name = matName;
        }

        public TrustViewModel(int uid, int companyNo) :
            this(companyNo)
        {
            this.UID = uid;
        }

        public TrustViewModel(int companyNo, SchoolComparisonListModel comparisonList = null)
        : this(companyNo)
        {
            base.ComparisonList = comparisonList;
        }

        public TrustViewModel(int uid, int companyNo, SchoolComparisonListModel comparisonList = null)
            : this(uid, companyNo)
        {
            base.ComparisonList = comparisonList;
        }

    }
}