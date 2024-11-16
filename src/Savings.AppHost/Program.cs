using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


builder.AddProject<Projects.Savings_API>("api");

builder.AddProject<Projects.Savings_SPA>("client");

builder.Build().Run();
