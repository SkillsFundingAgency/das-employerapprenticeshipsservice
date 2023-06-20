using AutoMapper;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class EmployerAgreementOrchestrator : UserVerificationOrchestratorBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IReferenceDataService _referenceDataService;
    private readonly IEncodingService _encodingService;

    protected EmployerAgreementOrchestrator()
    {
    }

    public EmployerAgreementOrchestrator(
        IMediator mediator,
        IMapper mapper,
        IReferenceDataService referenceDataService,
        IEncodingService encodingService) : base(mediator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _referenceDataService = referenceDataService;
        _encodingService = encodingService;
    }

    public virtual async Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(string hashedAccountId, string externalUserId)
    {
        try
        {
            var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

            var response = await _mediator.Send(new GetAccountEmployerAgreementsRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            return new OrchestratorResponse<EmployerAgreementListViewModel>
            {
                Data = new EmployerAgreementListViewModel
                {
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
            var response = await _mediator.Send(new GetEmployerAgreementRequest
            {
                HashedAgreementId = agreementid,
                HashedAccountId = hashedId,
                ExternalUserId = externalUserId
            });

            var employerAgreementView =
                _mapper.Map<AgreementDto, EmployerAccounts.Models.EmployerAgreement.EmployerAgreementView>(response.EmployerAgreement);

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

    public async Task<OrchestratorResponse<SignAgreementViewModel>> SignAgreement(string agreementid, string hashedAccountId, string externalUserId, DateTime signedDate)
    {
        try
        {
            var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
            var agreement = await _mediator.Send(new SignEmployerAgreementCommand
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId,
                SignedDate = signedDate,
                HashedAgreementId = agreementid
            });

            var unsignedAgreement = await _mediator.Send(new GetNextUnsignedEmployerAgreementRequest
            {
                ExternalUserId = externalUserId,
                AccountId = accountId
            });

            return new OrchestratorResponse<SignAgreementViewModel>
            {
                Data = new SignAgreementViewModel
                {
                    HasFurtherPendingAgreements = unsignedAgreement.AgreementId.HasValue,
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

    public virtual async Task<OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>> RemoveLegalAgreement(ConfirmOrganisationToRemoveViewModel model, string userId)
    {
        var accountId = _encodingService.Decode(model.HashedAccountId, EncodingType.AccountId);
        var accountLegalEntityId = _encodingService.Decode(model.HashedAccountLegalEntitytId, EncodingType.PublicAccountLegalEntityId);
        var response = new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>();

        try
        {
            await _mediator.Send(new RemoveLegalEntityCommand
            {
                AccountId = accountId,
                UserId = userId,
                AccountLegalEntityId = accountLegalEntityId
            });

            response.FlashMessage = new FlashMessageViewModel
            {
                Headline = $"You have removed {model.Name}.",
                Severity = FlashMessageSeverityLevel.Success
            };

            response.Status = HttpStatusCode.OK;
            response.Data = model;
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

        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var decodedAgreementId = _encodingService.Decode(agreementId, EncodingType.AccountId);

        try
        {
            var result = await _mediator.Send(new GetEmployerAgreementPdfRequest
            {
                AccountId = accountId,
                LegalAgreementId = decodedAgreementId,
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
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var decodedAgreementId = _encodingService.Decode(agreementId, EncodingType.AccountId);
        var signedPdfEmployerAgreement = new OrchestratorResponse<EmployerAgreementPdfViewModel>();

        try
        {
            var result = await _mediator.Send(new GetSignedEmployerAgreementPdfRequest
            {
                AccountId = accountId,
                LegalAgreementId = decodedAgreementId,
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

    public virtual async Task<OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>> GetConfirmRemoveOrganisationViewModel(string hashedAccountId, string accountLegalEntityHashedId, string userId)
    {
        var response = new OrchestratorResponse<ConfirmOrganisationToRemoveViewModel>();

        try
        {
            var result = await _mediator.Send(new GetAccountLegalEntityRemoveRequest
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

    public virtual async Task<OrchestratorResponse<OrganisationAgreementsViewModel>> GetOrganisationAgreements(string accountLegalEntityHashedId)
    {
        var response = new OrchestratorResponse<OrganisationAgreementsViewModel>();

        try
        {
            var result = await _mediator.Send(new GetOrganisationAgreementsRequest
            {
                AccountLegalEntityHashedId = accountLegalEntityHashedId
            });

            response.Data = new OrganisationAgreementsViewModel
            {
                AgreementId = accountLegalEntityHashedId,
                Agreements = _mapper.Map<ICollection<EmployerAgreementDto>, ICollection<OrganisationAgreementViewModel>>(result.Agreements)
            };
        }
        catch (InvalidRequestException ex)
        {
            return new OrchestratorResponse<OrganisationAgreementsViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Exception = ex
            };
        }
        catch (UnauthorizedAccessException)
        {
            return new OrchestratorResponse<OrganisationAgreementsViewModel>
            {
                Status = HttpStatusCode.Unauthorized
            };
        }

        return response;
    }

    public virtual async Task<OrchestratorResponse<SignEmployerAgreementViewModel>> GetSignedAgreementViewModel(string hashedAccountId, string agreementId, string externalUserId)
    {
        var response = new OrchestratorResponse<SignEmployerAgreementViewModel>();

        try
        {
            var result = await _mediator.Send(new GetEmployerAgreementRequest { HashedAccountId = hashedAccountId, HashedAgreementId = agreementId, ExternalUserId = externalUserId });
            var viewModel = _mapper.Map<GetEmployerAgreementResponse, SignEmployerAgreementViewModel>(result);

            var signedAgreementResponse = await _mediator.Send(new GetLastSignedAgreementRequest { AccountLegalEntityId = result.EmployerAgreement.LegalEntity.AccountLegalEntityId });
            viewModel.PreviouslySignedEmployerAgreement = _mapper.Map<EmployerAccounts.Models.EmployerAgreement.EmployerAgreementView>(signedAgreementResponse.LastSignedAgreement);

            response.Data = viewModel;
        }
        catch (InvalidRequestException ex)
        {
            return new OrchestratorResponse<SignEmployerAgreementViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Exception = ex
            };
        }
        catch (UnauthorizedAccessException)
        {
            return new OrchestratorResponse<SignEmployerAgreementViewModel>
            {
                Status = HttpStatusCode.Unauthorized
            };
        }

        return response;
    }

    public virtual async Task<OrchestratorResponse<NextUnsignedAgreementViewModel>> GetNextUnsignedAgreement(string hashedAccountId, string externalUserId)
    {
        var response = new OrchestratorResponse<NextUnsignedAgreementViewModel>();

        try
        {
            var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
            var result = await _mediator.Send(new GetNextUnsignedEmployerAgreementRequest { AccountId = accountId, ExternalUserId = externalUserId });

            var hashedAgreementId = result.AgreementId.HasValue ? _encodingService.Encode(result.AgreementId.Value, EncodingType.AccountId) : string.Empty;

            response.Data = new NextUnsignedAgreementViewModel
            {
                HasNextAgreement = !string.IsNullOrEmpty(hashedAgreementId),
                NextAgreementHashedId = hashedAgreementId
            };
        }
        catch (InvalidRequestException ex)
        {
            return new OrchestratorResponse<NextUnsignedAgreementViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Exception = ex
            };
        }
        catch (UnauthorizedAccessException)
        {
            return new OrchestratorResponse<NextUnsignedAgreementViewModel>
            {
                Status = HttpStatusCode.Unauthorized
            };
        }

        return response;
    }
}