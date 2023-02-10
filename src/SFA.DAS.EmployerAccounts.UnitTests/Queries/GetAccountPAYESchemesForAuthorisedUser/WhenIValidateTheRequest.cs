using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountPAYESchemesForAuthorisedUser
{
    public class WhenIValidateTheRequest
    {
        private GetAccountPayeSchemesForAuthorisedUserQueryValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();

            _validator = new GetAccountPayeSchemesForAuthorisedUserQueryValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheRequestIsValidIfTheUserIsAMemberOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { Role = Role.Viewer });

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountPayeSchemesForAuthorisedUserQuery { ExternalUserId = "123ABC", AccountId = 112 });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsFalse(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheRequestIsNotValidIfAllFieldsArentPopulatedAndTheRepositoryIsNotCalled()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountPayeSchemesForAuthorisedUserQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "User ID has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "Hashed account ID has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRequestIsMarkedAsInvalidIfTheUserDoesNotExist()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountPayeSchemesForAuthorisedUserQuery { ExternalUserId = "123ABC", AccountId = 113 });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("member", "Unauthorised: User not connected to account"), actual.ValidationDictionary);
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
