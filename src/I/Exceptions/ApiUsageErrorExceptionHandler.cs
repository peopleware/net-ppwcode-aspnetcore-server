using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class ApiUsageErrorExceptionHandler
    : BaseExceptionHandler<ApiUsageErrorExceptionHandler, ApiUsageError>
{
    public ApiUsageErrorExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment)
        : base(problemDetailsFactory, environment)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context, ApiUsageError? exception)
        => StatusCodes.Status400BadRequest;
}
