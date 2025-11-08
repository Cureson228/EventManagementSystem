using FluentValidation;
using System.Net;
using System.Text.Json;

namespace EventManagementSystemApi.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                context.Response.ContentType = "application/json";
                var statusCode = HttpStatusCode.InternalServerError;
                object response = new { message = "An unexpected error occurred." };

                switch (ex)
                {
                    case ValidationException validationEx:
                        statusCode = HttpStatusCode.BadRequest;
                        response = new
                        {
                            message = validationEx.Message,
                            errors = (validationEx.Errors ?? Enumerable.Empty<FluentValidation.Results.ValidationFailure>())
                            .Select(e => new
                            {
                                field = e.PropertyName,
                                error = e.ErrorMessage
                            })
                            .ToList()
                        };
                        break;

                    case UnauthorizedAccessException unauthorizedEx:
                        statusCode = HttpStatusCode.Unauthorized;
                        response = new { message = unauthorizedEx.Message };
                        break;

                    case KeyNotFoundException notFoundEx:
                        statusCode = HttpStatusCode.NotFound;
                        response = new { message = notFoundEx.Message };
                        break;

                    default:
                        response = new { message = ex.Message };
                        break;
                }

                context.Response.StatusCode = (int)statusCode;

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
            
        }
    }
}
