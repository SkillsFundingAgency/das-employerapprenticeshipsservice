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
    public class GetOrAddOrganisationTests : FluentTest<GetOrAddOrganisationTestsFixture>
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
        
        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldCreateAndAddMutatedOrganisationToAccount()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddOrganisation(),
                f => f.AssertOrganisationCreatedAndAddedToAccount(true));
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldReturnMutatedOrganisation()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertReturnCreatedOrganisation(r, true));
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldNotAddANewOrganisationToAccount()
        {
            Test(f => f.ArrangeOrganisationInAccount(), f => f.GetOrAddOrganisation(),
                f => f.AssertOriginalOrganisationsInAccount());
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldReturnExistingOrganisation()
        {
            Test(f => f.ArrangeOrganisationInAccount(), f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertReturnExistingOrganisation(r));
        }
        
        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldMutateExistingOrganisationInAccount()
        {
            Test(f => f.ArrangeOrganisationInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertExistingOrganisationMutatedInAccount());
        }

        [Test]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldReturnMutatedExistingOrganisation()
        {
            Test(f => f.ArrangeOrganisationInAccount().ArrangeOnGetActionGiven(),
                f => f.GetOrAddOrganisation(),
                (f, r) => f.AssertReturnExistingOrganisation(r, true));
        }
    }

    public class GetOrAddOrganisationTestsFixture : AccountExtensionsTestsFixture<Organisation>
    {
        public long AccountLegalEntityId { get; set; }
        public string MutatedOrganisationName { get; set; }

        public GetOrAddOrganisationTestsFixture()
        {
            AccountLegalEntityId = Fixture.Create<long>();
        }
        
        public GetOrAddOrganisationTestsFixture ArrangeOrganisationInAccount()
        {
            Account.Organisations.RandomElement().AccountLegalEntityId = AccountLegalEntityId;
            return this;
        }

        public GetOrAddOrganisationTestsFixture ArrangeOnAddActionGiven()
        {
            OnAdd = organisation => organisation.Name = MutatedOrganisationName = Fixture.Create<string>();
            return this;
        }

        public GetOrAddOrganisationTestsFixture ArrangeOnGetActionGiven()
        {
            OnGet = organisation => organisation.Name = MutatedOrganisationName = Fixture.Create<string>();
            return this;
        }
        
        public Organisation GetOrAddOrganisation()
        {
            OriginalAccount = Account.Clone();
            return Account.GetOrAddOrganisation(AccountLegalEntityId, OnAdd, OnGet);
        }

        public void AssertOrganisationCreatedAndAddedToAccount(bool mutated = false)
        {
            Account.Organisations.Should()
                .BeEquivalentTo(OriginalAccount.Organisations.Append(ExpectedCreatedOrganisation(mutated)));
        }

        public void AssertReturnCreatedOrganisation(Organisation returnedOrganisation, bool mutated = false)
        {
            returnedOrganisation.Should()
                .BeEquivalentTo(ExpectedCreatedOrganisation(mutated));
        }
        
        private Organisation ExpectedCreatedOrganisation(bool mutated)
        {
            var expectedOrganisation = new Organisation
            {
                AccountLegalEntityId = AccountLegalEntityId
            };
            if (mutated)
                expectedOrganisation.Name = MutatedOrganisationName;

            return expectedOrganisation;
        }

        public void AssertOriginalOrganisationsInAccount()
        {
            Account.Organisations.Should().BeEquivalentTo(OriginalAccount.Organisations);
        }
        
        public void AssertExistingOrganisationMutatedInAccount()
        {
            ExpectedExistingOrganisation(true);
            Account.Organisations.Should()
                .BeEquivalentTo(OriginalAccount.Organisations);
        }
        
        public void AssertReturnExistingOrganisation(Organisation returnedOrganisation, bool mutated = false)
        {
            returnedOrganisation.Should()
                .BeEquivalentTo(ExpectedExistingOrganisation(mutated));
        }
        
        private Organisation ExpectedExistingOrganisation(bool mutated = false)
        {
            var expectedExistingOrganisation = OriginalAccount.Organisations.Single(o => o.AccountLegalEntityId == AccountLegalEntityId);
            if (mutated)
                expectedExistingOrganisation.Name = MutatedOrganisationName;
            
            return expectedExistingOrganisation;
        }
    }
}