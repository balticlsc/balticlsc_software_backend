using System;
using Baltic.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Baltic.Web.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        public static string ErrorWithId(this ILogger logger, Exception ex, string messageTemplate, params object[] args)
        {
            var id = ex.ToString().GetHashCode().ToString("X").ToLower();
            var withId = logger.ForContext("EventType", id);
            withId.Error(ex, messageTemplate, args);
            return id;
        }
    }
}