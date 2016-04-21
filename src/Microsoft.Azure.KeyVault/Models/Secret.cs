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
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Sandboxable.Microsoft.Azure.KeyVault
{
    /// <summary>
    /// A Secret consisting of a value and id.
    /// </summary>
    [DataContract]
    public class Secret
    {
        internal const string PropertyValue = "value";
        internal const string PropertyContentType = "contentType";
        internal const string PropertyId = "id";
        internal const string PropertyAttributes = "attributes";
        internal const string PropertyTags = "tags";

        /// <summary>
        /// The secret value 
        /// </summary>
        [DataMember(Name = PropertyValue, IsRequired = false)]
        public string Value { get; set; }

        /// <summary>
        /// The content type of the secret
        /// </summary>
        [DataMember(Name = PropertyContentType, IsRequired = false)]
        public string ContentType { get; set; }

        /// <summary>
        /// The secret id
        /// </summary>
        private string id;

        [DataMember(Name = PropertyId, IsRequired = false, EmitDefaultValue = false)]
        public string Id
        {
            get { return this.id; }
            set
            {
                this.id = value;
                this.SecretIdentifier = !string.IsNullOrWhiteSpace(this.id) ? new SecretIdentifier(this.id) : null;
            }
        }

        /// <summary>
        /// The secret management attributes
        /// </summary>
        [DataMember(Name = PropertyAttributes, IsRequired = false, EmitDefaultValue = false)]
        public SecretAttributes Attributes { get; set; }

        /// <summary>
        /// The tags for the secret
        /// </summary>
        [DataMember(Name = PropertyTags, IsRequired = false, EmitDefaultValue = false)]
        public Dictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Identifier of the secret 
        /// </summary>        
        public SecretIdentifier SecretIdentifier { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Secret()
        {
            this.Attributes = new SecretAttributes();
        }
        
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}