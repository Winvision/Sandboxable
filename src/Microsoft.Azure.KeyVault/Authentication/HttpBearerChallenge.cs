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
    public sealed class HttpBearerChallenge
    {
        private const string Authorization = "authorization";
        private const string AuthorizationUri = "authorization_uri";
        private const string Bearer = "Bearer";

        /// <summary>
        /// Tests whether an authentication header is a Bearer challenge
        /// </summary>
        /// <remarks>
        /// This method is forgiving: if the parameter is null, or the scheme
        /// in the header is missing, then it will simply return false.
        /// </remarks>
        /// <param name="challenge">The AuthenticationHeaderValue to test</param>
        /// <returns>True if the header is a Bearer challenge</returns>
        public static bool IsBearerChallenge(string challenge)
        {
            if (string.IsNullOrEmpty(challenge))
                return false;

            if (!challenge.Trim().StartsWith(Bearer + " "))
                return false;

            return true;
        }

        private readonly Dictionary<string, string> parameters;
        private readonly string sourceAuthority;

        /// <summary>
        /// Parses an HTTP WWW-Authentication Bearer challenge from a server.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="challenge">The AuthenticationHeaderValue to parse</param>
        public HttpBearerChallenge(Uri requestUri, string challenge)
        {
            var authority = ValidateRequestURI(requestUri);
            var trimmedChallenge = ValidateChallenge(challenge);

            this.sourceAuthority = authority;
            this.SourceUri = requestUri;

            this.parameters = new Dictionary<string, string>();

            // Split the trimmed challenge into a set of name=value strings that
            // are comma separated. The value fields are expected to be within
            // quotation characters that are stripped here.
            var pairs = trimmedChallenge.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (pairs.Length > 0)
            {
                // Process the name=value strings
                foreach (var p in pairs)
                {
                    var pair = p.Split('=');
                    if (pair.Length == 2)
                    {
                        // We have a key and a value, now need to trim and decode
                        var key = pair[0].Trim().Trim('\"');
                        var value = pair[1].Trim().Trim('\"');

                        if (!string.IsNullOrEmpty(key))
                        {
                            this.parameters[key] = value;
                        }
                    }
                }
            }

            // Minimum set of parameters
            if (this.parameters.Count < 1)
            {
                throw new ArgumentException("Invalid challenge parameters", nameof(challenge));
            }

            // Must specify authorization or authorization_uri
            if (!this.parameters.ContainsKey(Authorization) && !this.parameters.ContainsKey(AuthorizationUri))
            {
                throw new ArgumentException("Invalid challenge parameters", nameof(challenge));
            }
        }

        /// <summary>
        /// Returns the value stored at the specified key.
        /// </summary>
        /// <remarks>
        /// If the key does not exist, will return false and the
        /// content of value will not be changed
        /// </remarks>
        /// <param name="key">The key to be retrieved</param>
        /// <param name="value">The value for the specified key</param>
        /// <returns>True when the key is found, false when it is not</returns>
        public bool TryGetValue(string key, out string value)
        {
            return this.parameters.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns the URI for the Authorization server if present,
        /// otherwise string.Empty
        /// </summary>
        public string AuthorizationServer
        {
            get
            {
                string value;

                if (this.parameters.TryGetValue("authorization_uri", out value))
                {
                    return value;
                }

                if (this.parameters.TryGetValue("authorization", out value))
                {
                    return value;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the Realm value if present, otherwise the Authority
        /// of the request URI given in the ctor
        /// </summary>
        public string Resource
        {
            get
            {
                string value;

                if (this.parameters.TryGetValue("resource", out value))
                {
                    return value;
                }

                return this.sourceAuthority;
            }
        }

        /// <summary>
        /// Returns the Scope value if present, otherwise string.Empty
        /// </summary>
        public string Scope
        {
            get
            {
                string value;

                if (this.parameters.TryGetValue("scope", out value))
                {
                    return value;
                }

                return string.Empty;
            }
        }

        public string SourceAuthority => this.sourceAuthority;

        public Uri SourceUri
        {
            get;
        }

        private static string ValidateChallenge(string challenge)
        {
            if (string.IsNullOrEmpty(challenge))
            {
                throw new ArgumentNullException(nameof(challenge));
            }

            var trimmedChallenge = challenge.Trim();

            if (!trimmedChallenge.StartsWith(Bearer + " "))
            {
                throw new ArgumentException("Challenge is not Bearer", nameof(challenge));
            }

            return trimmedChallenge.Substring(Bearer.Length + 1);
        }

        private static string ValidateRequestURI(Uri requestUri)
        {
            if (null == requestUri)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            if (!requestUri.IsAbsoluteUri)
            {
                throw new ArgumentException("The requestUri must be an absolute URI", nameof(requestUri));
            }

            if (!requestUri.Scheme.Equals("http", StringComparison.CurrentCultureIgnoreCase) && !requestUri.Scheme.Equals("https", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException("The requestUri must be HTTP or HTTPS", nameof(requestUri));
            }

            return requestUri.FullAuthority();
        }
    }
}