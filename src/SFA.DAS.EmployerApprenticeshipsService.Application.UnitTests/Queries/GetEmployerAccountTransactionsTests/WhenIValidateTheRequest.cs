﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactions;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    public class WhenIValidateTheRequest
    {
        private GetEmployerAccountTransactionsValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(() => null);

            _validator = new GetEmployerAccountTransactionsValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenItIsValidIfAllFieldsArePopUlated()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "123", HashedAccountId = "AD1" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenTheResultIsMarkedAsUnauthorizedIfTheUserIsNotAMemberOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller("AD1", "123")).ReturnsAsync(() => null);

            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "123", HashedAccountId = "AD1" });

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheResultIsMarkedAsAuthorizedIfNoUserHasBeenProvided()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "", HashedAccountId = "AD1" });

            //Assert
            Assert.IsTrue(result.IsValid());
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenIfTheFieldsArentPopulatedThenTheResultIsNotValidAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"),result.ValidationDictionary );
        }
    }
}
