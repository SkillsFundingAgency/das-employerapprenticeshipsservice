using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public abstract class EmployerVerificationOrchestratorBase
    {

        protected readonly IMediator Mediator;
        protected readonly ILogger Logger;
        protected readonly ICookieService CookieService;

        //Needed for tests
        protected EmployerVerificationOrchestratorBase()
        {
            
        }

        protected EmployerVerificationOrchestratorBase(IMediator mediator, ILogger logger, ICookieService cookieService)
        {
            Mediator = mediator;
            Logger = logger;
            CookieService = cookieService;
        }
        

        public virtual async Task<string> GetGatewayUrl(string redirectUrl)
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

        public async Task<SelectEmployerViewModel> GetCompanyDetails(SelectEmployerModel model)
        {
            var response = await Mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = model.EmployerRef
            });
            
            if (response == null)
            {
                Logger.Warn("No response from SelectEmployerViewModel");
                return new SelectEmployerViewModel();
            }

            Logger.Info($"Returning Data for {model.EmployerRef}");

            return new SelectEmployerViewModel
            {
                CompanyNumber = response.CompanyNumber,
                CompanyName = response.CompanyName,
                DateOfIncorporation = response.DateOfIncorporation,
                RegisteredAddress = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}"
            };
        }
    }
}