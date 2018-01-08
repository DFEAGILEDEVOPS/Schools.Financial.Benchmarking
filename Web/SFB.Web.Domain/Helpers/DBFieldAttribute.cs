using System;

namespace SFB.Web.Domain.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DBFieldAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Type;

        public DBFieldAttribute(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}