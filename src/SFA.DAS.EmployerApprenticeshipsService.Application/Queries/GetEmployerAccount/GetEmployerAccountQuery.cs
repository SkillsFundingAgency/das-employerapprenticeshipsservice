using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAccount
{
    public class GetEmployerAccountQuery : IAsyncRequest<GetEmployerAccountResponse>
    {
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
        public bool IsSystemUser { get; set; }      
    }
}
