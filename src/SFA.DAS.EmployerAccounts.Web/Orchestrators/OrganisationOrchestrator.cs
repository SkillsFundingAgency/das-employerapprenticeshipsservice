using AutoMapper;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;
using SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Validation;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class OrganisationOrchestrator : UserVerificationOrchestratorBase, IOrchestratorCookie
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ICookieStorageService<EmployerAccountData> _cookieService;
    private readonly IEncodingService _encodingService;
    private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

    public OrganisationOrchestrator(
        IMediator mediator,
        IMapper mapper,
        ICookieStorageService<EmployerAccountData> cookieService,
        IEncodingService encodingService)
        : base(mediator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _cookieService = cookieService;
        this._encodingService = encodingService;
    }
    
    protected OrganisationOrchestrator() { }

    public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(CreateNewLegalEntityViewModel request)
    {
        try
        {
            var result = await _mediator.Send(new CreateLegalEntityCommand
            {
                HashedAccountId = request.HashedAccountId,
                Code = request.Code,
                DateOfIncorporation = request.IncorporatedDate,
                Status = request.LegalEntityStatus,
                Source = request.Source,
                PublicSectorDataSource = request.PublicSectorDataSource,
                Sector = request.Sector,
                Name = request.Name,
                Address = request.Address,
                ExternalUserId = request.ExternalUserId
            });

            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Data = new EmployerAgreementViewModel
                {
                    EmployerAgreement = result.AgreementView
                },
                Status = HttpStatusCode.OK
            };
        }
        catch (UnauthorizedAccessException e)
        {
            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Status = HttpStatusCode.Unauthorized,
                Exception = e,
            };
        }
        catch (InvalidRequestException e)
        {
            return new OrchestratorResponse<EmployerAgreementViewModel>
            {
                Status = HttpStatusCode.BadRequest,
                Exception = e,
            };
        }
    }

    public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> ValidateLegalEntityName(OrganisationDetailsViewModel request)
    {
        var response = new OrchestratorResponse<OrganisationDetailsViewModel>
        {
            Data = request
        };

        var validator = new OrganisationDetailsViewModelValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            response.Data.ErrorDictionary = new Dictionary<string, string>();
            foreach (var validationError in validationResult.Errors)
            {
                response.Data.ErrorDictionary.Add(validationError.PropertyName, validationError.ErrorMessage);
            }

            response.Status = HttpStatusCode.BadRequest;

            response.FlashMessage = new FlashMessageViewModel
            {
                Headline = "Errors to fix",
                Message = "Check the following details:",
                Severity = FlashMessageSeverityLevel.Error,
                ErrorMessages = response.Data.ErrorDictionary
            };
        }

        return response;
    }

    public virtual OrchestratorResponse<OrganisationDetailsViewModel> AddOrganisationAddress(AddOrganisationAddressViewModel viewModel)
    {
        try
        {
            var request = _mapper.Map<CreateOrganisationAddressRequest>(viewModel.Address);

            var response =  _mediator.Send(request).Result;

            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = viewModel.OrganisationHashedId,
                    Name = viewModel.OrganisationName,
                    Address = response.Address,
                    DateOfInception = viewModel.OrganisationDateOfInception,
                    ReferenceNumber = viewModel.OrganisationReferenceNumber ?? string.Empty,
                    Type = viewModel.OrganisationType,
                    PublicSectorDataSource = viewModel.PublicSectorDataSource,
                    Status = viewModel.OrganisationStatus,
                    Sector = viewModel.Sector
                }
            };
        }
        catch (InvalidRequestException e)
        {
            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    ErrorDictionary = e.ErrorMessages
                },
                Status = HttpStatusCode.BadRequest,
                Exception = e,
                FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = e.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                }
            };
        }
    }

    public virtual EmployerAccountData GetCookieData(HttpContext context)
    {
        return _cookieService.Get(CookieName);
    }

    public virtual void CreateCookieData(HttpContext context, EmployerAccountData data)
    {
        _cookieService.Create(data, CookieName, 365);
    }

    public virtual OrchestratorResponse<OrganisationAddedNextStepsViewModel> GetOrganisationAddedNextStepViewModel(
        string organisationName,
        string hashedAgreementId)
    {
        return new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
        {
            Data = new OrganisationAddedNextStepsViewModel { OrganisationName = organisationName, HashedAgreementId = hashedAgreementId }
        };
    }

    public async Task<OrchestratorResponse<ReviewOrganisationAddressViewModel>> GetRefreshedOrganisationDetails(string hashedAccountLegalEntityId)
    {
        var accountLegalEntityId = _encodingService.Decode(hashedAccountLegalEntityId, EncodingType.PublicAccountLegalEntityId);

        var currentDetails = await Mediator.Send(new GetAccountLegalEntityRequest
        {
            AccountLegalEntityId = accountLegalEntityId
        });

        var refreshedDetails = await Mediator.Send(new GetOrganisationByIdRequest
        {
            Identifier = currentDetails.AccountLegalEntity.Identifier,
            OrganisationType = currentDetails.AccountLegalEntity.OrganisationType
        });

        var result = new OrchestratorResponse<ReviewOrganisationAddressViewModel>
        {
            Data = new ReviewOrganisationAddressViewModel
            {
                DataSourceFriendlyName = currentDetails.AccountLegalEntity.OrganisationType.GetFriendlyName(),
                OrganisationName = currentDetails.AccountLegalEntity.Name,
                OrganisationAddress = currentDetails.AccountLegalEntity.Address,
                RefreshedName = refreshedDetails.Organisation.Name,
                RefreshedAddress = refreshedDetails.Organisation.Address.FormatAddress(),
                HashedAccountLegalEntityId = hashedAccountLegalEntityId
            }
        };

        result.Data.UpdatesAvailable = CheckForUpdate(result.Data.OrganisationName, result.Data.RefreshedName, OrganisationUpdatesAvailable.Name) |
                                       CheckForUpdate(result.Data.OrganisationAddress, result.Data.RefreshedAddress, OrganisationUpdatesAvailable.Address);

        return result;
    }

    public async Task<OrchestratorResponse<OrganisationUpdatedNextStepsViewModel>> UpdateOrganisation(
        string accountLegalEntityPublicHashedId, 
        string organisationName, 
        string organisationAddress, 
        string hashedAccountId, 
        string userId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var accountLegalEntityId = _encodingService.Decode(accountLegalEntityPublicHashedId, EncodingType.PublicAccountLegalEntityId);    

        var result = new OrchestratorResponse<OrganisationUpdatedNextStepsViewModel>
        {
            Data = new OrganisationUpdatedNextStepsViewModel()
        };

        try
        {
            var request = new UpdateOrganisationDetailsCommand
            {
                AccountLegalEntityId = accountLegalEntityId,
                Name = organisationName,
                Address = organisationAddress,
                AccountId = accountId,
                UserId = userId
            };

            await _mediator.Send(request);
        }
        catch (Exception)
        {
            result.Data.ErrorMessage = "Failed to update the organisation's details.";
        }

        return result;
    }

    private static OrganisationUpdatesAvailable CheckForUpdate(string currentValue, string updatedValue, OrganisationUpdatesAvailable includeIfDifferent)
    {
        // The address will be stored with leading and trailing spaces removed, so the change comparison will exclude these.
        // Also, the names and addresses returned by CH search and get by id are inconsistent. Specifically the spacing within a 
        // name of address are different. To counter this one or spaces will be considered to be equivalent. 
        return !currentValue.IsEquivalent(updatedValue, StringEquivalenceOptions.IgnoreLeadingSpaces | StringEquivalenceOptions.IgnoreTrailingSpaces | StringEquivalenceOptions.MultipleSpacesAreEquivalent) 
            ? includeIfDifferent 
            : OrganisationUpdatesAvailable.None;
    }
}