using System;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper.Dtos;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders
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
