using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclarationsByAccountAndPeriod
{
    public class GetLevyDeclarationsByAccountAndPeriodRequest : IAsyncRequest<GetLevyDeclarationsByAccountAndPeriodResponse>
    {
        public string HashedAccountId { get; set; }
        public string PayrollYear { get; set; }
        public short PayrollMonth { get; set; }
    }
}
