using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Features
{
    [TestFixture]
    public class AgreementServiceTests : FluentTest<AgreementServiceTestsFixture>
    {
        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenGettingVersionNumber_ShouldCacheVersionNumber()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Signed)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => f.DistributedCache.Verify(c => c.GetOrAddAsync("AccountId:1", It.IsAny<Func<string, Task<int>>>())));
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenGettingVersionNumber_ShouldGetVersionNumberFromCache()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Signed)
                    .SetupDb()
                    .SetupCache(1, 9999),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().Be(9999));
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenAccountHasLegalEntityWithSignedV1Agreement_ShouldReturnVersionNumberOfV1Agreement()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Signed)
                    .AddAgreement(2, 1, 2, EmployerAgreementStatus.Pending)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().Be(1));
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenAccountHasLegalEntityWithSignedV1AgreementAndSignedV2Agreement_ShouldReturnVersionNumberOfV2Agreement()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Signed)
                    .AddAgreement(1, 1, 2, EmployerAgreementStatus.Signed)
                    .AddAgreement(2, 1, 1, EmployerAgreementStatus.Pending)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().Be(2));
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenAccountHasLegalEntityWithSignedV1AgreementAndPendingV2Agreement_ShouldReturnVersionNumberOfV1Agreement()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Signed)
                    .AddAgreement(1, 1, 2, EmployerAgreementStatus.Pending)
                    .AddAgreement(2, 1, 2, EmployerAgreementStatus.Pending)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().Be(1));
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenAccountHasLegalEntityWithPendingV1AgreementAndSignedV2Agreement_ShouldReturnVersionNumberOfV2Agreement()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Pending)
                    .AddAgreement(1, 1, 2, EmployerAgreementStatus.Signed)
                    .AddAgreement(2, 1, 1, EmployerAgreementStatus.Pending)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().Be(2));
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenAccountHasLegalEntityWithPendingV1AgreementAndPendingV2Agreement_ShouldReturnNull()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Pending)
                    .AddAgreement(1, 1, 2, EmployerAgreementStatus.Pending)
                    .AddAgreement(2, 1, 1, EmployerAgreementStatus.Signed)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().NotHaveValue());
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenAccountHasMultipleLegalEntitiesWithSignedAgreements_ShouldReturnLowestVersionNumber()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Signed)
                    .AddAgreement(1, 2, 2, EmployerAgreementStatus.Signed)
                    .AddAgreement(2, 1, 2, EmployerAgreementStatus.Pending)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().Be(1));
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenAccountHasMultipleLegalEntitiesWithPendingAndSignedAgreements_ShouldReturnNull()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Pending)
                    .AddAgreement(1, 2, 2, EmployerAgreementStatus.Signed)
                    .AddAgreement(2, 1, 1, EmployerAgreementStatus.Pending)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().NotHaveValue());
        }

        [Test]
        public Task GetLowestSignedAgreementVersionNumberAsync_WhenLegalEntityIsRemoved_ShouldIgnoreLegalEntity()
        {
            return RunAsync(
                f => f
                    .AddAgreement(1, 1, 1, EmployerAgreementStatus.Signed)
                    .AddAgreement(1, 2, 2, EmployerAgreementStatus.Signed)
                    .RemoveAccountLegalEntity(1, 1)
                    .SetupDb()
                    .SetupCache(),
                f => f.GetLowestSignedAgreementVersionNumberAsync(1),
                (f, r) => r.Should().Be(2));
        }

        [Test]
        public Task RemoveFromCacheAsync_WhenRemovingAccountDataFromCache_ShouldRemoveVersionNumberFromCache()
        {
            return RunAsync(
                f => f.RemoveFromCacheAsync(1),
                f => f.DistributedCache.Verify(c => c.RemoveFromCache("AccountId:1")));
        }
    }

    public class AgreementServiceTestsFixture : FluentTestFixture
    {
        public Mock<IDistributedCache> DistributedCache { get; set; }
        private readonly IAgreementService _service;

        private readonly EmployerAgreementBuilder _employerAgreementBuilder;

        public AgreementServiceTestsFixture()
        {
            DistributedCache = new Mock<IDistributedCache>();
            _employerAgreementBuilder = new EmployerAgreementBuilder();
            _service = new AgreementService(new Lazy<EmployerAccountsDbContext>(() => _employerAgreementBuilder.EmployerAccountDbContext), DistributedCache.Object);
        }

        public Task<int?> GetLowestSignedAgreementVersionNumberAsync(long accountId)
        {
            return _service.GetAgreementVersionAsync(accountId);
        }

        public AgreementServiceTestsFixture AddAgreement(long accountId, long legalEntityId, int templateVersionNumber, EmployerAgreementStatus status)
        {
            _employerAgreementBuilder
                .WithAccount(accountId, "ABC123")
                .WithLegalEntityId(legalEntityId)
                .WithAgreement(accountId, legalEntityId, templateVersionNumber, status);

            return this;
        }

        public AgreementServiceTestsFixture RemoveAccountLegalEntity(long accountId, long legalEntityId)
        {
            _employerAgreementBuilder
                .RemoveAccountLegalEntity(accountId, legalEntityId);

            return this;
        }

        public Task RemoveFromCacheAsync(long accountId)
        {
            return _service.RemoveFromCacheAsync(accountId);
        }

        public AgreementServiceTestsFixture SetupCache()
        {
            foreach (var account in _employerAgreementBuilder.Accounts)
            {
                DistributedCache.Setup(c => c.GetOrAddAsync("AccountId:" + account.Id.ToString(CultureInfo.InvariantCulture), It.IsAny<Func<string, Task<int>>>()))
                    .Returns<string, Func<string, Task<int>>>((k, f) => f(k));
            }

            return this;
        }

        public AgreementServiceTestsFixture SetupCache(long accountId, int templateVersionNumber)
        {
            DistributedCache.Setup(c => c.GetOrAddAsync("AccountId:" + accountId.ToString(CultureInfo.InvariantCulture), It.IsAny<Func<string, Task<int>>>()))
                .Returns<string, Func<string, Task<int>>>((k, f) => Task.FromResult(templateVersionNumber));

            return this;
        }

        public AgreementServiceTestsFixture SetupDb()
        {
            _employerAgreementBuilder
                .EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities()
                .SetupMockDbContext();

            return this;
        }
    }
}