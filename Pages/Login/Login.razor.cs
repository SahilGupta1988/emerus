using Microsoft.AspNetCore.Components;

namespace Emerus.ETM.Admin.Pages.Login
{
    public partial class Login : ComponentBase
    {
        private void HandleLogin()
        {
            Nav.NavigateTo("MicrosoftIdentity/Account/SignIn?redirectUri=/dashboard", true);
        }
    }
}
