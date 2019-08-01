using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
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

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateLegalEntityCommandTests.Given_User_Is_In_EOI_Whitelist
{
    [ExcludeFromCodeCoverage]
    public class Given_User_Is_In_EIO_Whitelist
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
        private Mock<IEventPublisher> _eventPublisher;
        private Mock<IAuthorizationService> _authorizationService;

        private const string ExpectedAccountLegalEntityPublicHashString = "ALEPUB";

        public Given_User_Is_In_EIO_Whitelist()
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

            _command = new CreateLegalEntityCommand
            {
                HashedAccountId = "ABC123",
                SignAgreement = true,
                SignedDate = DateTime.Now.AddDays(-10),
                ExternalUserId = _owner.UserRef,
                Name = "Org Ltd",
                Code = "3476782638",
                Source = OrganisationType.CompaniesHouse,
                Address = "123 test street"
            };

            _membershipRepository.Setup(
                    x => x.GetCaller(
                        _command.HashedAccountId,
                        _command.ExternalUserId))
                .ReturnsAsync(_owner);

            _accountRepository
                .Setup(
                    x => x.CreateLegalEntityWithAgreement(
                        It.Is<CreateLegalEntityWithAgreementParams>(
                            createParams => createParams.AccountId == _owner.AccountId)))
                .ReturnsAsync(_agreementView);

            _genericEventFactory = new Mock<IGenericEventFactory>();
            _legalEntityEventFactory = new Mock<ILegalEntityEventFactory>();
            _eventPublisher = new Mock<IEventPublisher>();
            _agreementService = new Mock<IAgreementService>();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(hs => hs.HashValue(It.IsAny<long>())).Returns<long>(value => $"*{value}*");
            _hashingService.Setup(hs => hs.DecodeValue(_command.HashedAccountId)).Returns(_owner.AccountId);

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
                .Returns(true);

            _commandHandler = new CreateLegalEntityCommandHandler(
                _accountRepository.Object,
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
                _authorizationService.Object);
        }

        public class When_LegalEntity_Is_Created : Given_User_Is_In_EIO_Whitelist
        {
            [Test]
            public async Task Then_LegalEntity_Is_Created_With_ExpressionOfInterest_Agreement()
            {
                await _commandHandler.Handle(_command);

                _accountRepository.Verify(
                    m => m.CreateLegalEntityWithAgreement(
                        It.Is<CreateLegalEntityWithAgreementParams>(
                            arg => arg.AgreementType.Equals(AgreementType.NoneLevyExpressionOfInterest))));
            }
        }
    }
}