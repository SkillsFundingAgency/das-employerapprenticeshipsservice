using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GetApprenticeshipFunding;

class WhenIChooseLessThan3Million
{
    [Test, MoqAutoData]
    public void ThenIShouldGoToGetSearchPensionRegulator([NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.GetApprenticeshipFunding(string.Empty, 2) as RedirectToActionResult;

        //Assert
        result.ActionName.Should().Be(ControllerConstants.SearchUsingAornActionName);
        result.ControllerName.Should().Be(ControllerConstants.SearchPensionRegulatorControllerName);
    }
}