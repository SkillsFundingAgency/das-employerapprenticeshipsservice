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
            Test(f => f.GetOrAddProvider(), f => f.AssertEntityCreatedAndAddedToAccount());
        }

        [Test]
        public void GetOrAddProvider_WhenProviderNotInAccount_ThenShouldReturnCreatedProvider()
        {
            Test(f => f.GetOrAddProvider(), (f, r) => f.AssertReturnCreatedEntity(r));
        }
        
        [Test]
        public void GetOrAddProvider_WhenProviderNotInAccountAndOnAddActionGiven_ThenShouldCreateAndAddMutatedProviderToAccount()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddProvider(),
                f => f.AssertEntityCreatedAndAddedToAccount(true));
        }

        [Test]
        public void GetOrAddProvider_WhenProviderNotInAccountAndOnAddActionGiven_ThenShouldReturnMutatedProvider()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddProvider(),
                (f, r) => f.AssertReturnCreatedEntity(r, true));
        }

        [Test]
        public void GetOrAddProvider_WhenProviderInAccount_ThenShouldNotAddANewProviderToAccount()
        {
            Test(f => f.ArrangeEntityInAccount(), f => f.GetOrAddProvider(),
                f => f.AssertOriginalEntityInAccount());
        }

        [Test]
        public void GetOrAddProvider_WhenProviderInAccount_ThenShouldReturnExistingProvider()
        {
            Test(f => f.ArrangeEntityInAccount(), f => f.GetOrAddProvider(),
                (f, r) => f.AssertReturnExistingEntity(r));
        }
        
        [Test]
        public void GetOrAddProvider_WhenProviderInAccountAndOnGetActionGiven_ThenShouldMutateExistingProviderInAccount()
        {
            Test(f => f.ArrangeEntityInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddProvider(),
                (f, r) => f.AssertExistingEntityMutatedInAccount());
        }

        [Test]
        public void GetOrAddProvider_WhenProviderInAccountAndOnGetActionGiven_ThenShouldReturnMutatedExistingProvider()
        {
            Test(f => f.ArrangeEntityInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddProvider(),
                (f, r) => f.AssertReturnExistingEntity(r, true));
        }
    }

    public class GetOrAddProviderTestsFixture
        : AccountExtensionsTestsFixture<GetOrAddProviderTestsFixture, Provider, long, string>
    {
        public GetOrAddProviderTestsFixture() 
            : base(account => account.Providers, 
                provider => provider.Ukprn,
                (provider, ukprn) => provider.Ukprn = ukprn,
                (provider, name) => provider.Name = name)
        {
        }
        
        public Provider GetOrAddProvider()
        {
            OriginalAccount = Account.Clone();
            return Account.GetOrAddProvider(EntityKey, OnAdd, OnGet);
        }
     }
}