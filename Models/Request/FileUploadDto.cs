namespace Emerus.ETM.Admin.Models.Request
{
    public class FileUploadDto
    {
        public Stream FileStream { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string DocumentType { get; set; } = default!;
        public Guid ContractorId { get; set; }
    }
}
