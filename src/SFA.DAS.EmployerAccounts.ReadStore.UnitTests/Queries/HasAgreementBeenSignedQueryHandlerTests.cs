using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.Testing;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Queries
{
    [TestFixture]
    [Parallelizable]
    public class HasAgreementBeenSignedQueryHandlerTests : FluentTest<HasAgreementBeenSignedQueryHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenMatchingAgreementFound_ShouldReturnTrue()
        {
            return TestAsync(f => f.AddMatchingAgreement(), f => f.Handle(), (f, r) => r.Should().BeTrue());
        }

        [Test]
        public Task Handle_WhenNoMatchingAgreementFound_ShouldReturnFalse()
        {
            return TestAsync(f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenNonMatchingBecauseOfAccountId_ShouldReturnFalse()
        {
            return TestAsync(f => f.AddNonMatchingAgreementOnAccountId(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenNonMatchingBecauseOfAgreementType_ShouldReturnFalse()
        {
            return TestAsync(f => f.AddNonMatchingAgreementOnAgreementType(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }

        [Test]
        public Task Handle_WhenNonMatchingBecauseOfAgreementVersion_ShouldReturnFalse()
        {
            return TestAsync(f => f.AddNonMatchingAgreementOnAgreementVersion(), f => f.Handle(), (f, r) => r.Should().BeFalse());
        }
    }

    public class HasAgreementBeenSignedQueryHandlerTestsFixture
    {
        internal HasAgreementBeenSignedQuery Query { get; set; }
        public CancellationToken CancellationToken { get; set; }
        internal IReadStoreRequestHandler<HasAgreementBeenSignedQuery, bool> Handler { get; set; }
        internal Mock<IAccountSignedAgreementsRepository> AccountSignedAgreementsRepository { get; set; }
        internal List<AccountSignedAgreement> AccountSignedAgreements { get; set; }


        public HasAgreementBeenSignedQueryHandlerTestsFixture()
        {
            Query = new HasAgreementBeenSignedQuery(112, 3, "Levy (TEST)");
            CancellationToken = CancellationToken.None;

            AccountSignedAgreements = new List<AccountSignedAgreement>();
            AccountSignedAgreementsRepository = new Mock<IAccountSignedAgreementsRepository>();
            AccountSignedAgreementsRepository.SetupInMemoryCollection(AccountSignedAgreements);

            Handler = new HasAgreementBeenSignedQueryHandler(AccountSignedAgreementsRepository.Object);
        }

        internal Task<bool> Handle()
        {
            return Handler.Handle(Query, CancellationToken);
        }

        public HasAgreementBeenSignedQueryHandlerTestsFixture AddMatchingAgreement()
        {
            AccountSignedAgreements.Add(CreateMatchingAccountSignedAgreementObject());

            return this;
        }

        public HasAgreementBeenSignedQueryHandlerTestsFixture AddNonMatchingAgreementOnAccountId()
        {
            AccountSignedAgreements.Add(CreateAccountSignedAgreementObjectNotMatchingOnAccountId());

            return this;
        }

        public HasAgreementBeenSignedQueryHandlerTestsFixture AddNonMatchingAgreementOnAgreementType()
        {
            AccountSignedAgreements.Add(CreateAccountSignedAgreementObjectNotMatchingOnAgreementType());

            return this;
        }

        public HasAgreementBeenSignedQueryHandlerTestsFixture AddNonMatchingAgreementOnAgreementVersion()
        {
            AccountSignedAgreements.Add(CreateAccountSignedAgreementObjectNotMatchingOnAgreementVersion());

            return this;
        }

        private AccountSignedAgreement CreateAccountSignedAgreementObjectNotMatchingOnAccountId()
        {
            return CreateMatchingAccountSignedAgreementObject().Set(x => x.AccountId, 214);
        }

        private AccountSignedAgreement CreateAccountSignedAgreementObjectNotMatchingOnAgreementType()
        {
            return CreateMatchingAccountSignedAgreementObject().Set(x => x.AgreementType, "expression of interest (TEST)");
        }

        private AccountSignedAgreement CreateAccountSignedAgreementObjectNotMatchingOnAgreementVersion()
        {
            return CreateMatchingAccountSignedAgreementObject().Set(x => x.AgreementVersion, 9);
        }

        private AccountSignedAgreement CreateMatchingAccountSignedAgreementObject()
        {
            return ObjectActivator.CreateInstance<AccountSignedAgreement>()
                .Set(x => x.AccountId, Query.AccountId)
                .Set(x => x.AgreementType, Query.AgreementType)
                .Set(x => x.AgreementVersion, Query.AgreementVersion);
        }
    }
}