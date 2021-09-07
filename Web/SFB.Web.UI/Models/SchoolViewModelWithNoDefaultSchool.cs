using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.UI.Models
{
    public class SchoolViewModelWithNoDefaultSchool : SchoolViewModel
    {
        public SchoolViewModelWithNoDefaultSchool() : base((EdubaseDataObject)null)
        {
        }

        public SchoolViewModelWithNoDefaultSchool(SchoolComparisonListModel manualComparisonList) : base(null, null, manualComparisonList)
        {
        }

        public SchoolViewModelWithNoDefaultSchool(SchoolComparisonListModel schoolComparisonList, SchoolComparisonListModel manualComparisonList) : base(null, schoolComparisonList, manualComparisonList)
        {
        }

        public override string Name => null;

        public override string Type => null;
    }
}