
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Refit;
using Savings.Model;
using Savings.SPA;
using Savings.SPA.Authorization;
using Savings.SPA.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

var configuredAuthentication = builder.Configuration["AuthenticationToUse"];

if (configuredAuthentication == AuthenticationToUse.AzureAD)
{
    builder.Services.AddMsalAuthentication(options =>
    {
        options.ProviderOptions.LoginMode = "redirect";
        builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
        options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration["AzureAd:DefaultScope"]);
    });
}

var httpClientBuilder = builder.Services.AddRefitClient<ISavingsApi>().ConfigureHttpClient((sp, c) =>
{
    c.BaseAddress = new Uri(builder.Configuration["SavingsApiServiceUrl"]);
    if (configuredAuthentication == AuthenticationToUse.ApiKey)
    {
        c.DefaultRequestHeaders.Add("X-API-Key", builder.Configuration["ApiKey"]);
    }
});

if (configuredAuthentication == AuthenticationToUse.AzureAD)
{
    httpClientBuilder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
}

await builder.Build().RunAsync();