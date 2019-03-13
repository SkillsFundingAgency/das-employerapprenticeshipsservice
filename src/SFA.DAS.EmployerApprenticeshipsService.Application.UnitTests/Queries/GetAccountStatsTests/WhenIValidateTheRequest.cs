using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Queries.GetAccountStats;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountStatsTests
{
    public class WhenIValidateTheRequest
    {
        private GetAccountStatsQueryValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();

            _validator = new GetAccountStatsQueryValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheRequestIsNotValidIfAllFieldsArentPopulatedAndTheRepositoryIsNotCalled()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountStatsQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "UserId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()),Times.Never);
        }

        [Test]
        public async Task ThenTheRequestIsMarkedAsInvalidIfTheUserDoesNotExist()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountStatsQuery { ExternalUserId = "123ABC", HashedAccountId = "1" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("member", "Unauthorised: User not connected to account"), actual.ValidationDictionary);
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheRequestIsValidIfTheUserIsAnOwnerOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { Role = Role.Owner });

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountStatsQuery { ExternalUserId = "123ABC", HashedAccountId = "1" });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsFalse(actual.IsUnauthorized);
        }
    }
}
