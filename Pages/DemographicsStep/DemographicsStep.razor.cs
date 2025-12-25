using Microsoft.AspNetCore.Components;

namespace Emerus.ETM.Admin.Pages.DemographicsStep
{
    public partial class DemographicsStep : ComponentBase
    {
        [Parameter]
        public OnboardingNew.DemographicsModel Model { get; set; } = default!;
    }
}
