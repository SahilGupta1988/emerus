using Emerus.ETM.Admin.Models.Response;

namespace Emerus.ETM.Admin.Services.Interfaces
{
    public interface ICommonService
    {
        Task<CurrentUserDetails> GetCurrentUserAsync();
    }
}
