using System;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{
    public class LegalEntityModelBuilder
    {
        public LegalEntityWithAgreementInput BuildEntityWithAgreementInput(string name, long accountId)
        {
            return new LegalEntityWithAgreementInput
            {
                AccountId = accountId,
                CompanyDateOfIncorporation = DateTime.Today.AddMonths(-12),
                CompanyName = name
            };
        }
    }
}
