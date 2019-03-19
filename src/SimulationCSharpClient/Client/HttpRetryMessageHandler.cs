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
        private const double ExponentialBackOff = 2.15;

        public HttpRetryMessageHandler()
            : base()
        {
            this.MaxRetries = 6;
        }

        public HttpRetryMessageHandler(int maxRetries = 6, HttpClientHandler handler = null)
            : base(handler ?? new HttpClientHandler())
        {
            this.MaxRetries = (maxRetries <= 0 || maxRetries > 6) ? 6 : maxRetries;
        }

        public int MaxRetries { get; private set; }

        public double TotalWaitDurationInSecs
        {
            get { return ExponentialBackOff * (1 - Math.Pow(ExponentialBackOff, (double)this.MaxRetries)) / (1 - ExponentialBackOff); } // Sum of GP Sn = (a * (1- r^n)) / (1-r) where a = r = this.ExponentialBackOff, n= this.MaxRetries
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Policy.Handle<Exception>()
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                    .WaitAndRetryAsync(this.MaxRetries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(ExponentialBackOff, retryAttempt))) // Back off; 2.15, 4.6,  9.9, 21.3, 45.9, 98.7 secs
                    .ExecuteAsync(
                    () =>
                    {
                        return base.SendAsync(request, cancellationToken);
                    });
        }
    }
}