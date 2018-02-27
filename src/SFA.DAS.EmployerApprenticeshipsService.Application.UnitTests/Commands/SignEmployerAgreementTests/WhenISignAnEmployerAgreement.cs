using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types.Events.Agreement;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Commitment;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.SignEmployerAgreementTests
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
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IAccountAgreementService> _accountAgreement;

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
            _validator.Setup(x => x.ValidateAsync(It.IsAny<SignEmployerAgreementCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});


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

            _messagePublisher = new Mock<IMessagePublisher>();

            _accountAgreement = new Mock<IAccountAgreementService>();

            _handler = new SignEmployerAgreementCommandHandler(
                _membershipRepository.Object, 
                _agreementRepository.Object, 
                _hashingService.Object, 
                _validator.Object,
                _agreementEventFactory.Object, 
                _genericEventFactory.Object,
                _mediator.Object,
                _messagePublisher.Object,
                _commintmentService.Object,
                _accountAgreement.Object);

            _owner = new MembershipView
            {
                UserId = 1,
                RoleId = (short) Role.Owner,
                FirstName = "Fred",
                LastName = "Bloggs"
            };

            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                .ReturnsAsync(_owner);
        }

        [Test]
        public void ThenTheValidatorIsCalledAndAnInvalidRequestExceptionIsThrownIfItIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<SignEmployerAgreementCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public void ThenIfTheUserIsNotConnectedToTheAccountThenAnUnauthorizedExceptionIsThrown()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(null);

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [TestCase(Role.Transactor)]
        [TestCase(Role.Viewer)]
        [TestCase(Role.None)]
        public void ThenIfTheUserIsNotAnOwnerThenAnUnauthorizedExceptionIsThrown(Role role)
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(new MembershipView {RoleId = (short)role});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenIfTheCommandIsValidTheRepositoryIsCalledWithThePassedParameters()
        {
            //Arrange
            var agreementId = 87761263;
            _hashingService.Setup(x => x.DecodeValue(_command.HashedAgreementId)).Returns(agreementId);

            //Act
            await _handler.Handle(_command);

            //Assert
            _agreementRepository.Verify(x=>x.SignAgreement(It.Is<SignEmployerAgreement>(c=>c.SignedDate.Equals(_command.SignedDate) 
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
                .ReturnsAsync(new List<Cohort>{new Cohort()});

            //Act
            await _handler.Handle(_command);

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<AgreementSignedMessage>(
                m => m.CohortCreated && m.AccountId == AccountId && m.AgreementId == AgreementId && m.OrganisationName == OrganisationName && m.LegalEntityId == LegalEntityId)));
        }

        [Test]
        public async Task ThenIfICannotGetCommitmentsForTheAccountIStillNotifyTheService()
        { 
            //Arrange
            _commintmentService.Setup(x => x.GetEmployerCommitments(It.IsAny<long>()))
                .ReturnsAsync(null);

            //Act
            await _handler.Handle(_command);

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<AgreementSignedMessage>(
                m => !m.CohortCreated && m.AccountId == AccountId && m.AgreementId == AgreementId && m.OrganisationName == OrganisationName && m.LegalEntityId == LegalEntityId)));
        }
    }
}