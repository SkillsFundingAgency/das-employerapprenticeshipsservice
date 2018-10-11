using Moq;
using NUnit.Framework;
using SFA.DAS.ExecutionPolicies;
using SFA.DAS.Http;
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
            var runner = new TestRunner<ServiceUnavailableException>(new ServiceUnavailableException());

            Assert.Throws<AggregateException>(() => _policy.ExecuteAsync(runner.Execute).Wait());

            Assert.AreEqual(6, runner.CallCount);
        }

        [Test]
        public void LimitedRetryWhenInternalServerErrorReturned()
        {
            var runner = new TestRunner<InternalServerErrorException>(new InternalServerErrorException());

            Assert.Throws<AggregateException>(() => _policy.ExecuteAsync(runner.Execute).Wait());

            Assert.AreEqual(6, runner.CallCount);
        }

        [Test]
        public void RetryForeverWhenTooManyRequestsReturned()
        {
            var runner = new TestRunner<TooManyRequestsException>(new TooManyRequestsException());

            _policy.ExecuteAsync(runner.Execute).Wait();

            Assert.AreEqual(10, runner.CallCount);
        }

        [Test]
        public void RetryForeverWhenRequestTimeoutReturned()
        {
            var runner = new TestRunner<RequestTimeOutException>(new RequestTimeOutException());

            _policy.ExecuteAsync(runner.Execute).Wait();

            Assert.AreEqual(10, runner.CallCount);
        }

        private class TestRunner<T> where T : Exception
        {
            private readonly T _exception;
            private readonly int _maxCallCount;
            public int CallCount { get; private set; }

            public TestRunner(T exception, int maxCallCount = 10)
            {
                _exception = exception;
                _maxCallCount = maxCallCount;
                CallCount = 0;
            }

            public async Task Execute()
            {
                CallCount++;

                if (CallCount >= _maxCallCount)
                    return;

                await Task.Delay(0);

                throw _exception;
            }
        }
    }
}
