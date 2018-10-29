using AutoMapper;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Application.Commands.CreateLegalEntity;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetPostcodeAddress;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Validation;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.Hashing;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

            if (accountLegalEntitiesHelper.IsLegalEntityAlreadyAddedToAccount(accountEntities, null, legalEntityCode, organisationType))
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

    }
}