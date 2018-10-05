using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System.Net;
using System.Threading.Tasks;

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
    }
}