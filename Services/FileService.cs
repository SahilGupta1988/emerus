using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Models.Request;
using Emerus.ETM.Admin.Repositories.Interfaces;
using Emerus.ETM.Admin.Services.Interfaces;

namespace Emerus.ETM.Admin.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository,IConfiguration config)
        {
            _fileRepository = fileRepository;
        }

        public async Task UploadAsync(FileUploadDto dto, string uploadedBy)
        {
            if (dto.FileStream == null || dto.FileStream.Length == 0)
                throw new Exception("Invalid file");

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.FileName)}";
            var blobPath = $"contractors/{dto.ContractorId}/{dto.DocumentType}/{fileName}";

            var result = await _fileRepository.UploadBlobAsync(blobPath,dto.FileStream).ConfigureAwait(false);
            if (result)
            {
                var document = MapToContractorDocument(dto, uploadedBy, blobPath);
                await _fileRepository.SaveDocumentAsync(document).ConfigureAwait(false);
            }
        }


        #region
        private static ContractorDocument MapToContractorDocument(FileUploadDto dto, string uploadedBy, string blobPath)
        {
            return new ContractorDocument
            {
                DocumentId = Guid.NewGuid(),
                RequestId = Guid.Parse("F9853E67-DDF6-4FEE-B5A3-C75A28BCA5F0"),
                DocumentType = dto.DocumentType,
                FileName = dto.FileName,
                StorageUrl = blobPath,
                Status = "Uploaded",
                UploadedByUpn = uploadedBy,
                UploadedAt = DateTime.UtcNow
            };
        }

        #endregion
    }
}
