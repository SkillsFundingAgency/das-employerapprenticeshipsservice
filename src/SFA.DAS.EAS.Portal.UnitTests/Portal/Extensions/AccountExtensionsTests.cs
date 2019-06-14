using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Testing;
using System.Linq;
using SFA.DAS.EAS.Portal.Extensions;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Extensions
{
    [TestFixture, Parallelizable]
    public class AccountExtensionsTests : FluentTest<AccountExtensionsTestsFixture>
    {
        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldCreateAndAddOrganisationToAccount()
        {
            Test(f => f.GetOrAddOrganisation(), f => f.AssertOrganisationCreatedAndAddedToAccount());
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldReturnCreatedOrganisation()
        {
            Test(f => f.GetOrAddOrganisation(), (f, r) => f.AssertReturnCreatedOrganisation(r));
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
            Fixture = new Fixture();
            AccountLegalEntityId = Fixture.Create<long>();
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

        public void AssertOrganisationCreatedAndAddedToAccount()
        {
            Account.Organisations.Should()
                .BeEquivalentTo(OriginalAccount.Organisations.Append(new Organisation
                { AccountLegalEntityId = AccountLegalEntityId }));
        }

        public void AssertReturnCreatedOrganisation(Organisation returnedOrganisation)
        {
            returnedOrganisation.Should()
                .BeEquivalentTo(new Organisation { AccountLegalEntityId = AccountLegalEntityId });
        }
    }
}
