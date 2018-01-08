using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFB.Web.UI.Helpers.Enums;

namespace SFB.Web.UI.Models
{
    public class Journey
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string StepTemplateView { get; set; }
        public string BackText { get; set; }
        public string NextText { get; set; }
        public string FinishText { get; set; }
        public int CurrentStepPos { get; set; }
        public List<JourneyStepViewModel> Steps { get; set; }
        public string DefaultController { get; set; }
        public string BeforeJourneyUrl { get; set; }
        public string AfterJourneyUrlSuccess { get; set; }
        public string AfterJourneyUrlFailure { get; set; }
        public JourneyStepOutOfBoundsAction OutOfBoundsAction { get; set; }
    }
}