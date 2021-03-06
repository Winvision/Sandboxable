﻿//
// Copyright (c) Microsoft.  All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System.Net.Http;
using System.Threading;

namespace Sandboxable.Microsoft.Azure
{
    public class ClientRequestTrackingHandler
        : MessageProcessingHandler
    {
        public string TrackingId { get; private set; }

        public ClientRequestTrackingHandler(string trackingId)
            : base()
        {
            TrackingId = trackingId;
        }

        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("client-tracking-id", TrackingId);
            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            response.Headers.Add("client-tracking-id", TrackingId);
            return response;
        }
    }
}
