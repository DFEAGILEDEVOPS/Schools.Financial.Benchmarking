namespace SFB.Web.ApplicationCore.Models
{
    public class SchoolSearchModel
    {
        public string Urn { get; set; }

        public string EstabType { get; set; }

        public SchoolSearchModel(string urn, string estabType)
        {
            Urn = urn;
            EstabType = estabType;
        }
    }
}
