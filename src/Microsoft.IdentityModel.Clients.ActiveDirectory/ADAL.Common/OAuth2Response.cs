//----------------------------------------------------------------------
// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
// Apache License 2.0
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Sandboxable.Microsoft.IdentityModel.Clients.ActiveDirectory
{
    [DataContract]
    public class TokenResponse
    {
        private const string CorrelationIdClaim = "correlation_id";

        [DataMember(Name = OAuthReservedClaim.TokenType, IsRequired = false)]
        public string TokenType { get; set; }

        [DataMember(Name = OAuthReservedClaim.AccessToken, IsRequired = false)]
        public string AccessToken { get; set; }

        [DataMember(Name = OAuthReservedClaim.RefreshToken, IsRequired = false)]
        public string RefreshToken { get; set; }

        [DataMember(Name = OAuthReservedClaim.Resource, IsRequired = false)]
        public string Resource { get; set; }

        [DataMember(Name = OAuthReservedClaim.IdToken, IsRequired = false)]
        public string IdToken { get; set; }

        [DataMember(Name = OAuthReservedClaim.CreatedOn, IsRequired = false)]
        public long CreatedOn { get; set; }

        [DataMember(Name = OAuthReservedClaim.ExpiresOn, IsRequired = false)]
        public long ExpiresOn { get; set; }

        [DataMember(Name = OAuthReservedClaim.ExpiresIn, IsRequired = false)]
        public long ExpiresIn { get; set; }

        [DataMember(Name = OAuthReservedClaim.Error, IsRequired = false)]
        public string Error { get; set; }

        [DataMember(Name = OAuthReservedClaim.ErrorDescription, IsRequired = false)]
        public string ErrorDescription { get; set; }

        [DataMember(Name = OAuthReservedClaim.ErrorCodes, IsRequired = false)]
        public string[] ErrorCodes { get; set; }

        [DataMember(Name = CorrelationIdClaim, IsRequired = false)]
        public string CorrelationId { get; set; }
    }

    [DataContract]
    public class IdToken
    {
        [DataMember(Name = IdTokenClaim.ObjectId, IsRequired = false)]
        public string ObjectId { get; set; }

        [DataMember(Name = IdTokenClaim.Subject, IsRequired = false)]
        public string Subject { get; set; }

        [DataMember(Name = IdTokenClaim.TenantId, IsRequired = false)]
        public string TenantId { get; set; }

        [DataMember(Name = IdTokenClaim.UPN, IsRequired = false)]
        public string UPN { get; set; }

        [DataMember(Name = IdTokenClaim.GivenName, IsRequired = false)]
        public string GivenName { get; set; }

        [DataMember(Name = IdTokenClaim.FamilyName, IsRequired = false)]
        public string FamilyName { get; set; }

        [DataMember(Name = IdTokenClaim.Email, IsRequired = false)]
        public string Email { get; set; }

        [DataMember(Name = IdTokenClaim.PasswordExpiration, IsRequired = false)]
        public long PasswordExpiration { get; set; }

        [DataMember(Name = IdTokenClaim.PasswordChangeUrl, IsRequired = false)]
        public string PasswordChangeUrl { get; set; }

        [DataMember(Name = IdTokenClaim.IdentityProvider, IsRequired = false)]
        public string IdentityProvider { get; set; }

        [DataMember(Name = IdTokenClaim.Issuer, IsRequired = false)]
        public string Issuer { get; set; }
    }
}

