using System;

namespace SFB.Web.Common.Attributes
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