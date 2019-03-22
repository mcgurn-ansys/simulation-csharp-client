using NUnit.Framework;
using SimulationCSharpClient.Client;
using System;
using System.Net.Http;

namespace SimulationCSharpClient.Tests.HttpRetryHandler
{
    [TestFixture]
    public class HttpClientHelperTests
    {
        [Test]
        public void ExponentialSleepDuration_Returns_ExponentialSleepTime_For_Each_Retry()
        {
            for (int maxRetries = 1; maxRetries <= HttpClientHelper.DefaultMaxRetries; maxRetries++)
            {
                // Arrange
                var expectedSleepTimeInSecs = Math.Pow(HttpClientHelper.ExponentialBackOff, maxRetries);

                // Act
                var actualSleepTimeInSecs = HttpClientHelper.ExponentialSleepDuration(maxRetries);

                // Assert
                Assert.AreEqual(Math.Round(expectedSleepTimeInSecs, 2), Math.Round(actualSleepTimeInSecs.TotalSeconds, 2));
            }
        }

        [Test]
        public void GetTotalSleepDurationInSecs_Returns_SumOf_SleepTimes_Between_Each_Retry()
        {
            for (int maxRetries = 1; maxRetries <= HttpClientHelper.DefaultMaxRetries; maxRetries++)
            {
                // Arrange
                var handler = new HttpRetryMessageHandler(maxRetries);

                double expectedSleepTimeInSecs = 0;
                for (int reAttemptNo = 1; reAttemptNo <= handler.MaxRetries; reAttemptNo++)
                {
                    expectedSleepTimeInSecs += HttpClientHelper.ExponentialSleepDuration(reAttemptNo).TotalSeconds;
                }

                // Act
                var actualSleepTimeInSecs = HttpClientHelper.GetTotalSleepDurationInSecs(handler.MaxRetries);

                // Assert
                Assert.IsTrue(Math.Abs(expectedSleepTimeInSecs - actualSleepTimeInSecs) <= 1);
            }
        }

        [Test]
        public void TotalTimeOutInSecs_Returns_TimeOut_Based_On_MaxRetries()
        {
            for (int maxRetries = 1; maxRetries <= HttpClientHelper.DefaultMaxRetries; maxRetries++)
            {
                // Arrange
                var handler = new HttpRetryMessageHandler(maxRetries);
                double expectedTimeOut = 20 * (handler.MaxRetries + 1);
                for (int reAttemptNo = 1; reAttemptNo <= handler.MaxRetries; reAttemptNo++)
                {
                    expectedTimeOut += HttpClientHelper.ExponentialSleepDuration(reAttemptNo).TotalSeconds;
                }

                // Act
                var actualTimeOut = HttpClientHelper.TimeOutInSecs(handler.MaxRetries);

                // Assert
                Assert.IsTrue(Math.Abs(expectedTimeOut - actualTimeOut) <= 1);
            }
        }

        [Test]
        public void GetHttpClientMaxRetriesHandler_SetsTimeOutBased_On_MaxRetries()
        {
            // Arrange
            var handler = new HttpRetryMessageHandler(5);
            double expectedTimeOut = 20 * (handler.MaxRetries + 1);
            for (int reAttemptNo = 1; reAttemptNo <= handler.MaxRetries; reAttemptNo++)
            {
                expectedTimeOut += HttpClientHelper.ExponentialSleepDuration(reAttemptNo).TotalSeconds;
            }

            // Act
            var httpClient = HttpClientHelper.GetHttpClientWithMaxRetriesHandler(handler.MaxRetries);
            var actualTimeOut = httpClient.Timeout.TotalSeconds;

            // Assert
            Assert.IsTrue(actualTimeOut >= 100);
            Assert.IsTrue(Math.Abs(expectedTimeOut - actualTimeOut) <= 1);
        }

        [Test]
        public void GetHttpClientMaxRetriesHandler_Sets_DefaultTimeOut_For_LessNumber_Of_Retries()
        {
            // Arrange
            var handler = new HttpRetryMessageHandler(1);
            var estimatedTimeOut = HttpClientHelper.TimeOutInSecs(handler.MaxRetries);

            var defaultTimeOut = new HttpClient().Timeout;

            // Act
            var httpClient = HttpClientHelper.GetHttpClientWithMaxRetriesHandler(handler.MaxRetries);
            var actualTimeOut = httpClient.Timeout.TotalSeconds;

            // Assert
            Assert.IsTrue(estimatedTimeOut < defaultTimeOut.TotalSeconds);
            Assert.AreEqual(defaultTimeOut, httpClient.Timeout);
        }
    }
}
