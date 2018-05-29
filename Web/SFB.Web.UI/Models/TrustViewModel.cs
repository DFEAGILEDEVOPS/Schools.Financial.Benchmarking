using SFB.Web.Common;

namespace SFB.Web.UI.Models
{
    public class TrustViewModel : EstablishmentViewModelBase
    {
        public int Code { get; set; }

        public string MatNo { get; set; }

        public override string Name { get; set; }

        public SchoolListViewModel SchoolList {get; set;}

        public override string Type => "MAT";

        public override EstabType EstabType => EstabType.MAT;

        public TrustViewModel(string matNo, string name)
        {
            this.MatNo = matNo;
            this.Name = name;            
        }

        public TrustViewModel(string matNo, string name, SchoolListViewModel schoolList, SchoolComparisonListModel comparisonList): this(matNo, name)
        {            
            this.SchoolList = schoolList;
            base.ComparisonList = comparisonList;            
        }
    }
}