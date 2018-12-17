using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper.Dtos;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders
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
