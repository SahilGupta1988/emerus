
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emerus.ETM.Admin.Data
{

    [Table("Users", Schema = "iam")]
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required, MaxLength(256)]
        public string UPN { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation: A user may have multiple contractor requests
        public ICollection<ContractorRequest> ContractorRequests { get; set; }
    }
}