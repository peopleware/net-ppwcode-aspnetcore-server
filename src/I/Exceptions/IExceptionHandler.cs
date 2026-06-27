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

using Microsoft.AspNetCore.Mvc.Filters;

namespace PPWCode.AspNetCore.Server.I.Exceptions;

/// <summary>
///     Handles an exception captured during request processing and, when applicable, translates it into an
///     HTTP response.
/// </summary>
/// <remarks>
///     Implementations are typically chained: each handler is offered the <see cref="ExceptionContext" /> in turn
///     and signals through the return value of <see cref="Handle" /> whether it took responsibility for the
///     exception.
/// </remarks>
public interface IExceptionHandler
{
    /// <summary>
    ///     Attempts to handle the exception in <paramref name="context" />. When the exception is handled, the
    ///     implementation sets <see cref="ExceptionContext.Result" /> to the response to return.
    /// </summary>
    /// <param name="context">The current exception context.</param>
    /// <returns>
    ///     <see langword="true" /> when this handler handled the exception; otherwise <see langword="false" />,
    ///     leaving the exception for another handler to process.
    /// </returns>
    bool Handle(ExceptionContext context);
}
