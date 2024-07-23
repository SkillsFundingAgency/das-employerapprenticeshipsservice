using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices;
using SFA.DAS.EAS.Support.Web.Controllers;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Challenge;

public abstract class WhenTestingChallengeController
{
    protected Mock<IChallengeHandler>? MockChallengeHandler;
    private Mock<HttpContext>? _mockContextBase;
    private Mock<HttpRequest>? _mockRequestBase;
    private Mock<HttpResponse>? _mockResponseBase;
    private Mock<ClaimsPrincipal>? _mockUser;
    private Mock<ILogger<ChallengeController>>? _logger;
    protected ChallengeController? Unit;
    private ControllerContext? _unitControllerContext;

    [SetUp]
    public void Setup()
    {
        MockChallengeHandler = new Mock<IChallengeHandler>();
        _logger = new Mock<ILogger<ChallengeController>>();
        Unit = new ChallengeController(MockChallengeHandler.Object, _logger.Object);

        _mockContextBase = new Mock<HttpContext>();

        _mockRequestBase = new Mock<HttpRequest>();
        _mockResponseBase = new Mock<HttpResponse>();
        _mockUser = new Mock<ClaimsPrincipal>();

        _mockContextBase.Setup(x => x.Request).Returns(_mockRequestBase.Object);
        _mockContextBase.Setup(x => x.Response).Returns(_mockResponseBase.Object);
        _mockContextBase.Setup(x => x.User).Returns(_mockUser.Object);

        _unitControllerContext = new ControllerContext();

        Unit.ControllerContext = _unitControllerContext;
    }
}