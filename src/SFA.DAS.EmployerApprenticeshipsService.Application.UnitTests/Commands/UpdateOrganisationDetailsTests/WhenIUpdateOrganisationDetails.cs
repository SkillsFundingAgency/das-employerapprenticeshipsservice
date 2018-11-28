﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdateOrganisationDetails;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.HashingService;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdateOrganisationDetailsTests
{
    public class WhenIUpdateOrganisationDetails
    {
        private UpdateOrganisationDetailsCommandHandler _handler;
        private Mock<IValidator<UpdateOrganisationDetailsCommand>> _validator;
        private UpdateOrganisationDetailsCommand _command;
        private Mock<IHashingService> _hashingService;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IEventPublisher> _eventPublisher;

        private const string HashedAccountId = "34RFD";
        private const long AccountId = 123455;
        private const string ExpectedOrganisationName = "Org Name";
        private const string ExpectedOrganisationAddress = "Org Address";
        private const long ExpectedAccountLegalEntityId = 2017;
        private readonly string _expectedUserId = Guid.NewGuid().ToString();

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<UpdateOrganisationDetailsCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<UpdateOrganisationDetailsCommand>())).Returns(new ValidationResult());

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(AccountId);

            _accountRepository = new Mock<IAccountRepository>();

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository
                .Setup(mr => mr.GetCaller(AccountId, _expectedUserId))
                .Returns<long, string>((accountId, userRef) => Task.FromResult(new MembershipView { AccountId = AccountId, FirstName = "Harry", LastName = "Potter" }));

            _eventPublisher = new Mock<IEventPublisher>();

            _command = new UpdateOrganisationDetailsCommand { AccountLegalEntityId = ExpectedAccountLegalEntityId, Name = ExpectedOrganisationName, Address = ExpectedOrganisationAddress, HashedAccountId = HashedAccountId, UserId = _expectedUserId };

            _handler = new UpdateOrganisationDetailsCommandHandler(
                _validator.Object,
                _accountRepository.Object,
                _membershipRepository.Object,
                _hashingService.Object,
                _eventPublisher.Object);
        }

        [Test]
        public void ThenTheValidatorIsCalledAndAnInvalidRequestExceptionThrownIfItIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<UpdateOrganisationDetailsCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenTheRepositoryIsCalled()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _accountRepository.Verify(r => r.UpdateLegalEntityDetailsForAccount(ExpectedAccountLegalEntityId, ExpectedOrganisationName, ExpectedOrganisationAddress));
        }

        [Test]
        public async Task ThenTheUpdatedLegalEntityEventIsPublished()
        {
            await _handler.Handle(_command);

            _eventPublisher.Verify(ep => ep.Publish(It.Is<UpdatedLegalEntityEvent>(e =>
                e.Name.Equals(ExpectedOrganisationName)
                && e.Address.Equals(ExpectedOrganisationAddress)
                && e.AccountLegalEntityId.Equals(ExpectedAccountLegalEntityId)
                && e.UserName.Equals("Harry Potter")
                && e.UserRef.Equals(Guid.Parse(_expectedUserId))
                )));
        }
    }
}
