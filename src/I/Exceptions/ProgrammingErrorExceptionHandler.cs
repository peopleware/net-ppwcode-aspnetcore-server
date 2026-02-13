using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

using PPWCode.Vernacular.Exceptions.V;

using ProgrammingError = PPWCode.Vernacular.Exceptions.V.ProgrammingError;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class ProgrammingErrorExceptionHandler
    : BaseExceptionHandler<ProgrammingErrorExceptionHandler, ProgrammingError>
{
    public ProgrammingErrorExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment)
        : base(problemDetailsFactory, environment)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context, ProgrammingError? exception)
        => exception is ApiUsageError // ApiUsageError => programming error on the frontend
               ? StatusCodes.Status400BadRequest
               : StatusCodes.Status500InternalServerError;

    /// <inheritdoc />
    protected override bool LogException
        => true;
}
