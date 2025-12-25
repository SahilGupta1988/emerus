using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emerus.ETM.Admin.Data
{
    [Table("ContractorDocument", Schema = "etm")]
    public class ContractorDocument
    {
        [Key]
        public Guid DocumentId { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        [Required, MaxLength(64)]
        public string DocumentType { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? FileName { get; set; }

        [Required, MaxLength(1024)]
        public string StorageUrl { get; set; } = string.Empty;

        [Required, MaxLength(32)]
        public string Status { get; set; } = string.Empty;

        [Required, MaxLength(256)]
        public string UploadedByUpn { get; set; } = string.Empty;

        [Required]
        public DateTime UploadedAt { get; set; }

        [MaxLength(256)]
        public string? VerifiedByUpn { get; set; }

        public DateTime? VerifiedAt { get; set; }

        [MaxLength(512)]
        public string? Notes { get; set; }
        public bool? Archived { get; set; }

        // Navigation property (FK -> etm.ContractorRequest.RequestId)
        [ForeignKey(nameof(RequestId))]
        public ContractorRequest ContractorRequest { get; set; } = null!;
    }
}