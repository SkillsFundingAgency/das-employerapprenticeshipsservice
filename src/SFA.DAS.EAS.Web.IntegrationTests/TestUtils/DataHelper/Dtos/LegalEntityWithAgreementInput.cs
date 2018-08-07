using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos
{
    public class LegalEntityWithAgreementInput
    {
        public long AccountId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public DateTime CompanyDateOfIncorporation { get; set; }
        public string Status { get; set; }
        public OrganisationType Source { get; set; }
        public short PublicSectorDataSource { get; set; }
        public string Sector { get; set; }
    }
}