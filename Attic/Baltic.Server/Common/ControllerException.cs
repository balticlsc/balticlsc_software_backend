#pragma warning disable 1591
using System;

namespace Baltic.Server.Common
{
    public class ControllerException : Exception
    {
        private const int Status500InternalServerError = 500;
        public int StatusCode { get; }

        public ControllerException(int statusCode = Status500InternalServerError)
        {
            StatusCode = statusCode;
        }

        public ControllerException(string message, int statusCode = Status500InternalServerError) : base(message)
        {
            StatusCode = statusCode;
        }

        public ControllerException(string message, Exception inner, int statusCode = Status500InternalServerError) : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}
