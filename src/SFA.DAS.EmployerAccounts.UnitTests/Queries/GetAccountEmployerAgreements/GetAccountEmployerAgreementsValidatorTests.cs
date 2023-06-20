using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountEmployerAgreements
{

    namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements.UnitTests
    {
        [TestFixture]
        public class GetAccountEmployerAgreementsValidatorTests
        {
            [Test, MoqAutoData]
            public async Task ValidateAsync_WhenExternalUserIdIsNullOrEmpty_AddsErrorToValidationResult(
                GetAccountEmployerAgreementsRequest request,
                GetAccountEmployerAgreementsValidator validator)
            {
                // Arrange
                request.ExternalUserId = string.Empty;

                // Act
                var result = await validator.ValidateAsync(request);

                // Assert
                result.IsValid().Should().BeFalse();
                result.ValidationDictionary.Should().Contain(e => e.Key == nameof(request.ExternalUserId));
            }

            [Test, MoqAutoData]
            public async Task ValidateAsync_WhenAccountIdIsZero_AddsErrorToValidationResult(
                GetAccountEmployerAgreementsRequest request,
                GetAccountEmployerAgreementsValidator validator)
            {
                // Arrange
                request.AccountId = 0;

                // Act
                var result = await validator.ValidateAsync(request);

                // Assert
                result.IsValid().Should().BeFalse();
                result.ValidationDictionary.Should().Contain(e => e.Key == nameof(request.AccountId));
            }

            [Test, MoqAutoData]
            public async Task ValidateAsync_WhenMembershipIsNull_SetsIsUnauthorizedToTrueInValidationResult(
                GetAccountEmployerAgreementsRequest request,
                [Frozen] Mock<IMembershipRepository> membershipRepositoryMock,
                GetAccountEmployerAgreementsValidator validator)
            {
                // Arrange
                membershipRepositoryMock
                    .Setup(r => r.GetCaller(request.AccountId, request.ExternalUserId))
                    .ReturnsAsync((MembershipView)null);

                // Act
                var result = await validator.ValidateAsync(request);

                // Assert
                result.IsUnauthorized.Should().BeTrue();
            }

            [Test, MoqAutoData]
            public async Task ValidateAsync_WhenMembershipIsNotNull_DoesNotSetIsUnauthorizedToTrueInValidationResult(
                GetAccountEmployerAgreementsRequest request,
                MembershipView membership,
                [Frozen] Mock<IMembershipRepository> membershipRepositoryMock,
                GetAccountEmployerAgreementsValidator validator)
            {
                // Arrange
                membershipRepositoryMock
                   .Setup(r => r.GetCaller(request.AccountId, request.ExternalUserId))
                   .ReturnsAsync(membership);

                // Act
                var result = await validator.ValidateAsync(request);

                // Assert
                result.IsUnauthorized.Should().BeFalse();
            }
        }
    }

}
