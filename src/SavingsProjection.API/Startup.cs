using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SavingsProjection.API.Authentication;
using SavingsProjection.API.Infrastructure;
using SavingsProjection.API.Services;
using SavingsProjection.API.Services.Abstract;
using SavingsProjection.Model;
using System.Reflection;

namespace SavingsProjection.API
{
    public class Startup
    {
        const string ApiKeys = "ApiKeys";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var authenticationToUse = Configuration["AuthenticationToUse"];

            if (authenticationToUse == AuthenticationToUse.Oidc)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.Audience = Configuration["IdentityProvider:Audience"];
                    options.Authority = Configuration["IdentityProvider:Authority"];
                });
            }
            else if (authenticationToUse == AuthenticationToUse.ApiKey)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthOptions.ApiKeySchemaName;
                    options.DefaultChallengeScheme = ApiKeyAuthOptions.ApiKeySchemaName;
                })
                .AddApiKeyAuth(options => options.AuthKeys = Configuration[ApiKeys].Split(","));
            }

            services.AddTransient<IProjectionCalculator, ProjectionCalculator>();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Savings Projection", Version = "v1" });

                if (authenticationToUse == AuthenticationToUse.Oidc)
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

            services.AddDbContext<SavingProjectionContext>(
                options => options.UseSqlite($"Data Source={Configuration["DatabasePath"]}", sqlOpt =>
                {
                    sqlOpt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                })
               );

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SavingProjectionContext context)
        {
            var authenticationToUse = Configuration["AuthenticationToUse"];

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            context.Database.EnsureCreated();

            app.UseCors("AllowAllOrigin");

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
