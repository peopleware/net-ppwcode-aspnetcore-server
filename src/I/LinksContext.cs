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

using Asp.Versioning;

namespace PPWCode.AspNetCore.Server.I;

/// <summary>
///     Base class that carries the API version information needed to generate version-aware links.
/// </summary>
public abstract class LinksContext
{
    /// <summary>
    ///     The name of the route parameter that holds the API version.
    /// </summary>
    public const string VersionRouteParameter = "version";

    /// <summary>
    ///     The default format used to render the <see cref="ApiVersion" /> into a route value.
    /// </summary>
    public const string DefaultApiVersionFormat = "V";

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinksContext" /> class.
    /// </summary>
    /// <param name="apiVersion">The API version for which links are generated.</param>
    /// <param name="apiVersionFormat">
    ///     The format used to render <paramref name="apiVersion" /> into a route value, or <see langword="null" /> to use
    ///     <see cref="DefaultApiVersionFormat" />.
    /// </param>
    protected LinksContext(ApiVersion apiVersion, string? apiVersionFormat = null)
    {
        ApiVersion = apiVersion;
        ApiVersionFormat = apiVersionFormat ?? DefaultApiVersionFormat;
    }

    /// <summary>
    ///     The API version for which links are generated.
    /// </summary>
    public ApiVersion ApiVersion { get; }

    /// <summary>
    ///     The format used to render the <see cref="ApiVersion" /> into a route value.
    /// </summary>
    public string ApiVersionFormat { get; }
}
