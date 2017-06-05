using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.CreateAccount;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;
using SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetLatestAccountAgreementTemplate;
using SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAccountOrchestrator : EmployerVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IHashingService _hashingService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        //Needed for tests
        protected EmployerAccountOrchestrator()
        {
        }

        public EmployerAccountOrchestrator(IMediator mediator, ILogger logger, ICookieStorageService<EmployerAccountData> cookieService,
            EmployerApprenticeshipsServiceConfiguration configuration, IHashingService hashingService)
            : base(mediator, logger, cookieService, configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _hashingService = hashingService;
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
                Logger.Info(ex,"Create Account Validation Error");
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
            var enteredData = GetCookieData(context);

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
                Sector = enteredData.Sector
            };

            return new OrchestratorResponse<SummaryViewModel>
            {
                Data = model
            };

        }


        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            return CookieService.Get(CookieName);
            
        }

        public virtual void CreateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            CookieService.Create(data,CookieName, 365);
        }

        public void UpdateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            CookieService.Update(CookieName, data);
        }

        public async Task<OrchestratorResponse<EmployerAgreementViewModel>> GetAccountAgreementTemplate(CreateAccountViewModel viewModel)
        {
            var response = new OrchestratorResponse<EmployerAgreementViewModel>();

            var templateResponse = await Mediator.SendAsync(new GetLatestAccountAgreementTemplateRequest());

            response.Data = new EmployerAgreementViewModel
            {
                EmployerAgreement = new EmployerAgreementView
                {
                    LegalEntityName = viewModel.OrganisationName,
                    LegalEntityCode = viewModel.OrganisationReferenceNumber,
                    LegalEntityAddress = viewModel.OrganisationAddress,
                    LegalEntityInceptionDate = viewModel.OrganisationDateOfInception,
                    Status = EmployerAgreementStatus.Pending,
                    TemplatePartialViewName = templateResponse.Template.PartialViewName
                }
            };

            return response;
        }

        public virtual void DeleteCookieData(HttpContextBase context)
        {
            CookieService.Delete(CookieName);
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
                    NewName = String.Empty
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
                    NewName = (model.NewName ?? String.Empty).Trim()
                });

                model.CurrentName = model.NewName;
                model.NewName = String.Empty;
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

        public virtual async Task<OrchestratorResponse<NotificationSettingsViewModel>> GetNotificationSettingsViewModel(
            string hashedAccountId, string userRef)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var response = await _mediator.SendAsync(new GetUserNotificationSettingsQuery
            {
                AccountId = accountId,
                UserRef = userRef
            });

            return new OrchestratorResponse<NotificationSettingsViewModel>
            {
                Data = new NotificationSettingsViewModel
                {
                    HashedId = userRef,
                    NotificationSettings = response.NotificationSettings
                },
            };
        }

        public virtual async Task UpdateNotificationSettings(
            string hashedAccountId, string userRef, List<UserNotificationSetting> settings)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            await _mediator.SendAsync(new UpdateUserNotificationSettingsCommand
            {
                AccountId = accountId,
                UserRef = userRef,
                Settings = settings
            });          
        }
    }
}