using SFB.Web.Common.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFB.Web.UI.Models
{
    public class DataObjectViewModelBase : ViewModelBase
    {
        protected EdubaseDataObject ContextDataModel { get; set; }

        protected Location GetLocation()
        {
            return ContextDataModel.Location;
        }
    }
}