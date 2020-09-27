using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Refit;
using SavingsProjection.SPA.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SavingsProjection.SPA
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddRefitClient<ISavingProjectionApi>().ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(builder.Configuration["SavingProjectionApiServiceUrl"]);
            });

            await builder.Build().RunAsync();
        }
    }
}
