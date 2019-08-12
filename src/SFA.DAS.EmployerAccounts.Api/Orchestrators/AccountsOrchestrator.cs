using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AccountsOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<PayeSchemeViewModel> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            _logger.Info($"Getting paye scheme {payeSchemeRef} for account {hashedAccountId}");

            var payeSchemeResult = await _mediator.SendAsync(new GetPayeSchemeByRefQuery { HashedAccountId = hashedAccountId, Ref = payeSchemeRef });
            if (payeSchemeResult.PayeScheme == null)
            {
                return null;
            }

            var viewModel = ConvertPayeSchemeToViewModel(hashedAccountId, payeSchemeResult);
            return viewModel;
        }

        private PayeSchemeViewModel ConvertPayeSchemeToViewModel(string hashedAccountId, GetPayeSchemeByRefResponse payeSchemeResult)
        {
            var payeSchemeViewModel = new PayeSchemeViewModel
            {
                DasAccountId = hashedAccountId,
                Name = payeSchemeResult.PayeScheme.Name,
                Ref = payeSchemeResult.PayeScheme.Ref,
                AddedDate = payeSchemeResult.PayeScheme.AddedDate,
                RemovedDate = payeSchemeResult.PayeScheme.RemovedDate
            };

            return payeSchemeViewModel;
        }
    }
}