using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess
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