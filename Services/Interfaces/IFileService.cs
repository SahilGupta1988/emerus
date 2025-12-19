using Emerus.ETM.Admin.Models.Request;

namespace Emerus.ETM.Admin.Services.Interfaces
{
    public interface IFileService
    {
        Task UploadAsync(FileUploadDto dto, string uploadedBy);
    }
}
