using Microsoft.AspNetCore.Components;

namespace Emerus.ETM.Admin.Pages.JobStep
{
    public partial class JobStep : ComponentBase
    {
        [Parameter]
        public OnboardingNew.JobModel Model { get; set; } = default!;
    }
}
