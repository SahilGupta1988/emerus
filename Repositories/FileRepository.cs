using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Emerus.ETM.Admin.Repositories
{
    public class FileRepository: IFileRepository
    {
        private readonly AppDbContext _context;
        private readonly BlobContainerClient _container;
        public FileRepository(AppDbContext context, IConfiguration config, SecretClient secretClient)
        {
            _context = context;
            var connectionString = secretClient.GetSecret("dev--Storage--ConnectionString").Value.Value;
            var containerName = secretClient.GetSecret("dev--Storage--ContainerName").Value.Value;
            var blobServiceClient = new BlobServiceClient(connectionString);
            _container = blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<bool> UploadBlobAsync(string blobPath, Stream stream)
        {
            var blobClient = _container.GetBlobClient(blobPath);
            await blobClient.UploadAsync(stream, overwrite: true);
            return true;
        }

        public async Task SaveDocument(ContractorDocument document)
        {
            _context.ContractorDocument.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ContractorDocument>> GetDocumentsByRequestId(Guid requestId)
        {
            return await _context.ContractorDocument.Where(x => x.RequestId == requestId && x.Archived != true).ToListAsync().ConfigureAwait(false);
        }

        public async Task<ContractorDocument> GetDocumentsByDocumentId(Guid documentId)
        {
            return await _context.ContractorDocument.Where(x => x.DocumentId == documentId).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<bool> UpdateDocument(ContractorDocument document)
        {
            _context.ContractorDocument.Update(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFileFromBlobByPath(string blobPath)
        {
            var blobClient = _container.GetBlobClient(blobPath);
            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }
    }
}
