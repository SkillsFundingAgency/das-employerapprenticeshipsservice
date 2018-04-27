using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.RemoveLegalEntity;
using SFA.DAS.EAS.Application.Commands.SignEmployerAgreement;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate;
using SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Queries.GetTeamUser;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly ILog _logger;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IMediator _mediator;

        protected EmployerAgreementOrchestrator()
        {
        }

        public EmployerAgreementOrchestrator(IMediator mediator, ILog logger, EmployerApprenticeshipsServiceConfiguration configuration) : base(mediator)
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

        public virtual async  Task<OrchestratorResponse<LegalAgreementsToRemoveViewModel>> GetLegalAgreementsToRemove(string hashedAccountId, string userId)
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

        public virtual async Task<OrchestratorResponse<bool>>  RemoveLegalAgreement(ConfirmLegalAgreementToRemoveViewModel model, string userId)
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

        public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
        {
            var userResponse = await Mediator.SendAsync(new GetTeamMemberQuery { HashedAccountId = hashedAccountId, TeamMemberId = userId });
            return userResponse.User.ShowWizard && userResponse.User.RoleId == (short)Role.Owner;
        }
    }
}