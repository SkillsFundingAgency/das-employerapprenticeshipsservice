using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAgreementOrchestratorTests
{
    public class WhenIRemoveTheLegalAgreement
    {
        private Mock<IMediator> _mediator;
        private Mock<IReferenceDataService> _referenceDataService;
        private EmployerAgreementOrchestrator _orchestrator;
        private EmployerAgreementBuilder EmployerAgreementBuilder { get; }
        private const string ExpectedHashedAccountId = "RT456";
        private const string ExpectedHashedAccountLegalEntitytId = "RRTE56";
        private const string ExpectedUserId = "TYG68UY";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            
            _referenceDataService = new Mock<IReferenceDataService>();

            _orchestrator = new EmployerAgreementOrchestrator(_mediator.Object, Mock.Of<IMapper>(), _referenceDataService.Object,new Lazy<EmployerAccountsDbContext>(() => EmployerAgreementBuilder.EmployerAccountDbContext));
        }
        
        [Test]
        public async Task ThenIfAnInvalidRequestExceptionIsThrownTheOrchestratorResponseContainsTheError()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveLegalEntityCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmOrganisationToRemoveViewModel(), ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }

        [Test]
        public async Task ThenIfAUnauthroizedAccessExceptionIsThrownThenTheOrchestratorResponseShowsAccessDenied()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemoveLegalEntityCommand>())).ThrowsAsync(new UnauthorizedAccessException());

            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmOrganisationToRemoveViewModel { Name = "TestName" }, ExpectedUserId);

            //Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, actual.Status);
        }

        [Test]
        public async Task ThenIfTheCommandIsValidTheFlashMessageIsPopulated()
        {
            //Act
            var actual = await _orchestrator.RemoveLegalAgreement(new ConfirmOrganisationToRemoveViewModel { Name = "TestName", HashedAccountId = ExpectedHashedAccountId, HashedAccountLegalEntitytId = ExpectedHashedAccountLegalEntitytId }, ExpectedUserId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<RemoveLegalEntityCommand>(
                c => c.HashedAccountId.Equals(ExpectedHashedAccountId)
                && c.HashedAccountLegalEntityId.Equals(ExpectedHashedAccountLegalEntitytId)
                && c.UserId.Equals(ExpectedUserId))), Times.Once);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.FlashMessage);
            Assert.AreEqual("You have removed TestName.", actual.FlashMessage.Headline);

        }
    }
}
