using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionQuery : IAsyncRequest<GetEmployerEnglishFractionResponse>
    {
        public string EmpRef { get; set; }
        public string UserId { get; set; }
        public string HashedAccountId { get; set; }
    }
}