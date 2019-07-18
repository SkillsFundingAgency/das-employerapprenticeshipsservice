using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerInformation
{
    public class GetEmployerInformationRequest : IAsyncRequest<GetEmployerInformationResponse>
    {
        public string Id { get; set; }
    }
}