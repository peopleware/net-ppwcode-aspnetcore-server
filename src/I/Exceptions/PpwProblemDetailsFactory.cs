// Copyright 2026 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

/// <summary>
///     A <see cref="ProblemDetailsFactory" /> that produces <see cref="ProblemDetails" /> and
///     <see cref="ValidationProblemDetails" /> responses with PPWCode-specific defaults.
/// </summary>
/// <remarks>
///     <para>
///         The factory falls back to sensible defaults for the status code, title, type and instance, and applies the
///         <see cref="ApiBehaviorOptions.ClientErrorMapping" /> configured for the application.
///     </para>
///     <para>
///         When the application runs in a development environment (see <see cref="EnvironmentsConsideredAsDevelopment" />),
///         diagnostic information such as the current activity id and the request trace identifier is added to the
///         <see cref="ProblemDetails.Extensions" />.
///     </para>
/// </remarks>
public class PpwProblemDetailsFactory : ProblemDetailsFactory
{
    /// <summary>
    ///     The set of environment names that are treated as development environments. Matching is case-insensitive.
    /// </summary>
    public static readonly ISet<string> EnvironmentsConsideredAsDevelopment =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Environments.Development,
            "DEV",
            "LOCAL"
        };

    private readonly IHostEnvironment _environment;
    private readonly ApiBehaviorOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PpwProblemDetailsFactory" /> class.
    /// </summary>
    /// <param name="options">The configured API behaviour options, used for the client error mapping.</param>
    /// <param name="environment">The host environment, used to decide whether diagnostic details are exposed.</param>
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

    /// <summary>
    ///     Fills in the title and type of <paramref name="problemDetails" /> from the configured
    ///     <see cref="ApiBehaviorOptions.ClientErrorMapping" />, falling back to the supplied values, and adds
    ///     diagnostic information when running in a development environment.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="problemDetails">The problem details to complete.</param>
    /// <param name="statusCode">The HTTP status code used to look up the client error mapping.</param>
    /// <param name="titleFallback">The title to use when neither the problem details nor the mapping provide one.</param>
    /// <param name="typeFallback">The type to use when neither the problem details nor the mapping provide one.</param>
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
