using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class EmployerAccountSetup
    {
        public EmployerAccountInput AccountInput { get; set; }
        public EmployerAccountOutput AccountOutput { get; set; }
        public List<LegalEntityWithAgreementSetup> LegalEntities { get; } = new List<LegalEntityWithAgreementSetup>();
    }
}