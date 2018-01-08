using System;

namespace SFB.Web.Domain.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrettyNameAttribute : Attribute
    {
        public readonly string Name;

        public PrettyNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}