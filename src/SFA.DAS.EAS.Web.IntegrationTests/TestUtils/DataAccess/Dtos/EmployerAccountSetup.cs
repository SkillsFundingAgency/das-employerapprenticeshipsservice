using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class EmployerAccountSetup
    {
        public EmployerAccountInput AccountInput { get; set; }
        public EmployerAccountOutput AccountOutput { get; set; }
        public List<LegalEntityWithAgreementSetup> LegalEntities { get; } = new List<LegalEntityWithAgreementSetup>();
    }
}