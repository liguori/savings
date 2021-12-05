
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Refit;
using SavingsProjection.Model;
using SavingsProjection.SPA;
using SavingsProjection.SPA.Authorization;
using SavingsProjection.SPA.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("app");

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

var httpClientBuilder = builder.Services.AddRefitClient<ISavingProjectionApi>().ConfigureHttpClient((sp, c) =>
{
    c.BaseAddress = new Uri(builder.Configuration["SavingProjectionApiServiceUrl"]);
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