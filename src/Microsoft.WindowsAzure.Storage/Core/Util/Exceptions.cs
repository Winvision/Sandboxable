//-----------------------------------------------------------------------
// <copyright file="Exceptions.cs" company="Microsoft">
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
//-----------------------------------------------------------------------

namespace Sandboxable.Microsoft.WindowsAzure.Storage.Core.Util
{
    using System;

#if WINDOWS_RT || ASPNET_K || PORTABLE
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Shared.Protocol;
#endif

    internal class Exceptions
    {

        internal static StorageException GenerateTimeoutException(RequestResult res, Exception inner)
        {
            if (res != null)
            {
                res.HttpStatusCode = 408; // RequestTimeout
            }

            TimeoutException timeoutEx = new TimeoutException(SR.TimeoutExceptionMessage, inner);
            return new StorageException(res, timeoutEx.Message, timeoutEx)
            {
                IsRetryable = false
            };
        }

        internal static StorageException GenerateCancellationException(RequestResult res, Exception inner)
        {
            if (res != null)
            {
                res.HttpStatusCode = 306;
                res.HttpStatusMessage = "Unused";
            }

            OperationCanceledException cancelEx = new OperationCanceledException(SR.OperationCanceled, inner);
            return new StorageException(res, cancelEx.Message, cancelEx) { IsRetryable = false };
        }
    }
}
