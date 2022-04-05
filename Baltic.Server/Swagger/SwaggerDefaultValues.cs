using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Baltic.Server.Swagger
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.ApiDescription.TryGetMethodInfo(out var mi)) return;
            if (!mi.GetCustomAttributes().OfType<AuthorizeAttribute>().Any()) return;
            
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();
            
            operation.Parameters.Add(new OpenApiParameter 
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Bearer access token",
                Required = false,
                Schema = new OpenApiSchema() 
                {
                    Type = "",
                    Default = new OpenApiString("Bearer xxx.xxx")
                }
            });
            operation.Responses.Add("401", new OpenApiResponse { Description = "The request has not been applied because it lacks valid authentication credentials for the target resource"});
            operation.Responses.Add("403", new OpenApiResponse { Description = "The server understood the request but refuses to authorize it"});                
        }
    }
}