using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emerus.ETM.Admin.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Emerus.ETM.Admin.Pages.OnboardingMyContractors
{
    public partial class OnboardingMyContractors : ComponentBase
    {
        public record ContractorRow(Guid Id, string ContractorName, string VendorName, string Facility, DateTime StartDate, string Status, DateTime CreatedAt, string? Comments);

        // populated from database on initialization
        private List<ContractorRow> AllRows { get; set; } = new();

        // UI state
        private string SearchText { get; set; } = string.Empty;
        private int PageSize { get; set; } = 10;
        private int CurrentPage { get; set; } = 1;

        [Inject]
        private AppDbContext DbContext { get; set; } = null!;

        //[Inject]
        //private NavigationManager Navigation { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            // Query recent contractor requests, include related person and partner.
            // Map fields safely with fallbacks so the UI always has values.
            AllRows = await DbContext.ContractorRequests
                .Include(r => r.ContractorPerson)
                .Include(r => r.Partner)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ContractorRow(
                    r.RequestId,
                    (r.ContractorPerson != null
                        ? ((string.IsNullOrWhiteSpace(r.ContractorPerson.PreferredFirstName) ? r.ContractorPerson.FirstName : r.ContractorPerson.PreferredFirstName) + " " + r.ContractorPerson.LastName)
                        : "Unknown"),
                    (r.Partner != null ? r.Partner.DisplayName : "Unknown"),
                    (r.PrimaryFacility ?? string.Empty),
                    (r.ContractorPerson != null
                        ? (r.ContractorPerson.StartDate ?? r.CreatedAt)
                        : r.CreatedAt),
                    r.Status,
                    r.CreatedAt,
                    r.Comments ?? string.Empty
                ))
                .ToListAsync();
        }

        private static string GetStatusBadgeClass(string? status)
        {
            var s = (status ?? string.Empty).Trim();

            return s switch
            {
                "Approved" => "bg-success",
                "Cancelled" => "bg-danger",
                "Draft" => "bg-secondary",
                "Completed" => "bg-primary",
                "Submitted" => "bg-info",
                "In Progress" => "bg-info",
                "Returned" => "bg-warning",
                _ => "bg-secondary"
            };
        }

        private static bool IsFinalStatus(string? status)
            => string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase)
               || string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase);

        private int FilteredCount => FilteredRows.Count();

        private int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredCount / (double)PageSize));

        private IEnumerable<ContractorRow> FilteredRows => AllRows
            .Where(r =>
                string.IsNullOrWhiteSpace(SearchText)
                    || r.ContractorName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || r.VendorName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || r.Facility.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    || (r.Status ?? string.Empty).Contains(SearchText, StringComparison.OrdinalIgnoreCase)
            );

        private IEnumerable<ContractorRow> PagedRows
        {
            get
            {
                if (CurrentPage > TotalPages) CurrentPage = TotalPages;
                return FilteredRows.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
            }
        }

        private void SetPageSize(int size)
        {
            PageSize = size;
            CurrentPage = 1;
        }

        private void PrevPage()
        {
            if (CurrentPage > 1) CurrentPage--;
        }

        private void NextPage()
        {
            if (CurrentPage < TotalPages) CurrentPage++;
        }

        private void EditDraft(Guid id)
        {
            Navigation.NavigateTo($"/onboarding/new?editId={id}");
        }

        private static string Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Length <= maxLength) return value;
            return value.Substring(0, maxLength - 1).TrimEnd() + "…";
        }
    }
}
