using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.PayBillTriage;

[TestFixture]
class WhenIGetPayeTriage
{
    [Test, MoqAutoData]
    public void Then_If_Does_Not_Have_Account_Should_Return_Triage([NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.PayBillTriage(string.Empty) as ViewResult;

        //Assert
        result.ViewName.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void Then_If_Has_Account_Should_Return_Shutter(string hashedAccountId, [NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.PayBillTriage(hashedAccountId) as RedirectToRouteResult;

        //Assert
        result.RouteName.Should().Be(RouteNames.AddPayeShutter);
    }
}