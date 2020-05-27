namespace SFB.Web.UI.Models
{
    public abstract class CompareEntityBase
    {
        public abstract string Id { get; }

        //public abstract string ShortName { get; }

        public abstract string Name { get; set; }

        public abstract string Type { get; set; }
    }
}