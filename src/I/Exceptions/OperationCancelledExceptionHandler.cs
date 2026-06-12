using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class OperationCancelledExceptionHandler
    : BaseExceptionHandler<OperationCancelledExceptionHandler, OperationCanceledException>
{
    public OperationCancelledExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment)
        : base(problemDetailsFactory, environment)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context, OperationCanceledException? exception)
        => 499; // client closed
}
