using Newtonsoft.Json;
using Polly;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimulationCSharpClient.Client
{
    public class HttpRetryMessageHandler : DelegatingHandler
    {
        private readonly int maxRetries;

        public HttpRetryMessageHandler()
            : base()
        {
            this.AttemptedTries = 0;
            this.maxRetries = 6;
        }

        public HttpRetryMessageHandler(int maxRetries = 6, HttpClientHandler handler = null)
            : base(handler ?? new HttpClientHandler())
        {
            this.maxRetries = (maxRetries <= 0 || maxRetries > 6) ? 6 : maxRetries;
            this.AttemptedTries = 0;
        }

        public int AttemptedTries { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Policy.Handle<Exception>()
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                    .WaitAndRetryAsync(this.maxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2.15, retryAttempt))) // Back off; 2.15, 4.6,  9.9, 21.3, 45.9, 98.7 secs
                    .ExecuteAsync(
                    () =>
                    {
                        this.AttemptedTries++;
                        return base.SendAsync(request, cancellationToken);
                    });
        }
    }
}