using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emerus.ETM.Admin.Data
{
    [Table("ContractorAccessRequest", Schema = "etm")]
    public class ContractorAccessRequest
    {
        [Key]
        public Guid AccessRequestId { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        [Required]
        public Guid TargetId { get; set; }

        [Required, MaxLength(32)]
        public string Environment { get; set; } = string.Empty;

        [Required, MaxLength(32)]
        public string ChangeType { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? MirrorUserUpn { get; set; }

        // JSON payload; nvarchar(max) -> use nullable string
        public string? DetailJson { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [MaxLength(256)]
        public string? CreatedByUpn { get; set; }

        // Navigation properties
        [ForeignKey(nameof(RequestId))]
        public ContractorRequest ContractorRequest { get; set; } = null!;

        //[ForeignKey(nameof(TargetId))]
        //public TargetCatalog Target { get; set; } = null!;
    }
}