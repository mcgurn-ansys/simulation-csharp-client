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
        public HttpRetryMessageHandler()
            : base()
        {
            this.MaxRetries = HttpClientHelper.DefaultMaxRetries;
        }

        public HttpRetryMessageHandler(int maxRetries = 0, HttpClientHandler handler = null)
            : base(handler ?? new HttpClientHandler())
        {
            this.MaxRetries = (maxRetries <= 0 || maxRetries > HttpClientHelper.DefaultMaxRetries) ? HttpClientHelper.DefaultMaxRetries : maxRetries;
        }

        public int MaxRetries { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Policy.Handle<Exception>()
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                    .WaitAndRetryAsync(this.MaxRetries, retryAttempt =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        return HttpClientHelper.ExponentialSleepDuration(retryAttempt); // Back off; 2.15, 4.6,  9.9, 21.3, 45.9, 98.7 secs
                    })
                    .ExecuteAsync(
                    () =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        return base.SendAsync(request, cancellationToken);
                    });
        }
    }
}