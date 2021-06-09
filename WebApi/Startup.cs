using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ondato.Domain.Configuration;
using Ondato.WebApi.Middleware;

namespace Ondato.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<OndatoApiKeyConfig>(Configuration.GetSection(nameof(OndatoApiKeyConfig)));
            services.Configure<ListCacheConfig>(Configuration.GetSection(nameof(ListCacheConfig)));
            services.AddControllers();

            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-5.0
            services.AddDistributedMemoryCache();
            //services.AddDistributedSqlServerCache(options =>
            //{
            //    options.ConnectionString = _config["DistCache_ConnectionString"];
            //    options.SchemaName = "dbo";
            //    options.TableName = "TestCache";
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ondato API", Version = "v1" });

                // https://pradeeploganathan.com/rest/add-security-requirements-oas3-swagger-netcore3-1-using-swashbuckle/
                var securityScheme = new OpenApiSecurityScheme
                {
                    Description = "API key needed to access the endpoints. X-API-KEY: My_API_Key",
                    Type = SecuritySchemeType.ApiKey, // this value is always "apiKey"
                    In = ParameterLocation.Header, // where to find apiKey, probably in a header
                    Name = "X-API-KEY", // header with API key
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKeyAuth" },
                };
                c.AddSecurityDefinition("ApiKeyAuth", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() },
                });
                c.OperationFilter<SwaggerOperationFilter>();
                // https://github.com/RicoSuter/NSwag/issues/2952
                c.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "time-span" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ondato API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            // Do your cleanup here
            Console.WriteLine("Terminating application...");
        }
    }
}
