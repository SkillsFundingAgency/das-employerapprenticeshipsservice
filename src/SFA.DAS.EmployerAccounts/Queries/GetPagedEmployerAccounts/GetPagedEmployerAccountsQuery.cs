namespace SFA.DAS.EmployerAccounts.Queries.GetPagedEmployerAccounts;

public class GetPagedEmployerAccountsQuery : IRequest<GetPagedEmployerAccountsResponse>
{
    public string ToDate { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}