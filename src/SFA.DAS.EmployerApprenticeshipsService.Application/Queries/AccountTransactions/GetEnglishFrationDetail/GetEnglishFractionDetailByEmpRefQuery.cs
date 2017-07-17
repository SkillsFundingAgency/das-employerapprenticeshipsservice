using MediatR;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailByEmpRefQuery : IAsyncRequest<GetEnglishFractionDetailResposne>
    {
        public long AccountId { get; set; }
        public string EmpRef { get; set; }
    }
}