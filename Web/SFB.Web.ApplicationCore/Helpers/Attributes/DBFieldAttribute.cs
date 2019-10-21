using System;

namespace SFB.Web.ApplicationCore.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DBFieldAttribute : Attribute
    {
        public readonly string DocName;
        public readonly string Name;
        public readonly string Type;

        public DBFieldAttribute(string name, string type, string docName = "")
        {
            this.Name = name;
            this.Type = type;
            this.DocName = docName;
        }
    }
}