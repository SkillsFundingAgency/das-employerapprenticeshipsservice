using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.Jobs
{
    public class PaymentIntegrityCheckerJob : IJob
    {
        private readonly IPaymentService _paymentService;
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IDasLevyRepository _levyRepository;
        private readonly ILog _logger;

        public PaymentIntegrityCheckerJob(
            IPaymentService paymentService,
            IEmployerAccountRepository employerAccountRepository,
            IDasLevyRepository levyRepository,
            ILog logger)
        {
            _paymentService = paymentService;
            _employerAccountRepository = employerAccountRepository;
            _levyRepository = levyRepository;
            _logger = logger;
        }

        public async Task Run()
        {
            var accounts = (await _employerAccountRepository.GetAllAccounts()).ToArray();

            _logger.Info($"Found {accounts.Length} to check payments for");

            var periodEnds = (await _levyRepository.GetAllPeriodEnds()).ToArray();

            _logger.Info($"Found {periodEnds.Length} period ends to process payments for");


            for (var periodEndIndex = 0; periodEndIndex < periodEnds.Length; periodEndIndex++)
            {
                var periodEnd = periodEnds[periodEndIndex];

                _logger.Info($"Processing payments for period end {periodEnd.Id} ({periodEndIndex + 1}/{periodEnds.Length})");

                for (var accountIndex = 0; accountIndex < accounts.Length; accountIndex++)
                {
                    var account = accounts[accountIndex];

                    try
                    {
                        _logger.Info($"Processing payments for account {account.Id} ({accountIndex + 1}/{accounts.Length})");

                    var expectedPayments = await _paymentService.GetAccountPayments(periodEnd.PeriodEndId, account.Id);

                        var actualPayments =
                        (await _levyRepository.GetAccountPaymentsByPeriodEnd(account.Id, periodEnd.PeriodEndId))
                            .ToArray();

                        var expectedPaymentsTotal = expectedPayments.Sum(x => x.Amount);
                        var actualPaymentsTotal = actualPayments.Sum(x => x.Amount);

                        var paymentMissing = actualPaymentsTotal != expectedPaymentsTotal;

                        //If the totals add up we check to see if all the payments are correct
                        //just in case a duplicate or incorrect record has the same amount as a correct one
                        if (!paymentMissing)
                        {
                            paymentMissing = expectedPayments.Except(actualPayments).Any();
                        }

                        if (paymentMissing)
                        {
                            _logger.Warn(
                            $"Some payments for account ID {account.Id} for period end {periodEnd.PeriodEndId} are missing");
                        }
                        else
                        {
                            _logger.Info($"All payments for account ID {account.Id} for period end {periodEnd.Id} are correct");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Failed to process payments for account Id: {account.Id} and Period end: {periodEnd.Id}");
                    }
                }
            }
        }
    }
}
