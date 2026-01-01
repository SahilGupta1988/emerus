using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Models.Request;
using Emerus.ETM.Admin.Repositories.Interfaces;
using Emerus.ETM.Admin.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Emerus.ETM.Admin.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<FileService> _logger;

        public FileService(IFileRepository fileRepository, IConfiguration config, ILogger<FileService> logger)
        {
            _fileRepository = fileRepository;
            _logger = logger;
        }

        public async Task UploadAsync(FileUploadDto dto, string uploadedBy)
        {
            _logger?.LogDebug("UploadAsync called for RequestId={RequestId} FileName={FileName} DocumentType={DocumentType}", dto.RequestId, dto.FileName, dto.DocumentType);

            if (dto.FileStream == null || dto.FileStream.Length == 0)
            {
                _logger?.LogWarning("UploadAsync received invalid file for RequestId={RequestId}", dto.RequestId);
                throw new Exception("Invalid file");
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.FileName)}";
            var blobPath = $"contractors/{dto.RequestId}/{dto.DocumentType}/{fileName}";

            try
            {
                var result = await _fileRepository.UploadBlobAsync(blobPath, dto.FileStream).ConfigureAwait(false);
                if (result)
                {
                    var document = MapToContractorDocument(dto, uploadedBy, blobPath);
                    await _fileRepository.SaveDocument(document).ConfigureAwait(false);
                    _logger?.LogInformation("File uploaded and document saved: DocumentId={DocumentId} RequestId={RequestId}", document.DocumentId, dto.RequestId);
                }
                else
                {
                    _logger?.LogWarning("UploadBlobAsync returned false for path {BlobPath}", blobPath);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "UploadAsync failed for RequestId={RequestId}", dto.RequestId);
                throw;
            }
        }

        public async Task<List<ContractorDocument>> GetDocumentByRequestIdAsync(Guid requestId)
        {
            _logger?.LogDebug("GetDocumentByRequestIdAsync called for RequestId={RequestId}", requestId);

            if (requestId == Guid.Empty)
            {
                _logger?.LogWarning("GetDocumentByRequestIdAsync called with empty RequestId");
                return new List<ContractorDocument>();
            }

            try
            {
                var results = await _fileRepository.GetDocumentsByRequestId(requestId).ConfigureAwait(false);
                _logger?.LogDebug("Retrieved {Count} documents for RequestId={RequestId}", results?.Count ?? 0, requestId);
                return results;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving documents for RequestId={RequestId}", requestId);
                return new List<ContractorDocument>();
            }
        }

        public async Task<bool> DeleteDocumentAsync(Guid documentId)
        {
            _logger?.LogDebug("DeleteDocumentAsync called for DocumentId={DocumentId}", documentId);

            if (documentId == Guid.Empty)
            {
                _logger?.LogWarning("DeleteDocumentAsync called with empty DocumentId");
                return false;
            }
            try
            {
                var document = await _file_repository_GetDocumentsByDocumentId(documentId).ConfigureAwait(false);
                if (document == null || document.StorageUrl == null)
                {
                    _logger?.LogWarning("DeleteDocumentAsync: document not found or missing StorageUrl for DocumentId={DocumentId}", documentId);
                    return false;
                }

                var result = await _fileRepository.DeleteFileFromBlobByPath(document.StorageUrl).ConfigureAwait(false);
                if (result)
                {
                    document.Archived = true;
                    var updated = await _fileRepository.UpdateDocument(document).ConfigureAwait(false);
                    _logger?.LogInformation("Document {DocumentId} archived: UpdateResult={UpdateResult}", documentId, updated);
                    return updated;
                }

                _logger?.LogWarning("DeleteFileFromBlobByPath returned false for path {StorageUrl}", document.StorageUrl);
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting document {DocumentId}", documentId);
                return false;
            }
        }

        // Added: update document status / verification metadata
        public async Task<bool> UpdateDocumentStatusAsync(Guid documentId, string newStatus, string verifiedByUpn, string? notes = null)
        {
            _logger?.LogDebug("UpdateDocumentStatusAsync called for DocumentId={DocumentId} NewStatus={NewStatus}", documentId, newStatus);

            if (documentId == Guid.Empty) 
            {
                _logger?.LogWarning("UpdateDocumentStatusAsync called with empty DocumentId");
                return false;
            }

            try
            {
                var document = await _fileRepository.GetDocumentsByDocumentId(documentId).ConfigureAwait(false);
                if (document == null)
                {
                    _logger?.LogWarning("UpdateDocumentStatusAsync: document not found {DocumentId}", documentId);
                    return false;
                }

                document.Status = newStatus;
                document.VerifiedByUpn = verifiedByUpn;
                document.VerifiedAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    document.Notes = notes;
                }

                var result = await _fileRepository.UpdateDocument(document).ConfigureAwait(false);
                _logger?.LogInformation("Document {DocumentId} status updated to {NewStatus} by {User} Result={Result}", documentId, newStatus, verifiedByUpn, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "UpdateDocumentStatusAsync failed for DocumentId={DocumentId}", documentId);
                return false;
            }
        }

        #region
        private static ContractorDocument MapToContractorDocument(FileUploadDto dto, string uploadedBy, string blobPath)
        {
            return new ContractorDocument
            {
                DocumentId = Guid.NewGuid(),
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

        // adapter to keep code consistent with earlier implementation - avoids refactor of repository method call sites in this file.
        private Task<ContractorDocument?> _file_repository_GetDocumentsByDocumentId(Guid documentId)
            => _fileRepository.GetDocumentsByDocumentId(documentId);
    }
}
