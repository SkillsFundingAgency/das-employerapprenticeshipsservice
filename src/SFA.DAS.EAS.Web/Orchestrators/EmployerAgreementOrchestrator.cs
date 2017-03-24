using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly ILogger _logger;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IMediator _mediator;

        protected EmployerAgreementOrchestrator()
        {
        }

        public EmployerAgreementOrchestrator(IMediator mediator, ILogger logger, EmployerApprenticeshipsServiceConfiguration configuration) : base(mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
            _logger = logger;
            _configuration = configuration;
        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> Create(
            string hashedId, string externalUserId, string name, string code, string address, DateTime incorporatedDate)
        {
            var response = new OrchestratorResponse<EmployerAgreementViewModel>();

            try
            {
                var request = new GetLatestEmployerAgreementTemplateRequest
                {
                    HashedAccountId = hashedId,
                    UserId = externalUserId
                };

                var templateResponse = await _mediator.SendAsync(request);

                response.Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = new EmployerAgreementView
                    {
                        LegalEntityName = name,
                        LegalEntityCode = code,
                        LegalEntityAddress = address,
                        LegalEntityInceptionDate = incorporatedDate,
                        Status = EmployerAgreementStatus.Pending,
                        TemplatePartialViewName = templateResponse.Template.PartialViewName
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

        public async Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(string hashedId,
            string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
                {
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId
                });

                return new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Data = new EmployerAgreementListViewModel
                    {
                        HashedAccountId = hashedId,
                        EmployerAgreements = response.EmployerAgreements,
                        ShowAgreements = _configuration.ShowAgreements()
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
            string agreementid, string hashedId, string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetEmployerAgreementRequest
                {
                    HashedAgreementId = agreementid,
                    HashedAccountId = hashedId,
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
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    Data = new EmployerAgreementViewModel(),
                    Exception = ex
                };
            }
            catch (UnauthorizedAccessException ex)
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

        public async Task<OrchestratorResponse> SignAgreement(string agreementid, string hashedId, string externalUserId,
            DateTime signedDate)
        {
            try
            {
                await _mediator.SendAsync(new SignEmployerAgreementCommand
                {
                    HashedAgreementId = agreementid,
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId,
                    SignedDate = signedDate
                });

                return new OrchestratorResponse
                {
                };
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse
                {
                    Exception = ex,
                    Status = HttpStatusCode.BadRequest
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new OrchestratorResponse
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }

       
        public async Task<OrchestratorResponse<AddLegalEntityViewModel>> GetAddLegalEntityViewModel(string hashedAccountId, string externalUserId)
        {
            var userRole = await GetUserAccountRole(hashedAccountId, externalUserId);

            return new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = hashedAccountId },
                Status = userRole.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
            };

        }

        public async Task<OrchestratorResponse<EmployerAgreementPdfViewModel>> GetPdfEmployerAgreement(string hashedAccountId, string agreementId, string userId)
        {
            var pdfEmployerAgreement = new OrchestratorResponse<EmployerAgreementPdfViewModel>();

            try
            {
                var result = await _mediator.SendAsync(new GetEmployerAgreementPdfRequest
                {
                    HashedAccountId = hashedAccountId,
                    HashedLegalAgreementId = agreementId,
                    UserId = userId
                });

                pdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel {PdfStream = result.FileStream};
            }
            catch (UnauthorizedAccessException ex)
            {
                pdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel();
                pdfEmployerAgreement.Status = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                pdfEmployerAgreement.Exception = ex;
                pdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel();
                pdfEmployerAgreement.Status = HttpStatusCode.NotFound;
            }
            
            return pdfEmployerAgreement;
        }

        public async Task<OrchestratorResponse<EmployerAgreementPdfViewModel>>  GetSignedPdfEmployerAgreement(string hashedAccountId, string agreementId, string userId)
        {

            var signedPdfEmployerAgreement = new OrchestratorResponse<EmployerAgreementPdfViewModel>();

            try
            {
                var result =
                    await
                        _mediator.SendAsync(new GetSignedEmployerAgreementPdfRequest
                        {
                            HashedAccountId = hashedAccountId,
                            HashedLegalAgreementId = agreementId,
                            UserId = userId
                        });

                signedPdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel {PdfStream = result.FileStream};

                return signedPdfEmployerAgreement;
            }
            catch (InvalidRequestException ex)
            {
                signedPdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel();
                signedPdfEmployerAgreement.Status = HttpStatusCode.BadRequest;
                signedPdfEmployerAgreement.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                signedPdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel();
                signedPdfEmployerAgreement.Status = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                signedPdfEmployerAgreement.Exception = ex;
                signedPdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel();
                signedPdfEmployerAgreement.Status = HttpStatusCode.NotFound;
            }

            return signedPdfEmployerAgreement;

        }
    }
}