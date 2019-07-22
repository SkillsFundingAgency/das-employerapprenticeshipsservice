using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.TypesExtensions;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.TypesExtensions.AccountExtensions
{
    [TestFixture, Parallelizable]
    public class GetOrAddOrganisationTests : FluentTest<GetOrAddOrganisationTestsFixture>
    {
        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldCreateAndAddOrganisationToAccount()
        {
            Test(f => f.GetOrAddOrganisation(), f => f.AssertEntityCreatedAndAddedToAccount());
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldReturnCreatedOrganisation()
        {
            Test(f => f.GetOrAddOrganisation(), (f, r) => f.AssertReturnCreatedEntity(r));
        }
        
        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldCreateAndAddMutatedOrganisationToAccount()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddOrganisation(),
                f => f.AssertEntityCreatedAndAddedToAccount(true));
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldReturnMutatedOrganisation()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertReturnCreatedEntity(r, true));
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldNotAddANewOrganisationToAccount()
        {
            Test(f => f.ArrangeEntityInAccount(), f => f.GetOrAddOrganisation(),
                f => f.AssertOriginalEntityInAccount());
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldReturnExistingOrganisation()
        {
            Test(f => f.ArrangeEntityInAccount(), f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertReturnExistingEntity(r));
        }
        
        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldMutateExistingOrganisationInAccount()
        {
            Test(f => f.ArrangeEntityInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertExistingEntityMutatedInAccount());
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldReturnMutatedExistingOrganisation()
        {
            Test(f => f.ArrangeEntityInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertReturnExistingEntity(r, true));
        }
    }

    public class GetOrAddOrganisationTestsFixture
        : AccountExtensionsTestsFixture<GetOrAddOrganisationTestsFixture, Organisation, long, string>
    {
        public GetOrAddOrganisationTestsFixture() 
            : base(account => account.Organisations, 
                organisation => organisation.AccountLegalEntityId,
                (organisation, accountLegalEntity) => organisation.AccountLegalEntityId = accountLegalEntity,
                (organisation, name) => organisation.Name = name)
        {
        }
        
        public Organisation GetOrAddOrganisation()
        {
            OriginalAccount = Account.Clone();
            return Account.GetOrAddOrganisation(EntityKey, OnAdd, OnGet);
        }
    }
}