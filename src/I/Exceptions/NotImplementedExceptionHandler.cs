using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class NotImplementedExceptionHandler
    : BaseExceptionHandler<NotImplementedExceptionHandler, NotImplementedException>
{
    public NotImplementedExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment)
        : base(problemDetailsFactory, environment)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context, NotImplementedException? exception)
        => StatusCodes.Status501NotImplemented;

    /// <inheritdoc />
    protected override bool LogException
        => true;
}
