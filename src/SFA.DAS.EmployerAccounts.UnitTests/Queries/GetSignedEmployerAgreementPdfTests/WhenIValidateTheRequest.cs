using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetSignedEmployerAgreementPdfTests
{
    public class WhenIValidateTheRequest
    {
        [Test]
        [MoqInlineAutoData(0)]
        [MoqInlineAutoData(-1)]
        [MoqInlineAutoData(-999)]
        public async Task ThenTheRequestIsNotValidIfAccountIdInvalidArentPopulatedAndTheRepositoryIsNotCalled(
            long accountId,
            GetSignedEmployerAgreementPdfRequest query,
            GetSignedEmployerAgreementPdfValidator validator)
        {
            //Arrange
            query.AccountId = accountId;

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        [MoqInlineAutoData(0)]
        [MoqInlineAutoData(-1)]
        [MoqInlineAutoData(-999)]
        public async Task ThenTheRequestIsNotValidIfLegalAgreementIdInvalidArentPopulatedAndTheRepositoryIsNotCalled(
            long agreementId,
            GetSignedEmployerAgreementPdfRequest query,
            GetSignedEmployerAgreementPdfValidator validator)
        {
            //Arrange
            query.LegalAgreementId = agreementId;

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("LegalAgreementId", "LegalAgreementId has not been supplied"), actual.ValidationDictionary);
        }

        [Test, MoqAutoData]
        public async Task ThenTheRequestIsNotValidIfUserIdNotSupplied_TheRepositoryIsNotCalled(
            GetSignedEmployerAgreementPdfRequest query,
            GetSignedEmployerAgreementPdfValidator validator)
        {
            //Arrange
            query.UserId = string.Empty;

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId has not been supplied"), actual.ValidationDictionary);
        }

        [Test, MoqAutoData]
        public async Task WhenTheRequestHasValdAccountId_TheMembershipIsFetched(
           [Frozen] Mock<IMembershipRepository> membershipRepoMock,
           GetSignedEmployerAgreementPdfRequest query,
           GetSignedEmployerAgreementPdfValidator validator)
        {
            //Arrange

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            membershipRepoMock.Verify(mock => mock.GetCaller(It.Is<long>(l => l == query.AccountId), It.Is<string>(s => s == query.UserId)));
        }

        [Test]
        [MoqInlineAutoData(Role.Viewer)]
        [MoqInlineAutoData(Role.Transactor)]
        public async Task WhenAccountMember_IsNotOwner_ThenUnauthorized(
            Role userRole,
            [Frozen] Mock<IMembershipRepository> membershipRepoMock,
            GetSignedEmployerAgreementPdfRequest query,
            GetSignedEmployerAgreementPdfValidator validator)
        {
            //Arrange
            membershipRepoMock.Setup(x => x.GetCaller(It.Is<long>(l => l == query.AccountId), It.Is<string>(s => s == query.UserId))).ReturnsAsync(new MembershipView { Role = userRole });

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsTrue(actual.IsUnauthorized);
        }

        [Test, MoqAutoData]
        public async Task WhenAccountMember_IsOwner_ThenAuthorized(
            [Frozen] Mock<IMembershipRepository> membershipRepoMock,
            GetSignedEmployerAgreementPdfRequest query,
            GetSignedEmployerAgreementPdfValidator validator)
        {
            //Arrange
            membershipRepoMock.Setup(x => x.GetCaller(It.Is<long>(l => l == query.AccountId), It.Is<string>(s => s == query.UserId))).ReturnsAsync(new MembershipView { Role = Role.Owner });

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.IsFalse(actual.IsUnauthorized);
        }

        [Test, MoqAutoData]
        public async Task ThenTheRequestIsMarkedAsInvalidIfTheUserDoesNotExist(
            [Frozen] Mock<IMembershipRepository> membershipRepoMock,
            GetSignedEmployerAgreementPdfRequest query,
            GetSignedEmployerAgreementPdfValidator validator)
        {
            //Arrange
            membershipRepoMock.Setup(x => x.GetCaller(It.Is<long>(l => l == query.AccountId), It.Is<string>(s => s == query.UserId))).ReturnsAsync(() => null);

            //Act
            var actual = await validator.ValidateAsync(query);

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
