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

        [Test]
        public void HttpRetryMessageHandler_ThrowsException_If_Cancellation_Requested()
        {
            // Arrange
            int maxReTries = 3;
            var cancellationTokenSource = new CancellationTokenSource();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhostTestApi.com");

            var handler = new HttpRetryMessageHandler(maxReTries)
            {
                InnerHandler = new TestHttpRetryMessageHandlerFailure()
            };

            var invoker = new HttpMessageInvoker(handler);

            // Act
            cancellationTokenSource.Cancel();

            var exception = Assert.Catch<Exception>(() =>
            {
                var result = invoker.SendAsync(httpRequestMessage, cancellationTokenSource.Token).Result;
            });

            // Assert
            Assert.IsNotNull(exception);
            Assert.IsTrue(this.IsOperationCanceledException(exception));
            Assert.AreEqual(((TestHttpRetryMessageHandlerFailure)handler.InnerHandler).AttemptedTries, 0);
        }

        [Test]
        public void HttpRetryMessageHandler_WaitAndRetryAsync_ThrowsException_If_Cancellation_Requested()
        {
            // Arrange
            int maxReTries = 3;
            var cancellationTokenSource = new CancellationTokenSource();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://localhostTestApi.com");

            var handler = new HttpRetryMessageHandler(maxReTries)
            {
                InnerHandler = new TestHttpMessageHandlerCancelAfterFirstTry()
                {
                    CancellationTokenSource = cancellationTokenSource
                }
            };

            var invoker = new HttpMessageInvoker(handler);

            // Act
            var exception = Assert.Catch<Exception>(() =>
            {
                var result = invoker.SendAsync(httpRequestMessage, cancellationTokenSource.Token).Result;
            });

            // Assert
            Assert.IsNotNull(exception);
            Assert.IsTrue(this.IsOperationCanceledException(exception));
            Assert.AreEqual(((TestHttpMessageHandlerCancelAfterFirstTry)handler.InnerHandler).AttemptedTries, 1);
        }

        private bool IsOperationCanceledException(Exception exception)
        {
            bool isOperationCanceledException = exception is OperationCanceledException;
            if (!isOperationCanceledException && exception is AggregateException)
            {
                ((AggregateException)exception).Handle((x) =>
                {
                    if (x is OperationCanceledException)
                    {
                        isOperationCanceledException = true;
                    }

                    return true;
                });
            }

            return isOperationCanceledException;
        }
    }
}
