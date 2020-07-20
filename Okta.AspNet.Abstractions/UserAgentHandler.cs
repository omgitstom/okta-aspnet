// <copyright file="UserAgentHandler.cs" company="Okta, Inc">
// Copyright (c) 2018-present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.AspNet.Abstractions
{
    // TODO: consider renaming this class to OktaHttpMessageHandler, to clarify to future contributors the (expanding) responsibility of this class
    public class UserAgentHandler : DelegatingHandler
    {
        private readonly Lazy<string> _userAgent;

        public UserAgentHandler(string frameworkName, Version frameworkVersion, OktaWebOptions oktaWebOptions = null)
        {
            _userAgent = new Lazy<string>(() => new UserAgentBuilder(frameworkName, frameworkVersion).GetUserAgent());
            if (string.IsNullOrEmpty(oktaWebOptions?.ProxyAddress))
            {
                InnerHandler = new HttpClientHandler();
            }
            else
            {
                InnerHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy
                    {
                        Address = new Uri(oktaWebOptions.ProxyAddress),
                    },
                };
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.ParseAdd(_userAgent.Value);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
