
using System;
using System.Linq;
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Services
{
    public abstract class JourneyService : IJourneyService
    {
        public virtual JourneyStepViewModel BeginJourney(Journey journey)
        {
            journey.CurrentStepPos = 1;
            return journey.Steps.SingleOrDefault(w => w.Position == 1);
        }

        public virtual Journey InitJourney(string name)
        {
            return new Journey
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                StepTemplateView = "JourneyStep",
                AfterJourneyUrlFailure = string.Empty,
                AfterJourneyUrlSuccess = string.Empty,
                BackText = "< Back",
                BeforeJourneyUrl = "/BenchmarkCriteria/ComparisonStrategy?urn={0}",
                CurrentStepPos = 0,
                DefaultController = "BenchmarkCriteria",
                FinishText = "Finish",
                NextText = "Next"
            };
        }

        public virtual JourneyStepViewModel EndJourney(Journey journey)
        {
            journey.Steps = null;
            return null;
        }

        public JourneyStepViewModel TakeJourneyStep(Journey journey, bool forward)
        {
            var nextPos = forward ? journey.CurrentStepPos + 1 : journey.CurrentStepPos - 1;
            if (nextPos < 1 || nextPos > journey.Steps.Count)
            {
                return null;
            }

            journey.CurrentStepPos = nextPos;
            return journey.Steps.SingleOrDefault(sod => sod.Position == nextPos);
        }

    }
}