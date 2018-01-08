namespace SFB.Web.UI.Models
{
    public class Filter
    {
        public string Id { get; set; }
        public string Group { get; set; }
        public string Label { get; set; }
        public bool Expanded { get; set; }
        public OptionSelect[] Metadata { get; set; }
    }
}