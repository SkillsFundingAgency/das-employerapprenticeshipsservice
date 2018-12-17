using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Adapters
{
    public class LegalEntityWithAgreementInputAdapter : CreateLegalEntityWithAgreementParams
    {
        public LegalEntityWithAgreementInputAdapter(LegalEntityWithAgreementInput input)
        {
            AccountId = input.AccountId();
            Address = input.CompanyAddress;
            Code = null;
            DateOfIncorporation = input.CompanyDateOfIncorporation;
            Name = input.CompanyName;
            PublicSectorDataSource = (byte) input.PublicSectorDataSource;
            Sector = input.Sector;
            Source = input.Source;
            Status = input.Status;
        }
    }
}