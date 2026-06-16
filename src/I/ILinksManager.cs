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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace PPWCode.AspNetCore.Server.I;

/// <summary>
///     Generates URLs for named routes, abstracting the underlying <see cref="IUrlHelper" />.
/// </summary>
public interface ILinksManager
{
    /// <summary>
    ///     Generates a URL with a relative path for the specified route.
    /// </summary>
    /// <param name="routeName">The name of the route used to generate the URL.</param>
    /// <param name="routeValues">An object that contains the route values.</param>
    /// <returns>The generated URL, or <see langword="null" /> when a URL could not be generated.</returns>
    /// <seealso cref="IUrlHelper.RouteUrl(UrlRouteContext)" />
    string? RouteUrl(string routeName, object? routeValues);

    /// <summary>
    ///     Generates an absolute URL for the specified route.
    /// </summary>
    /// <param name="routeName">The name of the route used to generate the URL.</param>
    /// <param name="routeValues">An object that contains the route values.</param>
    /// <returns>The generated absolute URL, or <see langword="null" /> when a URL could not be generated.</returns>
    /// <seealso cref="IUrlHelper.Link(string, object)" />
    string? Link(string routeName, object? routeValues);
}
