using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using static Emerus.ETM.Admin.Pages.AccessStep.AccessStep;

namespace Emerus.ETM.Admin.Pages.ReviewStep
{
    public partial class ReviewStep : ComponentBase
    {
        //[Parameter]
        //public OnboardingNew.DemographicsModel Demographics { get; set; } = default!;

        //[Parameter]
        //public OnboardingNew.JobModel JobInfo { get; set; } = default!;

        //[Parameter]
        //public OnboardingNew.AccessModel AccessInfo { get; set; } = default!;
        //// Fixed: TargetCatalog is a collection of TargetCatalogModel (defined in AccessStep)
        //[Parameter]
        //public List<TargetCatalogModel> TargetCatalog { get; set; } = new();


        //[Parameter]
        //public OnboardingNew.HardwareModel Hardware { get; set; } = default!;

        //[Parameter]
        //public OnboardingNew.BadgeModel Badge { get; set; } = default!;






        [Parameter]
        public OnboardingNew.DemographicsModel Demographics { get; set; } = default!;

        [Parameter]
        public OnboardingNew.JobModel JobInfo { get; set; } = default!;

        [Parameter]
        public OnboardingNew.AccessModel AccessInfo { get; set; } = default!;

        // Fixed: TargetCatalog is a collection of TargetCatalogModel (defined in AccessStep)
        [Parameter]
        public List<TargetCatalogModel> TargetCatalog { get; set; } = new();

        [Parameter]
        public OnboardingNew.HardwareModel Hardware { get; set; } = default!;

        [Parameter]
        public OnboardingNew.BadgeModel Badge { get; set; } = default!;

        // Optional callback so parent page (OnboardingNew) can jump to a step when user clicks Edit
        [Parameter]
        public EventCallback<int> OnEditStep { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            // If parent didn't provide a catalog, load from iam.TargetCatalog
            if (TargetCatalog == null || !TargetCatalog.Any())
            {
                var list = new List<TargetCatalogModel>();
                DbConnection? conn = null;
                try
                {
                    conn = Db.Database.GetDbConnection();
                    await conn.OpenAsync();

                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT TargetId, DisplayName FROM iam.TargetCatalog WHERE IsActive = 1 ORDER BY DisplayName";
                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var item = new TargetCatalogModel();

                        if (!reader.IsDBNull(0))
                        {
                            item.TargetId = reader.GetGuid(0);
                        }

                        if (!reader.IsDBNull(1))
                        {
                            item.DisplayName = reader.GetString(1);
                        }

                        list.Add(item);
                    }

                    TargetCatalog = list;
                }
                catch
                {
                    // keep TargetCatalog empty on error to keep UI resilient
                    TargetCatalog = new List<TargetCatalogModel>();
                }
                finally
                {
                    if (conn is not null && conn.State == ConnectionState.Open)
                    {
                        await conn.CloseAsync();
                    }
                }
            }
        }
    }
}
