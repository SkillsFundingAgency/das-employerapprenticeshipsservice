using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationQuery :IAsyncRequest<GetHmrcEmployerInformationResponse>
    {
        public string AuthToken { get; set; }
    }
}