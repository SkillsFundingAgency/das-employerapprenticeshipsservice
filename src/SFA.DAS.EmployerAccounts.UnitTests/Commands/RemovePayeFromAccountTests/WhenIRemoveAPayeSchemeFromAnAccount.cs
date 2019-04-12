﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Events.PayeScheme;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Testing;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.RemovePayeFromAccountTests
{
    public class WhenIRemoveAPayeSchemeFromAnAccount
    {
        private const string PayeScheme = "AB/00001C";
        private const string UserFirstName = "Bob";
        private const string UserLastName = "reen";
        private const string UserName = UserFirstName + " " + UserLastName;
        private const string HashedAccountId = "ABC123";
        private const long AccountId = 123456;
        private const string OrganisationName = "Test Corp";
        private static readonly Guid UserRef = Guid.NewGuid();

        private RemovePayeFromAccountCommandHandler _handler;
        private Mock<IValidator<RemovePayeFromAccountCommand>> _validator;
        private Mock<IPayeRepository> _accountRepository;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator;
        private Mock<IGenericEventFactory> _genericEventFactory;
        private Mock<IPayeSchemeEventFactory> _payeSchemeEventFactory;
        private TestableEventPublisher _eventPublisher;
        private Mock<IMembershipRepository> _mockMembershipRepository;

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IPayeRepository>();

            _validator = new Mock<IValidator<RemovePayeFromAccountCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult());

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(AccountId);

            _mediator = new Mock<IMediator>();
            _genericEventFactory = new Mock<IGenericEventFactory>();
            _payeSchemeEventFactory = new Mock<IPayeSchemeEventFactory>();

            _eventPublisher = new TestableEventPublisher();
            _mockMembershipRepository = new Mock<IMembershipRepository>();

            _mockMembershipRepository.Setup(a => a.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new MembershipView { FirstName = UserFirstName, LastName = UserLastName, AccountId = AccountId, UserRef = UserRef.ToString() }));

            _handler = new RemovePayeFromAccountCommandHandler(
                _mediator.Object,
                _validator.Object,
                _accountRepository.Object,
                _hashingService.Object,
                _genericEventFactory.Object,
                _payeSchemeEventFactory.Object,
                _eventPublisher,
                _mockMembershipRepository.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _handler.Handle(new RemovePayeFromAccountCommand(null, null, null, false, null));

            //Assert
            _validator.Verify(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>()), Times.Once);
        }

        [Test]
        public void ThenAnInvalidOperationExceptionIsThrownIfTheCommandIsNotValidAndTheRespositoryIsNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new RemovePayeFromAccountCommand(null, null, null, false, null)));

            //Assert
            _accountRepository.Verify(x => x.RemovePayeFromAccount(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ThenAnUnauthorizedExceptionIsThrownIfTheCommandIsValidAndUnauthorizeddAndTheRespositoryIsNotCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RemovePayeFromAccountCommand>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(new RemovePayeFromAccountCommand(null, null, null, false, null)));

            //Assert
            _accountRepository.Verify(x => x.RemovePayeFromAccount(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledWhenTheCommandIsValid()
        {
            //Act
            await _handler.Handle(new RemovePayeFromAccountCommand(HashedAccountId, PayeScheme, UserRef.ToString(), false, OrganisationName));

            //Assert
            _accountRepository.Verify(x => x.RemovePayeFromAccount(AccountId, PayeScheme), Times.Once);
        }

        [Test]
        public async Task ThenAnEventIsPublishedToNofifyThePayeSchemeHasBeenRemoved()
        {
            //Arrange
            var command = new RemovePayeFromAccountCommand(HashedAccountId, PayeScheme, UserRef.ToString(), true, OrganisationName);

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

            var command = new RemovePayeFromAccountCommand(HashedAccountId, PayeScheme, UserRef.ToString(), true, OrganisationName);


            //Act
            await _handler.Handle(command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("UserId") && y.NewValue.Equals(UserRef.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("PayeRef") && y.NewValue.Equals(PayeScheme)) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {UserRef.ToString()} has removed PAYE schema {PayeScheme} from account {AccountId}"))));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(AccountId.ToString()) && y.Type.Equals("Account")) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(PayeScheme) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Paye"))));
        }

        [Test]
        public async Task ThenAMessageIsQueuedForPayeSchemeRemoved()
        {
            //Arrange
            var command = new RemovePayeFromAccountCommand(HashedAccountId, PayeScheme, UserRef.ToString(), true, OrganisationName);

            //Act
            await _handler.Handle(command);

            //Assert
            _eventPublisher.Events.Should().HaveCount(1);

            var message = _eventPublisher.Events.OfType<DeletedPayeSchemeEvent>().Single();

            message.AccountId.Should().Be(AccountId);
            message.OrganisationName.Should().Be(OrganisationName);
            message.PayeRef.Should().Be(PayeScheme);
            message.UserName.Should().Be(UserName);
            message.UserRef.Should().Be(UserRef);
            Assert.IsTrue(DateTime.UtcNow - message.Created < TimeSpan.FromMinutes(1));
        }
    }
}
