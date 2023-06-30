using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GetApprenticeshipFunding;

class WhenIChooseGovernmentGateway
{
    [Test, MoqAutoData]
    public void ThenIShouldGoToGatewayInform([NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.GetApprenticeshipFunding(string.Empty, 1) as RedirectToActionResult;

        //Assert
        result.ActionName.Should().Be(ControllerConstants.GatewayInformActionName);
    }
}