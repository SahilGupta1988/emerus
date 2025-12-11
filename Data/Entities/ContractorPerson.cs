using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Emerus.ETM.Admin.Data
{

    [Table("ContractorPerson", Schema = "etm")]
    public class ContractorPerson
    {
        [Key]
        public Guid RequestId { get; set; } // if this is still the PK for another purpose, keep it

        [Required, MaxLength(128)]
        public string FirstName { get; set; }

        [MaxLength(128)]
        public string MiddleName { get; set; }

        [Required, MaxLength(128)]
        public string LastName { get; set; }

        [MaxLength(128)]
        public string PreferredFirstName { get; set; }

        [MaxLength(128)]
        public string PreferredLastName { get; set; }

        [Required, MaxLength(256)]
        public string PersonalEmail { get; set; }

        [MaxLength(32)]
        public string CellPhone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(32)]
        public string Gender { get; set; }

        [MaxLength(256)]
        public string AddressLine1 { get; set; }

        [MaxLength(256)]
        public string AddressLine2 { get; set; }

        [MaxLength(128)]
        public string City { get; set; }

        [MaxLength(64)]
        public string StateProvince { get; set; }

        [MaxLength(16)]
        public string PostalCode { get; set; }

        [MaxLength(2)]
        public string CountryCode { get; set; }

        [StringLength(4)]
        public string SsnLast4 { get; set; }

        public byte[] FullSsnEncrypted { get; set; }

        [MaxLength(64)]
        public string ExternalEmployeeId { get; set; }

        [MaxLength(256)]
        public string AgencyName { get; set; }

        [MaxLength(256)]
        public string JobTitle { get; set; }

        [MaxLength(64)]
        public string JobCode { get; set; }

        [MaxLength(256)]
        public string Department { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public bool PriorEmployeeFlag { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // Navigation Property
        public ContractorRequest ContractorRequest { get; set; }
    }
}