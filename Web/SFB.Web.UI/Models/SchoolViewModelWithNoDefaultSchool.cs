namespace SFB.Web.UI.Models
{
    public class SchoolViewModelWithNoDefaultSchool : SchoolViewModel
    {
        public SchoolViewModelWithNoDefaultSchool() : base(null)
        {
        }

        public SchoolViewModelWithNoDefaultSchool(SchoolComparisonListModel manualComparisonList) : base(null, null, manualComparisonList)
        {
        }

        public SchoolViewModelWithNoDefaultSchool(SchoolComparisonListModel schoolComparisonList, SchoolComparisonListModel manualComparisonList) : base(null, schoolComparisonList, manualComparisonList)
        {
        }

        public override string Name { get => null; }

        public override string Type => null;
    }
}