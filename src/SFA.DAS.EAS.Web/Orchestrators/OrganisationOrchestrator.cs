using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;
using SFA.DAS.EAS.Application.Queries.GetCharity;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetPostcodeAddress;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;
using SFA.DAS.EAS.Application.Queries.GetTeamUser;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Commands.UpdateOrganisationDetails;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Extensions;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntitiy;
using SFA.DAS.EAS.Application.Queries.GetLegalEntity;
using SFA.DAS.EAS.Application.Queries.GetOrganisationById;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EAS.Infrastructure.Hashing;
using SFA.DAS.EAS.Web.Validation;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class OrganisationOrchestrator : UserVerificationOrchestratorBase, IOrchestratorCookie
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;
        private readonly IAccountLegalEntityPublicHashingService _accountLegalEntityHashingService;

        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        protected OrganisationOrchestrator()
        {
        }

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

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateLegalEntity(
            CreateNewLegalEntityViewModel request)
        {
            var createLegalEntityResponse = await _mediator.SendAsync(new CreateLegalEntityCommand
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
                    EmployerAgreement = createLegalEntityResponse.AgreementView
                },
                Status = HttpStatusCode.OK
            };
        }

        public virtual OrchestratorResponse<OrganisationDetailsViewModel> GetAddOtherOrganisationViewModel(
            string hashedAccountId)
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

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> ValidateLegalEntityName(
            OrganisationDetailsViewModel request)
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

        public virtual OrchestratorResponse<OrganisationDetailsViewModel> AddOrganisationAddress(
            AddOrganisationAddressViewModel viewModel)
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
            _cookieService.Create(data,CookieName, 365);
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
                var addresses = await _mediator.SendAsync(new GetPostcodeAddressRequest {Postcode = request.Postcode});

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

        private static string BuildAddressString(GetEmployerInformationResponse response)
        {
            var addressBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(response.AddressLine1))
            {
                addressBuilder.Append($"{response.AddressLine1}, ");
            }

            if (!string.IsNullOrEmpty(response.AddressLine2))
            {
                addressBuilder.Append($"{response.AddressLine2}, ");
            }

            if (!string.IsNullOrEmpty(response.TownOrCity))
            {
                addressBuilder.Append($"{response.TownOrCity}, ");
            }

            if (!string.IsNullOrEmpty(response.County))
            {
                addressBuilder.Append(string.IsNullOrEmpty(response.AddressPostcode)
                    ? $"{response.County}"
                    : $"{response.County}, ");
            }

            if (!string.IsNullOrEmpty(response.AddressPostcode))
            {
                addressBuilder.Append(response.AddressPostcode);
            }

            return addressBuilder.ToString();
        }

        public async Task<OrchestratorResponse<OrganisationDetailsViewModel>> CheckLegalEntityIsNotAddedToAccount(string hashedLegalEntityId, string userIdClaim, string legalEntityCode, OrganisationType organisationType)
        {
            var accountLegalEntitiesHelper = new AccountLegalEntitiesHelper(_mediator);
            var accountEntities = await accountLegalEntitiesHelper.GetAccountLegalEntities(hashedLegalEntityId, userIdClaim);

            if(accountLegalEntitiesHelper.IsLegalEntityAlreadyAddedToAccount(accountEntities, null, legalEntityCode, organisationType))
            {
                var conflictResponse = new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Data = new OrganisationDetailsViewModel(),
                    Status = HttpStatusCode.Conflict
                };
                
                return conflictResponse;
            }

            return null;
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
                Data = new OrganisationAddedNextStepsViewModel { OrganisationName = organisationName, ShowWizard = showWizard, HashedAgreementId = hashedAgreementId}
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
                if (!string.Equals(currentValue?.Trim(), updatedValue?.Trim()))
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

        public virtual async Task<bool> UserShownWizard(string userId, string hashedAccountId)
        {
            var userResponse = await Mediator.SendAsync(new GetTeamMemberQuery { HashedAccountId = hashedAccountId, TeamMemberId = userId });
            return userResponse.User.ShowWizard && userResponse.User.RoleId == (short)Role.Owner;
        }

        public async Task<OrchestratorResponse<OrganisationUpdatedNextStepsViewModel>> UpdateOrganisation(string accountLegalEntityPublicHashedId, string organisationName, string organisationAddress)
        {
            var result = new OrchestratorResponse<OrganisationUpdatedNextStepsViewModel>
            {
                Data = new OrganisationUpdatedNextStepsViewModel()
            };

            try
            {
                var request = new UpdateOrganisationDetailsRequest
                {
                    AccountLegalEntityId = _accountLegalEntityHashingService.DecodeValue(accountLegalEntityPublicHashedId),
                    Name = organisationName,
                    Address = organisationAddress
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