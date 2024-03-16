using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using TRMPortal;
using TRMPortal.Authentication;

using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationService, AuthenticationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

builder.Services.AddSingleton<IAPIHelper, APIHelper>();
builder.Services.AddSingleton<ILoggedInUserModel, LoggedInUserModel>();
builder.Services.AddTransient<IProductEndpoint, ProductEndpoint>();
builder.Services.AddTransient<IUserEndpoint, UserEndpoint>();
builder.Services.AddTransient<ISaleEndpoint, SaleEndpoint>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddOidcAuthentication(options =>
//{
//	// Configure your authentication provider options here.
//	// For more information, see https://aka.ms/blazor-standalone-auth
//	builder.Configuration.Bind("Local", options.ProviderOptions);
//});

await builder.Build().RunAsync();
