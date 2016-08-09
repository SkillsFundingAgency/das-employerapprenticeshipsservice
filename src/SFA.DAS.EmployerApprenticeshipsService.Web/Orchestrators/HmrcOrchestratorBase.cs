using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public abstract class HmrcOrchestratorBase
    {

        protected readonly IMediator Mediator;
        protected readonly ILogger Logger;
        protected readonly ICookieService CookieService;

        protected HmrcOrchestratorBase(IMediator mediator, ILogger logger, ICookieService cookieService)
        {
            Mediator = mediator;
            Logger = logger;
            CookieService = cookieService;
        }
        

        public async Task<string> GetGatewayUrl(string redirectUrl)
        {
            var response = await Mediator.SendAsync(new GetGatewayInformationQuery
            {
                ReturnUrl = redirectUrl
            });

            return response.Url;
        }

        public async Task<HmrcTokenResponse> GetGatewayTokenResponse(string accessCode, string returnUrl)
        {
            var response = await Mediator.SendAsync(new GetGatewayTokenQuery
            {
                RedirectUrl = returnUrl,
                AccessCode = accessCode
            });

            return response.HmrcTokenResponse;
        }


        public async Task<GetHmrcEmployerInformationResponse> GetHmrcEmployerInformation(string authToken)
        {

            var response = await Mediator.SendAsync(new GetHmrcEmployerInformationQuery
            {
                AuthToken = authToken

            });

            return response;
        }
    }
}