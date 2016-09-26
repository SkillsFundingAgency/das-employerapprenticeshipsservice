using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLegalEntityAgreement;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        protected EmployerAgreementOrchestrator()
        {
        }

        public EmployerAgreementOrchestrator(IMediator mediator, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
            _logger = logger;
        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> Create(
            long accountId, string externalUserId, string name, string code, string address, DateTime incorporatedDate)
        {
            var response = new OrchestratorResponse<EmployerAgreementViewModel>();

            try
            {
                var request = new GetLatestEmployerAgreementTemplateRequest
                {
                    AccountId = accountId,
                    UserId = externalUserId
                };

                var templateResponse = await _mediator.SendAsync(request);

                response.Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        AccountId = accountId,
                        LegalEntityName = name,
                        LegalEntityCode = code,
                        LegalEntityRegisteredAddress = address,
                        LegalEntityIncorporatedDate = incorporatedDate,
                        Status = EmployerAgreementStatus.Pending,
                        TemplateRef = templateResponse.Template.Ref,
                        TemplateText = templateResponse.Template.Text
                    }
                };
            }
            catch (UnauthorizedAccessException)
            {
                response.Status = HttpStatusCode.Unauthorized;
            }
            catch (InvalidRequestException)
            {
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;
        }

        public async Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(long accountId,
            string externalUserId)
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

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetById(
            long agreementid, long accountId, string externalUserId)
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

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetByLegalEntityCode(
            long accountId, string legalEntityCode, string externalUserId)
        {
            var response = await _mediator.SendAsync(new GetLegalEntityAgreementRequest
            {
                AccountId = accountId,
                LegalEntityCode = legalEntityCode
            });

            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = response.EmployerAgreement
                }
            };
        }

        public async Task<OrchestratorResponse> SignAgreement(long agreementid, long accountId, string externalUserId,
            DateTime signedDate)
        {
            try
            {
                await _mediator.SendAsync(new SignEmployerAgreementCommand
                {
                    AgreementId = agreementid,
                    AccountId = accountId,
                    ExternalUserId = externalUserId,
                    SignedDate = signedDate
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

        public virtual async Task<OrchestratorResponse<FindOrganisationViewModel>> FindLegalEntity(long accountId,
            string companyNumber, string userIdClaim)
        {
            var accountEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                Id = accountId,
                UserId = userIdClaim
            });

            if (accountEntities.Entites.LegalEntityList.Any(
                x => x.Code.Equals(companyNumber, StringComparison.CurrentCultureIgnoreCase)))
            {
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindOrganisationViewModel(),
                    Status = HttpStatusCode.Conflict
                };
            }

            var response = await _mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = companyNumber
            });

            if (response == null)
            {
                _logger.Warn("No response from SelectEmployerViewModel");
                return new OrchestratorResponse<FindOrganisationViewModel>
                {
                    Data = new FindOrganisationViewModel(),
                    Status = HttpStatusCode.NotFound
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

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(
            CreateNewLegalEntity request)
        {
            if (request.SignedAgreement && !request.UserIsAuthorisedToSign)
            {
                var response = await _mediator.SendAsync(new GetLatestEmployerAgreementTemplateRequest
                {
                    AccountId = request.AccountId,
                    UserId = request.ExternalUserId
                });

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            AccountId = request.AccountId,
                            LegalEntityName = request.Name,
                            LegalEntityCode = request.Code,
                            LegalEntityRegisteredAddress = request.Address,
                            LegalEntityIncorporatedDate = request.IncorporatedDate,
                            Status = EmployerAgreementStatus.Pending,
                            TemplateRef = response.Template.Ref,
                            TemplateText = response.Template.Text
                        }
                    },
                    Status = HttpStatusCode.BadRequest
                };
            }

            var createLegalEntityResponse = await _mediator.SendAsync(new CreateLegalEntityCommand
            {
                AccountId = request.AccountId,
                LegalEntity = new LegalEntity
                {
                    Name = request.Name,
                    Code = request.Code,
                    RegisteredAddress = request.Address,
                    DateOfIncorporation = request.IncorporatedDate
                },
                SignAgreement = request.UserIsAuthorisedToSign && request.SignedAgreement,
                SignedDate = request.SignedDate,
                ExternalUserId = request.ExternalUserId
            });

            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = createLegalEntityResponse.AgreementView
                },
                Status = HttpStatusCode.OK
            };
        }
    }
}