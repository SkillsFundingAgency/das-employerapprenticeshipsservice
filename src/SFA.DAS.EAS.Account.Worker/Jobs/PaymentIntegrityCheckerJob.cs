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
            var accounts = await _employerAccountRepository.GetAllAccounts();

            var periodEnds = await _levyRepository.GetAllPeriodEnds();

            foreach (var periodEnd in periodEnds)
            {
                foreach (var account in accounts)
                {
                    var expectedPayments = await _paymentService.GetAccountPayments(periodEnd.Id, account.Id);

                    var actualPayments =
                        (await _levyRepository.GetAccountPaymentsByPeriodEnd(account.Id, periodEnd.Id))
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
                            $"Some payments for account ID {account.Id} for period end {periodEnd.Id} are missing");
                    }
                    else
                    {
                        _logger.Info($"All payments for account ID {account.Id} for period end {periodEnd.Id} are correct");
                    }
                }
            }
        }
    }
}
