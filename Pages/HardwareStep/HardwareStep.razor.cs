using Microsoft.AspNetCore.Components;

namespace Emerus.ETM.Admin.Pages.HardwareStep
{
    public partial class HardwareStep : ComponentBase
    {
        [Parameter]
        public OnboardingNew.HardwareModel Model { get; set; } = default!;
    }
}
