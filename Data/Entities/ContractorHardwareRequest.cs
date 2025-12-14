using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emerus.ETM.Admin.Data
{
    [Table("ContractorHardwareRequest", Schema = "etm")]
    public class ContractorHardwareRequest
    {
        [Key]
        public Guid RequestId { get; set; }

        [Required]
        public bool NeedsLaptop { get; set; }

        [Required]
        public bool NeedsDock { get; set; }

        [Required]
        public int MonitorCount { get; set; }

        [Required]
        public bool NeedsHeadset { get; set; }

        [Required]
        public bool NeedsHotspot { get; set; }

        [MaxLength(512)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation property (FK -> etm.ContractorRequest.RequestId)
        public ContractorRequest ContractorRequest { get; set; } = null!;
    }
}