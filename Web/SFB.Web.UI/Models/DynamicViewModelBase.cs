using System;
using System.Globalization;
using Microsoft.Azure.Documents;
using SFB.Web.Domain.Models;

namespace SFB.Web.UI.Models
{
    public class DynamicViewModelBase : ViewModelBase
    {
        public dynamic ContextDataModel { get; set; }

        public string GetString(string property)
        {
            try
            {
                var model = ContextDataModel as Document;
                if (model != null)
                {
                    return model.GetPropertyValue<string>(property);
                }
                    return ContextDataModel[property].ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public int GetInt(string property)
        {
            try
            {
                var model = ContextDataModel as Document;
                return model?.GetPropertyValue<int>(property) ?? int.Parse(ContextDataModel[property]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public DateTime? GetDate(string property)
        {
            try
            {
                var document = ContextDataModel as Document;
                if (document != null)
                {
                    if (document.GetPropertyValue<string>(property) == null)
                    {
                        return null;
                    }

                    return DateTime.Parse(document.GetPropertyValue<string>(property), CultureInfo.CurrentCulture, DateTimeStyles.None);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public DateTime? GetDateBinary(string property)//TODO: Remove this when date formats are fixed in DB
        {
            try
            {
                var document = ContextDataModel as Document;
                if (document != null)
                {
                    if (document.GetPropertyValue<string>(property) == null)
                    {
                        return null;
                    }

                    return DateTime.ParseExact(document.GetPropertyValue<string>(property), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);                    
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }            
        }

        public bool GetBoolean(string property)
        {
            try
            {
                if (ContextDataModel is Document)
                {
                    return (ContextDataModel as Document).GetPropertyValue<bool>(property);
                }
                else
                {
                    bool result = false;
                    if (ContextDataModel[property] == "1")
                    {
                        return true;
                    }
                    Boolean.TryParse(ContextDataModel[property], out result);
                    return result;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Location GetLocation()
        {
            if (ContextDataModel is Document)
            {
                return (ContextDataModel as Document).GetPropertyValue<Location>("Location");
            }
            else
            {
                return ContextDataModel["Location"];
            }
        }
    }
}