﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.OrganisationData;
using SFA.DAS.EmployerAccounts.Commands.PayeRefData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn
{
    [TestFixture]
    class WhenISearchThePensionRegulatorWithAnInUsePAYEScheme
    {
        private const string ExpectedAorn = "aorn";
        private const string ExpectedPayeRef = "payeref";
        private PensionRegulatorDetailsViewModel _expectedViewModel;
        private SearchPensionRegulatorController _controller;
        private Mock<IMediator> _mediator;
       
        [SetUp]
        public void Setup()
        {
            _expectedViewModel = new PensionRegulatorDetailsViewModel
            {
                ReferenceNumber = 12324456,
                Name = "Accddf",
                Type = OrganisationType.PensionsRegulator,
                Address = "Address",
                Status = "Status"
            };
            var orchestrator = new Mock<SearchPensionRegulatorOrchestrator>();

            orchestrator
                .Setup(x => x.GetOrganisationsByAorn(ExpectedAorn, ExpectedPayeRef))
                .ReturnsAsync(
                    new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>
                    {
                        Data = new SearchPensionRegulatorResultsViewModel
                        {
                            Results = new List<PensionRegulatorDetailsViewModel> { _expectedViewModel }
                        }
                    });

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeInUseQuery>())).ReturnsAsync(new GetPayeSchemeInUseResponse { PayeScheme = new PayeScheme() });

            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                _mediator.Object);
        }

        [Test]
        public async Task ThenThePayeErrorPageIsDisplayed()
        {
            var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
            var redirectResponse = (RedirectToRouteResult) response;

            Assert.AreEqual(ControllerConstants.PayeErrorActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.EmployerAccountControllerName, redirectResponse.RouteValues["controller"].ToString());
        }
    }
}