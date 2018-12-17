using System;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders
{
    public class LegalEntityModelBuilder
    {
        public LegalEntityWithAgreementInput BuildEntityWithAgreementInput(string name, Func<long> accountId)
        {
            return new LegalEntityWithAgreementInput
            {
                AccountId = accountId,
                CompanyDateOfIncorporation = DateTime.Today.AddMonths(-12),
                CompanyName = name
            };
        }

        public LegalEntityWithAgreementInput BuildEntityWithAgreementInput(string name, long accountId)
        {
            return BuildEntityWithAgreementInput(name, () => accountId);
        }
    }
}
