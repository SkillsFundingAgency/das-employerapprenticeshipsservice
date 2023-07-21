using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.PayBillTriage;

class WhenIChooseLessThan3Million
{
    [Test, MoqAutoData]
    public void ThenIShouldGoToGetApprenticehsipFunding([NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.PayBillTriage(3) as RedirectToRouteResult;

        //Assert
        result.RouteName.Should().Be(RouteNames.EmployerAccountGetApprenticeshipFunding);
    }
}