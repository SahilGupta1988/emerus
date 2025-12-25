using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Emerus.ETM.Admin.Pages.AccessStep
{
    public partial class AccessStep : ComponentBase
    {
        [Parameter]
        public OnboardingNew.AccessModel Model { get; set; } = default!;

        private List<TargetCatalogModel> AvailableTargets { get; set; } = new();

        private HashSet<Guid> SelectedAvailableTargets { get; } = new();
        private HashSet<Guid> SelectedRequestedTargets { get; } = new();

        private string FilterText { get; set; } = string.Empty;
        private IEnumerable<TargetCatalogModel> FilteredAvailableTargets =>
            string.IsNullOrWhiteSpace(FilterText)
                ? AvailableTargets
                : AvailableTargets.Where(t => t.DisplayName.Contains(FilterText, StringComparison.OrdinalIgnoreCase) || t.TargetId.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase));

        protected override async Task OnInitializedAsync()
        {
            await LoadAvailableTargetsAsync();
        }

        private async Task LoadAvailableTargetsAsync()
        {
            DbConnection? conn = null;
            try
            {
                conn = Db.Database.GetDbConnection();
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                // Query full rows (TargetId + DisplayName) from iam.TargetCatalog; restrict to active rows
                cmd.CommandText = "SELECT TargetId, DisplayName FROM iam.TargetCatalog WHERE IsActive = 1 ORDER BY DisplayName";
                using var reader = await cmd.ExecuteReaderAsync();

                var list = new List<TargetCatalogModel>();
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

                AvailableTargets = list;
            }
            catch (Exception)
            {
                // If DB call fails, do NOT populate sample values — leave the list empty so UI shows nothing
                AvailableTargets = new List<TargetCatalogModel>();
            }
            finally
            {
                if (conn is not null && conn.State == ConnectionState.Open)
                {
                    await conn.CloseAsync();
                }

                StateHasChanged();
            }
        }

        private void ToggleAvailableSelection(Guid id)
        {
            if (!SelectedAvailableTargets.Remove(id))
            {
                SelectedAvailableTargets.Add(id);
            }
        }

        private void ToggleRequestedSelection(Guid id)
        {
            if (!SelectedRequestedTargets.Remove(id))
            {
                SelectedRequestedTargets.Add(id);
            }
        }

        private void AddSelected()
        {
            foreach (var id in SelectedAvailableTargets.ToList())
            {
                if (!Model.RequestedTargets.Any(r => r.TargetId == id))
                {
                    Model.RequestedTargets.Add(new OnboardingNew.RequestTargetModel
                    {
                        AccessRequestId = Guid.NewGuid(),
                        RequestId = Guid.Empty,
                        TargetId = id
                    });
                }
                SelectedAvailableTargets.Remove(id);
            }
            StateHasChanged();
        }

        private void AddAll()
        {
            foreach (var t in AvailableTargets)
            {
                if (!Model.RequestedTargets.Any(r => r.TargetId == t.TargetId))
                {
                    Model.RequestedTargets.Add(new OnboardingNew.RequestTargetModel
                    {
                        AccessRequestId = Guid.NewGuid(),
                        RequestId = Guid.Empty,
                        TargetId = t.TargetId
                    });
                }
            }
            SelectedAvailableTargets.Clear();
            StateHasChanged();
        }

        private void RemoveSelected()
        {
            foreach (var id in SelectedRequestedTargets.ToList())
            {
                var existing = Model.RequestedTargets.FirstOrDefault(r => r.TargetId == id);
                if (existing is not null)
                {
                    Model.RequestedTargets.Remove(existing);
                }
                SelectedRequestedTargets.Remove(id);
            }
            StateHasChanged();
        }

        private void RemoveAll()
        {
            Model.RequestedTargets.Clear();
            SelectedRequestedTargets.Clear();
            StateHasChanged();
        }

        private void ClearFilter() => FilterText = string.Empty;

        public class TargetCatalogModel
        {
            public Guid TargetId { get; set; }
            public string DisplayName { get; set; } = string.Empty;
        }
    }
}
