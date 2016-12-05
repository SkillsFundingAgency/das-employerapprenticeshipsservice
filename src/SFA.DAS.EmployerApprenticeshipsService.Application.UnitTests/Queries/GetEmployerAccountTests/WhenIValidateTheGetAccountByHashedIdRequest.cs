using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTests
{
    public class WhenIValidateTheGetAccountByHashedIdRequest
    {
        private GetEmployerAccountByHashedIdValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        private const string ExpectedHashedId = "4567";
        private const string ExpectedUserId = "asdf4660";

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedId, ExpectedUserId)).ReturnsAsync(new MembershipView());

            _validator = new GetEmployerAccountByHashedIdValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheResultIsValidWhenAllFieldsArePopulatedAndTheUserIsPartOfTheAccount()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountHashedQuery { HashedAccountId = ExpectedHashedId, UserId = ExpectedUserId });

            //Assert
            Assert.IsTrue(result.IsValid());
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUnauthorizedFlagIsSetWhenTheUserIsNotPartOfTheAccount()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountHashedQuery());

            //Assert
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheDictionaryIsPopulatedWithValidationErrors()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountHashedQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId has not been supplied"), result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), result.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

    }
}
