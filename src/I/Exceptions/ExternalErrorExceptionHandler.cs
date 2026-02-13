using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

using PPWCode.Vernacular.Exceptions.V;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class ExternalErrorExceptionHandler
    : BaseExceptionHandler<ExternalErrorExceptionHandler, ExternalError>
{
    public ExternalErrorExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment)
        : base(problemDetailsFactory, environment)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context, ExternalError? exception)
        => StatusCodes.Status500InternalServerError;

    /// <inheritdoc />
    protected override bool LogException
        => true;
}
