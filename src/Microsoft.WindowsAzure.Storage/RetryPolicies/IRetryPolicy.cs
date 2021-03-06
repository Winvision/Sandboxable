﻿// -----------------------------------------------------------------------------------------
// <copyright file="IRetryPolicy.cs" company="Microsoft">
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

namespace Sandboxable.Microsoft.WindowsAzure.Storage.RetryPolicies
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a retry policy.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>
        /// Generates a new retry policy for the current request attempt.
        /// </summary>
        /// <returns>An <see cref="IRetryPolicy"/> object that represents the retry policy for the current request attempt.</returns>        
        IRetryPolicy CreateInstance();

        /// <summary>
        /// Determines whether the operation should be retried and the interval until the next retry.
        /// </summary>
        /// <param name="currentRetryCount">An integer specifying the number of retries for the given operation. A value of zero signifies this is the first error encountered.</param>
        /// <param name="statusCode">An integer containing the status code for the last operation.</param>
        /// <param name="lastException">An <see cref="Exception"/> object that represents the last exception encountered.</param>
        /// <param name="retryInterval">A <see cref="TimeSpan"/> indicating the interval to wait until the next retry.</param>
        /// <param name="operationContext">An <see cref="OperationContext"/> object that represents the context for the current operation.</param>
        /// <returns><c>true</c> if the operation should be retried; otherwise, <c>false</c>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Back compatibility")]
        bool ShouldRetry(int currentRetryCount, int statusCode, Exception lastException, out TimeSpan retryInterval, OperationContext operationContext);
    }
}
