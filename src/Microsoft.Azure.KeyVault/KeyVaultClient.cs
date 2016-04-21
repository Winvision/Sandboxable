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
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sandboxable.Hyak.Common;
using Sandboxable.Microsoft.Azure.KeyVault.Internal;

namespace Sandboxable.Microsoft.Azure.KeyVault
{
    public class KeyVaultClient
    {
        /// <summary>
        /// The authentication callback delegate which is to be implemented by the client code
        /// </summary>
        /// <param name="authority"> Identifier of the authority, a URL. </param>
        /// <param name="resource"> Identifier of the target resource that is the recipient of the requested token, a URL. </param>
        /// <param name="scope"> The scope of the authentication request. </param>
        /// <returns> access token </returns>
        public delegate Task<string> AuthenticationCallback(string authority, string resource, string scope);

        private readonly KeyVaultInternalClient internalClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authenticationCallback">The authentication callback</param>
        public KeyVaultClient(AuthenticationCallback authenticationCallback)
        {
            var credential = new KeyVaultCredential(authenticationCallback);
            this.internalClient = new KeyVaultInternalClient(credential);
        }

        /// <summary>
        /// Gets a secret.
        /// </summary>
        /// <param name="secretIdentifier">The URL for the secret.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A response message containing the secret</returns>
        public async Task<Secret> GetSecretAsync(string secretIdentifier, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(secretIdentifier))
            {
                throw new ArgumentNullException(nameof(secretIdentifier));
            }

            return await this.Do(async () =>
            {
                var response = await this.internalClient.Secrets.GetAsync(secretIdentifier, cancellationToken).ConfigureAwait(false);

                return JsonConvert.DeserializeObject<Secret>(response.Response);

            }).ConfigureAwait(false);
        }

        private async Task<T> Do<T>(Func<Task<T>> func)
        {
            try
            {
                return await func().ConfigureAwait(false);
            }
            catch (CloudException cloudException)
            {
                ErrorResponseMessage error;

                var errorText = cloudException.Response.Content;

                try
                {
                    error = JsonConvert.DeserializeObject<ErrorResponseMessage>(errorText);
                }
                catch (Exception)
                {
                    // Error deserialization failed, attempt to get some data for the client
                    error = new ErrorResponseMessage
                    {
                        Error = new Error
                        {
                            Code = "Unknown",
                            Message = $"HTTP {cloudException.Response.StatusCode}: {cloudException.Response.ReasonPhrase}. Details: {errorText}",
                        },
                    };
                }

                throw new KeyVaultClientException(cloudException.Response.StatusCode, cloudException.Request.RequestUri, error?.Error);
            }
        }
    }
}