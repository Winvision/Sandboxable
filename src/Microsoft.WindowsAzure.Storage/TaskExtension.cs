﻿// -----------------------------------------------------------------------------------------
// <copyright file="TaskExtensinon.cs" company="Microsoft">
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

namespace Sandboxable.Microsoft.WindowsAzure.Storage
{
    using System.Threading.Tasks;

    /// <summary>
    /// Task Extension to share code with WinRT calls
    /// </summary>
    internal static class TaskExtensinon
    {
        /// <summary>
        /// Return task itself
        /// </summary>
        /// <param name="task">input task.</param>
        /// <returns>The input task.</returns>
        public static Task AsTask(this Task task)
        {
            return task;
        }

        /// <summary>
        /// Return input task itself
        /// </summary>
        /// <typeparam name="TResult">The type of object that returns the result of the asynchronous operation.</typeparam>
        /// <param name="task">input task.</param>
        /// <returns>The input task.</returns>
        public static Task<TResult> AsTask<TResult>(this Task<TResult> task)
        {
            return task;
        }
    }
}
