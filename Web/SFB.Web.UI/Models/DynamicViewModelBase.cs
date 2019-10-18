using System;
using System.Globalization;
using SFB.Web.ApplicationCore.Entities;

namespace SFB.Web.UI.Models
{
    public class DynamicViewModelBase : ViewModelBase
    {
        public dynamic ContextDataModel { get; set; }

        public string GetString(string property)
        {
            try
            {
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
                return int.Parse(ContextDataModel[property]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public float GetFloat(string property)
        {
            try
            {
                return float.Parse(ContextDataModel[property]);
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
                if (ContextDataModel != null)
                {
                    if (ContextDataModel[property] == null)
                    {
                        return null;
                    }

                    return DateTime.Parse(ContextDataModel[property], CultureInfo.CurrentCulture, DateTimeStyles.None);
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DateTime? GetDateBinary(string property)
        {
            try
            {
                if (ContextDataModel != null)
                {
                    if (ContextDataModel[property] == null)
                    {
                        return null;
                    }

                    return DateTime.ParseExact(ContextDataModel[property], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
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
                var result = false;
                if (ContextDataModel[property] == "1")
                {
                    return true;
                }
                Boolean.TryParse(ContextDataModel[property], out result);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public LocationDataObject GetLocation()
        {
            return ContextDataModel["Location"];
        }
    }
}