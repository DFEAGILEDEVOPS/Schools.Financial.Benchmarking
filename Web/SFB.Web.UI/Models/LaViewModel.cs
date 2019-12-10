namespace SFB.Web.UI.Models
{
    public class LaViewModel : ViewModelBase
    {
        public LaViewModel(string id, string laName)
        {
            Id = id;
            LaName = laName;
        }

        public string Id { get; set; }

        public string LaName { get; set; }
    }
}