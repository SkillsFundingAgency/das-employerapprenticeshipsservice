using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountTeamMembers
{
    public class WhenIValidateTheRequest
    {
        private GetAccountTeamMembersValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();

            _validator = new GetAccountTeamMembersValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheRequestIsNotValidIfAllFieldsArentPopulatedAndTheRepositoryIsNotCalled()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTeamMembersQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "UserId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedId", "HashedId has not been supplied"), actual.ValidationDictionary);
            _membershipRepository.Verify(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()),Times.Never);
        }

        [Test]
        public async Task ThenTheRequestIsMarkedAsInvalidIfTheUserDoesNotExist()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTeamMembersQuery { ExternalUserId = "123ABC", HashedId = "1" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("member", "Unauthorised: User not connected to account"), actual.ValidationDictionary);
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheRequestIsMarkedAsInvaildIfTheUserIsNotAnOwner()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { RoleId = (short)Role.Viewer });

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTeamMembersQuery { ExternalUserId = "123ABC", HashedId = "1" });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("member", "Unauthorised: User is not an owner of this account"), actual.ValidationDictionary);
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheRequestIsValidIfTheUSerIsAnOwnerOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { RoleId = (short)Role.Owner });

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountTeamMembersQuery { ExternalUserId = "123ABC", HashedId = "1" });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsFalse(actual.IsUnauthorized);
        }
    }
}
