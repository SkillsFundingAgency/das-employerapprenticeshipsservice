using MediatR;

namespace SFA.DAS.EmployerFinance.Queries.GetEmployerSchemes
{
    public class GetEmployerSchemesQuery : IAsyncRequest<GetEmployerSchemesResponse>
    {
        public long Id { get; set; }
    }
}
