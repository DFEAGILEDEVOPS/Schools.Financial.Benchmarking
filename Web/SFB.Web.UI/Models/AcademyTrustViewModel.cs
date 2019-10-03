using SFB.Web.Common;
using SFB.Web.Common.DataObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFB.Web.UI.Models
{
    public class AcademyTrustViewModel : EstablishmentViewModelBase
    {
        public int Code { get; set; }

        public int CompanyNo { get; set; }

        public override string Name { get; set; }

        public List<SchoolViewModel> AcademiesList { get; set; }

        public Task<IEnumerable<EdubaseDataObject>> AcademiesListBuilderTask { get; }

        public override string Type => "MAT";

        public override EstablishmentType EstablishmentType => EstablishmentType.MAT;

        public AcademyTrustViewModel(int companyNo, string name)
        {
            this.CompanyNo = companyNo;
            this.Name = name;            
        }

        public AcademyTrustViewModel(int companyNo, string name, List<SchoolViewModel> academiesList, SchoolComparisonListModel comparisonList = null)
            : this(companyNo, name)
        {            
            this.AcademiesList = academiesList;
            base.ComparisonList = comparisonList;            
        }

        public AcademyTrustViewModel(int companyNo, string name, Task<IEnumerable<EdubaseDataObject>> academiesListBuilderTask, SchoolComparisonListModel comparisonList = null)
            : this(companyNo, name)
        {
            this.AcademiesListBuilderTask = academiesListBuilderTask;
            base.ComparisonList = comparisonList;
        }

    }
}