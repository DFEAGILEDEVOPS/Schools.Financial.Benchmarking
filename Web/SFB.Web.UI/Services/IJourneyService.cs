
using SFB.Web.UI.Models;

namespace SFB.Web.UI.Services
{
    public interface IJourneyService
    {
        Journey InitJourney(string name);
        JourneyStepViewModel TakeJourneyStep(Journey journey, bool forward);
        JourneyStepViewModel BeginJourney(Journey journey);
        JourneyStepViewModel EndJourney(Journey journey);
    }
}