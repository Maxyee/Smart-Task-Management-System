using System.Text.Json;
using SmartTaskManagement.Application.Common;

namespace SmartTaskManagement.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new Response<object>
            {
                Success = false,
                Message = "An error occurred while processing your request",
                StatusCode = 500
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response.StatusCode = 401;
                    response.Message = "Unauthorized access";
                    break;
                case KeyNotFoundException:
                    response.StatusCode = 404;
                    response.Message = "Resource not found";
                    break;
                case ArgumentException:
                case InvalidOperationException:
                    response.StatusCode = 400;
                    response.Message = exception.Message;
                    break;
                default:
                    response.StatusCode = 500;
                    response.Message = "Internal server error";
                    break;
            }

            context.Response.StatusCode = response.StatusCode;
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}