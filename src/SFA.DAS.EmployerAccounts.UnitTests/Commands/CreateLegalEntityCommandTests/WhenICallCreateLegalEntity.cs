using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenICallCreateLegalEntity : CreateLegalEntityCommandTests
    {
        private MembershipView _owner;
        private EmployerAgreementView _agreementView;
        private const string ExpectedAccountLegalEntityPublicHashString = "ALEPUB";

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            _owner = new MembershipView
            {
                AccountId = 1234,
                UserId = 9876,
                Email = "test@test.com",
                FirstName = "Bob",
                LastName = "Green",
                UserRef = Guid.Parse(Command.ExternalUserId),
                Role = Role.Owner,
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
                LegalEntityAddress = "123 test street",
                LegalEntityInceptionDate = DateTime.Now,
                AccountLegalEntityId = 830
            };

            MembershipRepository.Setup(x => x.GetCaller(Command.HashedAccountId, Command.ExternalUserId)).ReturnsAsync(_owner);

            AccountRepository.Setup(x => x.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(createParams => createParams.AccountId == _owner.AccountId))).ReturnsAsync(_agreementView);

            EncodingService.Setup(hs => hs.Encode(It.IsAny<long>(), It.IsAny<EncodingType>())).Returns((long value, EncodingType encodingType) => $"*{value}*");
            EncodingService.Setup(hs => hs.Decode(Command.HashedAccountId, EncodingType.AccountId)).Returns(_owner.AccountId);

            EncodingService.Setup(x => x.Encode(_agreementView.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId)).Returns(ExpectedAccountLegalEntityPublicHashString);
        }

        [Test]
        public async Task ThenTheLegalEntityIsCreated()
        {
            //Act
            var result = await CommandHandler.Handle(Command, CancellationToken.None);

            //Assert
            Assert.AreSame(_agreementView, result.AgreementView);
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledForTheLegalEntityWhenTheCommandIsValid()
        {
            //Act
            await CommandHandler.Handle(Command, CancellationToken.None);

            //Assert
            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Id") && y.NewValue.Equals(_agreementView.LegalEntityId.ToString())) != null &&
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Name") && y.NewValue.Equals(_agreementView.LegalEntityName)) != null &&
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Code") && y.NewValue.Equals(_agreementView.LegalEntityCode)) != null &&
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Address") && y.NewValue.Equals(_agreementView.LegalEntityAddress)) != null &&
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("DateOfInception") && y.NewValue.Equals(_agreementView.LegalEntityInceptionDate.Value.ToString("G"))) != null), It.IsAny<CancellationToken>()));

            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.Description.Equals($"User {_owner.Email} added legal entity {_agreementView.LegalEntityId} to account {_agreementView.AccountId}")), It.IsAny<CancellationToken>()));

            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_agreementView.AccountId.ToString()) && y.Type.Equals("Account")) != null), It.IsAny<CancellationToken>()));

            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.AffectedEntity.Id.Equals(_agreementView.LegalEntityId.ToString()) &&
                c.EasAuditMessage.AffectedEntity.Type.Equals("LegalEntity")), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledForTheAgreementWhenTheCommandIsValid()
        {
            //Act
            await CommandHandler.Handle(Command, CancellationToken.None);

            //Assert
            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("SignedDate") && y.NewValue.Equals(_agreementView.SignedDate.Value.ToString("G"))) != null &&
                c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("SignedBy") && y.NewValue.Equals($"{_owner.FirstName} {_owner.LastName}")) != null), It.IsAny<CancellationToken>()));

            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.Description.Equals($"User {_owner.Email} added signed agreement {_agreementView.Id} to account {_agreementView.AccountId}")), It.IsAny<CancellationToken>()));

            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_agreementView.AccountId.ToString()) && y.Type.Equals("Account")) != null), It.IsAny<CancellationToken>()));

            Mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                c.EasAuditMessage.AffectedEntity.Id.Equals(_agreementView.Id.ToString()) &&
                c.EasAuditMessage.AffectedEntity.Type.Equals("EmployerAgreement")), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenAnOrganisationCodeIsGeneratedIfOneIsNotSupplied()
        {
            //Act
            await CommandHandler.Handle(Command, CancellationToken.None);

            //Assert
            AccountRepository.Verify(r => r.CreateLegalEntityWithAgreement(It.Is<CreateLegalEntityWithAgreementParams>(cp => !string.IsNullOrEmpty(cp.Code))), Times.Once);
        }

        [Test]
        public async Task ThenAddedLegalEntityEventIsPublished()
        {
            await CommandHandler.Handle(Command, CancellationToken.None);

            EventPublisher.Verify(ep => ep.Publish(It.Is<AddedLegalEntityEvent>(e =>
                B(e))));
        }

        private bool B(AddedLegalEntityEvent e)
        {
            return e.AccountId.Equals(_owner.AccountId) &&
                   e.AgreementId.Equals(_agreementView.Id) &&
                   e.LegalEntityId.Equals(_agreementView.LegalEntityId) &&
                   e.AccountLegalEntityId.Equals(_agreementView.AccountLegalEntityId) &&
                   e.AccountLegalEntityPublicHashedId.Equals(ExpectedAccountLegalEntityPublicHashString) &&
                   e.OrganisationName.Equals(Command.Name) &&
                   e.UserName.Equals(_owner.FullName()) &&
                   e.UserRef.Equals(_owner.UserRef) &&
                   e.OrganisationReferenceNumber.Equals(_agreementView.LegalEntityCode) &&
                   e.OrganisationAddress.Equals(_agreementView.LegalEntityAddress) &&
                   e.OrganisationType.ToString().Equals(_agreementView.LegalEntitySource.ToString());
        }
    }
}
