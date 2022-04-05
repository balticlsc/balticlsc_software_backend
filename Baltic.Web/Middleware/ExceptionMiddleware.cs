using System;
using System.Threading.Tasks;
using Baltic.Web.Common;
using Baltic.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace Baltic.Web.Middleware
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string EventType { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private string _eventType;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _eventType = Log.Logger.ErrorWithId(ex, "Something went wrong: {ex}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var message = "Something went wrong please contact administrator";

            if (ex is ControllerException exception)
            {
                context.Response.StatusCode = exception.StatusCode;
                message = ex.Message;
            }

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                EventType = _eventType
            }.ToString());
        }
    }
}