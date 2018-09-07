using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    class WhenICreateALegalEntity
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IMapper> _mapper;
        private Mock<IHashingService> _hashingService;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();
            _hashingService = new Mock<IHashingService>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();

            _orchestrator = new OrganisationOrchestrator(
                _mediator.Object, 
                _logger.Object, 
                _mapper.Object,
                _cookieService.Object, 
                _hashingService.Object);
        }

        [Test]
        public async Task ThenTheLegalEntityShouldBeCreated()
        {
            //Assign
            var request = new CreateNewLegalEntityViewModel
            {
                HashedAccountId = "1",
                Name = "Test Corp",
                Code = "SD665734",
                Address = "1, Test Street",
                IncorporatedDate = DateTime.Now.AddYears(-20),
                ExternalUserId = "2",
                LegalEntityStatus = "active"
            };

            const long legalEntityId = 5;
            const long agreementEntityId = 6;

            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateLegalEntityCommand>()))
                     .ReturnsAsync(new CreateLegalEntityCommandResponse
                     {
                         AgreementView = new EmployerAgreementView
                         {
                             Id = agreementEntityId,
                             LegalEntityId = legalEntityId,
                             LegalEntityName = request.Name,
                             LegalEntityCode = request.Code,
                             LegalEntityAddress = request.Address,
                             LegalEntityStatus = request.LegalEntityStatus,
                             Status = EmployerAgreementStatus.Pending
                         }
                     });

            //Act
            await _orchestrator.CreateLegalEntity(request);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateLegalEntityCommand>(command =>
            command.Name.Equals(request.Name) &&
            command.Address.Equals(request.Address) &&
            command.Code.Equals(request.Code) &&
            command.DateOfIncorporation.Equals(request.IncorporatedDate) &&
            command.Status.Equals(request.LegalEntityStatus))));
        }
    }
}
