using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class SelectSchoolTypeJourneyStepViewModel : JourneyStepViewModel
    {
        public SearchTypes SchoolType { get; set; }
    }
}