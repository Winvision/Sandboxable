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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Sandboxable.Hyak.Common;

namespace Sandboxable.Microsoft.Azure.KeyVault.Internal
{
    internal class SecretOperations
    {
        /// <summary>
        /// Initializes a new instance of the SecretOperations class.
        /// </summary>
        /// <param name='client'>
        /// Reference to the service client.
        /// </param>
        internal SecretOperations(KeyVaultInternalClient client)
        {
            this.Client = client;
        }

        /// <summary>
        /// Gets a reference to the
        /// Microsoft.Azure.KeyVault.Internal.KeyVaultInternalClient.
        /// </summary>
        public KeyVaultInternalClient Client
        {
            get;
        }

        /// <summary>
        /// Gets a secret
        /// </summary>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public async Task<SecretResponseMessageWithRawJsonContent> GetAsync(string secretIdentifier, CancellationToken cancellationToken)
        {
            // Validate
            if (secretIdentifier == null)
            {
                throw new ArgumentNullException(nameof(secretIdentifier));
            }
            
            // Construct URL
            var url = "" + secretIdentifier;

            var queryParameters = new List<string>
            {
                "api-version=2015-06-01"
            };

            if (queryParameters.Count > 0)
            {
                url = url + "?" + string.Join("&", queryParameters);
            }

            url = url.Replace(" ", "%20");
            
            // Create HTTP transport objects
            HttpRequestMessage httpRequest = null;
            try
            {
                httpRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                // Set Headers
                httpRequest.Headers.Add("Accept", "application/json");
                httpRequest.Headers.Add("client-request-id", Guid.NewGuid().ToString());
                
                // Set Credentials
                cancellationToken.ThrowIfCancellationRequested();
                await this.Client.Credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
                
                // Send Request
                HttpResponseMessage httpResponse = null;
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    httpResponse = await this.Client.HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);

                    var statusCode = httpResponse.StatusCode;
                    if (statusCode >= HttpStatusCode.BadRequest)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var ex = CloudException.Create(httpRequest, null, httpResponse, await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false));
                        throw ex;
                    }
                    
                    // Deserialize Response
                    cancellationToken.ThrowIfCancellationRequested();

                    var responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var result = new SecretResponseMessageWithRawJsonContent
                    {
                        Response = responseContent,
                        StatusCode = statusCode
                    };

                    return result;
                }
                finally
                {
                    httpResponse?.Dispose();
                }
            }
            finally
            {
                httpRequest?.Dispose();
            }
        }
    }
}