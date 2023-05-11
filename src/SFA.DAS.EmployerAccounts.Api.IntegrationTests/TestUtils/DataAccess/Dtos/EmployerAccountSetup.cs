namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos;

public class EmployerAccountSetup
{
    public EmployerAccountInput? AccountInput { get; set; }
    public EmployerAccountOutput? AccountOutput { get; set; }
    public List<LegalEntityWithAgreementSetup> LegalEntities { get; } = new();
}