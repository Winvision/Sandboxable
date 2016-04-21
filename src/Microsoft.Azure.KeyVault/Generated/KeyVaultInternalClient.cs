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
using System.Net.Http;

namespace Sandboxable.Microsoft.Azure.KeyVault.Internal
{
    internal class KeyVaultInternalClient
    {
        /// <summary>
        /// Gets the API version.
        /// </summary>
        public string ApiVersion
        {
            get;
        }

        /// <summary>
        /// Gets the URI used as the base for all cloud service requests.
        /// </summary>
        public Uri BaseUri
        {
            get;
        }

        /// <summary>
        /// Gets or sets the credential
        /// </summary>
        public KeyVaultCredential Credentials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the initial timeout for Long Running Operations.
        /// </summary>
        public int LongRunningOperationInitialTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the retry timeout for Long Running Operations.
        /// </summary>
        public int LongRunningOperationRetryTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Operations for secrets in a vault
        /// </summary>
        public virtual SecretOperations Secrets
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the KeyVaultInternalClient class.
        /// </summary>
        public KeyVaultInternalClient()
        {
            this.Secrets = new SecretOperations(this);
            this.ApiVersion = "2015-06-01";
            this.LongRunningOperationInitialTimeout = -1;
            this.LongRunningOperationRetryTimeout = -1;
            this.HttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(300)
            };
        }
        
        public HttpClient HttpClient
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the KeyVaultInternalClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets or sets the credential
        /// </param>
        /// <param name='baseUri'>
        /// Optional. Gets the URI used as the base for all cloud service
        /// requests.
        /// </param>
        public KeyVaultInternalClient(KeyVaultCredential credentials, Uri baseUri)
            : this()
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }

            this.Credentials = credentials;
            this.BaseUri = baseUri;
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultInternalClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets or sets the credential
        /// </param>
        public KeyVaultInternalClient(KeyVaultCredential credentials)
            : this()
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            this.Credentials = credentials;
            this.BaseUri = null;
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultInternalClient class.
        /// </summary>
        /// <param name='httpClient'>
        /// The Http client
        /// </param>
        public KeyVaultInternalClient(HttpClient httpClient)
        {
            this.Secrets = new SecretOperations(this);
            this.ApiVersion = "2015-06-01";
            this.LongRunningOperationInitialTimeout = -1;
            this.LongRunningOperationRetryTimeout = -1;
            this.HttpClient = httpClient;
            this.HttpClient.Timeout = TimeSpan.FromSeconds(300);
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultInternalClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets or sets the credential
        /// </param>
        /// <param name='baseUri'>
        /// Optional. Gets the URI used as the base for all cloud service
        /// requests.
        /// </param>
        /// <param name='httpClient'>
        /// The Http client
        /// </param>
        public KeyVaultInternalClient(KeyVaultCredential credentials, Uri baseUri, HttpClient httpClient)
            : this(httpClient)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }
            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }

            this.Credentials = credentials;
            this.BaseUri = baseUri;
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultInternalClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets or sets the credential
        /// </param>
        /// <param name='httpClient'>
        /// The Http client
        /// </param>
        public KeyVaultInternalClient(KeyVaultCredential credentials, HttpClient httpClient)
            : this(httpClient)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            this.Credentials = credentials;
            this.BaseUri = null;
        }
    }
}