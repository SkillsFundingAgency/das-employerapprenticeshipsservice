using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.GetApprenticeshipFunding;

class WhenIDoNotSelectAnOption
{
    [Test, MoqAutoData]
    public void ThenIShouldReceiveAnError([NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.GetApprenticeshipFunding(string.Empty, null) as ViewResult;

        //Assert
        result.Model?.GetType().GetProperty("InError").Should().NotBeNull();
    }
}