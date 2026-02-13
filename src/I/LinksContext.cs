// Copyright 2025 by PeopleWare n.v..
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

public abstract class LinksContext
{
    public const string VersionRouteParameter = "version";
    public const string DefaultApiVersionFormat = "V";

    protected LinksContext(
        ApiVersion apiVersion,
        string? apiVersionFormat = null)
    {
        ApiVersion = apiVersion;
        ApiVersionFormat = apiVersionFormat ?? DefaultApiVersionFormat;
    }

    public ApiVersion ApiVersion { get; }

    public string ApiVersionFormat { get; }
}
