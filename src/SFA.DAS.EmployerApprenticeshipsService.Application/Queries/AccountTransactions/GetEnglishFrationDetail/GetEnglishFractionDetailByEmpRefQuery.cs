using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailByEmpRefQuery : IAsyncRequest<GetEnglishFractionDetailResposne>
    {
        public string EmpRef { get; set; }
        public string UserId { get; set; }
        public string AccountId { get; set; }
    }
}