using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper
{
    /// <summary>
    ///     Represents the current selections that have been made when building up the db.
    ///     This is used as the parent/context when inserting child objects.
    /// </summary>
    class DbBuilderContext
    {
        public EmployerAccountOutput ActiveEmployerAccount { get; set; }
        public LegalEnityWithAgreementOutput ActiveLegalEnity { get; set; }
        public UserOutput ActiveUser { get; set; }
    }
}