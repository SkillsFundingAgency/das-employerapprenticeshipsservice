using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Infrastructure.ExecutionPolicies;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.ExecutionPoliciesTests.CompanyHouse
{
    public class WhenISendABadRequest
    {
        private Mock<ILog> _logger;
        private TestPolicy _policy;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILog>();
            _policy = new TestPolicy(_logger.Object);
        }

        [Test]
        public void ThenIShouldNotRetryTheRequest()
        {
            //Act
            _policy.TestThrowException(new HttpException(400,"A bad request"));

            //Assert
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
        }

        private class TestPolicy : HmrcExecutionPolicy
        {
            public TestPolicy(ILog logger) : base(logger)
            {
            }

            public void ExecuteActionThatCausesException<T>(T exception) where T : Exception
            {
                this.Execute(() => throw new )
            }
        }
    }
}
