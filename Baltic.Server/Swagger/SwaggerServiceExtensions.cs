using System;
using System.Collections.Generic;
using System.IO;
using Baltic.Core.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Baltic.Server.Swagger
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "BalticLSC API",
                        Description = "Baltic Large-Scale Computing Service Api",
                        TermsOfService = new Uri("https://www.balticlsc.eu/terms"),
                        Contact = new OpenApiContact
                        {
                            Name = "BalticLSC",
                            Email = string.Empty,
                            Url = new Uri("https://www.balticlsc.eu/"),
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Use under License",
                            Url = new Uri("https://www.balticlsc.eu/license"),
                        },
                    });
                    options.SwaggerDoc("v2", new OpenApiInfo
                    {
                        Version = "v2",
                        Title = "BalticLSC Test API",
                        Description = "Baltic Large-Scale Computing Service Test Api",
                    });

                    options.AddSecurityDefinition("Bearer", 
                        new OpenApiSecurityScheme { 
                            In = ParameterLocation.Header,
                            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \r\n\r\n\"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InJyb3N6Y3p5ayIsInN1YiI6InJyb3N6Y3p5ayIsImVtYWlsIjoicmFkb3NsYXcucm9zemN6eWtAZWUucHcuZWR1LnBsIiwianRpIjoiOGYwNThmNDI0MmJhNDY0ZDlkMmFkNTY0ZDg4NWFlOGQiLCJzaWQiOiI4ZTdmZTRiOGFjNjg0YzIwYWQwMDRmNjEyNDc3NGY3ZCIsImV4cCI6MTYwNzEyMjk1MiwiaXNzIjoid3V0LmJhbHRpY2xzYy5ldSIsImF1ZCI6Ind1dC5iYWx0aWNsc2MuZXUifQ.9WPo-Q2sVpfJ2FcQ5D4JL-UZGkyeHmpU6bI9VgC558A\"", 
                            Name = "Authorization", 
                            Type = SecuritySchemeType.ApiKey,
                            Scheme = "Bearer"
                        });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer" 
                                },
                                Name = "Bearer",
                                Scheme = "Bearer"
                            }, new List<string>() }
                    });         
                    options.OperationFilter<SwaggerDefaultValues>();
                    var xmlFile = $"{SystemInfo.ExecutingAssemblyName}.xml";
                    options.IncludeXmlComments(Path.Combine(SystemInfo.ContentRootPath, xmlFile));
                    options.IncludeXmlComments(Path.Combine(SystemInfo.ContentRootPath, "Baltic.DiagramRegistry.xml"));
                    options.IncludeXmlComments(Path.Combine(SystemInfo.ContentRootPath, "Baltic.NetworkRegistry.xml"));
                    options.IncludeXmlComments(Path.Combine(SystemInfo.ContentRootPath, "Baltic.TaskManager.xml"));
                    options.IncludeXmlComments(Path.Combine(SystemInfo.ContentRootPath, "Baltic.UnitManager.xml"));                    
                    options.IncludeXmlComments(Path.Combine(SystemInfo.ContentRootPath, "Baltic.UnitRegistry.xml"));
                    options.IncludeXmlComments(Path.Combine(SystemInfo.ContentRootPath, "Baltic.UserRegistry.xml"));                    
                });
            }
            
            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration, IApiVersionDescriptionProvider apiVersionDescriptionProvider, bool isDevelopment)
        {
            if (configuration != null && isDevelopment && bool.TryParse(configuration["Swagger:Enable"], out var swaggerState))
            {
                if (swaggerState)
                {
                    app.UseSwagger();            
            
                    app.UseSwaggerUI(options =>
                    {
                        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)  
                        {  
                            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        } 
                        options.RoutePrefix = "swagger/ui";
                    });
                }
            }

            return app;
        }
    }
}