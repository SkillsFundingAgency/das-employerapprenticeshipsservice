using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator : IEmployerAgreementOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public EmployerAgreementOrchestrator(IMediator mediator, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(long accountId, string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
                {
                    AccountId = accountId,
                    ExternalUserId = externalUserId
                });

                return new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Data = new EmployerAgreementListViewModel
                    {
                        AccountId = accountId,
                        EmployerAgreements = response.EmployerAgreements
                    }
                };
            }
            catch (Exception)
            {
                return new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetById(long agreementid, long accountId, string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAgreementRequest
                {
                    AgreementId = agreementid,
                    AccountId = accountId,
                    ExternalUserId = externalUserId
                });

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = response.EmployerAgreement
                    }
                };
            }
            catch (Exception)
            {
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }
        
        public async Task<OrchestratorResponse> SignAgreement(long agreementid, long accountId, string externalUserId)
        {
            try
            {
                await _mediator.SendAsync(new SignEmployerAgreementCommand
                {
                    AgreementId = agreementid,
                    AccountId = accountId,
                    ExternalUserId = externalUserId
                });

                return new OrchestratorResponse();
            }
            catch (Exception)
            {
                return new OrchestratorResponse
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }

        public async Task<OrchestratorResponse<FindOrganisationViewModel>> FindLegalEntity(long accountId, string companyNumber)
        {
            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = companyNumber
            });

            if (response == null)
            {
                _logger.Warn("No response from SelectEmployerViewModel");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    FlashMessage = new FlashMessageViewModel()
                    {
                        Message = "No companies match the company house number you have entered.",
                        SubMessage = "Please try again."
                    },
                    Data = new FindOrganisationViewModel()
                };
            }

            _logger.Info($"Returning Data for {companyNumber}");

            return new OrchestratorResponse<FindOrganisationViewModel>
            {
                Data = new FindOrganisationViewModel
                {
                    AccountId = accountId,
                    CompanyNumber = response.CompanyNumber,
                    CompanyName = response.CompanyName,
                    DateOfIncorporation = response.DateOfIncorporation,
                    RegisteredAddress = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}"
                }
            };
        }
    }
}