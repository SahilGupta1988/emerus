using Microsoft.AspNetCore.Components;

namespace Emerus.ETM.Admin.Pages.OnboardingMyContractors
{
    public partial class OnboardingMyContractors : ComponentBase
    {
        public record ContractorRow(Guid Id, string ContractorName, string VendorName, string Facility, DateTime StartDate, string Status);

        private List<ContractorRow> AllRows = new()
    {
        new ContractorRow(Guid.NewGuid(), "Jane Doe", "Acme Health Staffing", "Houston Neighborhood", DateTime.Today.AddDays(3), "Submitted"),
        new ContractorRow(Guid.NewGuid(), "John Smith", "Acme Health Staffing", "Mesa Neighborhood", DateTime.Today.AddDays(5), "InProgress"),
        new ContractorRow(Guid.NewGuid(), "Maria Garcia", "Acme Health Staffing", "Phoenix Neighborhood", DateTime.Today.AddDays(10), "Draft")
    };

        private string SearchText { get; set; } = string.Empty;
        private string StatusFilter { get; set; } = string.Empty;

        private IEnumerable<ContractorRow> FilteredRows =>
            AllRows
                .Where(r => string.IsNullOrWhiteSpace(SearchText)
                    || r.ContractorName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || r.VendorName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .Where(r => string.IsNullOrWhiteSpace(StatusFilter) || r.Status.Equals(StatusFilter, StringComparison.OrdinalIgnoreCase));
    }
}
