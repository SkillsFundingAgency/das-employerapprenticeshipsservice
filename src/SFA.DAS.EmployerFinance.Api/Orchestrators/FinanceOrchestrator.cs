using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Finance.Api.Types;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.EmployerFinance.Api.Orchestrators
{
    public class FinanceOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;
        //private readonly IEmployerAccountsApiService _employerAccountsApiService; //TODO : replace with IEmployerFinanceApiService

        public FinanceOrchestrator(
            IMediator mediator,
            ILog logger,
            IMapper mapper,
            IHashingService hashingService
            //IEmployerAccountsApiService employerAccountsApiService
            )
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _hashingService = hashingService;
            //_employerAccountsApiService = employerAccountsApiService;
        }

        public async Task<OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>> GetLevy(string hashedAccountId)
        {
            _logger.Info($"Requesting levy declaration for account {hashedAccountId}");

            var levyDeclarations = await _mediator.SendAsync(new GetLevyDeclarationRequest { HashedAccountId = hashedAccountId });
            if (levyDeclarations.Declarations == null)
            {
                return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>> { Data = null };
            }

            var levyViewModels = levyDeclarations.Declarations.Select(x => _mapper.Map<LevyDeclarationViewModel>(x)).ToList();
            levyViewModels.ForEach(x => x.HashedAccountId = hashedAccountId);

            return new OrchestratorResponse<AccountResourceList<LevyDeclarationViewModel>>
            {
                Data = new AccountResourceList<LevyDeclarationViewModel>(levyViewModels),
                Status = HttpStatusCode.OK
            };
        }
    }
}