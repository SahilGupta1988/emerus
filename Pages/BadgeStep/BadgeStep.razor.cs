using Microsoft.AspNetCore.Components;

namespace Emerus.ETM.Admin.Pages.BadgeStep
{
    public partial class BadgeStep : ComponentBase
    {
        [Parameter]
        public OnboardingNew.BadgeModel Model { get; set; } = default!;
    }
}
