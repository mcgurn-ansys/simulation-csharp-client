using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SimulationCSharpClient.Client
{
    public static class HttpClientHelper
    {
        public static double ExponentialBackOff => 2.15;

        public static int DefaultMaxRetries => 6;

        public static HttpClient GetHttpClientWithMaxRetriesHandler(int maxRetries, HttpClientHandler handler = null)
        {
            var httpRetryMessageHandler = new HttpRetryMessageHandler(maxRetries, handler);
            var httpClient = new HttpClient(httpRetryMessageHandler);

            // TimeOut = total wait time between retries + Total Tries * 20( approximate processing time for each try) in secs
            var timeoutInSecs = TimeOutInSecs(httpRetryMessageHandler.MaxRetries);

            // set the calculated timeout to http client if it is >100 else keep the default TimeOut(100 secs)
            if (timeoutInSecs > 100)
            {
                httpClient.Timeout = TimeSpan.FromSeconds(timeoutInSecs);
            }

            return httpClient;
        }

        public static double GetTotalSleepDurationInSecs(int maxRetries)
        {
            // Back off; 2.15, 2.15^2,  2.15^3,  2.15^4,  2.15^5,  2.15^6
            // Sum of GP [a + ar + ar^2 + .....+ ar^(n-1)] Sn = (a * (1- r^n)) / (1-r) where a = r = this.ExponentialBackOff, n= this.MaxRetries
            return ExponentialBackOff * (1 - Math.Pow(ExponentialBackOff, (double)maxRetries)) / (1 - ExponentialBackOff);
        }

        public static TimeSpan ExponentialSleepDuration(int retryAttemptNo) => TimeSpan.FromSeconds(Math.Pow(ExponentialBackOff, retryAttemptNo));

        public static double TimeOutInSecs(int maxRetries) => GetTotalSleepDurationInSecs(maxRetries) + (20 * (maxRetries + 1));
    }
}
