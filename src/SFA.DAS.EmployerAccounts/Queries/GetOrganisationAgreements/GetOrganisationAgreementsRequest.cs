namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;

public class GetOrganisationAgreementsRequest : IRequest<GetOrganisationAgreementsResponse>
{
    public string AccountLegalEntityHashedId { get; set; }
}