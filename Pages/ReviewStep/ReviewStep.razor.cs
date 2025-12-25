using Microsoft.AspNetCore.Components;

namespace Emerus.ETM.Admin.Pages.ReviewStep
{
    public partial class ReviewStep : ComponentBase
    {
        [Parameter]
        public OnboardingNew.DemographicsModel Demographics { get; set; } = default!;

        [Parameter]
        public OnboardingNew.JobModel JobInfo { get; set; } = default!;

        [Parameter]
        public OnboardingNew.AccessModel AccessInfo { get; set; } = default!;

        [Parameter]
        public OnboardingNew.HardwareModel Hardware { get; set; } = default!;

        [Parameter]
        public OnboardingNew.BadgeModel Badge { get; set; } = default!;
    }
}
