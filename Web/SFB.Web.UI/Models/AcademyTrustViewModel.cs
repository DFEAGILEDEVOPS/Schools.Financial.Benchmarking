using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFB.Web.UI.Models
{
    public class AcademyTrustViewModel : EstablishmentViewModelBase
    {
        public override long Id => Uid;

        public int Code { get; set; }

        public int CompanyNo { get; set; }
        
        public int Uid { get; set; }

        public override string Name { get; set; }

        public List<SchoolViewModel> AcademiesList { get; set; }

        public Task<IEnumerable<EdubaseDataObject>> AcademiesListBuilderTask { get; }

        public override string Type => "MAT";

        public override EstablishmentType EstablishmentType => EstablishmentType.MAT;

        public override float TotalPupils { get; }

        public AcademyTrustViewModel(int uid, int companyNo, string name)
        {
            this.Uid = uid;
            this.CompanyNo = companyNo;
            this.Name = name;            
        }

        public AcademyTrustViewModel(int uid, int companyNo, string name, List<SchoolViewModel> academiesList, SchoolComparisonListModel comparisonList = null)
            : this(uid, companyNo, name)
        {            
            this.AcademiesList = academiesList;
            base.ComparisonList = comparisonList;            
        }

        public AcademyTrustViewModel(int uid, int companyNo, string name, Task<IEnumerable<EdubaseDataObject>> academiesListBuilderTask, SchoolComparisonListModel comparisonList = null)
            : this(uid, companyNo, name)
        {
            this.AcademiesListBuilderTask = academiesListBuilderTask;
            base.ComparisonList = comparisonList;
        }

    }
}