using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.PublishGenericEvent;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Events.Agreement;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.Commitments;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Testing;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.SignEmployerAgreementTests
{
    [TestFixture]
    public class WhenISignAnEmployerAgreement
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IEmployerAgreementRepository> _agreementRepository;
        private SignEmployerAgreementCommandHandler _handler;
        private SignEmployerAgreementCommand _command;
        private MembershipView _owner;
        private Mock<IHashingService> _hashingService;
        private Mock<IValidator<SignEmployerAgreementCommand>> _validator;
        private Mock<IEmployerAgreementEventFactory> _agreementEventFactory;
        private Mock<IGenericEventFactory> _genericEventFactory;
        private Mock<IMediator> _mediator;
        private EmployerAgreementView _agreement;
        private AgreementSignedEvent _agreementEvent;
        private Mock<ICommitmentService> _commintmentService;
        private TestableEventPublisher _eventPublisher;
        private Mock<IAgreementService> _agreementService;

        private const long AccountId = 223344;
        private const long AgreementId = 123433;
        private const long LegalEntityId = 111333;
        private const string OrganisationName = "Foo";
        private const string HashedLegalEntityId = "2635JHG";

        [SetUp]
        public void Setup()
        {
            _command = new SignEmployerAgreementCommand
            {
                HashedAccountId = "1AVCFD",
                HashedAgreementId = "2EQWE34",
                ExternalUserId = Guid.NewGuid().ToString(),
                SignedDate = DateTime.Now
            };

            _membershipRepository = new Mock<IMembershipRepository>();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(_command.HashedAccountId)).Returns(AccountId);
            _hashingService.Setup(x => x.DecodeValue(_command.HashedAgreementId)).Returns(AgreementId);
            _hashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns(HashedLegalEntityId);

            _validator = new Mock<IValidator<SignEmployerAgreementCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<SignEmployerAgreementCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });


            _agreement = new EmployerAgreementView
            {
                HashedAgreementId = "124GHJG",
                LegalEntityId = LegalEntityId,
                LegalEntityName = OrganisationName
            };

            _agreementRepository = new Mock<IEmployerAgreementRepository>();

            _agreementRepository.Setup(x => x.GetEmployerAgreement(It.IsAny<long>()))
                                .ReturnsAsync(_agreement);

            _agreementEventFactory = new Mock<IEmployerAgreementEventFactory>();

            _agreementEvent = new AgreementSignedEvent();

            _agreementEventFactory.Setup(
                    x => x.CreateSignedEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                          .Returns(_agreementEvent);

            _genericEventFactory = new Mock<IGenericEventFactory>();
            _mediator = new Mock<IMediator>();

            _commintmentService = new Mock<ICommitmentService>();

            _commintmentService.Setup(x => x.GetEmployerCommitments(It.IsAny<long>()))
                .ReturnsAsync(new List<Cohort>());

            _eventPublisher = new TestableEventPublisher();

            _agreementService = new Mock<IAgreementService>();

            _handler = new SignEmployerAgreementCommandHandler(
                _membershipRepository.Object,
                _agreementRepository.Object,
                _hashingService.Object,
                _validator.Object,
                _agreementEventFactory.Object,
                _genericEventFactory.Object,
                _mediator.Object,
                _eventPublisher,
                _commintmentService.Object,
                _agreementService.Object);

            _owner = new MembershipView
            {
                UserId = 1,
                Role = Role.Owner,
                FirstName = "Fred",
                LastName = "Bloggs",
                UserRef = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                .ReturnsAsync(_owner);
        }

        [Test]
        public void ThenTheValidatorIsCalledAndAnInvalidRequestExceptionIsThrownIfItIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<SignEmployerAgreementCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public void ThenIfTheUserIsNotConnectedToTheAccountThenAnUnauthorizedExceptionIsThrown()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(() => null);

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [TestCase(Role.Transactor)]
        [TestCase(Role.Viewer)]
        [TestCase(Role.None)]
        public void ThenIfTheUserIsNotAnOwnerThenAnUnauthorizedExceptionIsThrown(Role role)
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(new MembershipView { Role = role });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenIfTheCommandIsValidTheRepositoryIsCalledWithThePassedParameters()
        {
            //Arrange
            const int agreementId = 87761263;
            _hashingService.Setup(x => x.DecodeValue(_command.HashedAgreementId)).Returns(agreementId);

            //Act
            await _handler.Handle(_command);

            //Assert
            _agreementRepository.Verify(x => x.SignAgreement(It.Is<SignEmployerAgreement>(c => c.SignedDate.Equals(_command.SignedDate)
                                  && c.AgreementId.Equals(agreementId)
                                  && c.SignedDate.Equals(_command.SignedDate)
                                  && c.SignedById.Equals(_owner.UserId)
                                  && c.SignedByName.Equals($"{_owner.FirstName} {_owner.LastName}")
                                )));
        }

        [Test]
        public async Task ThenAnEventShouldBePublished()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _agreementRepository.Verify(x => x.GetEmployerAgreement(AgreementId), Times.Once);
            _hashingService.Verify(x => x.HashValue(_agreement.LegalEntityId), Times.Once);
            _agreementEventFactory.Verify(x => x.CreateSignedEvent(_command.HashedAccountId, HashedLegalEntityId,
                _command.HashedAgreementId), Times.Once);
            _genericEventFactory.Verify(x => x.Create(_agreementEvent), Times.Once);
            _mediator.Verify(x => x.SendAsync(It.IsAny<PublishGenericEventCommand>()), Times.Once);

        }

        [Test]
        public async Task ThenTheServiceShouldBeNotified()
        {
            //Arrange
            _commintmentService.Setup(x => x.GetEmployerCommitments(It.IsAny<long>()))
                .ReturnsAsync(new List<Cohort> { new Cohort() });

            //Act
            await _handler.Handle(_command);

            //Assert
            _eventPublisher.Events.Should().HaveCount(1);

            var message = _eventPublisher.Events.First().As<SignedAgreementEvent>();

            message.AccountId.Should().Be(AccountId);
            message.AgreementId.Should().Be(AgreementId);
            message.OrganisationName.Should().Be(OrganisationName);
            message.LegalEntityId.Should().Be(LegalEntityId);
            message.CohortCreated.Should().BeTrue();
            message.UserName.Should().Be(_owner.FullName());
            message.UserRef.Should().Be(_owner.UserRef);
        }

        [Test]
        public async Task ThenIfICannotGetCommitmentsForTheAccountIStillNotifyTheService()
        {
            //Arrange
            _commintmentService.Setup(x => x.GetEmployerCommitments(It.IsAny<long>()))
                .ReturnsAsync(() => null);

            //Act
            await _handler.Handle(_command);

            //Assert
            _eventPublisher.Events.Should().HaveCount(1);

            var message = _eventPublisher.Events.First().As<SignedAgreementEvent>();

            message.AccountId.Should().Be(AccountId);
            message.AgreementId.Should().Be(AgreementId);
            message.OrganisationName.Should().Be(OrganisationName);
            message.LegalEntityId.Should().Be(LegalEntityId);
            message.CohortCreated.Should().BeFalse();
            message.UserName.Should().Be(_owner.FullName());
            message.UserRef.Should().Be(_owner.UserRef);

        }

        [Test]
        public async Task TheShouldInvalidateAccountAgreementCache()
        {
            await _handler.Handle(_command);

            _agreementService.Verify(s => s.RemoveFromCacheAsync(AccountId));
        }
    }
}