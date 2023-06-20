namespace SFA.DAS.EmployerAccounts.Queries.GetCharity;

public class GetCharityQueryRequest : IRequest<GetCharityQueryResponse>
{
    public int RegistrationNumber { get; set; }
}