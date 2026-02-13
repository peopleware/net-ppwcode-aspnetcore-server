using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

public class PpwProblemDetailsFactory : ProblemDetailsFactory
{
    public static readonly ISet<string> EnvironmentsConsideredAsDevelopment =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Environments.Development,
            "DEV",
            "LOCAL"
        };

    private readonly IHostEnvironment _environment;
    private readonly ApiBehaviorOptions _options;

    public PpwProblemDetailsFactory(
        IOptions<ApiBehaviorOptions> options,
        IHostEnvironment environment)
    {
        _environment = environment;
        _options = options.Value;
    }

    /// <inheritdoc />
    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        // Default status code is 500 if not provided
        statusCode ??= StatusCodes.Status500InternalServerError;

        ProblemDetails problemDetails =
            new()
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance ?? httpContext.Request.GetEncodedPathAndQuery()
            };

        ApplyProblemDetailsDefaults(
            httpContext,
            problemDetails,
            statusCode.Value,
            "An error occurred",
            $"https://httpstatuses.com/{statusCode}");

        return problemDetails;
    }

    /// <inheritdoc />
    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        // Default status code is 400 if not provided
        statusCode ??= StatusCodes.Status400BadRequest;

        ValidationProblemDetails problemDetails =
            new(modelStateDictionary)
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = detail,
                Instance = instance ?? httpContext.Request.Path
            };

        ApplyProblemDetailsDefaults(
            httpContext,
            problemDetails,
            statusCode.Value,
            "Validation Error",
            $"https://httpstatuses.com/{statusCode}");

        return problemDetails;
    }

    protected virtual void ApplyProblemDetailsDefaults(
        HttpContext httpContext,
        ProblemDetails problemDetails,
        int statusCode,
        string titleFallback,
        string? typeFallback = null)
    {
        if (_options.ClientErrorMapping.TryGetValue(statusCode, out ClientErrorData? clientErrorData))
        {
            problemDetails.Title ??= clientErrorData.Title;
            problemDetails.Type ??= clientErrorData.Link;
        }

        problemDetails.Title ??= titleFallback;
        problemDetails.Type ??= typeFallback;

        if (EnvironmentsConsideredAsDevelopment.Contains(_environment.EnvironmentName))
        {
            if (Activity.Current?.Id is not null)
            {
                problemDetails.Extensions["activityId"] = Activity.Current.Id;
            }

            if (!string.IsNullOrEmpty(httpContext.TraceIdentifier))
            {
                problemDetails.Extensions["traceIdentifier"] = httpContext.TraceIdentifier;
            }
        }
    }
}
