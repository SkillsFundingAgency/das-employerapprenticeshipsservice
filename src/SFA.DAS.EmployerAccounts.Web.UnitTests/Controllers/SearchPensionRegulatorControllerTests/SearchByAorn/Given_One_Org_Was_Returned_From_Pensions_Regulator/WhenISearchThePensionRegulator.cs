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
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.SearchPensionRegulatorControllerTests.SearchByAorn.Given_One_Org_Was_Returned_From_Pensions_Regulator
{
    [TestFixture]
    class WhenISearchThePensionRegulator
    {
        private const string ExpectedAorn = "1234567890ABC";
        private const string ExpectedPayeRef = "000/TGFDSAS";
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

            _controller = new SearchPensionRegulatorController(
                Mock.Of<IAuthenticationService>(),
                orchestrator.Object,
                Mock.Of<IMultiVariantTestingService>(),
                Mock.Of<ICookieStorageService<FlashMessageViewModel>>(),
                _mediator.Object);
        }

        [Test]
        public async Task ThenTheOrganisationDetailsAreSaved()
        {
            await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });

            _mediator.Verify(x => x.SendAsync(It.Is<SaveOrganisationData>(y => OrganisationDataMatchesViewModel(y))));
        }

        [Test]
        public async Task ThenThePayeDetailsAreSaved()
        {
            await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });

            _mediator.Verify(x => x.SendAsync(It.Is<SavePayeRefData>(y => y.PayeRefData.AORN == ExpectedAorn && y.PayeRefData.PayeReference == ExpectedPayeRef)));
        }

        [Test]
        public async Task ThenTheCheckYourDetailsPageIsDisplayed()
        {
            var response = await _controller.SearchPensionRegulatorByAorn(new SearchPensionRegulatorByAornViewModel { Aorn = ExpectedAorn, PayeRef = ExpectedPayeRef });
            var redirectResponse = (RedirectToRouteResult)response;

            Assert.AreEqual(ControllerConstants.SummaryActionName, redirectResponse.RouteValues["action"].ToString());
            Assert.AreEqual(ControllerConstants.EmployerAccountControllerName, redirectResponse.RouteValues["controller"].ToString());
        }

        private bool OrganisationDataMatchesViewModel(SaveOrganisationData saveOrganisationData)
        {
            return _expectedViewModel.Address == saveOrganisationData.OrganisationData.OrganisationRegisteredAddress
                   && _expectedViewModel.Name == saveOrganisationData.OrganisationData.OrganisationName
                   && _expectedViewModel.ReferenceNumber.ToString() == saveOrganisationData.OrganisationData.OrganisationReferenceNumber
                   && _expectedViewModel.Status == saveOrganisationData.OrganisationData.OrganisationStatus
                   && _expectedViewModel.Type == OrganisationType.PensionsRegulator
                   && saveOrganisationData.OrganisationData.NewSearch
                   && !saveOrganisationData.OrganisationData.PensionsRegulatorReturnedMultipleResults;
        }
    }
}
