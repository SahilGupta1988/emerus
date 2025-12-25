using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Models.Request;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Emerus.ETM.Admin.Pages.ComplianceStep
{
    public partial class ComplianceStep : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await GetDocumentByRequestId();
        }

        private List<ContractorDocument> Documents = new();
        private Guid ContractorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        private async Task OnFileChange(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles())
            {
                try
                {
                    await using var stream = file.OpenReadStream(10 * 1024 * 1024);

                    var dto = new FileUploadDto
                    {
                        FileStream = stream,
                        FileName = file.Name,
                        DocumentType = GetDocumentType(file.Name),
                        ContractorId = ContractorId
                    };

                    await _fileService.UploadAsync(dto, "system").ConfigureAwait(false);
                    // Documents.Add(file.Name);
                    await GetDocumentByRequestId();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(
                        $"Upload failed for {file.Name}: {ex.Message}");
                }
            }
        }

        private string GetDocumentType(string fileName)
        {
            fileName = fileName.ToLowerInvariant();

            if (fileName.Contains("nupc"))
                return "NUPC";
            if (fileName.Contains("iaa"))
                return "IAA";
            if (fileName.EndsWith(".pdf"))
                return "PDF";

            return "Other";
        }

        private async Task GetDocumentByRequestId()
        {
            Guid requestId = Guid.Parse("F9853E67-DDF6-4FEE-B5A3-C75A28BCA5F0");
            Documents = await _fileService.GetDocumentByRequestIdAsync(requestId).ConfigureAwait(false);
        }

        private async Task DeleteDocument(ContractorDocument document)
        {
            var result = await _fileService.DeleteDocumentAsync(document.DocumentId).ConfigureAwait(false);
            if (result) Documents.Remove(document);
        }
    }
}
