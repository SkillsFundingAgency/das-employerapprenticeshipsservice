using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{
    public class AccountModelBuilder
    {
        public EmployerAccountInput CreateAccountInput(string accountName, string payeReference, long userId)
        {
            return new EmployerAccountInput
            {
                OrganisationName = accountName,
                PayeReference = payeReference,
                UserId = userId
            };
        }
    }
}
