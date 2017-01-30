using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    class WhenICreateALegalEntity
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IMapper> _mapper;
        private Mock<ICookieService> _cookieService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _mapper = new Mock<IMapper>();
            _cookieService = new Mock<ICookieService>();

            _orchestrator = new OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object, _cookieService.Object);
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
                SignedAgreement = true,
                UserIsAuthorisedToSign = true,
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
            command.LegalEntity.Name.Equals(request.Name) &&
            command.LegalEntity.Code.Equals(request.Code) &&
            command.LegalEntity.RegisteredAddress.Equals(request.Address) &&
            command.LegalEntity.DateOfIncorporation.Equals(request.IncorporatedDate) &&
            command.LegalEntity.CompanyStatus.Equals(request.LegalEntityStatus) &&
            command.SignAgreement.Equals(request.SignedAgreement))));
        }

        [TestCase(Role.Owner, HttpStatusCode.OK)]
        [TestCase(Role.Viewer, HttpStatusCode.Unauthorized)]
        [TestCase(Role.Transactor, HttpStatusCode.Unauthorized)]
        public async Task ThenOnlyOwnersCanSearchAndAddNewLegalEntities(Role userRole, HttpStatusCode expectedResponse)
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountRoleQuery>())).ReturnsAsync(new GetUserAccountRoleResponse { UserRole = userRole });

            //Act
            var actual = await _orchestrator.GetAddLegalEntityViewModel("5454654", "ADSD123");

            //Assert
            Assert.AreEqual(expectedResponse, actual.Status);
        }
    }
}
