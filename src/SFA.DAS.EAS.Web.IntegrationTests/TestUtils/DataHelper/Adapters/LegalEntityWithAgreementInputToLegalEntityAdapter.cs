using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Adapters
{
    public class LegalEntityWithAgreementInputToLegalEntityAdapter : LegalEntity
    {
        public LegalEntityWithAgreementInputToLegalEntityAdapter(LegalEntityWithAgreementInput input)
        {
            this.Name = input.CompanyName;
            this.Code = null;
            this.CompanyStatus = input.Status;
            this.DateOfIncorporation = input.CompanyDateOfIncorporation;
            this.Id = 0L;
            this.PublicSectorDataSource = input.PublicSectorDataSource;
            this.RegisteredAddress = input.CompanyAddress;
            this.Sector = input.Sector;
            this.Source = input.Source;
        }
    }
}