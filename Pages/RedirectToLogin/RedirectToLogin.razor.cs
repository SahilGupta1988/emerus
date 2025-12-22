using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace Emerus.ETM.Admin.Pages.RedirectToLogin
{
    public partial class RedirectToLogin : ComponentBase
    {
        private string accessDeniedMessage = string.Empty;

        protected override async void OnAfterRender(bool firstRender)
        {
            if (!firstRender) return;

            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (!user.Identity.IsAuthenticated)
            {
                Navigation.NavigateTo("/");
                return;
            }

            var role = user.Claims.FirstOrDefault(c =>
                        c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(role))
            {
                Navigation.NavigateTo("/");
                return;
            }

            accessDeniedMessage = "Access Denied: You do not have permission to view this page.";
            StateHasChanged();
        }
    }
}
