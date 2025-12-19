using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Repositories.Interfaces;

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

        public async Task SaveDocumentAsync(ContractorDocument document)
        {
            _context.ContractorDocument.Add(document);
            await _context.SaveChangesAsync();
        }
    }
}
