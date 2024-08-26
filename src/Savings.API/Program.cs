
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Savings.API.Authentication;
using Savings.API.Infrastructure;
using Savings.API.Services;
using Savings.API.Services.Abstract;
using Savings.Model;
using System.Reflection;
using System.Text.Json.Serialization;

const string ApiKeys = "ApiKeys";

var builder = WebApplication.CreateBuilder(args);


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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Savings", Version = "v1" });

    if (authenticationToUse == AuthenticationToUse.AzureAD)
    {
        c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please insert JWT with Bearer into field",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
    }
    else if (authenticationToUse == AuthenticationToUse.ApiKey)
    {
        //Add API Key Informations
        c.AddSecurityDefinition(ApiKeyAuthOptions.ApiKeySchemaName, new OpenApiSecurityScheme
        {
            Description = "Api key needed to access the endpoints. " + ApiKeyAuthOptions.HeaderName + ": My_API_Key",
            In = ParameterLocation.Header,
            Name = ApiKeyAuthOptions.HeaderName,
            Type = SecuritySchemeType.ApiKey
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
                     {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = ApiKeyAuthOptions.HeaderName,
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = ApiKeyAuthOptions.ApiKeySchemaName
                            },
                         },
                         new string[] {}
                     }
                });
    }
});

builder.Services.AddDbContext<SavingsContext>(
               options => options.UseSqlite($"Data Source={builder.Configuration["DatabasePath"]}", sqlOpt =>
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
    app.UseSwagger();
    app.UseSwaggerUI();
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