using Emerus.ETM.Admin.Data;

namespace Emerus.ETM.Admin.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<bool> UploadBlobAsync(string blobPath, Stream stream);
        Task SaveDocumentAsync(ContractorDocument document);
    }
}
