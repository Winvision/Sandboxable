﻿// 
// Copyright (c) Microsoft and contributors.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the License for the specific language governing permissions and
// limitations under the License.
// 

// Warning: This code was generated by a tool.
// 
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using System;
using System.Net.Http;
using Sandboxable.Hyak.Common;
using Sandboxable.Microsoft.Azure;

namespace Sandboxable.Microsoft.Azure.Management.KeyVault
{
    /// <summary>
    /// The Windows Azure management API provides a RESTful set of web services
    /// that interact with Azure Key Vault.
    /// </summary>
    public class KeyVaultManagementClient : ServiceClient<KeyVaultManagementClient>, IKeyVaultManagementClient
    {
        /// <summary>
        /// Gets the API version.
        /// </summary>
        public string ApiVersion { get; }

        /// <summary>
        /// Gets the URI used as the base for all cloud service requests.
        /// </summary>
        public Uri BaseUri { get; }

        /// <summary>
        /// Gets subscription credentials which uniquely identify Microsoft
        /// Azure subscription. The subscription ID forms part of the URI for
        /// every service call.
        /// </summary>
        public SubscriptionCloudCredentials Credentials { get; }

        /// <summary>
        /// Gets or sets the initial timeout for Long Running Operations.
        /// </summary>
        public int LongRunningOperationInitialTimeout { get; set; }

        /// <summary>
        /// Gets or sets the retry timeout for Long Running Operations.
        /// </summary>
        public int LongRunningOperationRetryTimeout { get; set; }

        /// <summary>
        /// Vault operations
        /// </summary>
        public virtual IVaultOperations Vaults { get; }

        /// <summary>
        /// Initializes a new instance of the KeyVaultManagementClient class.
        /// </summary>
        public KeyVaultManagementClient()
            : base()
        {
            this.Vaults = new VaultOperations(this);
            this.ApiVersion = "2014-04-01";
            this.LongRunningOperationInitialTimeout = -1;
            this.LongRunningOperationRetryTimeout = -1;
            this.HttpClient.Timeout = TimeSpan.FromSeconds(300);
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultManagementClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets subscription credentials which uniquely identify
        /// Microsoft Azure subscription. The subscription ID forms part of
        /// the URI for every service call.
        /// </param>
        /// <param name='baseUri'>
        /// Optional. Gets the URI used as the base for all cloud service
        /// requests.
        /// </param>
        public KeyVaultManagementClient(SubscriptionCloudCredentials credentials, Uri baseUri)
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
            
            this.Credentials.InitializeServiceClient(this);
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultManagementClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets subscription credentials which uniquely identify
        /// Microsoft Azure subscription. The subscription ID forms part of
        /// the URI for every service call.
        /// </param>
        public KeyVaultManagementClient(SubscriptionCloudCredentials credentials)
            : this()
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            this.Credentials = credentials;
            this.BaseUri = new Uri("https://management.azure.com");
            
            this.Credentials.InitializeServiceClient(this);
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultManagementClient class.
        /// </summary>
        /// <param name='httpClient'>
        /// The Http client
        /// </param>
        public KeyVaultManagementClient(HttpClient httpClient)
            : base(httpClient)
        {
            this.Vaults = new VaultOperations(this);
            this.ApiVersion = "2014-04-01";
            this.LongRunningOperationInitialTimeout = -1;
            this.LongRunningOperationRetryTimeout = -1;
            this.HttpClient.Timeout = TimeSpan.FromSeconds(300);
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultManagementClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets subscription credentials which uniquely identify
        /// Microsoft Azure subscription. The subscription ID forms part of
        /// the URI for every service call.
        /// </param>
        /// <param name='baseUri'>
        /// Optional. Gets the URI used as the base for all cloud service
        /// requests.
        /// </param>
        /// <param name='httpClient'>
        /// The Http client
        /// </param>
        public KeyVaultManagementClient(SubscriptionCloudCredentials credentials, Uri baseUri, HttpClient httpClient)
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
            
            this.Credentials.InitializeServiceClient(this);
        }
        
        /// <summary>
        /// Initializes a new instance of the KeyVaultManagementClient class.
        /// </summary>
        /// <param name='credentials'>
        /// Required. Gets subscription credentials which uniquely identify
        /// Microsoft Azure subscription. The subscription ID forms part of
        /// the URI for every service call.
        /// </param>
        /// <param name='httpClient'>
        /// The Http client
        /// </param>
        public KeyVaultManagementClient(SubscriptionCloudCredentials credentials, HttpClient httpClient)
            : this(httpClient)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            this.Credentials = credentials;
            this.BaseUri = new Uri("https://management.azure.com");
            
            this.Credentials.InitializeServiceClient(this);
        }
    }
}

