using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Jobs
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

            var lastestPeriodEnd = await _levyRepository.GetLatestPeriodEnd();

            foreach (var account in accounts)
            {
                var expectedPayments = (IEnumerable<Payment>)await _paymentService.GetAccountPayments(lastestPeriodEnd.Id, account.Id);

                var actualPayments =
                    (await _levyRepository.GetAccountPaymentsByPeriodEnd(account.Id, lastestPeriodEnd.Id)).ToArray();


                var expectedPaymentsTotal = expectedPayments?.Sum(x => x.Amount) ?? 0;
                var actualPaymentsTotal = actualPayments.Sum(x => x.Amount);

                var paymentMissing = actualPaymentsTotal != expectedPaymentsTotal;

                if (!paymentMissing)
                {
                    paymentMissing = expectedPayments?.Except(actualPayments).Any() ?? false;
                }

                if (paymentMissing)
                {
                    _logger.Warn(
                        $"Some payments for account ID {account.Id} for period end {lastestPeriodEnd.Id} are missing");
                }
            }
        }
    }
}
