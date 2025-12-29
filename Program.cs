using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Emerus.ETM.Admin.Data;
using Emerus.ETM.Admin.Repositories;
using Emerus.ETM.Admin.Repositories.Interfaces;
using Emerus.ETM.Admin.Services;
using Emerus.ETM.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);



// Register EF Core DbContext with SQL Server using the "DefaultConnection" from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------- AUTHENTICATION (AZURE AD) --------------------
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);

        // App controls session lifetime, not Azure AD
        options.UseTokenLifetime = false;

        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                context.ProtocolMessage.Prompt = "login";
                return Task.CompletedTask;
            }
        };
    });

// -------------------- COOKIE SETTINGS (SESSION BASED) --------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = null;

    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;

    options.LoginPath = "/MicrosoftIdentity/Account/SignIn";
    options.LogoutPath = "/MicrosoftIdentity/Account/SignOut";
});

// -------------------- ROLE CLAIM --------------------
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
});

// -------------------- KEY VAULT --------------------
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    return new SecretClient(
        new Uri(config["KeyVault:Url"]),
        new DefaultAzureCredential()
    );
});

// -------------------- FRAMEWORK SERVICES --------------------
builder.Services.AddAuthorization();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

// Register application services and repositories
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICommonService, CommonService>();

var app = builder.Build();

// -------------------- MIDDLEWARE PIPELINE --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Redirect Microsoft SignedOut page directly to login
app.Use(async (context, next) =>
{
    if (context.Request.Path.Equals("/MicrosoftIdentity/Account/SignedOut", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/");
        return;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
