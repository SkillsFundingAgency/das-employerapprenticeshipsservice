using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUnsignedEmployerAgreementTests
{
    public class WhenIValidateTheRequest
    {
        [Test]
        [MoqInlineAutoData(0)]
        [MoqInlineAutoData(-1)]
        [MoqInlineAutoData(-999)]
        public async Task ThenTheRequestIsNotValidIfAccountIdInvalidArentPopulatedAndTheRepositoryIsNotCalled(
            long accountId,
            GetNextUnsignedEmployerAgreementRequest query,
            GetNextUnsignedEmployerAgreementValidator validator)
        {
            //Arrange
            query.AccountId = accountId;

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
        }

        [Test, MoqAutoData]
        public async Task ThenTheRequestIsNotValidIfUserIdNotSupplied_TheRepositoryIsNotCalled(
            GetNextUnsignedEmployerAgreementRequest query,
            GetNextUnsignedEmployerAgreementValidator validator)
        {
            //Arrange
            query.ExternalUserId = string.Empty;

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
        }

        [Test, MoqAutoData]
        public async Task WhenTheRequestHasValdAccountId_TheMembershipIsFetched(
           [Frozen] Mock<IMembershipRepository> membershipRepoMock,
           GetNextUnsignedEmployerAgreementRequest query,
           GetNextUnsignedEmployerAgreementValidator validator)
        {
            //Arrange

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            membershipRepoMock.Verify(mock => mock.GetCaller(It.Is<long>(l => l == query.AccountId), It.Is<string>(s => s == query.ExternalUserId)));
        }

        [Test]
        [MoqInlineAutoData(Role.Viewer)]
        [MoqInlineAutoData(Role.Transactor)]
        [MoqInlineAutoData(Role.Owner)]
        public async Task WhenAccountMemberThenAuthorized(
            Role userRole,
            [Frozen] Mock<IMembershipRepository> membershipRepoMock,
            GetNextUnsignedEmployerAgreementRequest query,
            GetNextUnsignedEmployerAgreementValidator validator)
        {
            //Arrange
            membershipRepoMock.Setup(x => x.GetCaller(It.Is<long>(l => l == query.AccountId), It.Is<string>(s => s == query.ExternalUserId))).ReturnsAsync(new MembershipView { Role = userRole });

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsFalse(actual.IsUnauthorized);
        }

        [Test, MoqAutoData]
        public async Task ThenTheRequestIsMarkedAsInvalidIfTheUserDoesNotExist(
            [Frozen] Mock<IMembershipRepository> membershipRepoMock,
            GetNextUnsignedEmployerAgreementRequest query,
            GetNextUnsignedEmployerAgreementValidator validator)
        {
            //Arrange
            membershipRepoMock.Setup(x => x.GetCaller(It.Is<long>(l => l == query.AccountId), It.Is<string>(s => s == query.ExternalUserId))).ReturnsAsync(() => null);

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
