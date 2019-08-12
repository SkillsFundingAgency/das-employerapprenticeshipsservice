using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class UsersOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;

        public UsersOrchestrator(IMediator mediator, ILog logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        //public async Task<OrchestratorResponse<ICollection<AccountDetailViewModel>>> GetUserAccounts(string userRef)
        //{
        //    _logger.Info($"Requesting user's accounts for user Ref  {userRef}");

        //    var accounts = await _mediator.SendAsync(new GetUserAccountsQuery { UserRef = userRef });

        //    var viewModels = accounts.Accounts.AccountList.Select(x => _mapper.Map<AccountDetailViewModel>(x)).ToList();

        //    return new OrchestratorResponse<ICollection<AccountDetailViewModel>>
        //    {
        //        Data = viewModels,
        //        Status = HttpStatusCode.OK
        //    };
        //}

        public async Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userRef)
        {
            _logger.Info($"Requesting user's accounts for user Ref  {userRef}");

            var accounts = await _mediator.SendAsync(new GetUserAccountsQuery { UserRef = userRef });

            //todo: doesn't map many fields, such as ApprenticeshipEmployerType!
            // detangled api returns...
            // [{"AccountId":4,"HashedAccountId":"G6M7RV","PublicHashedAccountId":"XWGMWV","DasAccountName":"PINK MOOMIN LTD","DateRegistered":"0001-01-01T00:00:00","OwnerEmail":null,"LegalEntities":null,"PayeSchemes":null,"Balance":0.0,"TransferAllowance":0.0,"RemainingTransferAllowance":0.0,"StartingTransferAllowance":0.0,"DasAccountId":"G6M7RV","AccountAgreementType":0,"ApprenticeshipEmployerType":"0"}]
            // check original
            var viewModels = accounts.Accounts.AccountList.Select(x => _mapper.Map<AccountDetailViewModel>(x)).ToList();

            return viewModels;
        }
    }
}