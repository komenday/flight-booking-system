using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FBS.API.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly string[] DefaultErrorMessages = ["An unexpected error occurred. Please try again later."];

    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var (statusCode, title, errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred",
                validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                new Dictionary<string, string[]>
                {
                    { "error", new[] { exception.Message } }
                }
            ),
            InvalidOperationException => (
                StatusCodes.Status400BadRequest,
                "Invalid Operation",
                new Dictionary<string, string[]>
                {
                    { "error", new[] { exception.Message } }
                }
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                new Dictionary<string, string[]>
                {
                    { "error", DefaultErrorMessages }
                }
            )
        };

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = statusCode,
            Title = title,
            Type = GetTypeForStatusCode(statusCode),
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, CachedJsonSerializerOptions), cancellationToken);

        return true;
    }

    private static string GetTypeForStatusCode(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        StatusCodes.Status404NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        StatusCodes.Status500InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        _ => "https://tools.ietf.org/html/rfc7231"
    };
}