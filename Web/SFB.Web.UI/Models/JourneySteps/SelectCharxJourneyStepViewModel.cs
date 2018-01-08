using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFB.Web.UI.Models
{
    public class SelectCharxJourneyStepViewModel : JourneyStepViewModel
    {
        public dynamic AllCharx { get; set; }
        public dynamic SelectedCharx { get; set; }
        public bool IsSimple { get; set; }
    }
}