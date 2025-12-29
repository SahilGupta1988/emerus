using Emerus.ETM.Admin.Models.Response;
using Emerus.ETM.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Emerus.ETM.Admin.Services
{
    public class CommonService : ICommonService
    {
        private readonly AuthenticationStateProvider _authStateProvider;

        public CommonService(AuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }
        public async Task<CurrentUserDetails> GetCurrentUserAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                return new CurrentUserDetails
                {
                    IsAuthenticated = false
                };
            }

            return new CurrentUserDetails
            {
                IsAuthenticated = true,
                UserName =
                    user.FindFirst("name")?.Value ??
                    user.FindFirst(ClaimTypes.Name)?.Value ??
                    "Unknown User",

                UserEmail =
                    user.FindFirst("preferred_username")?.Value ??
                    user.FindFirst(ClaimTypes.Email)?.Value,

                Role = user.FindFirst(ClaimTypes.Role)?.Value
            };
        }
    }
}
