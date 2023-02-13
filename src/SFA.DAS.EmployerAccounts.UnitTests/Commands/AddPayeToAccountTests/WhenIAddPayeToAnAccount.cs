using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus;
using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Events.PayeScheme;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.EmployerAccounts.UnitTests.ObjectMothers;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Testing.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.AddPayeToAccountTests
{
    public class WhenIAddPayeToAnAccount
    {
        private AddPayeToAccountCommandHandler _addPayeToAccountCommandHandler;
        private Mock<IValidator<AddPayeToAccountCommand>> _validator;
        private Mock<IPayeRepository> _accountRepository;
        private TestableEventPublisher _eventPublisher;
        private Mock<IEncodingService> _encodingService;
        private Mock<IMediator> _mediator;
        private Mock<IGenericEventFactory> _genericEventFactory;
        private Mock<IPayeSchemeEventFactory> _payeSchemeEventFactory;

        private const string ExpectedHashedAccountId = "GG7840";
        private const long ExpectedAccountId = 54564;
        private const string ExpectedPayeName = "Paye Scheme 1";
        private User _user;

        [SetUp]
        public void Arrange()
        {
            _eventPublisher = new TestableEventPublisher();

            _accountRepository = new Mock<IPayeRepository>();

            _validator = new Mock<IValidator<AddPayeToAccountCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddPayeToAccountCommand>())).ReturnsAsync(new ValidationResult());

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(ExpectedHashedAccountId, EncodingType.AccountId)).Returns(ExpectedAccountId);

            _mediator = new Mock<IMediator>();
            _genericEventFactory = new Mock<IGenericEventFactory>();
            _payeSchemeEventFactory = new Mock<IPayeSchemeEventFactory>();

            _addPayeToAccountCommandHandler = new AddPayeToAccountCommandHandler(
                _validator.Object,
                _accountRepository.Object,
                _eventPublisher,
                _encodingService.Object,
                _mediator.Object,
                _genericEventFactory.Object,
                _payeSchemeEventFactory.Object);

            _user = new User
            {
                FirstName = "Bob",
                LastName = "Green",
                Ref = Guid.NewGuid()
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetUserByRefQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetUserByRefResponse { User = _user });
        }

        [Test]
        public void ThenTheValidatorIsCalled()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AddPayeToAccountCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _addPayeToAccountCommandHandler.Handle(new AddPayeToAccountCommand(), CancellationToken.None));

            //Assert
            _validator.Verify(x => x.ValidateAsync(It.IsAny<AddPayeToAccountCommand>()), Times.Once);
            _accountRepository.Verify(x => x.AddPayeToAccount(It.IsAny<Paye>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledIfTheCommandIsValid()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(hashedAccountId: ExpectedHashedAccountId);

            //Act
            await _addPayeToAccountCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            _accountRepository.Verify(x => x.AddPayeToAccount(
                                                AssertPayeScheme(command)), Times.Once);
        }

        [Test]
        public async Task ThenAPaymentSchemeAddedEventIsPublished()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(hashedAccountId: ExpectedHashedAccountId);

            //Act
            await _addPayeToAccountCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            _eventPublisher.Events.Should().HaveCount(1);

            var message = _eventPublisher.Events.OfType<AddedPayeSchemeEvent>().Single();

            message.PayeRef.Should().Be(command.Empref);
            message.AccountId.Should().Be(ExpectedAccountId);
            message.UserName.Should().Be(_user.FullName);
            message.UserRef.Should().Be(_user.Ref);

            _mediator.Verify(x => x.Send(It.IsAny<AccountLevyStatusCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ThenAnAccountLevyStatusCommandIsPublishedForAnAornPaye()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(aorn: "Aorn", hashedAccountId: ExpectedHashedAccountId);

            //Act
            await _addPayeToAccountCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<AccountLevyStatusCommand>(c =>
                c.AccountId.Equals(ExpectedAccountId) && 
                c.ApprenticeshipEmployerType.Equals(ApprenticeshipEmployerType.NonLevy)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenAnEventIsPublishedToNofifyThePayeSchemeHasBeenAdded()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create();

            //Act
            await _addPayeToAccountCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            _payeSchemeEventFactory.Verify(x => x.CreatePayeSchemeAddedEvent(command.HashedAccountId, command.Empref));
            _genericEventFactory.Verify(x => x.Create(It.IsAny<PayeSchemeAddedEvent>()), Times.Once);
            _mediator.Verify(x => x.Send(It.IsAny<PublishGenericEventCommand>(), It.IsAny<CancellationToken>()));

        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheCreateInvitationCommandIsValid()
        {
            //Arrange
            var command = AddPayeToNewLegalEntityCommandObjectMother.Create(hashedAccountId: ExpectedHashedAccountId);

            //Act
            await _addPayeToAccountCommandHandler.Handle(command, CancellationToken.None);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Ref") && y.NewValue.Equals(command.Empref)) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccessToken") && y.NewValue.Equals(command.AccessToken)) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("RefreshToken") && y.NewValue.Equals(command.RefreshToken)) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Name") && y.NewValue.Equals(command.EmprefName)) != null
                    ), It.IsAny<CancellationToken>()));
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"Paye scheme {command.Empref} added to account {ExpectedAccountId}")), It.IsAny<CancellationToken>()));
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(ExpectedAccountId.ToString()) && y.Type.Equals("Account")) != null
                    ), It.IsAny<CancellationToken>()));
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(command.Empref.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Paye")
                    ), It.IsAny<CancellationToken>()));
        }

        private static Paye AssertPayeScheme(AddPayeToAccountCommand command)
        {
            return It.Is<Paye>(
                c => c.AccessToken.Equals(command.AccessToken) &&
                   c.RefreshToken.Equals(command.RefreshToken) &&
                   c.EmpRef.Equals(command.Empref) &&
                   c.AccountId.Equals(ExpectedAccountId) &&
                   c.RefName.Equals(ExpectedPayeName)
                );
        }
    }
}
