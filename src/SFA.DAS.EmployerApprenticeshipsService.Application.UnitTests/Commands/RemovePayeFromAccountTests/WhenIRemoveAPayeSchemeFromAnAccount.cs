﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types.Events.PayeScheme;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
using IGenericEventFactory = SFA.DAS.EAS.Application.Factories.IGenericEventFactory;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RemovePayeFromAccountTests
{
    public class WhenIRemoveAPayeSchemeFromAnAccount
    {
        private RemovePayeFromAccountCommandHandler _handler;
        private Mock<IValidator<RemovePayeFromAccountCommand>> _validator;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator;
        private Mock<IGenericEventFactory> _genericEventFactory;
        private Mock<IPayeSchemeEventFactory> _payeSchemeEventFactory;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IMembershipRepository> _mockMembershipRepository;

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();

            _validator = new Mock<IValidator<RemovePayeFromAccountCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult());

            _hashingService = new Mock<IHashingService>();

            _mediator = new Mock<IMediator>();
            _genericEventFactory = new Mock<IGenericEventFactory>();
            _payeSchemeEventFactory = new Mock<IPayeSchemeEventFactory>();

            _messagePublisher = new Mock<IMessagePublisher>();
            _mockMembershipRepository=new Mock<IMembershipRepository>();

            _mockMembershipRepository.Setup(a => a.GetCaller(It.IsAny<long>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new MembershipView { FirstName = "fn", LastName = "ln" }));

            _handler = new RemovePayeFromAccountCommandHandler(
                _mediator.Object, 
                _validator.Object,
                _accountRepository.Object, 
                _hashingService.Object,
                _genericEventFactory.Object,
                _payeSchemeEventFactory.Object,
                _messagePublisher.Object,
                _mockMembershipRepository.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _handler.Handle(new RemovePayeFromAccountCommand(null, null, Guid.Empty, false, null));

            //Assert
            _validator.Verify(x=>x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>()), Times.Once);
        }

        [Test]
        public void ThenAnInvalidOperationExceptionIsThrownIfTheCommandIsNotValidAndTheRespositoryIsNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new RemovePayeFromAccountCommand(null, null, Guid.Empty, false, null)));

            //Assert
            _accountRepository.Verify(x => x.RemovePayeFromAccount(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }
        
        [Test]
        public void ThenAnUnauthorizedExceptionIsThrownIfTheCommandIsValidAndUnauthorizeddAndTheRespositoryIsNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(new RemovePayeFromAccountCommand(null, null, Guid.Empty, false, null)));

            //Assert
            _accountRepository.Verify(x => x.RemovePayeFromAccount(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledWhenTheCommandIsValid()
        {
            //Arrange
            var accountId = 8487533;
            var hashedId = "12FFF";
            var payeRef = "fkn/123";
            var userId = Guid.NewGuid();

            _hashingService.Setup(x => x.DecodeValue(hashedId)).Returns(accountId);

            //Act
            await _handler.Handle(new RemovePayeFromAccountCommand(hashedId, payeRef, userId,false,"companyName"));

            //Assert
            _accountRepository.Verify(x=>x.RemovePayeFromAccount(accountId,payeRef), Times.Once);
        }

        [Test]
        public async Task ThenAnEventIsPublishedToNofifyThePayeSchemeHasBeenRemoved()
        {
            //Arrange
            var command = new RemovePayeFromAccountCommand("ABC123", "3674826874623", Guid.NewGuid(), true, "companyName");

            //Act
            await _handler.Handle(command);

            //Assert
            _payeSchemeEventFactory.Verify(x => x.CreatePayeSchemeRemovedEvent(command.HashedAccountId, command.PayeRef));
            _genericEventFactory.Verify(x => x.Create(It.IsAny<PayeSchemeRemovedEvent>()), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<PublishGenericEventCommand>()));
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheCommandIsValid()
        {
            //Arrange
            var accountId = 123456;
            var command = new RemovePayeFromAccountCommand("ABC123", "3674826874623", Guid.NewGuid(), true, "companyName");


            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(accountId);

            //Act
            await _handler.Handle(command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(accountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("ExternalUserId") && y.NewValue.Equals(command.ExternalUserId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("PayeRef") && y.NewValue.Equals(command.PayeRef)) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {command.ExternalUserId} has removed PAYE schema {command.PayeRef} from account {accountId}"))));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(accountId.ToString()) && y.Type.Equals("Account")) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(command.PayeRef) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Paye"))));
        }

        [Test]
        public async Task ThenAMessageIsQueuedForPayeSchemeRemoved()
        {
            //Arrange
            var command = new RemovePayeFromAccountCommand("ABC123", "3674826874623", Guid.NewGuid(), true,"companyName");

            //Act
            await _handler.Handle(command);

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<PayeSchemeDeletedMessage>(c => c.PayeScheme.Equals(command.PayeRef))), Times.Once);
        }
    }
}
