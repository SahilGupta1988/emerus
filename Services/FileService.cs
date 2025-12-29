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
            var blobPath = $"contractors/{dto.RequestId}/{dto.DocumentType}/{fileName}";

            var result = await _fileRepository.UploadBlobAsync(blobPath,dto.FileStream).ConfigureAwait(false);
            if (result)
            {
                var document = MapToContractorDocument(dto, uploadedBy, blobPath);
                await _fileRepository.SaveDocument(document).ConfigureAwait(false);
            }
        }

        public async Task<List<ContractorDocument>> GetDocumentByRequestIdAsync(Guid requestId)
        {
            if (requestId == Guid.Empty)
                return new List<ContractorDocument>();

            var results = await _fileRepository.GetDocumentsByRequestId(requestId).ConfigureAwait(false);
            return results;
        }

        public async Task<bool> DeleteDocumentAsync(Guid documentId)
        {
            if (documentId == Guid.Empty)
                return false;
            var document = await _fileRepository.GetDocumentsByDocumentId(documentId).ConfigureAwait(false);
            if(document == null || document.StorageUrl == null) return false;

            var result = await _fileRepository.DeleteFileFromBlobByPath(document.StorageUrl).ConfigureAwait(false);
            if (result)
            {
                document.Archived = true;
                return await _fileRepository.UpdateDocument(document).ConfigureAwait(false);
            }
            return false;
        }


        #region
        private static ContractorDocument MapToContractorDocument(FileUploadDto dto, string uploadedBy, string blobPath)
        {
            return new ContractorDocument
            {
                DocumentId = Guid.NewGuid(),
                //RequestId = Guid.Parse("F9853E67-DDF6-4FEE-B5A3-C75A28BCA5F0"),
                RequestId = dto.RequestId,
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
