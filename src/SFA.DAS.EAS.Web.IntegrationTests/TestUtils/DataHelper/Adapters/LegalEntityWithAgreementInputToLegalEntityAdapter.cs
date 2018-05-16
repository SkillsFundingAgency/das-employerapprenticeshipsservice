using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Adapters
{
    public class LegalEntityWithAgreementInputToLegalEntityAdapter : LegalEntity
    {
        public LegalEntityWithAgreementInputToLegalEntityAdapter(LegalEntityWithAgreementInput input)
        {
            Name = input.CompanyName;
            Code = null;
            Status = input.Status;
            DateOfIncorporation = input.CompanyDateOfIncorporation;
            Id = 0L;
            PublicSectorDataSource = (byte) input.PublicSectorDataSource;
            RegisteredAddress = input.CompanyAddress;
            Sector = input.Sector;
            Source = (byte) input.Source;
        }
    }
}