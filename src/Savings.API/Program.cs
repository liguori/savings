
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Savings.API.Authentication;
using Savings.API.Infrastructure;
using Savings.API.OpenApi;
using Savings.API.Services;
using Savings.API.Services.Abstract;
using Savings.Model;
using System.Reflection;
using System.Text.Json.Serialization;

const string ApiKeys = "ApiKeys";

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddControllers(opt =>
{
    opt.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
}).AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var authenticationToUse = builder.Configuration["AuthenticationToUse"];

if (authenticationToUse == AuthenticationToUse.AzureAD)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.Audience = builder.Configuration["IdentityProvider:Audience"];
        options.Authority = builder.Configuration["IdentityProvider:Authority"];
    });
}
else if (authenticationToUse == AuthenticationToUse.ApiKey)
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = ApiKeyAuthOptions.ApiKeySchemaName;
        options.DefaultChallengeScheme = ApiKeyAuthOptions.ApiKeySchemaName;
    })
    .AddApiKeyAuth(options => options.AuthKeys = builder.Configuration[ApiKeys]?.Split(","));
}

builder.Services.AddTransient<IProjectionCalculator, ProjectionCalculator>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<DocumentSecuritySchemeTransformer>();
    options.AddOperationTransformer<OperationSecurityTransformer>();
});

builder.Services.AddDbContext<SavingsContext>(options => options.UseSqlite($"Data Source={builder.Configuration["DatabasePath"]}", sqlOpt =>
               {
                   sqlOpt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
               })
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigin",
    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "Savings API"));
    app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<SavingsContext>()?.Database.EnsureCreated();
}
app.UseCors("AllowAllOrigin");

app.UseHttpsRedirection();

if (authenticationToUse != AuthenticationToUse.None)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

var controllerBuilder = app.MapControllers();

if (authenticationToUse != AuthenticationToUse.None)
{
    controllerBuilder.RequireAuthorization();
}

app.Run();