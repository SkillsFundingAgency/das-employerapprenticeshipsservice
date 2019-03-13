using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;
using SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiy;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;
using SFA.DAS.EmployerAccounts.Queries.GetPostcodeAddress;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Web.Validation;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Extensions;
using SFA.DAS.Hashing;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class OrganisationOrchestrator : UserVerificationOrchestratorBase, IOrchestratorCookie
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityHashingService;

        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        public OrganisationOrchestrator(
            IMediator mediator,
            ILog logger,
            IMapper mapper,
            ICookieStorageService<EmployerAccountData> cookieService,
            IAccountLegalEntityPublicHashingService accountLegalEntityHashingService)
            : base(mediator)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _cookieService = cookieService;
            _accountLegalEntityHashingService = accountLegalEntityHashingService;
        }


        protected OrganisationOrchestrator()
        {
            
        }
        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(CreateNewLegalEntityViewModel request)
        {

            var result = await _mediator.SendAsync(new CreateLegalEntityCommand
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

        public virtual OrchestratorResponse<OrganisationDetailsViewModel> GetAddOtherOrganisationViewModel(string hashedAccountId)
        {
            var response = new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = hashedAccountId
                }
            };

            return response;
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

                var response = _mediator.Send(request);

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

        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            return _cookieService.Get(CookieName);
        }

        public virtual void CreateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            _cookieService.Create(data, CookieName, 365);
        }

        public virtual async Task<OrchestratorResponse<SelectOrganisationAddressViewModel>> GetAddressesFromPostcode(
            FindOrganisationAddressViewModel request)
        {
            var viewModel = new SelectOrganisationAddressViewModel
            {
                Postcode = request.Postcode,
                OrganisationName = request.OrganisationName,
                OrganisationDateOfInception = request.OrganisationDateOfInception,
                OrganisationHashedId = request.OrganisationHashedId,
                OrganisationReferenceNumber = request.OrganisationReferenceNumber,
                OrganisationStatus = request.OrganisationStatus,
                OrganisationType = request.OrganisationType,
                PublicSectorDataSource = request.PublicSectorDataSource,
                Sector = request.Sector
            };

            try
            {
                var addresses = await _mediator.SendAsync(new GetPostcodeAddressRequest { Postcode = request.Postcode });

                viewModel.Addresses = addresses?.Addresses?.Select(x => _mapper.Map<AddressViewModel>(x)).ToList();

                return new OrchestratorResponse<SelectOrganisationAddressViewModel>
                {
                    Data = viewModel,
                    Status = HttpStatusCode.OK
                };
            }
            catch (InvalidRequestException e)
            {
                viewModel.ErrorDictionary = e.ErrorMessages;

                return new OrchestratorResponse<SelectOrganisationAddressViewModel>
                {
                    Data = viewModel,
                    Status = HttpStatusCode.BadRequest,
                    Exception = e
                };
            }
        }

        public OrchestratorResponse<OrganisationDetailsViewModel> StartConfirmOrganisationDetails(AddOrganisationAddressViewModel request)
        {
            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = request.OrganisationHashedId,
                    Name = request.OrganisationName,
                    Address = request.OrganisationAddress,
                    DateOfInception = request.OrganisationDateOfInception,
                    ReferenceNumber = request.OrganisationReferenceNumber ?? string.Empty,
                    Type = request.OrganisationType,
                    PublicSectorDataSource = request.PublicSectorDataSource,
                    Status = request.OrganisationStatus,
                    Sector = request.Sector
                }
            };
        }

        public OrchestratorResponse<OrganisationDetailsViewModel> StartConfirmOrganisationDetails(FindOrganisationAddressViewModel request)
        {
            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    HashedId = request.OrganisationHashedId,
                    Name = request.OrganisationName,
                    Address = request.OrganisationAddress,
                    DateOfInception = request.OrganisationDateOfInception,
                    ReferenceNumber = request.OrganisationReferenceNumber ?? string.Empty,
                    Type = request.OrganisationType,
                    PublicSectorDataSource = request.PublicSectorDataSource,
                    Status = request.OrganisationStatus,
                    Sector = request.Sector
                }
            };
        }

        public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
        {
            var userResponse = await Mediator.SendAsync(new GetTeamMemberQuery { HashedAccountId = hashedAccountId, TeamMemberId = userId });
            return userResponse.User.ShowWizard && userResponse.User.Role == Role.Owner;
        }

        public virtual Task<OrchestratorResponse<OrganisationAddedNextStepsViewModel>> GetOrganisationAddedNextStepViewModel(
            string organisationName,
            string userId,
            string hashedAccountId)
        {
            return this.GetOrganisationAddedNextStepViewModel(organisationName, userId, hashedAccountId, string.Empty);
        }

        public virtual async Task<OrchestratorResponse<OrganisationAddedNextStepsViewModel>> GetOrganisationAddedNextStepViewModel(
            string organisationName,
            string userId,
            string hashedAccountId,
            string hashedAgreementId)
        {
            var showWizard = await UserShownWizard(userId, hashedAccountId);

            return new OrchestratorResponse<OrganisationAddedNextStepsViewModel>
            {
                Data = new OrganisationAddedNextStepsViewModel { OrganisationName = organisationName, ShowWizard = showWizard, HashedAgreementId = hashedAgreementId }
            };
        }



        public async Task<OrchestratorResponse<ReviewOrganisationAddressViewModel>> GetRefreshedOrganisationDetails(string accountLegalEntityPublicHashedId)
        {
            var currentDetails = await Mediator.SendAsync(new GetAccountLegalEntityRequest
            {
                AccountLegalEntityId = _accountLegalEntityHashingService.DecodeValue(accountLegalEntityPublicHashedId)
            });

            var refreshedDetails = await Mediator.SendAsync(new GetOrganisationByIdRequest
            {
                Identifier = currentDetails.AccountLegalEntity.Identifier,
                OrganisationType = currentDetails.AccountLegalEntity.OrganisationType
            });

            OrganisationUpdatesAvailable CheckForUpdate(string currentValue, string updatedValue, OrganisationUpdatesAvailable includeIfDifferent)
            {
                // The address will be stored with leading and trailing spaces removed, so the change comparison will exclude these.
                // Also, the names and addresses returned by CH search and get by id are inconsistent. Specifically the spacing within a 
                // name of address are different. To counter this one or spaces will be considered to be equivalent. 
                if (!currentValue.IsEquivalent(updatedValue, StringEquivalenceOptions.IgnoreLeadingSpaces | StringEquivalenceOptions.IgnoreTrailingSpaces | StringEquivalenceOptions.MultipleSpacesAreEquivalent))
                {
                    return includeIfDifferent;
                }

                return OrganisationUpdatesAvailable.None;
            }

            var result = new OrchestratorResponse<ReviewOrganisationAddressViewModel>
            {
                Data = new ReviewOrganisationAddressViewModel
                {
                    DataSourceFriendlyName = currentDetails.AccountLegalEntity.OrganisationType.GetFriendlyName(),
                    AccountLegalEntityPublicHashedId = accountLegalEntityPublicHashedId,
                    OrganisationName = currentDetails.AccountLegalEntity.Name,
                    OrganisationAddress = currentDetails.AccountLegalEntity.Address,
                    RefreshedName = refreshedDetails.Organisation.Name,
                    RefreshedAddress = refreshedDetails.Organisation.Address.FormatAddress(),
                }
            };

            result.Data.UpdatesAvailable = CheckForUpdate(result.Data.OrganisationName, result.Data.RefreshedName, OrganisationUpdatesAvailable.Name) |
                                           CheckForUpdate(result.Data.OrganisationAddress, result.Data.RefreshedAddress, OrganisationUpdatesAvailable.Address);

            return result;
        }



        public async Task<OrchestratorResponse<OrganisationUpdatedNextStepsViewModel>> UpdateOrganisation(
            string accountLegalEntityPublicHashedId, string organisationName, string organisationAddress, string hashedAccountId, string userId)
        {
            var result = new OrchestratorResponse<OrganisationUpdatedNextStepsViewModel>
            {
                Data = new OrganisationUpdatedNextStepsViewModel()
            };

            try
            {
                var request = new UpdateOrganisationDetailsCommand
                {
                    AccountLegalEntityId = _accountLegalEntityHashingService.DecodeValue(accountLegalEntityPublicHashedId),
                    Name = organisationName,
                    Address = organisationAddress,
                    HashedAccountId = hashedAccountId,
                    UserId = userId
                };

                await _mediator.SendAsync(request);
            }
            catch (Exception)
            {
                result.Data.ErrorMessage = "Failed to update the organisation's details.";
            }

            return result;
        }
    }
}