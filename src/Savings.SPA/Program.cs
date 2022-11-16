
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Refit;
using Savings.Model;
using Savings.SPA;
using Savings.SPA.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();

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
    builder.Services.AddScoped(sp =>
    {
        var authorizationMessageHandler = sp.GetRequiredService<AuthorizationMessageHandler>();
        authorizationMessageHandler.InnerHandler = new HttpClientHandler();
        authorizationMessageHandler = authorizationMessageHandler.ConfigureHandler(
            authorizedUrls: new[] { builder.Configuration["SavingsApiServiceUrl"] },
            scopes: new[] { builder.Configuration["AzureAd:DefaultScope"] });
        return new HttpClient(authorizationMessageHandler)
        {
            BaseAddress = new Uri(builder.Configuration["SavingsApiServiceUrl"] ?? string.Empty)
        };
    });
}

await builder.Build().RunAsync();