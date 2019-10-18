using System;

namespace SFB.Web.ApplicationCore.Models
{
    public class SimpleCriteria : IEquatable<SimpleCriteria>
    {
        public bool? IncludeFsm { get; set; }
        public bool? IncludeSen { get; set; }
        public bool? IncludeEal { get; set; }
        public bool? IncludeLa { get; set; }

        public bool Equals(SimpleCriteria other)
        {
            return this.IncludeFsm == other.IncludeFsm 
                && this.IncludeSen == other.IncludeSen
                && this.IncludeEal == other.IncludeEal
                && this.IncludeLa == other.IncludeLa;
        }
    }
}
