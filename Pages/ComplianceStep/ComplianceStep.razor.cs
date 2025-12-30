using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Models.Request;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Emerus.ETM.Admin.Pages.ComplianceStep
{
    public partial class ComplianceStep : ComponentBase
    {
        // Parameter supplied by parent (OnboardingNew.razor)
        [Parameter]
        public Guid SavedRequestId { get; set; }

        // track last seen id so we only reload when it changes
        private Guid _lastSeenRequestId = Guid.Empty;

        protected override async Task OnInitializedAsync()
        {
            // If parent already provided a SavedRequestId, load documents on init.
            if (SavedRequestId != Guid.Empty)
            {
                _lastSeenRequestId = SavedRequestId;
                await GetDocumentByRequestId();
            }
            else
            {
                await GetDocumentByRequestId();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            // Called when parent updates parameters (e.g. after saving a draft)
            if (SavedRequestId != Guid.Empty && SavedRequestId != _lastSeenRequestId)
            {
                _lastSeenRequestId = SavedRequestId;
                await GetDocumentByRequestId();
            }
        }

        private List<ContractorDocument> Documents = new();
        //private Guid ContractorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        private async Task OnFileChange(InputFileChangeEventArgs e)
        {
            string? requesterUpn = null;
            var currentUser = _commonService is not null ? await _commonService.GetCurrentUserAsync() : null;
            if (currentUser?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(currentUser.UserEmail))
            {
                requesterUpn = currentUser.UserEmail.Trim();
                requesterUpn = requesterUpn.ToLowerInvariant();
            }

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
                        //ContractorId = ContractorId,
                        RequestId = SavedRequestId
                    };

                    await _fileService.UploadAsync(dto, requesterUpn).ConfigureAwait(false);
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
            // Use the SavedRequestId parameter passed from the parent.
            if (SavedRequestId == Guid.Empty)
            {
                Documents = new List<ContractorDocument>();
                return;
            }

            Documents = await _fileService.GetDocumentByRequestIdAsync(SavedRequestId).ConfigureAwait(false);
        }

        private async Task DeleteDocument(ContractorDocument document)
        {
            var result = await _fileService.DeleteDocumentAsync(document.DocumentId).ConfigureAwait(false);
            if (result) Documents.Remove(document);
        }
    }
}
