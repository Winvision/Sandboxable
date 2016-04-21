//
// Copyright © Microsoft Corporation, All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
// ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
// PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache License, Version 2.0 for the specific language
// governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Sandboxable.Hyak.Common;

namespace Sandboxable.Microsoft.Azure.KeyVault
{
    public class KeyVaultCredential : CloudCredentials
    {        
        public event KeyVaultClient.AuthenticationCallback OnAuthenticate;

        public string Token { get; set; }

        public KeyVaultCredential(KeyVaultClient.AuthenticationCallback authenticationCallback)
        {
            this.OnAuthenticate = authenticationCallback;
        }

        private async Task<string> PreAuthenticate(Uri url)
        {
            if (this.OnAuthenticate != null)
            {
                var challenge = HttpBearerChallengeCache.GetInstance().GetChallengeForURL(url);

                if (challenge != null)
                {
                    return await this.OnAuthenticate(challenge.AuthorizationServer, challenge.Resource, challenge.Scope).ConfigureAwait(false);
                }
            }

            return null;
        }

        protected async Task<string> PostAuthenticate(HttpResponseMessage response)
        {
            // An HTTP 401 Not Authorized error; handle if an authentication callback has been supplied
            if (this.OnAuthenticate != null)
            {
                // Extract the WWW-Authenticate header and determine if it represents an OAuth2 Bearer challenge
                var authenticateHeader = response.Headers.WwwAuthenticate.ElementAt(0).ToString();

                if (HttpBearerChallenge.IsBearerChallenge(authenticateHeader))
                {
                    var challenge = new HttpBearerChallenge(response.RequestMessage.RequestUri, authenticateHeader);
                    
                    // Update challenge cache
                    HttpBearerChallengeCache.GetInstance().SetChallengeForURL(response.RequestMessage.RequestUri, challenge);

                    // We have an authentication challenge, use it to get a new authorization token
                    return await this.OnAuthenticate(challenge.AuthorizationServer, challenge.Resource, challenge.Scope).ConfigureAwait(false);
                }
            }

            return null;
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var accessToken = await this.PreAuthenticate(request.RequestUri).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                HttpResponseMessage response;
                var client = new HttpClient();
                using (var r = new HttpRequestMessage(request.Method, request.RequestUri))
                {                    
                    response = await client.SendAsync(r, cancellationToken).ConfigureAwait(false);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    accessToken = await this.PostAuthenticate(response).ConfigureAwait(false);

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                }
            }                          
        }
    }
}