﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IReferenceDataService _referenceDataService;

        protected EmployerAgreementOrchestrator()
        {
        }

        public EmployerAgreementOrchestrator(
            IMediator mediator,
            IMapper mapper,
            IReferenceDataService referenceDataService) : base(mediator)
        {
            _mediator = mediator;
            _mapper = mapper;
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


        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetById(
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

        public async Task<OrchestratorResponse<SignAgreementViewModel>> SignAgreement(string agreementid, string hashedId, string externalUserId, DateTime signedDate)
        {
            try
            {
                var agreement = await _mediator.SendAsync(new SignEmployerAgreementCommand
                {
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId,
                    SignedDate = signedDate,
                    HashedAgreementId = agreementid
                });

                var unsignedAgreement = await _mediator.SendAsync(new GetNextUnsignedEmployerAgreementRequest
                { 
                    ExternalUserId = externalUserId,
                    HashedAccountId = hashedId
                });

                return new OrchestratorResponse<SignAgreementViewModel>
                {
                    Data = new SignAgreementViewModel
                    {
                        HasFurtherPendingAgreements = !string.IsNullOrEmpty(unsignedAgreement.HashedAgreementId),
                        SignedAgreementType = agreement.AgreementType,
                        LegalEntityName = agreement.LegalEntityName
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

        public virtual async Task<OrchestratorResponse<bool>> RemoveLegalAgreement(ConfirmOrganisationToRemoveViewModel model, string userId)
        {
            var response = new OrchestratorResponse<bool>();
            
            try
            {
                await _mediator.SendAsync(new RemoveLegalEntityCommand
                {
                    HashedAccountId = model.HashedAccountId,
                    UserId = userId,
                    HashedAccountLegalEntityId = model.
                    HashedAccountLegalEntitytId = model.HashedAccountLegalEntitytId
                });

                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = $"You have removed {model.Name}.",
                    Severity = FlashMessageSeverityLevel.Success
                };

                response.Status = HttpStatusCode.OK;
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

        public virtual async Task<OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>> GetConfirmRemoveOrganisationViewModel(string accountLegalEntityHashedId, string hashedAccountId, string userId)
        {
            var response = new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>();

            try
            {
                var result = await _mediator.SendAsync(new GetAccountLegalEntityRemoveRequest
                {
                    HashedAccountId = hashedAccountId,
                    UserId = userId,
                    HashedAccountLegalEntityId = accountLegalEntityHashedId
                });

                response.Data = new ConfirmOrganisationToRemoveViewModel
                {
                    HashedAccountId = hashedAccountId,
                    HashedAccountLegalEntitytId = accountLegalEntityHashedId,
                    HasSignedAgreement = result.HasSignedAgreement,
                    CanBeRemoved = result.CanBeRemoved,
                    Name = result.Name
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

        public virtual async Task<OrchestratorResponse<ICollection<OrganisationAgreementViewModel>>> GetOrganisationAgreements(string accountLegalEntityHashedId)
        {
            var response = new OrchestratorResponse<ICollection<OrganisationAgreementViewModel>>();

            try
            {
                var result = await _mediator.SendAsync(new GetOrganisationAgreementsRequest
                {
                    AccountLegalEntityHashedId = accountLegalEntityHashedId
                });
                
                response.Data = _mapper.Map<ICollection<EmployerAgreementDto>, ICollection<OrganisationAgreementViewModel>>(result.Agreements);
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<ICollection<OrganisationAgreementViewModel>>
                {
                    Status = HttpStatusCode.BadRequest,                    
                    Exception = ex
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new OrchestratorResponse<ICollection<OrganisationAgreementViewModel>>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }

            return response;
        }
    }
}
   