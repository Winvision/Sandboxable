﻿//-----------------------------------------------------------------------
// <copyright file="ContainerResultSegment.cs" company="Microsoft">
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

namespace Sandboxable.Microsoft.WindowsAzure.Storage.Blob
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a segment of <see cref="CloudBlobContainer"/> results and contains continuation and pagination information.
    /// </summary>
    public class ContainerResultSegment
    {
        internal ContainerResultSegment(IEnumerable<CloudBlobContainer> containers, BlobContinuationToken continuationToken)
        {
            this.Results = containers;
            this.ContinuationToken = continuationToken;
        }

        /// <summary>
        /// Gets an enumerable collection of <see cref="CloudBlobContainer"/> results.
        /// </summary>
        /// <value>An enumerable collection of <see cref="CloudBlobContainer"/> objects.</value>
        public IEnumerable<CloudBlobContainer> Results { get; private set; }

        /// <summary>
        /// Gets the continuation token used to retrieve the next segment of <see cref="CloudBlobContainer"/> results.
        /// </summary>
        /// <value>A <see cref="BlobContinuationToken"/> object.</value>
        public BlobContinuationToken ContinuationToken { get; private set; }
    }
}
