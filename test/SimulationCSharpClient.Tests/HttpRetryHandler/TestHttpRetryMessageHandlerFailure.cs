using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimulationCSharpClient.Tests.HttpRetryHandler
{
    public class TestHttpRetryMessageHandlerFailure : DelegatingHandler
    {
        public int AttemptedTries { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.AttemptedTries++;
            return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        }
    }
}
