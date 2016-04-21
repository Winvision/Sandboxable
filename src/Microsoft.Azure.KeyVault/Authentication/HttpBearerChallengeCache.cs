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
using System.Collections.Generic;

namespace Sandboxable.Microsoft.Azure.KeyVault
{
    public sealed class HttpBearerChallengeCache
    {
        private static readonly HttpBearerChallengeCache Instance = new HttpBearerChallengeCache();

        public static HttpBearerChallengeCache GetInstance()
        {
            return Instance;
        }

        private readonly Dictionary<string, HttpBearerChallenge> cache;
        private readonly object cacheLock;

        private HttpBearerChallengeCache()
        {
            this.cache = new Dictionary<string, HttpBearerChallenge>();
            this.cacheLock = new object();
        }
        
        public HttpBearerChallenge GetChallengeForURL(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpBearerChallenge value = null;

            lock (this.cacheLock)
            {
                this.cache.TryGetValue(url.FullAuthority(), out value);
            }

            return value;
        }

        public void RemoveChallengeForURL(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            lock (this.cacheLock)
            {
                this.cache.Remove(url.FullAuthority());
            }
        }

        public void SetChallengeForURL(Uri url, HttpBearerChallenge value)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (string.Compare(url.FullAuthority(), value.SourceAuthority, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new ArgumentException("Source URL and Challenge URL do not match");
            }

            lock (this.cacheLock)
            {
                this.cache[url.FullAuthority()] = value;
            }
        }

        public void Clear()
        {
            lock (this.cacheLock)
            {
                this.cache.Clear();
            }
        }
    }
}