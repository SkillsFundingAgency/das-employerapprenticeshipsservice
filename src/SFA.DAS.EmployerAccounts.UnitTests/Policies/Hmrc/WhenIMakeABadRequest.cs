using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Exceptions.Hmrc;
using SFA.DAS.EmployerAccounts.Policies.Hmrc;

namespace SFA.DAS.EmployerAccounts.UnitTests.Policies.Hmrc;

public class WhenIMakeABadRequest
{
    private Mock<ILogger<HmrcExecutionPolicy>> _logger;
    private HmrcExecutionPolicy _policy;

    [SetUp]
    public void Arrange()
    {
        _logger = new Mock<ILogger<HmrcExecutionPolicy>>();
        _policy = new HmrcExecutionPolicy(_logger.Object);
    }

    [Test]
    public void ThenIShouldNotRetryTheRequest()
    {
        //Arrange
        Task TestAction(ref int callCount)
        {
            callCount++;
            throw new HttpException((int)HttpStatusCode.BadRequest, "A bad request");
        }

        // Act
        int actualNumberOfCalls = 0;
        Assert.ThrowsAsync<HttpException>(() => _policy.ExecuteAsync(() => TestAction(ref actualNumberOfCalls)));

        //Assert
        const int expectedNumberOfCalls = 1;
        Assert.AreEqual(expectedNumberOfCalls, actualNumberOfCalls);
    }
}