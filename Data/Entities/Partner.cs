using System;

namespace Emerus.ETM.Admin.Data
{
    public class Partner
    {
        // Maps to etm.Partner.PartnerCode (PK)
        public string PartnerCode { get; set; } = string.Empty;

        // Maps to etm.Partner.DisplayName
        public string DisplayName { get; set; } = string.Empty;

        // Maps to etm.Partner.Description (nullable)
        public string? Description { get; set; }

        // Maps to etm.Partner.IsActive
        public bool IsActive { get; set; }

        // Maps to etm.Partner.CreatedAt
        public DateTime CreatedAt { get; set; }

        // Maps to etm.Partner.UpdatedAt
        public DateTime UpdatedAt { get; set; }
    }
}