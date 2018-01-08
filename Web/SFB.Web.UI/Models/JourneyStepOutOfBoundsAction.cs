namespace SFB.Web.UI.Models
{
    public enum JourneyStepOutOfBoundsAction
    {
        None,
        DisableBackButton,
        RedirectToBeforeJourneyUrl,
        RedirectToErrorStep,
        ShowErrorInStep
    }
}