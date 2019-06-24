﻿using System;
using System.Linq;
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
using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Web.Models;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class EmployerAccountOrchestrator : EmployerVerificationOrchestratorBase
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        //Needed for tests
        protected EmployerAccountOrchestrator()
        {
        }

        public EmployerAccountOrchestrator(IMediator mediator, ILog logger, ICookieStorageService<EmployerAccountData> cookieService,
            EmployerAccountsConfiguration configuration)
            : base(mediator, cookieService, configuration)
        {
            _mediator = mediator;
            _logger = logger;
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

        public virtual async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateOrUpdateAccount(CreateAccountModel model, HttpContextBase context)
        {
            if (string.IsNullOrWhiteSpace(model?.HashedAccountId?.Value))
            {
                return await CreateNewAccount(model);
            }

            return await UpdateExistingAccount(model);
        }

        private async Task<OrchestratorResponse<EmployerAgreementViewModel>> UpdateExistingAccount(CreateAccountModel model)
        {
            try
            {
                await AddPayeToExistingAccount(model);

                await AddLegalEntityToExistingAccount(model);

                await UpdateAccountNameToLegalEntityName(model);

                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel
                    {
                        EmployerAgreement = new EmployerAgreementView
                        {
                            HashedAccountId = model.HashedAccountId.Value
                        }
                    },
                    Status = HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                _logger.Info($"Create Account Validation Error: {e.Message}");
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel(),
                    Status = HttpStatusCode.BadRequest,
                    Exception = e,
                    FlashMessage = new FlashMessageViewModel()
                };
            }
        }

        private async Task UpdateAccountNameToLegalEntityName(CreateAccountModel model)
        {
            await _mediator.SendAsync(new RenameEmployerAccountCommand
            {
                HashedAccountId = model.HashedAccountId.Value,
                ExternalUserId = model.UserId,
                NewName = model.OrganisationName
            });
        }

        private async Task AddLegalEntityToExistingAccount(CreateAccountModel model)
        {
            await Mediator.SendAsync(new CreateLegalEntityCommand
            {
                HashedAccountId = model.HashedAccountId.Value,
                Code = model.OrganisationReferenceNumber,
                DateOfIncorporation = model.OrganisationDateOfInception,
                Status = model.OrganisationStatus,
                Source = model.OrganisationType,
                PublicSectorDataSource = Convert.ToByte(model.PublicSectorDataSource),
                Sector = model.Sector,
                Name = model.OrganisationName,
                Address = model.OrganisationAddress,
                ExternalUserId = model.UserId
            });
        }

        private async Task AddPayeToExistingAccount(CreateAccountModel model)
        {
            await Mediator.SendAsync(new AddPayeToAccountCommand
            {
                HashedAccountId = model.HashedAccountId.Value,
                AccessToken = model.AccessToken,
                RefreshToken = model.RefreshToken,
                Empref = model.PayeReference,
                ExternalUserId = model.UserId,
                EmprefName = model.EmployerRefName
            });
        }

        private async Task<OrchestratorResponse<EmployerAgreementViewModel>> CreateNewAccount(CreateAccountModel model)
        {
            try
            {
                var result = await Mediator.SendAsync(new CreateAccountCommand
                {
                    ExternalUserId = model.UserId,
                    OrganisationType = model.OrganisationType,
                    OrganisationName = model.OrganisationName,
                    OrganisationReferenceNumber = model.OrganisationReferenceNumber,
                    OrganisationAddress = model.OrganisationAddress,
                    OrganisationDateOfInception = model.OrganisationDateOfInception,
                    PayeReference = model.PayeReference,
                    AccessToken = model.AccessToken,
                    RefreshToken = model.RefreshToken,
                    OrganisationStatus = model.OrganisationStatus,
                    EmployerRefName = model.EmployerRefName,
                    PublicSectorDataSource = model.PublicSectorDataSource,
                    Sector = model.Sector
                });

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
                _logger.Info($"Create Account Validation Error: {ex.Message}");
                return new OrchestratorResponse<EmployerAgreementViewModel>
                {
                    Data = new EmployerAgreementViewModel(),
                    Status = HttpStatusCode.BadRequest,
                    Exception = ex,
                    FlashMessage = new FlashMessageViewModel()
                };
            }
        }

        public virtual async Task<OrchestratorResponse<EmployerAccountViewModel>> CreateMinimalUserAccountForSkipJourney(CreateUserAccountViewModel viewModel, HttpContextBase context)
        {
            try
            {
                var existingUserAccounts =
                    await Mediator.SendAsync(new GetUserAccountsQuery { UserRef = viewModel.UserId });

                if (existingUserAccounts?.Accounts?.AccountList?.Any() == true)
                    return new OrchestratorResponse<EmployerAccountViewModel>
                    {
                        Data = new EmployerAccountViewModel
                        {
                            HashedId = existingUserAccounts.Accounts.AccountList.First().HashedId
                        },
                        Status = HttpStatusCode.OK
                    };

                var result = await Mediator.SendAsync(new CreateUserAccountCommand
                {
                    ExternalUserId = viewModel.UserId,
                    OrganisationName = viewModel.OrganisationName
                });

                return new OrchestratorResponse<EmployerAccountViewModel>
                {
                    Data = new EmployerAccountViewModel
                    {
                        HashedId = result.HashedAccountId
                    },
                    Status = HttpStatusCode.OK
                };
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info($"Create User Account Validation Error: {ex.Message}");
                return new OrchestratorResponse<EmployerAccountViewModel>
                {
                    Data = new EmployerAccountViewModel(),
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
                OrganisationType = enteredData.EmployerAccountOrganisationData.OrganisationType,
                OrganisationName = enteredData.EmployerAccountOrganisationData.OrganisationName,
                RegisteredAddress = enteredData.EmployerAccountOrganisationData.OrganisationRegisteredAddress?.Split(','),
                OrganisationReferenceNumber = enteredData.EmployerAccountOrganisationData.OrganisationReferenceNumber,
                OrganisationDateOfInception = enteredData.EmployerAccountOrganisationData.OrganisationDateOfInception,
                PayeReference = enteredData.EmployerAccountPayeRefData.PayeReference,
                EmployerRefName = enteredData.EmployerAccountPayeRefData.EmployerRefName,
                EmpRefNotFound = enteredData.EmployerAccountPayeRefData.EmpRefNotFound,
                OrganisationStatus = enteredData.EmployerAccountOrganisationData.OrganisationStatus,
                PublicSectorDataSource = enteredData.EmployerAccountOrganisationData.PublicSectorDataSource,
                Sector = enteredData.EmployerAccountOrganisationData.Sector,
                NewSearch = enteredData.EmployerAccountOrganisationData.NewSearch,
                AORN = enteredData.EmployerAccountPayeRefData.AORN
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

        public virtual void DeleteCookieData()
        {
            CookieService.Delete(CookieName);
        }
    }
}