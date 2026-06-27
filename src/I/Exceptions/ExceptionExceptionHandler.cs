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

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

/// <inheritdoc />
/// <remarks>
///     <inheritdoc cref="BaseExceptionHandler{ExceptionExceptionHandler,Exception}" path="/remarks/node()" />
///     <para>
///         Use this exception handler to handle all exceptions, such as unhandled exceptions. This should be registered
///         last.
///     </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class ExceptionExceptionHandler : BaseExceptionHandler<ExceptionExceptionHandler, Exception>
{
    /// <inheritdoc />
    public ExceptionExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IHostEnvironment environment)
        : base(problemDetailsFactory, environment)
    {
    }

    /// <inheritdoc />
    protected override bool LogException
        => true;

    /// <inheritdoc />
    protected override int? GetStatusCode(ExceptionContext context, Exception? contextException)
        => StatusCodes.Status500InternalServerError;
}
