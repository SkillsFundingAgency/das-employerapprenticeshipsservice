using System;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper
{
    static class DbDtoBuilderExtensions
    {
        public static EmployerAccountInput BuildEmployerAccountInput(this DbBuilder dbBuilder, string accountName, string payeReference)
        {
            return new EmployerAccountInput
            { 
                OrganisationName = accountName,
                PayeReference = payeReference,
                UserId = dbBuilder.Context.ActiveUser.UserId 
            };
        }

        public static LegalEntityWithAgreementInput BuildEntityWithAgreementInput(this DbBuilder dbBuilder, string name) 
        {
            return new LegalEntityWithAgreementInput
            {
                AccountId = dbBuilder.Context.ActiveEmployerAccount.AccountId,
                CompanyDateOfIncorporation = DateTime.Today.AddMonths(-12),
                CompanyName = name
            };
        }

        public static UserInput BuildUserInput(this DbBuilder dbBuilder)
        {
            var userRef = Guid.NewGuid().ToString();
            return new UserInput
            {
                UserRef = userRef,
                Email = userRef.Substring(0,6)+".madeupdomain.co.uk"
            };
        }
    }
}