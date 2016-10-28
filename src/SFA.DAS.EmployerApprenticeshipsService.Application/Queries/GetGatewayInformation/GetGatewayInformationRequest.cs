using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetGatewayInformation
{
    public class GetGatewayInformationQuery : IAsyncRequest<GetGatewayInformationResponse>
    {
        public string ReturnUrl { get; set; }
    }
}
