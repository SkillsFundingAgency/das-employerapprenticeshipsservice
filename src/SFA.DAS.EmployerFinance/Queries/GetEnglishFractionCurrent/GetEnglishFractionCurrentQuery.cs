using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFractionCurrent
{
    public class GetEnglishFractionCurrentQuery : IAsyncRequest<GetEnglishFractionCurrentResponse>
    {
        public string HashedAccountId { get; set; }
        public string[] EmpRefs { get; set; }
    }
}