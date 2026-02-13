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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

using PPWCode.Vernacular.Contracts.I;

using Endpoint = Microsoft.AspNetCore.Http.Endpoint;

namespace PPWCode.AspNetCore.Server.I;

public class LinksManager : ILinksManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly object _locker = new();
    private IUrlHelper? _urlHelper;

    public LinksManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public string? RouteUrl(string routeName, object? routeValues)
        => GetUrlHelper().RouteUrl(routeName, routeValues);

    /// <inheritdoc />
    public string? Link(string routeName, object? routeValues)
        => GetUrlHelper().Link(routeName, routeValues);

    private IUrlHelper GetUrlHelper()
    {
        if (_urlHelper is null)
        {
            lock (_locker)
            {
                if (_urlHelper is null)
                {
                    if (_httpContextAccessor.HttpContext is not null)
                    {
                        Endpoint? endpoint = _httpContextAccessor.HttpContext.GetEndpoint();
                        if (endpoint is not null)
                        {
                            RouteData routeData = new();
                            IDataTokensMetadata? dataTokens = endpoint.Metadata.GetMetadata<IDataTokensMetadata>();
                            routeData.PushState(router: null, _httpContextAccessor.HttpContext.Request.RouteValues, new RouteValueDictionary(dataTokens?.DataTokens));
                            ActionDescriptor? action = endpoint.Metadata.GetMetadata<ActionDescriptor>();
                            if (action is not null)
                            {
                                ActionContext actionContext = new(_httpContextAccessor.HttpContext, routeData, action);
                                IServiceProvider services = _httpContextAccessor.HttpContext.RequestServices;
                                _urlHelper =
                                    services
                                        .GetRequiredService<IUrlHelperFactory>()
                                        .GetUrlHelper(actionContext);
                            }
                        }
                    }
                }
            }

            Contract.Assert(_urlHelper is not null);
        }

        return _urlHelper;
    }
}
