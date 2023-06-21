using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GetApprenticeshipFunding;

class WhenISelectMoreThan3Million
{
    [Test, MoqAutoData]
    public void ThenIShouldGoToSkipRegistration([NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.GetApprenticeshipFunding(string.Empty, 1) as RedirectToActionResult;

        //Assert
        result.ControllerName.Should().Be(ControllerConstants.EmployerAccountControllerName);
        result.ActionName.Should().Be(ControllerConstants.GatewayInformActionName);
    }
}