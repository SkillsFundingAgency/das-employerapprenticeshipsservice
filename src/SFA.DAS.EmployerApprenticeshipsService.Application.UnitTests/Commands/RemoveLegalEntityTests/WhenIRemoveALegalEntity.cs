using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types.Events.Agreement;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Commands.RemoveLegalEntity;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RemoveLegalEntityTests
{
    public class WhenIRemoveALegalEntity
    {
        private RemoveLegalEntityCommandHandler _handler;
        private Mock<IValidator<RemoveLegalEntityCommand>> _validator;
        private Mock<ILog> _logger;
        private Mock<IEmployerAgreementRepository> _repository;
        private RemoveLegalEntityCommand _command;
        private Mock<IMediator> _mediator;
        private Mock<IHashingService> _hashingService;
        private Mock<IGenericEventFactory> _genericEventHandler;
        private Mock<IEmployerAgreementEventFactory> _employerAgreementEventFactory;

        private const string ExpectedHashedAccountId = "34RFD";
        private const long ExpectedAccountId = 123455;
        private const long ExpectedLegalEntityId = 98854;
        private const string ExpectedUserId = "LASDA234E";
        private const long ExpectedEmployerAgreementId = 5533678;
        private const string ExpectedHashedEmployerAgreementId = "FGDFH45645";

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<RemoveLegalEntityCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemoveLegalEntityCommand>())).ReturnsAsync(new ValidationResult());

            _logger = new Mock<ILog>();

            _mediator = new Mock<IMediator>();

            _repository = new Mock<IEmployerAgreementRepository>();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountId)).Returns(ExpectedAccountId);
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedEmployerAgreementId)).Returns(ExpectedEmployerAgreementId);
            
            _employerAgreementEventFactory = new Mock<IEmployerAgreementEventFactory>();
            _employerAgreementEventFactory.Setup(x => x.RemoveAgreementEvent(ExpectedHashedEmployerAgreementId)).Returns(new AgreementRemovedEvent {HashedAgreementId = ExpectedHashedEmployerAgreementId});
            _genericEventHandler = new Mock<IGenericEventFactory>();
            _genericEventHandler.Setup(x => x.Create(It.Is<AgreementRemovedEvent>(c => c.HashedAgreementId.Equals(ExpectedHashedEmployerAgreementId)))).Returns(new GenericEvent {Payload = ExpectedHashedEmployerAgreementId});

            _command = new RemoveLegalEntityCommand { HashedAccountId = ExpectedHashedAccountId, UserId = ExpectedUserId,HashedLegalAgreementId = ExpectedHashedEmployerAgreementId };
            _handler = new RemoveLegalEntityCommandHandler(_validator.Object, _logger.Object, _repository.Object, _mediator.Object, _hashingService.Object, _genericEventHandler.Object, _employerAgreementEventFactory.Object);
        }

        [Test]
        public void ThenTheValidatorIsCalledAnAndInvalidRequestExceptionThrownIfIsItNotValid()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemoveLegalEntityCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new RemoveLegalEntityCommand()));
        }

        [Test]
        public void ThenTheValidatorIsCalledAndAnUnauthorizedAccessExceptionIsThrownIfItIsNotAuthorized()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemoveLegalEntityCommand>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(new RemoveLegalEntityCommand()));
            _logger.Verify(x => x.Info(It.IsAny<string>()));
        }

        [Test]
        public async Task ThenTheRepositoryIsCalled()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _validator.Verify(x => x.ValidateAsync(It.Is<RemoveLegalEntityCommand>(c =>
                                                                          c.HashedAccountId.Equals(ExpectedHashedAccountId)
                                                                          && c.UserId.Equals(ExpectedUserId))));
            _repository.Verify(x => x.RemoveLegalEntityFromAccount(ExpectedEmployerAgreementId));
        }

        [Test]
        public async Task ThenTheAuditIsWrittenToWhenTheItemIsRemoved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Status") && y.NewValue.Equals(EmployerAgreementStatus.Removed.ToString())) != null 
                    )));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"EmployerAgreement {ExpectedHashedEmployerAgreementId} removed from account {ExpectedAccountId}"))));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(ExpectedAccountId.ToString()) && y.Type.Equals("Account")) != null
                    )));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(ExpectedHashedEmployerAgreementId.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("EmployerAgreement")
                    )));
        }

        [Test]
        public async Task ThenTheEventIsFired()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _genericEventHandler.Verify(x=>x.Create(It.IsAny<AgreementRemovedEvent>()),Times.Once);
            _mediator.Verify(x=>x.SendAsync(It.Is<PublishGenericEventCommand>(c=>c.Event.Payload.Equals(ExpectedHashedEmployerAgreementId))));
        }
        
    }
}
