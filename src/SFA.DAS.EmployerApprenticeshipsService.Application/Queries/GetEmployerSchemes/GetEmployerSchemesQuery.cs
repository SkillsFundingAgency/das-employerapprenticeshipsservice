using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerSchemes
{
    public class GetEmployerSchemesQuery : IAsyncRequest<GetEmployerSchemesResponse>
    {
        public long Id { get; set; }
    }
}
