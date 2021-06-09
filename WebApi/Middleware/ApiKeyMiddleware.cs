using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Ondato.WebApi.Middleware
{
    /// <summary>
    /// X-API-KEY authorization header middleware.
    /// https://swagger.io/docs/specification/authentication/api-keys/
    /// http://codingsonata.com/secure-asp-net-core-web-api-using-api-key-authentication/
    /// </summary>
    internal class ApiKeyMiddleware
    {
        private const string ApiKeysHeaderName = "X-API-KEY";

        private readonly RequestDelegate _next;
        private readonly IOptions<OndatoApiKeyConfig> _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IOptions<OndatoApiKeyConfig> configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeysHeaderName, out var apiKeyValues))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync($"{nameof(ApiKeyMiddleware)}: Api Key was not provided.");
                return;
            }

            //var apiKeys = _configuration.GetSection("OndatoApiConfig:ApiKeys").Get<string[]>();
            var apiKeys = _configuration.Value.ApiKeys;

            if (apiKeys is null || !apiKeys.Contains(apiKeyValues[0]))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync($"{nameof(ApiKeyMiddleware)}: Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }
}
