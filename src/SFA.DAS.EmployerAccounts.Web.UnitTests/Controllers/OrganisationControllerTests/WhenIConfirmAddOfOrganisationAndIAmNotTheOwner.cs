using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.OrganisationControllerTests;

public class WhenIConfirmAddOfOrganisationAndIAmNotTheOwner
{
    private OrganisationController _controller;
    private Mock<OrganisationOrchestrator> _orchestrator;
    private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;

    [SetUp]
    public void Arrange()
    {
        _orchestrator = new Mock<OrganisationOrchestrator>();
        _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

        _orchestrator.Setup(x => x.CreateLegalEntity(It.IsAny<CreateNewLegalEntityViewModel>()))
            .Throws<UnauthorizedAccessException>();

        _controller = new OrganisationController(
            _orchestrator.Object,
            _flashMessage.Object,
            Mock.Of<IHttpContextAccessor>());
    }

    [Test]
    public async Task ThenIAmRedirectedToAccessDenied()
    {
        //Act & Asert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _controller.Confirm("", "", "", "", null, "", OrganisationType.Other, 1, null, false));
    }
}