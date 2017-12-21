﻿//-----------------------------------------------------------------------
// <copyright file="TableServiceContext.cs" company="Microsoft">
//    Copyright 2013 Microsoft Corporation
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// -----------------------------------------------------------------------------------------

namespace Sandboxable.Microsoft.WindowsAzure.Storage.Table.DataServices
{
    using Sandboxable.Microsoft.WindowsAzure.Storage.Auth.Protocol;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Core;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Core.Auth;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Core.Executor;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Core.Util;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Shared.Protocol;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Table.Protocol;
    using System;
    using System.Data.Services.Client;
    using System.Data.Services.Common;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a <see cref="DataServiceContext"/> object for use with the Windows Azure Table service.
    /// </summary>
    /// <remarks>The <see cref="TableServiceContext"/> class does not support concurrent queries or requests.</remarks>
    [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
    public class TableServiceContext : DataServiceContext, IDisposable
    {
        private IAuthenticationHandler authenticationHandler;

        private TablePayloadFormat payloadFormat = TablePayloadFormat.Json;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableServiceContext"/> class.
        /// </summary>
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public TableServiceContext(CloudTableClient client)
            : base(client.BaseUri, DataServiceProtocolVersion.V3)
        {
            CommonUtility.AssertNotNull("client", client);

            if (client.BaseUri == null)
            {
                throw new ArgumentNullException("client");
            }

            if (!client.BaseUri.IsAbsoluteUri)
            {
                string errorMessage = string.Format(CultureInfo.CurrentCulture, SR.RelativeAddressNotPermitted, client.BaseUri.ToString());

                throw new ArgumentException(errorMessage, "client");
            }

            this.SendingRequest += this.TableServiceContext_SendingRequest;

            this.IgnoreMissingProperties = true;
            this.MergeOption = MergeOption.PreserveChanges;
            this.ServiceClient = client;

            // Since the default is JSON light, this is valid. If users change it to Atom or NoMetadata, this gets updated.
            if (this.payloadFormat == TablePayloadFormat.Json)
            {
                this.Format.UseJson(new TableStorageModel(client.AccountName));
            }
        }

        #region Cancellation Support
        internal void InternalCancel()
        {
            lock (this.cancellationLock)
            {
                this.cancellationRequested = true;
                if (this.currentRequest != null)
                {
                    this.currentRequest.Abort();
                }
            }
        }

        internal void ResetCancellation()
        {
            lock (this.cancellationLock)
            {
                this.cancellationRequested = false;
                this.currentRequest = null;
            }
        }

        private object cancellationLock = new object();

        private bool cancellationRequested = false;

        private HttpWebRequest currentRequest = null;
        #endregion

        #region Signing + Execution

        // Action to hook up response header parsing
        private Action<HttpWebRequest> sendingSignedRequestAction;

        internal Action<HttpWebRequest> SendingSignedRequestAction
        {
            get { return this.sendingSignedRequestAction; }
            set { this.sendingSignedRequestAction = value; }
        }

        // Only one concurrent operation per context is supported.
        private Semaphore contextSemaphore = new Semaphore(1, 1);

        internal Semaphore ContextSemaphore
        {
            get { return this.contextSemaphore; }
        }

        /// <summary>
        /// Callback on DataContext object sending request.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Data.Services.Client.SendingRequestEventArgs"/> instance containing the event data.</param>       
        private void TableServiceContext_SendingRequest(object sender, SendingRequestEventArgs e)
        {
            HttpWebRequest request = e.Request as HttpWebRequest;
            
            // Check timeout
            int timeoutDex = request.RequestUri.Query.LastIndexOf("&timeout=", System.StringComparison.Ordinal);
            if (timeoutDex > 0)
            {
                timeoutDex += 9; // Magic number -> length of "&timeout="
                int endDex = request.RequestUri.Query.IndexOf('&', timeoutDex);
                string timeoutString = endDex > 0
                                           ? request.RequestUri.Query.Substring(timeoutDex, endDex - timeoutDex)
                                           : request.RequestUri.Query.Substring(timeoutDex);

                int result = -1;
                if (int.TryParse(timeoutString, out result) && result > 0)
                {
                    request.Timeout = result * 1000; // Convert to ms
                }
            }

            // Sign request
            if (this.ServiceClient.Credentials.IsSharedKey)
            {
                this.AuthenticationHandler.SignRequest(request, null /* operationContext */);
            }
            else if (this.ServiceClient.Credentials.IsSAS)
            {
                Uri transformedUri = this.ServiceClient.Credentials.TransformUri(request.RequestUri);

                // Recreate the request
                HttpWebRequest newRequest = WebRequest.Create(transformedUri) as HttpWebRequest;
                TableUtilities.CopyRequestData(newRequest, request);
                e.Request = newRequest;
                request = newRequest;
            }

            lock (this.cancellationLock)
            {
                if (this.cancellationRequested)
                {
                    throw new OperationCanceledException(SR.OperationCanceled);
                }

                this.currentRequest = request;
            }

            if (!this.ServiceClient.Credentials.IsSAS)
            {
                // SAS will be handled directly by the queries themselves prior to transformation
                request.Headers.Add(
                    Constants.HeaderConstants.StorageVersionHeader,
                    Constants.HeaderConstants.TargetStorageVersion);
            }

            CommonUtility.ApplyRequestOptimizations(request, -1);

            if (this.sendingSignedRequestAction != null)
            {
                this.sendingSignedRequestAction(request);
            }
        }
        #endregion

        /// <summary>
        /// Gets the <see cref="CloudTableClient"/> object that represents the Table service.
        /// </summary>
        /// <value>A client object that specifies the Table service endpoint.</value>
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public CloudTableClient ServiceClient { get; private set; }

        /// <summary>
        /// Gets the authentication handler used to sign HTTP requests.
        /// </summary>
        /// <value>The authentication handler.</value>
        private IAuthenticationHandler AuthenticationHandler
        {
            get
            {
                if (this.authenticationHandler == null)
                {
                    if (this.ServiceClient.Credentials.IsSharedKey)
                    {
                        // Always use Shared Key Lite because Data Services adds a Content-Type HTTP header to requests 
                        // that is not available during signing
                        this.authenticationHandler = new SharedKeyAuthenticationHandler(
                            SharedKeyLiteTableCanonicalizer.Instance,
                            this.ServiceClient.Credentials,
                            this.ServiceClient.Credentials.AccountName);
                    }
                    else
                    {
                        this.authenticationHandler = new NoOpAuthenticationHandler();
                    }
                }

                return this.authenticationHandler;
            }
        }

        /// <summary>
        /// Saves changes, using the retry policy specified for the service context.
        /// </summary>
        /// <returns>A <see cref="DataServiceResponse"/> that represents the result of the operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public DataServiceResponse SaveChangesWithRetries()
        {
            return this.SaveChangesWithRetries(this.SaveChangesDefaultOptions);
        }

        /// <summary>
        /// Saves changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="options">A <see cref="System.Data.Services.Client.SaveChangesOptions"/> enumeration value.</param>
        /// <param name="requestOptions">A <see cref="TableRequestOptions"/> object that specifies additional options for the request.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns> A <see cref="DataServiceResponse"/> that represents the result of the operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public DataServiceResponse SaveChangesWithRetries(SaveChangesOptions options, TableRequestOptions requestOptions = null, OperationContext operationContext = null)
        {
            requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
            operationContext = operationContext ?? new OperationContext();
            TableCommand<DataServiceResponse, DataServiceResponse> cmd = this.GenerateSaveChangesCommand(options, requestOptions);
            return TableExecutor.ExecuteSync(cmd, requestOptions.RetryPolicy, operationContext);
        }

        /// <summary>
        /// Begins an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="callback">An <see cref="AsyncCallback"/> delegate that will receive notification when the asynchronous operation completes.</param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>An <see cref="IAsyncResult"/> that references the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public ICancellableAsyncResult BeginSaveChangesWithRetries(AsyncCallback callback, object state)
        {
            return this.BeginSaveChangesWithRetries(this.SaveChangesDefaultOptions, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="options">A <see cref="System.Data.Services.Client.SaveChangesOptions"/> enumeration value.</param>
        /// <param name="callback">An <see cref="AsyncCallback"/> delegate that will receive notification when the asynchronous operation completes.</param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>An <see cref="IAsyncResult"/> that references the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public ICancellableAsyncResult BeginSaveChangesWithRetries(SaveChangesOptions options, AsyncCallback callback, object state)
        {
            return this.BeginSaveChangesWithRetries(options, null /* RequestOptions */, null /* OperationContext */, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="options">A <see cref="System.Data.Services.Client.SaveChangesOptions"/> enumeration value.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <param name="callback">An <see cref="AsyncCallback"/> delegate that will receive notification when the asynchronous operation completes.</param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <param name="requestOptions"> </param>
        /// <returns>An <see cref="IAsyncResult"/> that references the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public ICancellableAsyncResult BeginSaveChangesWithRetries(SaveChangesOptions options, TableRequestOptions requestOptions, OperationContext operationContext, AsyncCallback callback, object state)
        {
            requestOptions = TableRequestOptions.ApplyDefaults(requestOptions, this.ServiceClient);
            operationContext = operationContext ?? new OperationContext();
            TableCommand<DataServiceResponse, DataServiceResponse> cmd = this.GenerateSaveChangesCommand(options, requestOptions);
            return TableExecutor.BeginExecuteAsync(cmd, requestOptions.RetryPolicy, operationContext, callback, state);
        }

        /// <summary>
        /// Ends an asynchronous operation to save changes.
        /// </summary>
        /// <param name="asyncResult">An <see cref="IAsyncResult"/> that references the pending asynchronous operation.</param>
        /// <returns> A <see cref="DataServiceResponse"/> that represents the result of the operation.</returns>
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public DataServiceResponse EndSaveChangesWithRetries(IAsyncResult asyncResult)
        {
            return TableExecutor.EndExecuteAsync<DataServiceResponse, DataServiceResponse>(asyncResult);
        }
        
#if TASK
        /// <summary>
        /// Initiates an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <returns>A <see cref="Task{T}"/> object that represents the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public Task<DataServiceResponse> SaveChangesWithRetriesAsync()
        {
            return this.SaveChangesWithRetriesAsync(CancellationToken.None);
        }

        /// <summary>
        /// Initiates an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A <see cref="Task{T}"/> object that represents the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public Task<DataServiceResponse> SaveChangesWithRetriesAsync(CancellationToken cancellationToken)
        {
            return AsyncExtensions.TaskFromApm(this.BeginSaveChangesWithRetries, this.EndSaveChangesWithRetries, cancellationToken);
        }
        
        /// <summary>
        /// Initiates an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="options">A <see cref="System.Data.Services.Client.SaveChangesOptions"/> enumeration value.</param>
        /// <returns>A <see cref="Task{T}"/> object that represents the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public Task<DataServiceResponse> SaveChangesWithRetriesAsync(SaveChangesOptions options)
        {
            return this.SaveChangesWithRetriesAsync(options, CancellationToken.None);
        }

        /// <summary>
        /// Initiates an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="options">A <see cref="System.Data.Services.Client.SaveChangesOptions"/> enumeration value.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A <see cref="Task{T}"/> object that represents the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public Task<DataServiceResponse> SaveChangesWithRetriesAsync(SaveChangesOptions options, CancellationToken cancellationToken)
        {
            return AsyncExtensions.TaskFromApm(this.BeginSaveChangesWithRetries, this.EndSaveChangesWithRetries, options, cancellationToken);
        }
        
        /// <summary>
        /// Initiates an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="options">A <see cref="System.Data.Services.Client.SaveChangesOptions"/> enumeration value.</param>
        /// <param name="requestOptions">A <see cref="TableRequestOptions"/> object that specifies additional options for the request.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns>A <see cref="Task{T}"/> object that represents the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public Task<DataServiceResponse> SaveChangesWithRetriesAsync(SaveChangesOptions options, TableRequestOptions requestOptions, OperationContext operationContext)
        {
            return this.SaveChangesWithRetriesAsync(options, requestOptions, operationContext, CancellationToken.None);
        }
        
        /// <summary>
        /// Initiates an asynchronous operation to save changes, using the retry policy specified for the service context.
        /// </summary>
        /// <param name="options">A <see cref="System.Data.Services.Client.SaveChangesOptions"/> enumeration value.</param>
        /// <param name="requestOptions">A <see cref="TableRequestOptions"/> object that specifies additional options for the request.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
        /// <returns>A <see cref="Task{T}"/> object that represents the asynchronous operation.</returns>
        [DoesServiceRequest]
        [Obsolete("Support for accessing Windows Azure Tables via WCF Data Services is now obsolete. It's recommended that you use the Microsoft.WindowsAzure.Storage.Table namespace for working with tables.")]
        public Task<DataServiceResponse> SaveChangesWithRetriesAsync(SaveChangesOptions options, TableRequestOptions requestOptions, OperationContext operationContext, CancellationToken cancellationToken)
        {
            return AsyncExtensions.TaskFromApm(this.BeginSaveChangesWithRetries, this.EndSaveChangesWithRetries, options, requestOptions, operationContext, cancellationToken);
        }
#endif

        internal TableCommand<DataServiceResponse, DataServiceResponse> GenerateSaveChangesCommand(SaveChangesOptions options, TableRequestOptions requestOptions)
        {
            TableCommand<DataServiceResponse, DataServiceResponse> cmd = new TableCommand<DataServiceResponse, DataServiceResponse>();

            if (requestOptions.ServerTimeout.HasValue)
            {
                this.Timeout = (int)requestOptions.ServerTimeout.Value.TotalSeconds;
            }

            cmd.ExecuteFunc = () => this.SaveChanges(options);
            cmd.Begin = (callback, state) => this.BeginSaveChanges(options, callback, state);
            cmd.End = this.EndSaveChanges;
            cmd.ParseResponse = this.ParseDataServiceResponse;
            cmd.ParseDataServiceError = ODataErrorHelper.ReadDataServiceResponseFromStream;
            cmd.Context = this;
            requestOptions.ApplyToStorageCommand(cmd);

            return cmd;
        }

        private DataServiceResponse ParseDataServiceResponse(DataServiceResponse resp, RequestResult reqResult, TableCommand<DataServiceResponse, DataServiceResponse> cmd)
        {
            if (reqResult.Exception != null)
            {
                throw reqResult.Exception;
            }

            return resp;
        }

        /// <summary>
        /// Releases all resources used by the TableServiceContext.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the TableServiceContext and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.contextSemaphore != null)
                {
                    this.contextSemaphore.Dispose();
                    this.contextSemaphore = null;
                }
            }
        }
    }
}