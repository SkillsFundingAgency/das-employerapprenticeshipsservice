using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.TypesExtensions
{
    [TestFixture, Parallelizable]
    public class AccountExtensionsTests : FluentTest<AccountExtensionsTestsFixture>
    {
        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldCreateAndAddOrganisationToAccount()
        {
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccount_ThenShouldReturnCreatedOrganisation()
        {
        }
        
        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldCreateAndAddMutatedOrganisationToAccount()
        {
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationNotInAccountAndOnAddActionGiven_ThenShouldReturnMutatedOrganisation()
        {
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldNotAddANewOrganisationToAccount()
        {
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccount_ThenShouldReturnExistingOrganisation()
        {
        }
        
        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldAddMutatedExistingOrganisationToAccount()
        {
        }

        [Test, Ignore("Not implemented yet")]
        public void GetOrAddOrganisation_WhenOrganisationInAccountAndOnGetActionGiven_ThenShouldReturnMutatedExistingOrganisation()
        {
        }
    }

    public class AccountExtensionsTestsFixture
    {
    }
}