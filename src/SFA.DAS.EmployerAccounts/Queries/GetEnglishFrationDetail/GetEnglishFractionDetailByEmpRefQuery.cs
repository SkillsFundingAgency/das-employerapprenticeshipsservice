using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailByEmpRefQuery : IAsyncRequest<GetEnglishFractionDetailResposne>
    {
        public long AccountId { get; set; }
        public string EmpRef { get; set; }
    }
}