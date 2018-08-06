namespace SFB.Web.UI.Models
{
    public class SchoolNotFoundViewModel : ViewModelBase
    {
        public SchoolNotFoundViewModel()
        {
        }

        public dynamic Suggestions { get; set; }

        public string SearchKey { get; set; }
    }
}