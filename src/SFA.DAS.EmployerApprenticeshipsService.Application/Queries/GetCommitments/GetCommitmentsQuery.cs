using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitments
{
    public sealed class GetCommitmentsQuery : IAsyncRequest<GetCommitmentsResponse>
    {
        public string AccountHashId { get; set; }
    }
}
