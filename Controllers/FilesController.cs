using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Emerus.ETM.Admin.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emerus.ETM.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Approver,Requestor")]
    public class FilesController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;
        private readonly SecretClient _secretClient;

        public FilesController(IFileRepository fileRepository, SecretClient secretClient)
        {
            _fileRepository = fileRepository;
            _secretClient = secretClient;
        }

        /// <summary>
        /// Download a stored contractor document by document id.
        /// </summary>
        [HttpGet("download/{documentId:guid}")]
        public async Task<IActionResult> Download(Guid documentId)
        {
            if (documentId == Guid.Empty)
                return BadRequest();

            var document = await _fileRepository.GetDocumentsByDocumentId(documentId).ConfigureAwait(false);
            if (document is null || string.IsNullOrWhiteSpace(document.StorageUrl))
                return NotFound();

            // Recreate BlobContainerClient using KeyVault secrets (same approach as FileRepository)
            try
            {
                var connectionString = _secretClient.GetSecret("dev--Storage--ConnectionString").Value.Value;
                var containerName = _secretClient.GetSecret("dev--Storage--ContainerName").Value.Value;

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var blobClient = containerClient.GetBlobClient(document.StorageUrl); // StorageUrl is the blob path
                var exists = await blobClient.ExistsAsync().ConfigureAwait(false);
                if (!exists.Value)
                    return NotFound();

                var download = await blobClient.DownloadAsync().ConfigureAwait(false);
                var stream = download.Value.Content;
                var contentType = download.Value.ContentType;
                if (string.IsNullOrWhiteSpace(contentType))
                {
                    contentType = "application/octet-stream";
                }

                var fileName = !string.IsNullOrWhiteSpace(document.FileName) ? document.FileName : Path.GetFileName(document.StorageUrl);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                // Log if you have logging; return 500 for unexpected errors
                return Problem(detail: ex.Message);
            }
        }
    }
}