﻿// -----------------------------------------------------------------------------------------
// <copyright file="HttpContentFactory.cs" company="Microsoft">
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

namespace Sandboxable.Microsoft.WindowsAzure.Storage.Shared.Protocol
{
    using Sandboxable.Microsoft.WindowsAzure.Storage.Core;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Core.Executor;
    using System;
    using System.IO;
    using System.Net.Http;

    internal static class HttpContentFactory
    {
        public static HttpContent BuildContentFromStream<T>(Stream stream, long offset, long? length, string md5, RESTCommand<T> cmd, OperationContext operationContext)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            
            HttpContent retContent = new RetryableStreamContent(stream);
            retContent.Headers.ContentLength = length;
#if !PORTABLE
            if (md5 != null)
            {
                retContent.Headers.ContentMD5 = Convert.FromBase64String(md5);
            }
#endif

            return retContent;
        }
    }
}
