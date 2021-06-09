using System.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ondato.WebApi.Middleware
{
    /// <summary>
    /// Removes "text/plain" MIME type.
    /// Adds "401 Unauthorized" response.
    /// https://github.com/domaindrivendev/Swashbuckle.AspNetCore#extend-generator-with-operation-schema--document-filters
    /// </summary>
    internal class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var (_, response) in operation.Responses)
            {
                var wrongContents = response.Content.Where(o => o.Key == "text/plain").ToList();

                foreach (var wrongContent in wrongContents)
                {
                    response.Content.Remove(wrongContent);
                }
            }

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        }
    }
}
