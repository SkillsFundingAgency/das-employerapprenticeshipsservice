using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementsRemove;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
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

        public virtual async Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(string hashedId,
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
                        EmployerAgreementsData = response
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
                    AgreementId = agreementid,
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId
                });

                var employerAgreementView =
                    _mapper.Map<AgreementDto, EmployerAgreementView>(response.EmployerAgreement);

                var organisationLookupByIdPossible = await _referenceDataService.IsIdentifiableOrganisationType(employerAgreementView.LegalEntitySource);

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = employerAgreementView,
                        OrganisationLookupPossible = organisationLookupByIdPossible
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
            catch (UnauthorizedAccessException)
            {
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }

        public async Task<OrchestratorResponse<SignAgreementViewModel>> SignAgreement(string agreementid, string hashedId, string externalUserId,
            DateTime signedDate, string companyName)
        {
            try
            {
                await _mediator.SendAsync(new SignEmployerAgreementCommand
                {
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId,
                    SignedDate = signedDate,
                    HashedAgreementId = agreementid,
                    OrganisationName = companyName
                });

                var agreements = await _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
                {
                    ExternalUserId = externalUserId,
                    HashedAccountId = hashedId
                });

                return new OrchestratorResponse<SignAgreementViewModel>
                {
                    Data = new SignAgreementViewModel
                    {
                        HasFurtherPendingAgreements = agreements.HasPendingAgreements
                    }
                };
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<SignAgreementViewModel>
                {
                    Exception = ex,
                    Status = HttpStatusCode.BadRequest
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new OrchestratorResponse<SignAgreementViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }
        public virtual async Task<OrchestratorResponse<bool>> RemoveLegalAgreement(ConfirmLegalAgreementToRemoveViewModel model, string userId)
        {
            var response = new OrchestratorResponse<bool>();
            try
            {
                if (model.RemoveOrganisation == null)
                {
                    response.Status = HttpStatusCode.BadRequest;
                    response.FlashMessage =
                        FlashMessageViewModel.CreateErrorFlashMessageViewModel(new Dictionary<string, string>
                        {
                            {"RemoveOrganisation", "Confirm you wish to remove the organisation"}
                        });
                    return response;
                }

                if (model.RemoveOrganisation == 1)
                {
                    response.Status = HttpStatusCode.Continue;
                    return response;
                }

                await _mediator.SendAsync(new RemoveLegalEntityCommand
                {
                    HashedAccountId = model.HashedAccountId,
                    UserId = userId,
                    HashedLegalAgreementId = model.HashedAgreementId
                });

                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = $"You have removed {model.Name}.",
                    Severity = FlashMessageSeverityLevel.Success
                };
                response.Data = true;
            }
            catch (InvalidRequestException ex)
            {

                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(ex.ErrorMessages);
                response.Exception = ex;
            }
            catch (UnauthorizedAccessException ex)
            {
                response.Status = HttpStatusCode.Unauthorized;
                response.Exception = ex;
            }

            return response;
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

        public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
        {
            var userResponse = await Mediator.SendAsync(new GetTeamMemberQuery { HashedAccountId = hashedAccountId, TeamMemberId = userId });
            return userResponse.User.ShowWizard && userResponse.User.Role == Role.Owner;
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
    }
}