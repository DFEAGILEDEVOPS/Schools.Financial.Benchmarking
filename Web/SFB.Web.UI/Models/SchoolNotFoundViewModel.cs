namespace SFB.Web.UI.Models
{
    public class SchoolNotFoundViewModel : DynamicViewModelBase
    {
        public SchoolNotFoundViewModel()
        {
        }

        public dynamic Suggestions { get; set; }

        public string SearchKey { get; set; }
    }
}