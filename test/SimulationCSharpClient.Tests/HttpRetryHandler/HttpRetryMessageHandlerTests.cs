using Moq;
using Moq.Protected;
using NUnit.Framework;
using SimulationCSharpClient.Client;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimulationCSharpClient.Tests.HttpRetryHandler
{
    [TestFixture]
    public class HttpRetryMessageHandlerTests
    {
        [Test]
        public void HttpGet_Success()
        {
            // Arrange
            var retryMessageHandler = new HttpRetryMessageHandler(6);

            using (var httpClient = new HttpClient(retryMessageHandler))
            {
                // Act
                var response = httpClient.GetStringAsync("http://www.google.com").Result;

                // Assert
                Assert.IsTrue(!string.IsNullOrEmpty(response));
            }
        }

        [Test]
        public void HttpGet_Fail_But_RetryForMaxTries()
        {
            int maxReTries = 3;
            var httpRetryMessageHandler = new HttpRetryMessageHandler(maxReTries);
            using (var httpClient = new HttpClient(httpRetryMessageHandler))
            {
                try
                {
                    // Act
                    var response = httpClient.GetStringAsync("https://3dsimulationNoSiteExistsWiththisName.com").Result;

                    // Assert
                    Assert.Fail("Expected 'Site Not Reachable'.");
                }
                catch
                {
                }

                // Assert
                Assert.IsTrue(httpRetryMessageHandler.AttemptedTries == maxReTries + 1, $"Expected to retry for {maxReTries} times and total expected tries is {maxReTries + 1}"); // 3 Retries and one inital try.
            }
        }

        [Test]
        public void HttpGet_Fail_But_RetryFor_6_ReTries_Only_OnInvalid_MaxTries()
        {
            int maxReTries = -1;
            int expectedMaxReTries = 6;
            var httpRetryMessageHandler = new HttpRetryMessageHandler(maxReTries);
            using (var httpClient = new HttpClient(httpRetryMessageHandler))
            {
                try
                {
                    // Act
                    var response = httpClient.GetStringAsync("https://3dsimulationNoSiteExistsWiththisName.com").Result;

                    // Assert
                    Assert.Fail("Expected 'Site Not Reachable'.");
                }
                catch
                {
                }

                // Assert
                Assert.IsFalse(httpRetryMessageHandler.AttemptedTries == maxReTries + 1, $"Expected not to retry for {maxReTries} times and total expected tries should not equal to {maxReTries + 1}"); // 3 Retries and one inital try.
                Assert.AreEqual(httpRetryMessageHandler.AttemptedTries, expectedMaxReTries + 1, $"Expected to retry for {expectedMaxReTries} times"); // 6 Retries and one inital try.
            }
        }
    }
}
