using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Linq;
using SFB.Web.ApplicationCore.Models;
using System.Configuration;

namespace SFB.Web.UI.Models
{
    public class TrustViewModel : EstablishmentViewModelBase
    {
        private string _trustName;

        public override long Id => UID.GetValueOrDefault();

        public int Code { get; set; }

        public int CompanyNo { get; set; }

        public int? _uid;
        public int? UID
        {
            set { _uid = value; }
            get {
                if (_uid.HasValue)
                {
                    return _uid;
                }
                return LatestYearFinancialData?.FinancialDataObjectModel?.UID; 
            }
        }

        public override string Name {
            get {
                if(AcademiesInFinanceList?.Count > 0)
                {
                    return AcademiesInFinanceList.FirstOrDefault().TrustName;
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

        public List<AcademySummaryDataObject> AcademiesInFinanceList {get; set;}

        public List<EdubaseDataObject> AcademiesInContextList {get; set;}

        public override string Type => "MAT";

        public override float TotalPupils { get; }

        public override EstablishmentType EstablishmentType => EstablishmentType.MAT;

        public TrustHistoryModel TrustHistory { get; internal set; }

        public TrustViewModel(int companyNo)
        {            
            this.CompanyNo = companyNo;
            var underReviewTrusts = ConfigurationManager.AppSettings["UnderReviewTrusts"]?.Split(',').ToList() ?? new List<string>();
            this.IsUnderReview = underReviewTrusts.Contains(this.CompanyNo.ToString());
        }

        public TrustViewModel(int companyNo, string matName): this(companyNo)
        {            
            this.Name = matName;
        }

        public TrustViewModel(int uid, int companyNo): this(companyNo)
        {
            this.UID = uid;
        }

        public TrustViewModel(int companyNo, SchoolComparisonListModel comparisonList = null): this(companyNo)
        {
            base.ComparisonList = comparisonList;
        }

        public TrustViewModel(int uid, int companyNo,  SchoolComparisonListModel comparisonList = null)
            : this(uid, companyNo)
        {            
            base.ComparisonList = comparisonList;            
        }
    }
}