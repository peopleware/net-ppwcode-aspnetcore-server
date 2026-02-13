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

using Microsoft.AspNetCore.Mvc;

namespace PPWCode.AspNetCore.Server.I;

public interface ILinksManager
{
    /// <inheritdoc cref="IUrlHelper.RouteUrl" />
    string? RouteUrl(string routeName, object? routeValues);

    /// <inheritdoc cref="IUrlHelper.Link" />
    string? Link(string routeName, object? routeValues);
}
