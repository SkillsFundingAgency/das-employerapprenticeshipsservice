using AutoFixture;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.Worker.TypesExtensions;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.TypesExtensions
{
    [TestFixture, Parallelizable]
    public class AccountExtensionsTests : FluentTest<AccountExtensionsTestsFixture>
    {
        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldCreateAndAddOrganisationToAccount()
        {
            Test(f => f.GetOrAddOrganisation());
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldReturnCreatedOrganisation()
        {
            Test(f => f.GetOrAddOrganisation());
        }
        
        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldCreateAndAddMutatedOrganisationToAccount()
        {
            Test(f => f.GetOrAddOrganisation());
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldReturnMutatedOrganisation()
        {
            Test(f => f.GetOrAddOrganisation());
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldNotAddANewOrganisationToAccount()
        {
            Test(f => f.ArrangeOrganisationInAccount(), f => f.GetOrAddOrganisation());
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldReturnExistingOrganisation()
        {
            Test(f => f.ArrangeOrganisationInAccount(), f => f.GetOrAddOrganisation());
        }
        
        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldAddMutatedExistingOrganisationToAccount()
        {
            Test(f => f.ArrangeOrganisationInAccount(), f => f.GetOrAddOrganisation());
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldReturnMutatedExistingOrganisation()
        {
            Test(f => f.ArrangeOrganisationInAccount(), f => f.GetOrAddOrganisation());
        }
    }

    public class AccountExtensionsTestsFixture
    {
        public Account Account { get; set; }
        public Account OriginalAccount { get; set; }
        public Fixture Fixture { get; set; }
        public long AccountLegalEntityId { get; set; }

        public AccountExtensionsTestsFixture()
        {
            AccountLegalEntityId = Fixture.Create<long>();
            Fixture = new Fixture();
            Account = Fixture.Create<Account>();
            Account.Deleted = null;
        }

        public void ArrangeOrganisationInAccount()
        {
            Account.Organisations.RandomElement().AccountLegalEntityId = AccountLegalEntityId;
        }
        
        public Organisation GetOrAddOrganisation()
        {
            OriginalAccount = Account.Clone();
            return Account.GetOrAddOrganisation(AccountLegalEntityId);
        }
    }
}