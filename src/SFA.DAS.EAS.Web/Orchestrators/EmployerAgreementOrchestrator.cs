using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Queries.GetTeamUser;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;
using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IReferenceDataService _referenceDataService;

        protected EmployerAgreementOrchestrator()
        {
        }

        public EmployerAgreementOrchestrator(
            IMediator mediator,
            ILog logger,
            IMapper mapper,
            EmployerApprenticeshipsServiceConfiguration configuration,
            IReferenceDataService referenceDataService) : base(mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (referenceDataService == null)
                throw new ArgumentNullException(nameof(referenceDataService));

            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _referenceDataService = referenceDataService;
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

                pdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel { PdfStream = result.FileStream };
            }
            catch (UnauthorizedAccessException)
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

        public async Task<OrchestratorResponse<EmployerAgreementPdfViewModel>> GetSignedPdfEmployerAgreement(string hashedAccountId, string agreementId, string userId)
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

                signedPdfEmployerAgreement.Data = new EmployerAgreementPdfViewModel { PdfStream = result.FileStream };

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
            catch (UnauthorizedAccessException)
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

        public virtual async Task<OrchestratorResponse<LegalAgreementsToRemoveViewModel>> GetLegalAgreementsToRemove(string hashedAccountId, string userId)
        {
            var response = new OrchestratorResponse<LegalAgreementsToRemoveViewModel>();
            try
            {
                var result = await _mediator.SendAsync(new GetAccountEmployerAgreementsRemoveRequest
                {
                    HashedAccountId = hashedAccountId,
                    UserId = userId
                });

                response.Data = new LegalAgreementsToRemoveViewModel
                {
                    Agreements = result.Agreements

                };
            }
            catch (InvalidRequestException ex)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                response.Status = HttpStatusCode.Unauthorized;
                response.Exception = ex;
            }
            return response;
        }

        public virtual async Task<OrchestratorResponse<ConfirmLegalAgreementToRemoveViewModel>> GetConfirmRemoveOrganisationViewModel(string agreementId, string hashedAccountId, string userId)
        {
            var response = new OrchestratorResponse<ConfirmLegalAgreementToRemoveViewModel>();
            try
            {
                var result = await _mediator.SendAsync(new GetAccountEmployerAgreementRemoveRequest
                {
                    HashedAccountId = hashedAccountId,
                    UserId = userId,
                    HashedAgreementId = agreementId
                });
                response.Data = new ConfirmLegalAgreementToRemoveViewModel
                {
                    HashedAccountId = result.Agreement.HashedAccountId,
                    HashedAgreementId = result.Agreement.HashedAgreementId,
                    Id = result.Agreement.Id,
                    Name = result.Agreement.Name,
                    AgreementStatus = result.Agreement.Status
                };
            }
            catch (InvalidRequestException ex)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                response.Status = HttpStatusCode.Unauthorized;
                response.Exception = ex;
            }

            return response;
        }

      
        public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
        {
            var userResponse = await Mediator.SendAsync(new GetTeamMemberQuery { HashedAccountId = hashedAccountId, TeamMemberId = userId });
            return userResponse.User.ShowWizard && userResponse.User.RoleId == (short)Role.Owner;
        }
    }
}