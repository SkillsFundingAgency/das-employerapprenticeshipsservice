using System;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using Moq;
using SFA.DAS.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests.Given_User_Is_Not_In_EOI_Whitelist
{
    [ExcludeFromCodeCoverage]
    public  class Given_User_Is_Not_In_EIO_Whitelist
    {
        protected Mock<IAccountRepository> AccountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IMediator> _mediator;
        private Mock<IGenericEventFactory> _genericEventFactory;
        protected CreateLegalEntityCommandHandler CommandHandler;
        protected CreateLegalEntityCommand Command;
        protected MembershipView Owner;
        private EmployerAgreementView _agreementView;
        private Mock<ILegalEntityEventFactory> _legalEntityEventFactory;
        private Mock<IHashingService> _hashingService;
        private Mock<IAccountLegalEntityPublicHashingService> _accountLegalEntityPublicHashingService;
        private Mock<IAgreementService> _agreementService;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IValidator<CreateLegalEntityCommand>> _validator;
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IAuthorizationService> _authorizationService;
        protected Mock<IEmployerAccountRepository> EmployerAccountsRepository;

        private const string ExpectedAccountLegalEntityPublicHashString = "ALEPUB";

        public Given_User_Is_Not_In_EIO_Whitelist()
        {
            AccountRepository = new Mock<IAccountRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _mediator = new Mock<IMediator>();

            Owner = new MembershipView
            {
                AccountId = 1234,
                UserId = 9876,
                Email = "test@test.com",
                FirstName = "Bob",
                LastName = "Green",
                UserRef = Guid.NewGuid().ToString(),
                Role = Role.Owner,
            };

            _agreementView = new EmployerAgreementView
            {
                Id = 123,
                AccountId = Owner.AccountId,
                SignedDate = DateTime.Now,
                SignedByName = $"{Owner.FirstName} {Owner.LastName}",
                LegalEntityId = 5246,
                LegalEntityName = "Test Corp",
                LegalEntityCode = "3476782638",
                LegalEntitySource = OrganisationType.CompaniesHouse,
                LegalEntityAddress = "123 test street",
                LegalEntityInceptionDate = DateTime.Now,
                AccountLegalEntityId = 830
            };

            Command = new CreateLegalEntityCommand
            {
                HashedAccountId = "ABC123",
                SignAgreement = true,
                SignedDate = DateTime.Now.AddDays(-10),
                ExternalUserId = Owner.UserRef,
                Name = "Org Ltd",
                Code = "3476782638",
                Source = OrganisationType.CompaniesHouse,
                Address = "123 test street"
            };

            _membershipRepository.Setup(
                    x => x.GetCaller(
                        Command.HashedAccountId,
                        Command.ExternalUserId))
                .ReturnsAsync(Owner);

            AccountRepository
                .Setup(
                    x => x.CreateLegalEntityWithAgreement(
                        It.Is<CreateLegalEntityWithAgreementParams>(
                            createParams => createParams.AccountId == Owner.AccountId)))
                .ReturnsAsync(_agreementView);

            _genericEventFactory = new Mock<IGenericEventFactory>();
            _legalEntityEventFactory = new Mock<ILegalEntityEventFactory>();
            _eventPublisher = new Mock<IEventPublisher>();
            _agreementService = new Mock<IAgreementService>();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(hs => hs.HashValue(It.IsAny<long>())).Returns<long>(value => $"*{value}*");
            _hashingService.Setup(hs => hs.DecodeValue(Command.HashedAccountId)).Returns(Owner.AccountId);

            _accountLegalEntityPublicHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            _accountLegalEntityPublicHashingService.Setup(x => x.HashValue(_agreementView.AccountLegalEntityId))
                .Returns(ExpectedAccountLegalEntityPublicHashString);

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();

            _validator = new Mock<IValidator<CreateLegalEntityCommand>>();

            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateLegalEntityCommand>()))
                .ReturnsAsync(new ValidationResult() {IsUnauthorized = false});

            _authorizationService = new Mock<IAuthorizationService>();
            _authorizationService
                .Setup(
                    m => m.IsAuthorized(FeatureType.ExpressionOfInterest))
                .Returns(false);

            EmployerAccountsRepository = new Mock<IEmployerAccountRepository>();

            CommandHandler = new CreateLegalEntityCommandHandler(
                AccountRepository.Object,
                _membershipRepository.Object,
                _mediator.Object,
                _genericEventFactory.Object,
                _legalEntityEventFactory.Object,
                _eventPublisher.Object,
                _hashingService.Object,
                _accountLegalEntityPublicHashingService.Object,
                _agreementService.Object,
                _employerAgreementRepository.Object,
                _validator.Object,
                _authorizationService.Object,
                EmployerAccountsRepository.Object);
        }
    }
}