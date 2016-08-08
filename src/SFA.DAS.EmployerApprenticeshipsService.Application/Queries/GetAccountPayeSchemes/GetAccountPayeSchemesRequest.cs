using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesRequest : IAsyncRequest<GetAccountPayeSchemesResponse>
    {
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}