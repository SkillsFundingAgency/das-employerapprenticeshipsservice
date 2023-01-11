namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;

public class GetOrganisationAgreementsRequest : IAsyncRequest<GetOrganisationAgreementsResponse>
{
    public string AccountLegalEntityHashedId { get; set; }
}