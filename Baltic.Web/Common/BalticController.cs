using System;
using System.Linq;
using System.Net;
using Baltic.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Serilog;

namespace Baltic.Web.Common
{
    public class BalticController : Controller
    {
        protected string Sid => User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sid).Select(c => c.Value).SingleOrDefault();
        protected string UserName => User.Identity.Name;
        protected IUserSessionRoutine UserSession { get; }
        
        public BalticController(IUserSessionRoutine userSession)
        {
            UserSession = userSession;
        }
        
        protected IActionResult HandleError(string message, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            Log.Error("Error: {ex}", message);
            return new JsonResult(new { Success = false, Message = message})
            {
                StatusCode = (int)statusCode
            };
        }
        
        protected IActionResult HandleError(Exception ex)
        {
            Log.Debug(ex.ToString());
            return HandleError(ex.Message, HttpStatusCode.InternalServerError);
        }

        protected IActionResult Error(string message)
        {
            return HandleError(message);
        }
        
        protected IActionResult Ok()
        {
            return new JsonResult(new { Success = true, Message = "ok"})
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        protected IActionResult Ok(string message)
        {
            return new JsonResult(new { Success = true, Message = message})
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }

        protected IActionResult Ok(object value, string message = "ok", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new JsonResult(new { Success = true, Message = message, Data = value })
            {
                StatusCode = (int)statusCode
            };
        }
    }
}