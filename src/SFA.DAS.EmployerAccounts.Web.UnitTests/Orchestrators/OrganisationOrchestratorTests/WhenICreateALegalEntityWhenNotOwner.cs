using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    class WhenICreateALegalEntityWhenNotOwner
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IMapper> _mapper;
        private Mock<IAccountLegalEntityPublicHashingService> _hashingService;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        [SetUp()]
        public void Setup()
        {

            _mediator = new Mock<IMediator>();
            _mapper = new Mock<IMapper>();
            _hashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();

            _orchestrator = new OrganisationOrchestrator(
                _mediator.Object,
                _mapper.Object,
                _cookieService.Object,
                _hashingService.Object);
        }

        [Test]
        public async Task ThenTheLegalEntityShouldNotBeCreated()
        {
            //Assign
            var request = new CreateNewLegalEntityViewModel
            {
                HashedAccountId = "1",
                Name = "Test Corp",
                Code = "SD665734",
                Source = OrganisationType.CompaniesHouse,
                Address = "1, Test Street",
                IncorporatedDate = DateTime.Now.AddYears(-20),
                ExternalUserId = "3", // ??????
                LegalEntityStatus = "active"
            };

            _mediator.Setup(x => x.Send(It.IsAny<CreateLegalEntityCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            //Act & Assert
            var response = await _orchestrator.CreateLegalEntity(request);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.Status);
        }
    }
}