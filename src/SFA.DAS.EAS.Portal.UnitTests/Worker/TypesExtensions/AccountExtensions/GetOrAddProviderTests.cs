using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Worker.TypesExtensions;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.TypesExtensions.AccountExtensions
{
    [TestFixture, Parallelizable]
    public class GetOrAddProviderTests: FluentTest<GetOrAddProviderTestsFixture>
    {
        [Test]
        public void GetOrAddProvider_WhenProviderNotInAccount_ThenShouldCreateAndAddProviderToAccount()
        {
            Test(f => f.GetOrAddProvider(), f => f.AssertProviderCreatedAndAddedToAccount());
        }

        [Test]
        public void GetOrAddProvider_WhenProviderNotInAccount_ThenShouldReturnCreatedProvider()
        {
            Test(f => f.GetOrAddProvider(), (f, r) => f.AssertReturnCreatedProvider(r));
        }
        
        [Test]
        public void GetOrAddProvider_WhenProviderNotInAccountAndOnAddActionGiven_ThenShouldCreateAndAddMutatedProviderToAccount()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddProvider(),
                f => f.AssertProviderCreatedAndAddedToAccount(true));
        }

        [Test]
        public void GetOrAddProvider_WhenProviderNotInAccountAndOnAddActionGiven_ThenShouldReturnMutatedProvider()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddProvider(),
                (f, r) => f.AssertReturnCreatedProvider(r, true));
        }

        [Test]
        public void GetOrAddProvider_WhenProviderInAccount_ThenShouldNotAddANewProviderToAccount()
        {
            Test(f => f.ArrangeProviderInAccount(), f => f.GetOrAddProvider(),
                f => f.AssertOriginalProvidersInAccount());
        }

        [Test]
        public void GetOrAddProvider_WhenProviderInAccount_ThenShouldReturnExistingProvider()
        {
            Test(f => f.ArrangeProviderInAccount(), f => f.GetOrAddProvider(),
                (f, r) => f.AssertReturnExistingProvider(r));
        }
        
        [Test]
        public void GetOrAddProvider_WhenProviderInAccountAndOnGetActionGiven_ThenShouldMutateExistingProviderInAccount()
        {
            Test(f => f.ArrangeProviderInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddProvider(),
                (f, r) => f.AssertExistingProviderMutatedInAccount());
        }

        [Test]
        public void GetOrAddProvider_WhenProviderInAccountAndOnGetActionGiven_ThenShouldReturnMutatedExistingProvider()
        {
            Test(f => f.ArrangeProviderInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddProvider(),
                (f, r) => f.AssertReturnExistingProvider(r, true));
        }
    }

    public class GetOrAddProviderTestsFixture
    {
        public Account Account { get; set; }
        public Account OriginalAccount { get; set; }
        public Fixture Fixture { get; set; }
        public Action<Provider> OnAdd { get; set; }
        public Action<Provider> OnGet { get; set; }
        public long Ukprn { get; set; }
        public string MutatedProviderName { get; set; }

        public GetOrAddProviderTestsFixture()
        {
            Fixture = new Fixture();
            Ukprn = Fixture.Create<long>();
            Account = Fixture.Create<Account>();
            Account.Deleted = null;
        }

        public GetOrAddProviderTestsFixture ArrangeProviderInAccount()
        {
            Account.Providers.RandomElement().Ukprn = Ukprn;
            return this;
        }

        public GetOrAddProviderTestsFixture ArrangeOnAddActionGiven()
        {
            OnAdd = Provider => Provider.Name = MutatedProviderName = Fixture.Create<string>();
            return this;
        }

        public GetOrAddProviderTestsFixture ArrangeOnGetActionGiven()
        {
            OnGet = Provider => Provider.Name = MutatedProviderName = Fixture.Create<string>();
            return this;
        }
        
        public Provider GetOrAddProvider()
        {
            OriginalAccount = Account.Clone();
            return Account.GetOrAddProvider(Ukprn, OnAdd, OnGet);
        }

        public void AssertProviderCreatedAndAddedToAccount(bool mutated = false)
        {
            Account.Providers.Should()
                .BeEquivalentTo(OriginalAccount.Providers.Append(ExpectedCreatedProvider(mutated)));
        }

        public void AssertReturnCreatedProvider(Provider returnedProvider, bool mutated = false)
        {
            returnedProvider.Should()
                .BeEquivalentTo(ExpectedCreatedProvider(mutated));
        }
        
        private Provider ExpectedCreatedProvider(bool mutated)
        {
            var expectedProvider = new Provider
            {
                Ukprn = Ukprn
            };
            if (mutated)
                expectedProvider.Name = MutatedProviderName;

            return expectedProvider;
        }

        public void AssertOriginalProvidersInAccount()
        {
            Account.Providers.Should().BeEquivalentTo(OriginalAccount.Providers);
        }
        
        public void AssertExistingProviderMutatedInAccount()
        {
            ExpectedExistingProvider(true);
            Account.Providers.Should()
                .BeEquivalentTo(OriginalAccount.Providers);
        }
        
        public void AssertReturnExistingProvider(Provider returnedProvider, bool mutated = false)
        {
            returnedProvider.Should()
                .BeEquivalentTo(ExpectedExistingProvider(mutated));
        }
        
        private Provider ExpectedExistingProvider(bool mutated = false)
        {
            var expectedExistingProvider = OriginalAccount.Providers.Single(o => o.Ukprn == Ukprn);
            if (mutated)
                expectedExistingProvider.Name = MutatedProviderName;
            
            return expectedExistingProvider;
        }
    }
}