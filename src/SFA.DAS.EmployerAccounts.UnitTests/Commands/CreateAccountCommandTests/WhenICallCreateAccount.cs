﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Services;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus.Testing.Services;
using SFA.DAS.Validation;
using IAccountEventFactory = SFA.DAS.EmployerAccounts.Factories.IAccountEventFactory;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateAccountCommandTests
{
    public class WhenICallCreateAccount
    {
        private Mock<IAccountRepository> _accountRepository;
        private CreateAccountCommandHandler _handler;
        private Mock<IMediator> _mediator;
        private Mock<IValidator<CreateAccountCommand>> _validator;
        private Mock<IHashingService> _hashingService;
        private Mock<IPublicHashingService> _externalhashingService;
        private Mock<IAccountLegalEntityPublicHashingService> _accountLegalEntityHashingService;

        private Mock<IGenericEventFactory> _genericEventFactory;
        private Mock<IAccountEventFactory> _accountEventFactory;
        private Mock<IMembershipRepository> _mockMembershipRepository;
        private Mock<IEmployerAgreementRepository> _mockEmployerAgreementRepository;
        private Mock<IAuthorizationService> _mockAuthorizationService;
        private TestableEventPublisher _eventPublisher;

        private const long ExpectedAccountId = 12343322;
        private const long ExpectedLegalEntityId = 2222;
        private const long ExpectedEmployerAgreementId = 864;
        private const long ExpectedAccountLegalEntityId = 333;
        private const string ExpectedOrganisationReferenceNumber = "ORN";
        private const string ExpectedOrganisationAddress = "123 Fake Street";
        private const string ExpectedHashString = "123ADF23";
        private const string ExpectedPublicHashString = "SCUFF";
        private const string ExpectedAccountLegalEntityPublicHashString = "ALEPUB";

        private User _user;

        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();
            _accountRepository.Setup(x => x.GetPayeSchemesByAccountId(ExpectedAccountId)).ReturnsAsync(new List<PayeView> { new PayeView { LegalEntityId = ExpectedLegalEntityId } });
            _accountRepository.Setup(x => x.CreateAccount(It.IsAny<CreateAccountParams>())).ReturnsAsync(new CreateAccountResult { AccountId = ExpectedAccountId, LegalEntityId = ExpectedLegalEntityId, EmployerAgreementId = ExpectedEmployerAgreementId, AccountLegalEntityId = ExpectedAccountLegalEntityId });

            _eventPublisher = new TestableEventPublisher();
            _mediator = new Mock<IMediator>();

            _user = new User { Id = 33, FirstName = "Bob", LastName = "Green", Ref = Guid.NewGuid() };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserByRefQuery>()))
                .ReturnsAsync(new GetUserByRefResponse { User = _user });

            _validator = new Mock<IValidator<CreateAccountCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateAccountCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(ExpectedAccountId)).Returns(ExpectedHashString);

            _externalhashingService = new Mock<IPublicHashingService>();
            _externalhashingService.Setup(x => x.HashValue(ExpectedAccountId)).Returns(ExpectedPublicHashString);

            _accountLegalEntityHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            _accountLegalEntityHashingService.Setup(x => x.HashValue(ExpectedAccountLegalEntityId)).Returns(ExpectedAccountLegalEntityPublicHashString);

            _genericEventFactory = new Mock<IGenericEventFactory>();
            _accountEventFactory = new Mock<IAccountEventFactory>();

            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _mockAuthorizationService.Setup(x => x.IsAuthorized("EmployerFeature.ExpressionOfInterest")).Returns(false);

            _mockMembershipRepository = new Mock<IMembershipRepository>();
            _mockMembershipRepository.Setup(r => r.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new MembershipView() { FirstName = _user.FirstName, LastName = _user.LastName }));

            _mockEmployerAgreementRepository = new Mock<IEmployerAgreementRepository>();

            _handler = new CreateAccountCommandHandler(
                _accountRepository.Object,
                _mediator.Object,
                _validator.Object,
                _hashingService.Object,
                _externalhashingService.Object,
                _accountLegalEntityHashingService.Object,
                _genericEventFactory.Object,
                _accountEventFactory.Object,
                _mockMembershipRepository.Object,
                _mockEmployerAgreementRepository.Object,
                _eventPublisher,
                _mockAuthorizationService.Object);
        }

        [Test]
        public async Task ThenTheIdHashingServiceIsCalledAfterTheAccountIsCreated()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand { PayeReference = "123/abc,456/123", AccessToken = "123rd", RefreshToken = "45YT", ExternalUserId = _user.Ref.ToString() };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            _hashingService.Verify(x => x.HashValue(ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenTheIdPublicHashingServiceIsCalledAfterTheAccountIsCreated()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand { PayeReference = "123/abc,456/123", AccessToken = "123rd", RefreshToken = "45YT", ExternalUserId = _user.Ref.ToString() };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            _externalhashingService.Verify(x => x.HashValue(ExpectedAccountId), Times.Once);
        }


        [Test]
        public async Task ThenTheAccountIsUpdatedWithTheHashes()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand { PayeReference = "123/abc,456/123", AccessToken = "123rd", RefreshToken = "45YT", ExternalUserId = _user.Ref.ToString() };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            _accountRepository.Verify(x => x.UpdateAccountHashedIds(ExpectedAccountId, ExpectedHashString, ExpectedPublicHashString), Times.Once);
        }

        [Test]
        public async Task ThenTheHashedIdsAreReturnedInTheResponse()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand { PayeReference = "123/abc,456/123", AccessToken = "123rd", RefreshToken = "45YT", ExternalUserId = _user.Ref.ToString() };
            var agreementHash = "dfjngfddfgfd";
            _hashingService.Setup(x => x.HashValue(ExpectedEmployerAgreementId)).Returns(agreementHash);

            //Act
            var actual = await _handler.Handle(createAccountCommand);

            //Assert
            Assert.IsAssignableFrom<CreateAccountCommandResponse>(actual);
            Assert.AreEqual(ExpectedHashString, actual.HashedAccountId);
            Assert.AreEqual(agreementHash, actual.HashedAgreementId);
        }

        [Test]
        public void ThenTheValidatorIsCalledAndAInvalidRequestExceptionIsThrownWhenInvalid()
        {
            var command = new CreateAccountCommand();

            //Assert
            _validator.Setup(x => x.ValidateAsync(command)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(command));

            //Assert
            _validator.Verify(x => x.ValidateAsync(command), Times.Once);
        }

        [TestCase(true, AgreementType.NonLevyExpressionOfInterest)]
        [TestCase(false, AgreementType.Levy)]
        public async Task WillCreateNewAccountWithCorrectAgreementType(bool eoiWhitelisted, AgreementType agreementType)
        {
            const int accountId = 23;

            var cmd = new CreateAccountCommand
            {
                ExternalUserId = _user.Ref.ToString(),
                OrganisationReferenceNumber = "QWERTY",
                OrganisationName = "Qwerty Corp",
                OrganisationAddress = "Innovation Centre, Coventry, CV1 2TT",
                OrganisationDateOfInception = DateTime.Today.AddDays(-1000),
                Sector = "Sector",
                PayeReference = "120/QWERTY",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                OrganisationStatus = "active",
                EmployerRefName = "Paye Scheme 1",
                Aorn = "Aorn"
            };

            _mockAuthorizationService.Setup(x => x.IsAuthorized("EmployerFeature.ExpressionOfInterest")).Returns(eoiWhitelisted);

            _accountRepository.Setup(x => x.CreateAccount(It.IsAny<CreateAccountParams>())).ReturnsAsync(new CreateAccountResult { AccountId = accountId, LegalEntityId = 0L, EmployerAgreementId = 0L });

            var expectedHashedAccountId = "DJRR4359";
            _hashingService.Setup(x => x.HashValue(accountId)).Returns(expectedHashedAccountId);

            var expectedPublicHashedAccountId = "SCUFF";
            _externalhashingService.Setup(x => x.HashValue(accountId)).Returns(expectedPublicHashedAccountId);

            await _handler.Handle(cmd);

            _accountRepository.Verify(x => x.CreateAccount(It.Is<CreateAccountParams>(y =>
                    y.EmployerNumber == cmd.OrganisationReferenceNumber &&
                    y.EmployerName == cmd.OrganisationName &&
                    y.EmployerRegisteredAddress == cmd.OrganisationAddress &&
                    y.EmployerDateOfIncorporation == cmd.OrganisationDateOfInception &&
                    y.EmployerRef == cmd.PayeReference &&
                    y.AccessToken == cmd.AccessToken &&
                    y.RefreshToken == cmd.RefreshToken &&
                    y.CompanyStatus == cmd.OrganisationStatus &&
                    y.EmployerRefName == cmd.EmployerRefName &&
                    y.Source == (short)cmd.OrganisationType &&
                    y.PublicSectorDataSource == cmd.PublicSectorDataSource &&
                    y.Sector == cmd.Sector &&
                    y.Aorn == cmd.Aorn &&
                    y.AgreementType == agreementType
                )));
        }

        [Test]
        public async Task ThenIfTheCommandIsValidTheCreateAuditCommandIsCalledForEachComponent()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand
            {
                PayeReference = "123/abc,456/123",
                AccessToken = "123rd",
                RefreshToken = "45YT",
                OrganisationType = OrganisationType.CompaniesHouse,
                OrganisationName = "OrgName",
                EmployerRefName = "123AB",
                ExternalUserId = _user.Ref.ToString(),
                OrganisationAddress = "Address",
                OrganisationDateOfInception = new DateTime(2017, 01, 30),
                OrganisationReferenceNumber = "TYG56",
                OrganisationStatus = "Active",
                PublicSectorDataSource = 2,
                Sector = "Sector"
            };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            _mediator.Verify(
                x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(ExpectedAccountId.ToString())) != null &&
                    c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(ExpectedAccountId.ToString())) != null
                )));
        }

        [Test]
        public async Task ThenAnOrganisationCodeIsGeneratedIfOneIsNotSupplied()
        {
            //Arrange
            var createAccountCommand = new CreateAccountCommand { PayeReference = "123/abc,456/123", AccessToken = "123rd", RefreshToken = "45YT", OrganisationStatus = "active", ExternalUserId = _user.Ref.ToString() };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert

            _accountRepository.Verify(x => x.CreateAccount(It.IsAny<CreateAccountParams>()), Times.Once);
        }

        [Test]
        public async Task ThenAPayeSchemeAddedEventIsPublished()
        {
            //Arrange
            var expectedPayeRef = "123/abc";

            var createAccountCommand = new CreateAccountCommand { PayeReference = expectedPayeRef, AccessToken = "123rd", RefreshToken = "45YT", OrganisationStatus = "active", ExternalUserId = _user.Ref.ToString() };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            var payeAddedEvent = _eventPublisher.Events.OfType<AddedPayeSchemeEvent>().Single();

            payeAddedEvent.PayeRef.Should().Be(expectedPayeRef);
            payeAddedEvent.AccountId.Should().Be(ExpectedAccountId);
            payeAddedEvent.UserName.Should().Be(_user.FullName);
            payeAddedEvent.UserRef.Should().Be(_user.Ref);
        }

        [Test]
        public async Task ThenACreatedAccountEventIsPublished()
        {
            const string organisationName = "Org";

            //Arrange
            var createAccountCommand = new CreateAccountCommand { PayeReference = "123EDC", AccessToken = "123rd", RefreshToken = "45YT", OrganisationStatus = "active", OrganisationName = organisationName, ExternalUserId = _user.Ref.ToString() };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            var createdAccountEvent = _eventPublisher.Events.OfType<CreatedAccountEvent>().Single();

            createdAccountEvent.AccountId.Should().Be(ExpectedAccountId);
            createdAccountEvent.HashedId.Should().Be(ExpectedHashString);
            createdAccountEvent.PublicHashedId.Should().Be(ExpectedPublicHashString);
            createdAccountEvent.Name.Should().Be(organisationName);
            createdAccountEvent.UserName.Should().Be(_user.FullName);
            createdAccountEvent.UserRef.Should().Be(_user.Ref);
        }

        [TestCase(OrganisationType.Charities, Types.Models.OrganisationType.Charities)]
        [TestCase(OrganisationType.CompaniesHouse, Types.Models.OrganisationType.CompaniesHouse)]
        [TestCase(OrganisationType.PublicBodies, Types.Models.OrganisationType.PublicBodies)]
        [TestCase(OrganisationType.Other, Types.Models.OrganisationType.Other)]
        [TestCase(OrganisationType.PensionsRegulator, Types.Models.OrganisationType.PensionsRegulator)]
        public async Task ThenAnAddedLegalEntityEventIsPublished(OrganisationType inputOrganisationType, Types.Models.OrganisationType expectedOrganisationType)
        {
            const string organisationName = "Org";

            //Arrange
            var createAccountCommand = new CreateAccountCommand
            {
                PayeReference = "123EDC",
                AccessToken = "123rd",
                RefreshToken = "45YT",
                OrganisationStatus = "active",
                OrganisationName = organisationName,
                ExternalUserId = _user.Ref.ToString(),
                OrganisationType = inputOrganisationType,
                OrganisationReferenceNumber = ExpectedOrganisationReferenceNumber,
                OrganisationAddress = ExpectedOrganisationAddress
            };

            //Act
            await _handler.Handle(createAccountCommand);

            //Assert
            var addedLegalEntityEvent = _eventPublisher.Events.OfType<AddedLegalEntityEvent>().Single();

            addedLegalEntityEvent.AgreementId.Should().Be(ExpectedEmployerAgreementId);
            addedLegalEntityEvent.LegalEntityId.Should().Be(ExpectedLegalEntityId);
            addedLegalEntityEvent.OrganisationName.Should().Be(organisationName);
            addedLegalEntityEvent.AccountId.Should().Be(ExpectedAccountId);
            addedLegalEntityEvent.AccountLegalEntityId.Should().Be(ExpectedAccountLegalEntityId);
            addedLegalEntityEvent.AccountLegalEntityPublicHashedId.Should().Be(ExpectedAccountLegalEntityPublicHashString);
            addedLegalEntityEvent.UserName.Should().Be(_user.FullName);
            addedLegalEntityEvent.UserRef.Should().Be(_user.Ref);
            addedLegalEntityEvent.OrganisationReferenceNumber.Should().Be(ExpectedOrganisationReferenceNumber);
            addedLegalEntityEvent.OrganisationAddress.Should().Be(ExpectedOrganisationAddress);
            addedLegalEntityEvent.OrganisationType.Should().Be(expectedOrganisationType);
        }
    }
}