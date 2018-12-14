using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;
using System;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{
    public class AccountModelBuilder
    {
        public EmployerAccountInput CreateAccountInput(string accountName, string payeReference, Func<long> userId)
        {
            return new EmployerAccountInput
            {
                OrganisationName = accountName,
                PayeReference = payeReference,
                UserId = userId
            };
        }

        public EmployerAccountInput CreateAccountInput(string accountName, string payeReference, long userId)
        {
            return CreateAccountInput(accountName, payeReference, () => userId);
        }
    }
}
