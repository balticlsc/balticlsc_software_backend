#pragma warning disable 1591
using System;
using Baltic.Server.Middleware;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Baltic.Server.Extensions
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
        //{
        //    app.UseExceptionHandler(appError =>
        //    {
        //        appError.Run(async context =>
        //        {
        //            context.Response.ContentType = "application/json";

        //            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

        //            if (contextFeature != null)
        //            {
        //                Log.Error($"Something went wrong:{contextFeature.Error}");

        //                await context.Response.WriteAsync(new ErrorDetails()
        //                {
        //                    StatusCode = context.Response.StatusCode,
        //                    Message = "Something went wrong.We are working on getting this fixed as soon as we can.You may be able to try again."
        //                }.ToString());
        //            }
        //        });
        //    });
        //}
    }
}