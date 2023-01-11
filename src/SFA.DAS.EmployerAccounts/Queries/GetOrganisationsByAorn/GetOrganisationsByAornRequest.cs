namespace SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;

public class GetOrganisationsByAornRequest : IAsyncRequest<GetOrganisationsByAornResponse>
{
    public GetOrganisationsByAornRequest(string aorn, string payeRef)
    {
        Aorn = aorn;
        PayeRef = payeRef;
    }

    public string Aorn { get; }
    public string PayeRef { get; }
}