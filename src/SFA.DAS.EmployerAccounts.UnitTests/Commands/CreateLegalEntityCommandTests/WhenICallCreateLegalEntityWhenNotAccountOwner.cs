using System;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests
{
    public class WhenICallCreateLegalEntityWhenNotAccountOwner
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
        private Mock<IAccountLegalEntityPublicHashingService> _accountLegalEntityPublicHashingService;
        private Mock<IAgreementService> _agreementService;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IValidator<CreateLegalEntityCommand>> _validator;

        private const string ExpectedAccountLegalEntityPublicHashString = "ALEPUB";

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
                UserRef = Guid.NewGuid().ToString(),
                Role = Role.Viewer
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
                LegalEntityInceptionDate = DateTime.Now
            };

            _command = new CreateLegalEntityCommand
            {
                HashedAccountId = "ABC123",
                SignAgreement = true,
                SignedDate = DateTime.Now.AddDays(-10),
                ExternalUserId = _owner.UserRef
            };

            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                .ReturnsAsync(_owner);

            _genericEventFactory = new Mock<IGenericEventFactory>();
            _legalEntityEventFactory = new Mock<ILegalEntityEventFactory>();
            _hashingService = new Mock<IHashingService>();
            _agreementService = new Mock<IAgreementService>();

            _hashingService.Setup(hs => hs.HashValue(It.IsAny<long>())).Returns<long>(value => $"*{value}*");
            _hashingService.Setup(hs => hs.DecodeValue(_command.HashedAccountId)).Returns(_owner.AccountId);

            _accountLegalEntityPublicHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            _accountLegalEntityPublicHashingService.Setup(x => x.HashValue(_agreementView.AccountLegalEntityId)).Returns(ExpectedAccountLegalEntityPublicHashString);

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();

            _validator = new Mock<IValidator<CreateLegalEntityCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateLegalEntityCommand>()))
                .ReturnsAsync(new ValidationResult() { IsUnauthorized = true });

            _commandHandler = new CreateLegalEntityCommandHandler(
                _accountRepository.Object,
                _membershipRepository.Object,
                _mediator.Object,
                _genericEventFactory.Object,
                _legalEntityEventFactory.Object,
                Mock.Of<IEventPublisher>(),
                _hashingService.Object,
                _accountLegalEntityPublicHashingService.Object,
                _agreementService.Object,
                _employerAgreementRepository.Object,
                _validator.Object
            );
        }

        [Test]
        public void ThenTheLegalEntityIsNotCreated()
        {
            //Act &
            //Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _commandHandler.Handle(_command));
        }
    }
}