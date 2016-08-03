using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformationQuery :IAsyncRequest<GetHmrcEmployerInformationResponse>
    {
        public string Empref { get; set; }
        public string AuthToken { get; set; }
    }
}