using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.Savings_API>("api");

builder.AddProject<Projects.Savings_SPA>("client")
    .WithExternalHttpEndpoints();

builder.Build().Run();
