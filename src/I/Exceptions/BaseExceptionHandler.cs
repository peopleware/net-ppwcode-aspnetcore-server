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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

/// <summary>
///     Base class for <see cref="IExceptionHandler" /> implementations that translate a specific
///     <typeparamref name="TException" /> into an HTTP response, typically a <see cref="ProblemDetails" /> payload.
/// </summary>
/// <remarks>
///     <para>
///         The handling pipeline in <see cref="Handle" /> works as follows:
///         <list type="number">
///             <item><see cref="CanHandle" /> decides whether this handler applies to the current exception.</item>
///             <item>
///                 When <see cref="LogException" /> is <see langword="true" />, the exception is logged through
///                 <see cref="LogContext" />.
///             </item>
///             <item>
///                 When <see cref="GetStatusCode" /> returns a status code, a <see cref="ProblemDetails" /> is produced
///                 (optionally enriched through <see cref="EnrichProblemDetails" />) and used as the response.
///             </item>
///             <item>
///                 Otherwise <see cref="CreateActionResult" /> is given the chance to produce a custom
///                 <see cref="IActionResult" />.
///             </item>
///         </list>
///     </para>
///     <para>
///         Derived classes customize the behaviour by overriding the relevant <see langword="virtual" /> members; most
///         handlers only need to override <see cref="GetStatusCode" />.
///     </para>
/// </remarks>
/// <typeparam name="THandler">
///     The concrete handler type, used as the category for the <see cref="ILogger{TCategoryName}" />.
/// </typeparam>
/// <typeparam name="TException">The exception type this handler is responsible for.</typeparam>
public abstract class BaseExceptionHandler<THandler, TException> : IExceptionHandler
    where THandler : IExceptionHandler
    where TException : Exception
{
    private readonly IHostEnvironment _environment;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseExceptionHandler{THandler, TException}" /> class.
    /// </summary>
    /// <param name="problemDetailsFactory">The factory used to create the <see cref="ProblemDetails" /> response.</param>
    /// <param name="environment">The host environment, used to decide whether development-only details are exposed.</param>
    protected BaseExceptionHandler(
        ProblemDetailsFactory problemDetailsFactory,
        IHostEnvironment environment)
    {
        _problemDetailsFactory = problemDetailsFactory;
        _environment = environment;
    }

    /// <summary>
    ///     Indicates whether a handled exception should be logged through <see cref="LogContext" />.
    ///     Defaults to <see langword="false" />.
    /// </summary>
    protected virtual bool LogException
        => false;

    /// <summary>
    ///     Indicates whether the current <see cref="IHostEnvironment.EnvironmentName" /> is considered a development
    ///     environment, in which case additional exception details are added to the <see cref="ProblemDetails" />.
    /// </summary>
    protected bool IsDevelopment
        => PpwProblemDetailsFactory.EnvironmentsConsideredAsDevelopment.Contains(_environment.EnvironmentName);

    /// <inheritdoc />
    public bool Handle(ExceptionContext context)
    {
        if (CanHandle(context))
        {
            TException? contextException = context.Exception as TException;
            int? statusCode = GetStatusCode(context, contextException);

            if (LogException)
            {
                ILogger<THandler> logger = CreateLogger<THandler>(context);
                if (statusCode is >= 400 and < 500)
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation(
                            "A {StatusCode} was reported for {Method} {Uri}",
                            statusCode.Value,
                            context.HttpContext.Request.Method,
                            context.HttpContext.Request.GetEncodedPathAndQuery());
                    }
                }
                else
                {
                    LogContext(logger, context);
                }
            }

            if (statusCode is not null)
            {
                ProblemDetails problemDetail =
                    _problemDetailsFactory
                        .CreateProblemDetails(
                            context.HttpContext,
                            statusCode: statusCode.Value);
                if (IsDevelopment && contextException is not null)
                {
                    problemDetail.Extensions.Add("Exception", FormatException(contextException));
                }

                EnrichProblemDetails(context, contextException, problemDetail);
                context.Result = new ObjectResult(problemDetail);
                return true;
            }

            IActionResult? actionResult = CreateActionResult(context);
            if (actionResult is not null)
            {
                context.Result = actionResult;
                return true;
            }
        }

        return false;
    }

    private ILogger<T> CreateLogger<T>(ExceptionContext context)
        where T : IExceptionHandler
    {
        HttpContext httpContext = context.HttpContext;
        ILoggerFactory factory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        return factory.CreateLogger<T>();
    }

    /// <summary>
    ///     Determines whether this handler is able to handle the exception in <paramref name="context" />.
    ///     By default this is the case when the exception is assignable to <typeparamref name="TException" />.
    /// </summary>
    /// <param name="context">The current exception context.</param>
    /// <returns><see langword="true" /> when this handler can handle the exception; otherwise <see langword="false" />.</returns>
    protected virtual bool CanHandle(ExceptionContext context)
        => context.Exception is TException;

    /// <summary>
    ///     Logs the handled exception. Only invoked when <see cref="LogException" /> is <see langword="true" />.
    /// </summary>
    /// <param name="logger">The logger for <typeparamref name="THandler" />.</param>
    /// <param name="context">The current exception context.</param>
    protected virtual void LogContext(ILogger<THandler> logger, ExceptionContext context)
    {
        if (logger.IsEnabled(LogLevel.Error))
        {
            logger.LogError(context.Exception, "Handled exception");
        }
    }

    /// <summary>
    ///     Produces a custom <see cref="IActionResult" /> for the exception. Only invoked when
    ///     <see cref="GetStatusCode" /> returned <see langword="null" />. Returns <see langword="null" /> by default,
    ///     leaving the exception unhandled.
    /// </summary>
    /// <param name="context">The current exception context.</param>
    /// <returns>The result to use as the response, or <see langword="null" /> when no result is produced.</returns>
    protected virtual IActionResult? CreateActionResult(ExceptionContext context)
        => null;

    /// <summary>
    ///     Returns the HTTP status code used to build the <see cref="ProblemDetails" /> response, or
    ///     <see langword="null" /> to fall back to <see cref="CreateActionResult" />. Returns <see langword="null" />
    ///     by default.
    /// </summary>
    /// <param name="context">The current exception context.</param>
    /// <param name="contextException">
    ///     The exception cast to <typeparamref name="TException" />, or <see langword="null" /> when the cast failed.
    /// </param>
    /// <returns>The HTTP status code, or <see langword="null" />.</returns>
    protected virtual int? GetStatusCode(ExceptionContext context, TException? contextException)
        => null;

    /// <summary>
    ///     Allows derived classes to add extra information to the <see cref="ProblemDetails" /> before it is returned.
    ///     Does nothing by default.
    /// </summary>
    /// <param name="context">The current exception context.</param>
    /// <param name="contextException">
    ///     The exception cast to <typeparamref name="TException" />, or <see langword="null" /> when the cast failed.
    /// </param>
    /// <param name="problemDetail">The problem details to enrich.</param>
    protected virtual void EnrichProblemDetails(
        ExceptionContext context,
        TException? contextException,
        ProblemDetails problemDetail)
    {
    }

    /// <summary>
    ///     Builds a structured, serialization-friendly representation of an exception (including its stack trace and
    ///     inner exceptions), added to the <see cref="ProblemDetails" /> extensions when <see cref="IsDevelopment" />
    ///     is <see langword="true" />.
    /// </summary>
    /// <param name="exception">The exception to format.</param>
    /// <returns>An object describing the exception, including its inner exception chain.</returns>
    protected virtual object FormatException(Exception exception)
        => new
           {
               Type = exception.GetType().FullName,
               exception.Message,
               StackTrace =
                   exception
                       .StackTrace?
                       .Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries),
               InnerException =
                   exception.InnerException is { } inner
                       ? FormatException(inner)
                       : null
           };
}
