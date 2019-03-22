using Moq;
using Moq.Protected;
using NUnit.Framework;
using SimulationCSharpClient.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void HttpGet_MaxRetries_OnFailure()
        {
            // Arrange
            int maxReTries = 2;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhostTestApi.com");

            var handler = new HttpRetryMessageHandler(maxReTries)
            {
                InnerHandler = new TestHttpRetryMessageHandlerFailure()
            };

            var invoker = new HttpMessageInvoker(handler);

            // Act
            var result = invoker.SendAsync(httpRequestMessage, CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(((TestHttpRetryMessageHandlerFailure)handler.InnerHandler).AttemptedTries, maxReTries + 1);
        }

        [Test]
        public void HttpGet_Success_In_FirstTry()
        {
            // Arrange
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhostTestApi.com");

            var handler = new HttpRetryMessageHandler()
            {
                InnerHandler = new TestHttpRetryMessageHandlerSuccess()
            };
            var invoker = new HttpMessageInvoker(handler);

            // Act
            var result = invoker.SendAsync(httpRequestMessage, CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(((TestHttpRetryMessageHandlerSuccess)handler.InnerHandler).AttemptedTries, 1);
        }
    }
}
