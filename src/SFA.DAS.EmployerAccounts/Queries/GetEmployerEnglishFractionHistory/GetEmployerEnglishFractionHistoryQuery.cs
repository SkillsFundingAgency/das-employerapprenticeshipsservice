namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;

public class GetEmployerEnglishFractionHistoryQuery : IRequest<GetEmployerEnglishFractionHistoryResponse>
{
    public string EmpRef { get; set; }
    public string UserId { get; set; }
    public string HashedAccountId { get; set; }
}