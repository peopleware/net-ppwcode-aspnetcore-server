using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

using PPWCode.Vernacular.Persistence.V.Exceptions;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class NotFoundExceptionHandler
    : BaseExceptionHandler<NotFoundExceptionHandler, NotFoundException>
{
    public NotFoundExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment)
        : base(problemDetailsFactory, environment)
    {
    }

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context, NotFoundException? exception)
        => StatusCodes.Status404NotFound;

    /// <inheritdoc />
    protected override void EnrichProblemDetails(ExceptionContext context, NotFoundException? notFound, ProblemDetails problemDetail)
    {
        const string PersistentObjectTypeKey = "IdNotFoundException.PersistentObjectType";
        const string PersistenceIdKey = "IdNotFoundException.PersistenceId";

        base.EnrichProblemDetails(context, notFound, problemDetail);

        if (IsDevelopment)
        {
            if (notFound is not null)
            {
                if (notFound.Data.Contains(PersistenceIdKey))
                {
                    problemDetail.Extensions["PersistenceId"] = Convert.ToString(notFound.Data[PersistenceIdKey]);
                }

                if (notFound.Data.Contains(PersistentObjectTypeKey))
                {
                    problemDetail.Extensions["PersistentObjectType"] = Convert.ToString(notFound.Data[PersistentObjectTypeKey]);
                }
            }
        }
    }
}
