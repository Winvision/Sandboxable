﻿//-----------------------------------------------------------------------
// <copyright file="TableHttpWebResponseParsers.cs" company="Microsoft">
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

namespace Sandboxable.Microsoft.WindowsAzure.Storage.Table.Protocol
{
    using Sandboxable.Microsoft.WindowsAzure.Storage.Core.Util;
    using Sandboxable.Microsoft.WindowsAzure.Storage.Shared.Protocol;
    using System.IO;
    using System.Net;

    /// <summary>
    /// Provides a set of methods for parsing a response stream from the Table service.
    /// </summary>
    public static class TableHttpWebResponseParsers
    {
        /// <summary>
        /// Gets the request ID from the response.
        /// </summary>
        /// <param name="response">The web response.</param>
        /// <returns>A unique value associated with the request.</returns>
        public static string GetRequestId(HttpWebResponse response)
        {
            return Response.GetRequestId(response);
        }

        /// <summary>
        /// Reads service properties from a stream.
        /// </summary>
        /// <param name="inputStream">The stream from which to read the service properties.</param>
        /// <returns>The service properties stored in the stream.</returns>
        public static ServiceProperties ReadServiceProperties(Stream inputStream)
        {
            return HttpResponseParsers.ReadServiceProperties(inputStream);
        }

        /// <summary>
        /// Reads service stats from a stream.
        /// </summary>
        /// <param name="inputStream">The stream from which to read the service stats.</param>
        /// <returns>The service stats stored in the stream.</returns>
        public static ServiceStats ReadServiceStats(Stream inputStream)
        {
            return HttpResponseParsers.ReadServiceStats(inputStream);
        }

        /// <summary>
        /// Reads the share access policies from a stream in XML.
        /// </summary>
        /// <param name="inputStream">The stream of XML policies.</param>
        /// <param name="permissions">The permissions object to which the policies are to be written.</param>
        public static void ReadSharedAccessIdentifiers(Stream inputStream, TablePermissions permissions)
        {
            CommonUtility.AssertNotNull("permissions", permissions);

            HttpResponseParsers.ReadSharedAccessIdentifiers(permissions.SharedAccessPolicies, new TableAccessPolicyResponse(inputStream));
        }
    }
}