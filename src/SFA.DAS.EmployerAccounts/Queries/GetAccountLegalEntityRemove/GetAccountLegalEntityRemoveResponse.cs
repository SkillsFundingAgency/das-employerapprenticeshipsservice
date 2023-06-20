namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;

public class GetAccountLegalEntityRemoveResponse
{
    public string Name { get; set; }
    public bool CanBeRemoved { get; set; }
    public bool HasSignedAgreement { get; set; }
}