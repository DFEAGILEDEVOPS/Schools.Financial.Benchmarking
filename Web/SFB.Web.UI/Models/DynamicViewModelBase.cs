using System;
using System.Globalization;
using Microsoft.Azure.Documents;
using SFB.Web.Domain.Models;

namespace SFB.Web.UI.Models
{
    public class DynamicViewModelBase : ViewModelBase
    {
        public dynamic DataModel { get; set; }

        public string GetString(string property)
        {
            try
            {
                var model = DataModel as Document;
                if (model != null)
                {
                    return model.GetPropertyValue<string>(property);
                }
                    return DataModel[property].ToString();
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
                var model = DataModel as Document;
                return model?.GetPropertyValue<int>(property) ?? int.Parse(DataModel[property]);
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
                var document = DataModel as Document;
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
            catch(Exception e)
            {
                return null;
            }
        }

        public bool GetBoolean(string property)
        {
            try
            {
                if (DataModel is Document)
                {
                    return (DataModel as Document).GetPropertyValue<bool>(property);
                }
                else
                {
                    bool result = false;
                    if (DataModel[property] == "1")
                    {
                        return true;
                    }
                    Boolean.TryParse(DataModel[property], out result);
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
            if (DataModel is Document)
            {
                return (DataModel as Document).GetPropertyValue<Location>("Location");
            }
            else
            {
                return DataModel["Location"];
            }
        }
    }
}