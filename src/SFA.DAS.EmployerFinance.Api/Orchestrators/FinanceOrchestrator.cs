using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Api.Types;
using SFA.DAS.EmployerFinance.Queries.GetAccountBalances;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclarationsByAccountAndPeriod;
using SFA.DAS.EmployerFinance.Queries.GetTransferAllowance;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.Orchestrators
{
    public class FinanceOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;

        public FinanceOrchestrator(
            IMediator mediator,
            ILog logger,
            IMapper mapper,
            IHashingService hashingService)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _hashingService = hashingService;
        }

        public async Task<List<LevyDeclaration>> GetLevy(string hashedAccountId)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}");

            var response = await _mediator.SendAsync(new GetLevyDeclarationRequest { HashedAccountId = hashedAccountId });
            if (response.Declarations == null)
            {
                return null;
            }

            var levyDeclarations = response.Declarations.Select(x => _mapper.Map<LevyDeclaration>(x)).ToList();
            levyDeclarations.ForEach(x => x.HashedAccountId = hashedAccountId);

            return levyDeclarations;
        }

        public async Task<List<LevyDeclaration>> GetLevy(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}, year {payrollYear} and month {payrollMonth}");

            var response = await _mediator.SendAsync(new GetLevyDeclarationsByAccountAndPeriodRequest { HashedAccountId = hashedAccountId, PayrollYear = payrollYear, PayrollMonth = payrollMonth });
            if (response.Declarations == null)
            {
                return null;
            }

            var levyDeclarations = response.Declarations.Select(x => _mapper.Map<LevyDeclaration>(x)).ToList();
            levyDeclarations.ForEach(x => x.HashedAccountId = hashedAccountId);

            return levyDeclarations;
        }

        public async Task<GetAccountBalancesResponse> GetAccountBalances(List<long> accountIds)
        {
            _logger.Info($"Requesting GetAccountBalances for the accounts");

            var transactionResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {   
                AccountIds = accountIds
            });

            return transactionResult;
        }

        public async Task<GetAccountBalancesResponse> GetAccountBalances(List<string> accountIds)
        {
            _logger.Info($"Requesting GetAccountBalances for the accounts");
          
            var transactionResult = await _mediator.SendAsync(new GetAccountBalancesRequest
            {
                AccountIds = accountIds.Select(x => _hashingService.DecodeValue(x)).ToList()
            });

            return transactionResult;
        }

        public async Task<GetTransferAllowanceResponse> GetTransferAllowance(string hashedAccountId)
        {
            _logger.Info($"Requesting GetTransferAllowance for the hashedAccountId {hashedAccountId} ");

            var transferAllowance = await _mediator.SendAsync(new GetTransferAllowanceQuery
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId)
            });

            return transferAllowance;
        }
    }
}