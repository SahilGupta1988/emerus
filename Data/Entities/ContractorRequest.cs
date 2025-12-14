
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emerus.ETM.Admin.Data
{

    [Table("ContractorRequest", Schema = "etm")]
    public class ContractorRequest
    {
        [Key]
        public Guid RequestId { get; set; }

        [Required, MaxLength(256)]
        public string RequestedByUpn { get; set; }

        [Required, MaxLength(32)]
        public string PartnerCode { get; set; }

        [MaxLength(128)]
        public string PrimaryFacility { get; set; }

        [Required, MaxLength(32)]
        public string RequestType { get; set; }

        [Required, MaxLength(32)]
        public string Status { get; set; }

        public Guid? UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        // ===========================
        // Navigation Properties
        // ===========================

        // FK -> etm.Partner (PartnerCode)
        public Partner Partner { get; set; }

        // FK -> iam.Users (UserId)
        public User User { get; set; }

        // One-to-One with ContractorPerson
        public ContractorPerson ContractorPerson { get; set; }


        // One-to-One with ContractorHardwareRequest
        public ContractorHardwareRequest ContractorHardwareRequest { get; set; }

    }
}