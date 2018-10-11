using HMRC.ESFA.Levy.Api.Types.Exceptions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ExecutionPolicies;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.UnitTests.ExecutionPolicies.HmrcExecutionPolicyTests
{
    [TestFixture]
    public class PolicyRetryTest
    {
        private HmrcExecutionPolicy _policy;

        [SetUp]
        public void Init()
        {
            _policy = new HmrcExecutionPolicy(Mock.Of<ILog>(), TimeSpan.FromMilliseconds(0));
        }

        [Test]
        public void LimitedRetryWhenServiceUnavailableReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(503, "service unavailable"));

            Assert.Throws<AggregateException>(() => _policy.ExecuteAsync(runner.Execute).Wait());

            Assert.AreEqual(6, runner.CallCount);
        }

        [Test]
        public void LimitedRetryWhenInternalServerErrorReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(500, "internal server error"));

            Assert.Throws<AggregateException>(() => _policy.ExecuteAsync(runner.Execute).Wait());

            Assert.AreEqual(6, runner.CallCount);
        }

        [Test]
        public void RetryForeverWhenTooManyRequestsReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(429, "too many requests"));

            _policy.ExecuteAsync(runner.Execute).Wait();

            Assert.AreEqual(runner.MaxCallCount, runner.CallCount);
        }

        [Test]
        public void RetryForeverWhenRequestTimeoutReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(408, "request timeout"));

            _policy.ExecuteAsync(runner.Execute).Wait();

            Assert.AreEqual(runner.MaxCallCount, runner.CallCount);
        }

        private ApiHttpException CreateTestException(int httpCode, string message)
        {
            return new ApiHttpException(httpCode, message, string.Empty, string.Empty);
        }

        private class TestRunner<T> where T : Exception
        {
            private readonly T _exception;

            public int CallCount { get; private set; }
            public int MaxCallCount { get; }

            public TestRunner(T exception, int maxCallCount = 10)
            {
                _exception = exception;
                MaxCallCount = maxCallCount;
                CallCount = 0;
            }

            public async Task Execute()
            {
                CallCount++;

                if (CallCount >= MaxCallCount)
                    return;

                await Task.Delay(0);

                throw _exception;
            }
        }
    }
}
