using System.Net;
using System.Text.Json;
using TravoRiders.Application.Common.Exceptions;
using TravoRiders.Application.Common.Responses;

namespace TravoRiders.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unhandled exception occurred");
                await HandleExceptionAsync(context, exception);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse();

            switch (exception)
            {
                case AppException appEx:
                    context.Response.StatusCode = appEx.StatusCode;
                    response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = appEx.Message
                    };
                    break;

                case ArgumentNullException argNullEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = $"Argument null: {argNullEx.ParamName}"
                    };
                    break;

                case ArgumentException argEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = argEx.Message
                    };
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = keyNotFoundEx.Message ?? "Resource not found"
                    };
                    break;

                case InvalidOperationException invOpEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = invOpEx.Message
                    };
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Unauthorized access"
                    };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "An internal server error occurred. Please try again later."
                    };
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
