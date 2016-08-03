using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation
{
    public class GetHmrcEmployerInformatioQuery :IAsyncRequest<GetHmrcEmployerInformatioResponse>
    {
        public string Empref { get; set; }
        public string AuthToken { get; set; }
    }
}