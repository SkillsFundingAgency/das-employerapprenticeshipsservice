using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class EmployerAccountOrchestrator : EmployerVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IHashingService _hashingService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        //Needed for tests
        protected EmployerAccountOrchestrator()
        {
        }

        public EmployerAccountOrchestrator(IMediator mediator, ILog logger, ICookieStorageService<EmployerAccountData> cookieService,
            EmployerAccountsConfiguration configuration, IHashingService hashingService)
            : base(mediator, logger, cookieService, configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _hashingService = hashingService;
        }


        public async Task<OrchestratorResponse<EmployerAccountViewModel>> GetEmployerAccount(string hashedAccountId)
        {
            var response = await Mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedAccountId
            });

            return new OrchestratorResponse<EmployerAccountViewModel>
            {
                Data = new EmployerAccountViewModel
                {
                    HashedId = hashedAccountId,
                    Name = response.Account.Name
                }
            };
        }
        
        public virtual async Task<OrchestratorResponse<RenameEmployerAccountViewModel>> GetRenameEmployerAccountViewModel(string hashedAccountId, string userId)
        {
            var response = await Mediator.SendAsync(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = hashedAccountId,
                UserId = userId
            });

            return new OrchestratorResponse<RenameEmployerAccountViewModel>
            {
                Data = new RenameEmployerAccountViewModel
                {
                    HashedId = hashedAccountId,
                    CurrentName = response.Account.Name,
                    NewName = string.Empty
                }
            };
        }

        public virtual async Task<OrchestratorResponse<RenameEmployerAccountViewModel>> RenameEmployerAccount(RenameEmployerAccountViewModel model, string userId)
        {
            var response = new OrchestratorResponse<RenameEmployerAccountViewModel> { Data = model };

            var userRoleResponse = await GetUserAccountRole(model.HashedId, userId);

            if (!userRoleResponse.UserRole.Equals(Role.Owner))
            {
                return new OrchestratorResponse<RenameEmployerAccountViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }

            try
            {
                await _mediator.SendAsync(new RenameEmployerAccountCommand
                {
                    HashedAccountId = model.HashedId,
                    ExternalUserId = userId,
                    NewName = (model.NewName ?? string.Empty).Trim()
                });

                model.CurrentName = model.NewName;
                model.NewName = string.Empty;
                response.Data = model;
                response.Status = HttpStatusCode.OK;
            }
            catch (InvalidRequestException ex)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.Data.ErrorDictionary = ex.ErrorMessages;
                response.Exception = ex;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
            }

            return response;
        }


        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateAccount(CreateAccountViewModel viewModel, HttpContextBase context)
        {
            try
            {
                var result = await Mediator.SendAsync(new CreateAccountCommand
                {
                    ExternalUserId = viewModel.UserId,
                    OrganisationType = viewModel.OrganisationType,
                    OrganisationName = viewModel.OrganisationName,
                    OrganisationReferenceNumber = viewModel.OrganisationReferenceNumber,
                    OrganisationAddress = viewModel.OrganisationAddress,
                    OrganisationDateOfInception = viewModel.OrganisationDateOfInception,
                    PayeReference = viewModel.PayeReference,
                    AccessToken = viewModel.AccessToken,
                    RefreshToken = viewModel.RefreshToken,
                    OrganisationStatus = viewModel.OrganisationStatus,
                    EmployerRefName = viewModel.EmployerRefName,
                    PublicSectorDataSource = viewModel.PublicSectorDataSource,
                    Sector = viewModel.Sector
                });

                CookieService.Delete(CookieName);

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            HashedAccountId = result.HashedAccountId
                        }
                    },
                    Status = HttpStatusCode.OK
                };
            }
            catch (InvalidRequestException ex)
            {
                Logger.Info($"Create Account Validation Error: {ex.Message}");
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel(),
                    Status = HttpStatusCode.BadRequest,
                    Exception = ex,
                    FlashMessage = new FlashMessageViewModel()
                };
            }

        }


        public virtual OrchestratorResponse<SummaryViewModel> GetSummaryViewModel(HttpContextBase context)
        {
            var enteredData = GetCookieData();

            var model = new SummaryViewModel
            {
                OrganisationType = enteredData.OrganisationType,
                OrganisationName = enteredData.OrganisationName,
                RegisteredAddress = enteredData.OrganisationRegisteredAddress,
                OrganisationReferenceNumber = enteredData.OrganisationReferenceNumber,
                OrganisationDateOfInception = enteredData.OrganisationDateOfInception,
                PayeReference = enteredData.PayeReference,
                EmployerRefName = enteredData.EmployerRefName,
                EmpRefNotFound = enteredData.EmpRefNotFound,
                OrganisationStatus = enteredData.OrganisationStatus,
                PublicSectorDataSource = enteredData.PublicSectorDataSource,
                Sector = enteredData.Sector,
                NewSearch = enteredData.NewSearch
            };

            return new OrchestratorResponse<SummaryViewModel>
            {
                Data = model
            };

        }


        public virtual EmployerAccountData GetCookieData()
        {
            return CookieService.Get(CookieName);

        }

        public virtual void CreateCookieData(EmployerAccountData data)
        {
            CookieService.Create(data, CookieName, 365);
        }

        public void UpdateCookieData(EmployerAccountData data)
        {
            CookieService.Update(CookieName, data);
        }
        
        public virtual void DeleteCookieData()
        {
            CookieService.Delete(CookieName);
        }

    }
}