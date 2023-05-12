using AutoFixture.NUnit3;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerTeamControllerTests;

public class WhenIChooseIIfIKnowApprenticehipStartDate 
{
    [Test, MoqAutoData]
    public void IfIChooseYesIContinueTheJourney(
        string hashedAccountId,
        [NoAutoProperties] EmployerTeamController controller)
    {
        //Act
        var result = controller.TriageWillApprenticeshipTrainingStart(hashedAccountId, new TriageViewModel { TriageOption = TriageOptions.Yes }) as RedirectToRouteResult;

        //Assert
        Assert.AreEqual(RouteNames.TriageWhenApprenticeshipForExistingEmployee, result.RouteName);
    }

    [Test, MoqAutoData]
    public void IfIChooseNoICannotSetupAnApprentice(
        string hashedAccountId,
        [NoAutoProperties] EmployerTeamController controller)
    {
        //Act
        var result = controller.TriageWillApprenticeshipTrainingStart(hashedAccountId, new TriageViewModel { TriageOption = TriageOptions.No }) as RedirectToRouteResult;

        //Assert
        Assert.AreEqual(RouteNames.TriageCannotSetupWithoutStartDate, result.RouteName);
    }

    [Test, MoqAutoData]
    public void IfIChooseDontKnowICannotSetupAnApprentice(
        string hashedAccountId,
        [NoAutoProperties] EmployerTeamController controller)
    {
        //Act
        var result = controller.TriageWillApprenticeshipTrainingStart(hashedAccountId, new TriageViewModel { TriageOption = TriageOptions.Unknown }) as RedirectToRouteResult;

        //Assert
        Assert.AreEqual(RouteNames.TriageCannotSetupWithoutApproximateStartDate, result.RouteName);
    }
}