using Emerus.ETM.Admin.Data;

namespace Emerus.ETM.Admin.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<bool> UploadBlobAsync(string blobPath, Stream stream);
        Task SaveDocument(ContractorDocument document);
        Task<List<ContractorDocument>> GetDocumentsByRequestId(Guid requestId);
        Task<ContractorDocument> GetDocumentsByDocumentId(Guid documentId);
        Task<bool> UpdateDocument(ContractorDocument document);
        Task<bool> DeleteFileFromBlobByPath(string blobPath);
    }
}
