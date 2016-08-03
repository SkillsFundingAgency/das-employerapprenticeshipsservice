using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayToken;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        
        public EmployerAccountOrchestrator(IMediator mediator, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task<SelectEmployerViewModel> GetCompanyDetails(SelectEmployerModel model)
        {
            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = model.EmployerRef
            });
            
            if (response == null)
            {
                _logger.Warn("No response from SelectEmployerViewModel");
                return new SelectEmployerViewModel();
            }

            _logger.Info($"Returning Data for {model.EmployerRef}");

            return new SelectEmployerViewModel
            {
                CompanyNumber = response.CompanyNumber,
                CompanyName = response.CompanyName,
                DateOfIncorporation = response.DateOfIncorporation,
                RegisteredAddress = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}"
            };
        }

        public async Task CreateAccount(CreateAccountModel model)
        {
            await _mediator.SendAsync(new CreateAccountCommand
            {
                UserId = model.UserId,
                CompanyNumber = model.CompanyNumber,
                CompanyName = model.CompanyName,
                EmployerRef = model.EmployerRef
            });
        }

        public async Task<string> GetGatewayUrl(string redirectUrl)
        {
            var response = await _mediator.SendAsync(new GetGatewayInformationQuery
            {
                ReturnUrl = redirectUrl
            });

            return response.Url;
        }

        public async Task<HmrcTokenResponse> GetGatewayTokenResponse(string accessCode, string returnUrl)
        {
            var response = await _mediator.SendAsync(new GetGatewayTokenQuery
            {
                RedirectUrl= returnUrl,
                AccessCode = accessCode
            });

            return response.HmrcTokenResponse;
        }


        public async Task<EmpRefLevyInformation> GetHmrcEmployerInformation(string authToken, string empref)
        {

            var response = await _mediator.SendAsync(new GetHmrcEmployerInformatioQuery()
            {
                AuthToken = authToken,
                Empref = empref
            });

            return response.EmployerLevyInformation;
        }
    }
}