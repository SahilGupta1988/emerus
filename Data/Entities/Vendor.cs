namespace Emerus.ETM.Admin.Data
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int UsersCount { get; set; }
    }

    public class ContractorRequest
    {
        public int Id { get; set; }
        public string ContractorName { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string Facility { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Reviewer { get; set; } = string.Empty;
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Column { get; set; } = string.Empty; // Pending, In Progress, Completed
        public string Meta { get; set; } = string.Empty;
    }
}