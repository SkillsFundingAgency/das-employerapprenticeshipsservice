using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFrationHistory
{
    public class GetEnglishFractionHistoryQuery : IAsyncRequest<GetEnglishFractionHistoryResposne>
    {
        public string HashedAccountId { get; set; }
        public string EmpRef { get; set; }
    }
}