using System;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Infrastructure.ExecutionPolicies;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.ExecutionPoliciesTests.CompanyHouse
{
    public class WhenILookUpACompanyThatDoesNotExist
    {
        private Mock<ILogger> _logger;
        private TestPolicy _policy;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();
            _policy = new TestPolicy(_logger.Object);
        }

        [Test]
        public void ThenIShouldNotLogA404ErrorAsAnError()
        {
            //Act
            _policy.TestThrowException(new ResourceNotFoundException(""));

            //Assert
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ThenIShouldLogA404ErrorAsAnInfo()
        {
            //Act
            _policy.TestThrowException(new ResourceNotFoundException(""));

            //Assert
            _logger.Verify(x => x.Info(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ThenIShouldLogAnyOtherErrorErrorAsAnError()
        {
            //Act
            _policy.TestThrowException(new Exception(""));

            //Assert
            _logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }


        private class TestPolicy : CompaniesHouseExecutionPolicy
        {
            public TestPolicy(ILogger logger) : base(logger)
            {
            }

            public void TestThrowException<T>(T exception) where T : Exception
            {
                this.OnException<T>(exception);
            }
        }
    }
}
