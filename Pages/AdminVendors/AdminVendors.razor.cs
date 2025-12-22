using Emerus.ETM.Admin.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Emerus.ETM.Admin.Pages.AdminVendors
{
    public partial class AdminVendors : ComponentBase
    {
        private List<Partner>? Partners;

        protected override async Task OnInitializedAsync()
        {
            //Vendors = await Db.Vendors.AsNoTracking().ToListAsync();
            Partners = await Db.Partners
            .AsNoTracking()
            .OrderBy(p => p.DisplayName)
            .ToListAsync();

        }
    }
}
