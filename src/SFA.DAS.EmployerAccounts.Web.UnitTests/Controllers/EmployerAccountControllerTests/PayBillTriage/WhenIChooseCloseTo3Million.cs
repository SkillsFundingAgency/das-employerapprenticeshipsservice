﻿using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.PayBillTriage;

class WhenIChooseCloseTo3Million
{
    [Test, MoqAutoData]
    public void ThenIShouldGoToGatewayInform([NoAutoProperties] EmployerAccountController controller)
    {
        //Act
        var result = controller.PayBillTriage(2) as RedirectToActionResult;

        //Assert
        result.ActionName.Should().Be(ControllerConstants.GatewayInformActionName);
    }
}