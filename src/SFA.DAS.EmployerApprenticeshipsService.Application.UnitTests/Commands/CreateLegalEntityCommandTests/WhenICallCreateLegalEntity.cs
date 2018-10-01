using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EmployerAccounts.Messages.Events;


namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenICallCreateLegalEntity
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IMediator> _mediator;
        private Mock<IGenericEventFactory> _genericEventFactory;
        private CreateLegalEntityCommandHandler _commandHandler;
        private CreateLegalEntityCommand _command;
        private MembershipView _owner;
        private EmployerAgreementView _agreementView;
        private Mock<ILegalEntityEventFactory> _legalEntityEventFactory;
        private Mock<IHashingService> _hashingService;
        private Mock<IAgreementService> _agreementService;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IEventPublisher> _eventPublisher;

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _mediator = new Mock<IMediator>();

            _owner = new MembershipView
            {
                AccountId = 1234,
                UserId = 9876,
                Email = "test@test.com",
                FirstName = "Bob",
                LastName = "Green",
                UserRef = Guid.NewGuid().ToString()
            };

            _agreementView = new EmployerAgreementView
            {
                Id = 123,
                AccountId = _owner.AccountId,
                SignedDate = DateTime.Now,
                SignedByName = $"{_owner.FirstName} {_owner.LastName}",
                LegalEntityId = 5246,
                LegalEntityName = "Test Corp",
                LegalEntityCode = "3476782638",
                LegalEntitySource = OrganisationType.CompaniesHouse,
                LegalEntityAddress = "12, test street",
                LegalEntityInceptionDate = DateTime.Now,
                AccountLegalentityId = 830
            };

            _command = new CreateLegalEntityCommand
            {
                HashedAccountId = "ABC123",
                SignAgreement = true,
                SignedDate = DateTime.Now.AddDays(-10),
                ExternalUserId = _owner.UserRef,
                Name = "Org Ltd"
            };

            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                                 .ReturnsAsync(_owner);

            _accountRepository
                .Setup(x => x.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(
                    createParams => createParams.AccountId == _owner.AccountId)))
                .ReturnsAsync(_agreementView);

            _genericEventFactory = new Mock<IGenericEventFactory>();
            _legalEntityEventFactory = new Mock<ILegalEntityEventFactory>();
            _eventPublisher = new Mock<IEventPublisher>();
            _hashingService = new Mock<IHashingService>();
            _agreementService = new Mock<IAgreementService>();

            _hashingService.Setup(hs => hs.HashValue(It.IsAny<long>())).Returns<long>(value => $"*{value}*");
            _hashingService.Setup(hs => hs.DecodeValue(_command.HashedAccountId)).Returns(_owner.AccountId);
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();

            _commandHandler = new CreateLegalEntityCommandHandler(
                _accountRepository.Object,
                _membershipRepository.Object,
                _mediator.Object,
                _genericEventFactory.Object,
                _legalEntityEventFactory.Object,
                _eventPublisher.Object,
                _hashingService.Object,
                _agreementService.Object,
                _employerAgreementRepository.Object
                );
        }

        [Test]
        public async Task ThenTheLegalEntityIsCreated()
        {
            //Act
            var result = await _commandHandler.Handle(_command);

            //Assert
            Assert.AreSame(_agreementView, result.AgreementView);
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledForTheLegalEntityWhenTheCommandIsValid()
        {
            //Act
            await _commandHandler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Id") && y.NewValue.Equals(_agreementView.LegalEntityId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Name") && y.NewValue.Equals(_agreementView.LegalEntityName)) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Code") && y.NewValue.Equals(_agreementView.LegalEntityCode)) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Address") && y.NewValue.Equals(_agreementView.LegalEntityAddress)) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("DateOfInception") && y.NewValue.Equals(_agreementView.LegalEntityInceptionDate.Value.ToString("G"))) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} added legal entity {_agreementView.LegalEntityId} to account {_agreementView.AccountId}"))));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_agreementView.AccountId.ToString()) && y.Type.Equals("Account")) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(_agreementView.LegalEntityId.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("LegalEntity"))));
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledForTheAgreementWhenTheCommandIsValid()
        {
            //Act
            await _commandHandler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("SignedDate") && y.NewValue.Equals(_agreementView.SignedDate.Value.ToString("G"))) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("SignedBy") && y.NewValue.Equals($"{_owner.FirstName} {_owner.LastName}")) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} added signed agreement {_agreementView.Id} to account {_agreementView.AccountId}"))));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_agreementView.AccountId.ToString()) && y.Type.Equals("Account")) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(_agreementView.Id.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("EmployerAgreement"))));
        }

        [Test]
        public async Task ThenAnOrganisationCodeIsGeneratedIfOneIsNotSupplied()
        {
            //Act
            await _commandHandler.Handle(_command);

            //Assert
            _accountRepository.Verify(r => r.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(cp => !string.IsNullOrEmpty(cp.Code))), Times.Once);
        }

        [Test]
        public async Task ThenAHashedAgreementIdIsSupplied()
        {
            //Act
            var employerAgreementView = await _commandHandler.Handle(_command);

            //Assert
            var expectedHashedAgreementId = $"*{employerAgreementView.AgreementView.Id}*";
            Assert.AreEqual(expectedHashedAgreementId, employerAgreementView.AgreementView.HashedAgreementId);
        }

        [Test]
        public async Task TheShouldInvalidateAccountAgreementCache()
        {
            await _commandHandler.Handle(_command);

            _agreementService.Verify(s => s.RemoveFromCacheAsync(_owner.AccountId));
        }

        [Test]
        public async Task ThenAddedLegalEntityEventIsPublished()
        {
            await _commandHandler.Handle(_command);

            _eventPublisher.Verify(ep => ep.Publish(It.Is<AddedLegalEntityEvent>(cmd =>
                cmd.AccountId.Equals(_owner.AccountId) &&
                cmd.AgreementId.Equals(_agreementView.Id) &&
                cmd.LegalEntityId.Equals(_agreementView.LegalEntityId) &&
                cmd.AccountLegalEntityId.Equals(_agreementView.AccountLegalentityId) &&
                cmd.OrganisationName.Equals(_command.Name) &&
                cmd.UserName.Equals(_owner.FullName()) &&
                cmd.UserRef.Equals(Guid.Parse(_owner.UserRef)))));
            //cmd.Created.)));
        }
    }
}
