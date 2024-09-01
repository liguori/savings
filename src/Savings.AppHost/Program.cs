using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.Savings_API>("api");

builder.AddProject<Projects.Savings_SPA>("client")
    .WithEndpoint("https", endpoint => endpoint.IsProxied = false); //Disable the proxy in order to allow the WASM debugger to work on the same port configured in the launchSettings.json

builder.Build().Run();
