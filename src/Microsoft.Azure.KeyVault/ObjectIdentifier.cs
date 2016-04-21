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
using System.Globalization;

namespace Sandboxable.Microsoft.Azure.KeyVault
{
    public class ObjectIdentifier
    {
        protected static bool IsObjectIdentifier(string collection, string identifier)
        {
            if (string.IsNullOrEmpty(collection))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (string.IsNullOrEmpty(identifier))
            {
                return false;
            }

            try
            {
                var baseUri = new Uri(identifier, UriKind.Absolute);

                // We expect an identifier with either 3 or 4 segments: host + collection + name [+ version]
                if (baseUri.Segments.Length != 3 && baseUri.Segments.Length != 4)
                {
                    return false;
                }

                if (!string.Equals(baseUri.Segments[1], collection + "/"))
                {
                    return false;
                }

                return true;
            }
            catch (UriFormatException)
            {
            }

            return false;
        }

        protected ObjectIdentifier(string vault, string collection, string name, string version = null)
        {
            if (string.IsNullOrEmpty(vault))
            {
                throw new ArgumentNullException(nameof(vault));
            }

            if (string.IsNullOrEmpty(collection))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var baseUri = new Uri(vault, UriKind.Absolute);

            this.Name = name;
            this.Version = version;
            this.Vault = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", baseUri.Scheme, baseUri.FullAuthority());
            this.VaultWithoutScheme = baseUri.Authority;
            this.BaseIdentifier = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", this.Vault, collection, this.Name);
            this.Identifier = string.IsNullOrEmpty(this.Version) ? this.Name : string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.Name, this.Version);
            this.Identifier = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", this.Vault, collection, this.Identifier);
        }

        protected ObjectIdentifier(string collection, string identifier)
        {
            if (string.IsNullOrEmpty(collection))
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            var baseUri = new Uri(identifier, UriKind.Absolute);

            // We expect and identifier with either 3 or 4 segments: host + collection + name [+ version]
            if (baseUri.Segments.Length != 3 && baseUri.Segments.Length != 4)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. Bad number of segments: {1}", identifier, baseUri.Segments.Length));
            }

            if (!string.Equals(baseUri.Segments[1], collection + "/"))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid ObjectIdentifier: {0}. segment [1] should be '{1}/', found '{2}'", identifier, collection, baseUri.Segments[1]));
            }

            this.Name = baseUri.Segments[2].Substring(0, baseUri.Segments[2].Length).TrimEnd('/');

            if (baseUri.Segments.Length == 4)
            {
                this.Version = baseUri.Segments[3].Substring(0, baseUri.Segments[3].Length).TrimEnd('/');
            }

            this.Vault = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", baseUri.Scheme, baseUri.FullAuthority());
            this.VaultWithoutScheme = baseUri.Authority;
            this.BaseIdentifier = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", this.Vault, collection, this.Name);
            this.Identifier = string.IsNullOrEmpty(this.Version) ? this.Name : string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.Name, this.Version);
            this.Identifier = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}", this.Vault, collection, this.Identifier);
        }

        /// <summary>
        /// The base identifier for an object, does not include the object version.
        /// </summary>
        public string BaseIdentifier
        {
            get;
        }

        /// <summary>
        /// The identifier for an object, includes the objects version.
        /// </summary>
        public string Identifier
        {
            get;
        }

        /// <summary>
        /// The name of the object.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// The vault containing the object
        /// </summary>
        public string Vault
        {
            get;
        }

        public string VaultWithoutScheme
        {
            get;
        }

        /// <summary>
        /// The version of the object.
        /// </summary>
        public string Version
        {
            get;
        }

        public override string ToString()
        {
            return this.Identifier;
        }
    }
}