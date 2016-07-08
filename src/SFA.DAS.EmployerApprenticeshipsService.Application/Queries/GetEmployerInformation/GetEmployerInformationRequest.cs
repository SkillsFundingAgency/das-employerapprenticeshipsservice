using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation
{
    public class GetEmployerInformationRequest : IAsyncRequest<GetEmployerInformationResponse>
    {
        public string Id { get; set; }
    }
}