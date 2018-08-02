﻿using System;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.ExecutionPolicies;
using SFA.DAS.Http;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.ExecutionPoliciesTests.CompanyHouse
{
    public class WhenIMakeABadRequest
    {
        private Mock<ILog> _logger;
        private HmrcExecutionPolicy _policy;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILog>();
            _policy = new HmrcExecutionPolicy(_logger.Object);
        }

        [Test]
        public void ThenIShouldNotRetryTheRequest()
        {
            //Arrange
            Task TestAction(ref int callCount)
            {
                callCount++;
                throw new HttpException((int) HttpStatusCode.BadRequest, "A bad request");
            }

            // Act
            int actualNumberOfCalls = 0;
            Assert.ThrowsAsync<HttpException>(() => _policy.ExecuteAsync(() => TestAction(ref actualNumberOfCalls)));

            //Assert
            const int expectedNumberOfCalls = 1;
            Assert.AreEqual(expectedNumberOfCalls, actualNumberOfCalls);
        }
    }
}
