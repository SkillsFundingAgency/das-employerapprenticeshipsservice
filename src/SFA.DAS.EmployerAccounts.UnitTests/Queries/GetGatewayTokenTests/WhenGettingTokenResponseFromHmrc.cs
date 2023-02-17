using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetGatewayTokenTests;

public class WhenGettingTokenResponseFromHmrc
{
    private GetGatewayTokenHandler _getGatewayTokenHandler;
    private Mock<IHmrcService> _hmrcService;
    private readonly string _expectedAccessToken = "kjhg345";

    [SetUp]
    public void Arrange()
    {
        _hmrcService = new Mock<IHmrcService>();
        _hmrcService.Setup(x => x.GetAuthenticationToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new HmrcTokenResponse { AccessToken = _expectedAccessToken });

        _getGatewayTokenHandler = new GetGatewayTokenHandler(_hmrcService.Object);
    }

    [Test]
    public async Task ThenTheHmrcServiceIsCalledWithThePassedAccessCode()
    {
        //Arrange
        var code = "gf56df";
        var url = "http://myurl.local";

        //Act
        await _getGatewayTokenHandler.Handle(new GetGatewayTokenQuery { AccessCode = code, RedirectUrl = url }, CancellationToken.None);

        //Assert
        _hmrcService.Verify(x => x.GetAuthenticationToken(url, code));
    }

    [Test]
    public async Task ThenTheReturnValueIsPopulatedFromTheHmrcService()
    {
        //Act
        var actual = await _getGatewayTokenHandler.Handle(new GetGatewayTokenQuery { AccessCode = "", RedirectUrl = "" }, CancellationToken.None);

        //Assert
        Assert.IsAssignableFrom<GetGatewayTokenQueryResponse>(actual);
        Assert.AreEqual(_expectedAccessToken, actual.HmrcTokenResponse.AccessToken);
    }
}