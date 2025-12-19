using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Models.Request;

namespace Emerus.ETM.Admin.Services.Interfaces
{
    public interface IFileService
    {
        Task UploadAsync(FileUploadDto dto, string uploadedBy);
        Task<List<ContractorDocument>> GetDocumentByRequestIdAsync(Guid requestId);
        Task<bool> DeleteDocumentAsync(Guid documentId);
    }
}
