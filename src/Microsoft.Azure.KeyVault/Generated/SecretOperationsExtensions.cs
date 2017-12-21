// 
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sandboxable.Microsoft.Azure.KeyVault.Internal;

namespace Sandboxable.Microsoft.Azure.KeyVault.Internal
{
    internal static partial class SecretOperationsExtensions
    {
        /// <summary>
        /// Delete the specified secret
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent Delete(this ISecretOperations operations, string secretIdentifier)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).DeleteAsync(secretIdentifier);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// Delete the specified secret
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> DeleteAsync(this ISecretOperations operations, string secretIdentifier)
        {
            return operations.DeleteAsync(secretIdentifier, CancellationToken.None);
        }
        
        /// <summary>
        /// Gets a secret
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent Get(this ISecretOperations operations, string secretIdentifier)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).GetAsync(secretIdentifier);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// Gets a secret
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> GetAsync(this ISecretOperations operations, string secretIdentifier)
        {
            return operations.GetAsync(secretIdentifier, CancellationToken.None);
        }
        
        /// <summary>
        /// List the secrets in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='vault'>
        /// Required.
        /// </param>
        /// <param name='top'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent List(this ISecretOperations operations, string vault, int? top)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).ListAsync(vault, top);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// List the secrets in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='vault'>
        /// Required.
        /// </param>
        /// <param name='top'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> ListAsync(this ISecretOperations operations, string vault, int? top)
        {
            return operations.ListAsync(vault, top, CancellationToken.None);
        }
        
        /// <summary>
        /// List the next page of secrets in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='nextLink'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent ListNext(this ISecretOperations operations, string nextLink)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).ListNextAsync(nextLink);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// List the next page of secrets in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='nextLink'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> ListNextAsync(this ISecretOperations operations, string nextLink)
        {
            return operations.ListNextAsync(nextLink, CancellationToken.None);
        }
        
        /// <summary>
        /// List the versions of a secret in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='vault'>
        /// Required.
        /// </param>
        /// <param name='secretName'>
        /// Required.
        /// </param>
        /// <param name='top'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent ListVersions(this ISecretOperations operations, string vault, string secretName, int? top)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).ListVersionsAsync(vault, secretName, top);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// List the versions of a secret in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='vault'>
        /// Required.
        /// </param>
        /// <param name='secretName'>
        /// Required.
        /// </param>
        /// <param name='top'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> ListVersionsAsync(this ISecretOperations operations, string vault, string secretName, int? top)
        {
            return operations.ListVersionsAsync(vault, secretName, top, CancellationToken.None);
        }
        
        /// <summary>
        /// List the versions of a secret in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='nextLink'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent ListVersionsNext(this ISecretOperations operations, string nextLink)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).ListVersionsNextAsync(nextLink);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// List the versions of a secret in the specified vault
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='nextLink'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> ListVersionsNextAsync(this ISecretOperations operations, string nextLink)
        {
            return operations.ListVersionsNextAsync(nextLink, CancellationToken.None);
        }
        
        /// <summary>
        /// Sets a secret in the specified vault.
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <param name='request'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent Set(this ISecretOperations operations, string secretIdentifier, SecretRequestMessageWithRawJsonContent request)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).SetAsync(secretIdentifier, request);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// Sets a secret in the specified vault.
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <param name='request'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> SetAsync(this ISecretOperations operations, string secretIdentifier, SecretRequestMessageWithRawJsonContent request)
        {
            return operations.SetAsync(secretIdentifier, request, CancellationToken.None);
        }
        
        /// <summary>
        /// Update the specified secret
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <param name='request'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static SecretResponseMessageWithRawJsonContent Update(this ISecretOperations operations, string secretIdentifier, SecretRequestMessageWithRawJsonContent request)
        {
            return Task.Factory.StartNew((object s) => 
            {
                return ((ISecretOperations)s).UpdateAsync(secretIdentifier, request);
            }
            , operations, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// Update the specified secret
        /// </summary>
        /// <param name='operations'>
        /// Reference to the
        /// Microsoft.Azure.KeyVault.Internal.ISecretOperations.
        /// </param>
        /// <param name='secretIdentifier'>
        /// Required.
        /// </param>
        /// <param name='request'>
        /// Required.
        /// </param>
        /// <returns>
        /// Represents the response to a secret operation request.
        /// </returns>
        public static Task<SecretResponseMessageWithRawJsonContent> UpdateAsync(this ISecretOperations operations, string secretIdentifier, SecretRequestMessageWithRawJsonContent request)
        {
            return operations.UpdateAsync(secretIdentifier, request, CancellationToken.None);
        }
    }
}