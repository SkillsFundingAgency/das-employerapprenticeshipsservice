﻿using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;
namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenICreateALegalEntityOfOtherType
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IMapper> _mapper;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IAccountLegalEntityPublicHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _hashingService = new Mock<IAccountLegalEntityPublicHashingService>();

            _orchestrator = new OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object, _cookieService.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenTheNameIsMandatory()
        {
            var request = new OrganisationDetailsViewModel();
            var result = await _orchestrator.ValidateLegalEntityName(request);

            Assert.IsFalse(result.Data.Valid);
            Assert.IsTrue(result.Data.ErrorDictionary.ContainsKey("Name"));
        }

        [Test]
        public async Task ThenTheLegalEntityIsValidIfNameIsProvided()
        {
            var request = new OrganisationDetailsViewModel
            {
                Name = "Test Organisation"
            };
            var result = await _orchestrator.ValidateLegalEntityName(request);

            Assert.IsTrue(result.Data.Valid);
        }
    }
}