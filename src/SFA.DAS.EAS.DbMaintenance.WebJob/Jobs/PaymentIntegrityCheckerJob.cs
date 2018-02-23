using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Jobs
{
    public class PaymentIntegrityCheckerJob : IJob
    {
        private readonly IPaymentService _paymentService;
        private readonly IDasLevyRepository _levyRepository;

        public PaymentIntegrityCheckerJob(IPaymentService paymentService, IDasLevyRepository levyRepository)
        {
            _paymentService = paymentService;
            _levyRepository = levyRepository;
        }

        public Task Run()
        {
            throw new NotImplementedException();

            //Get all account Ids

            //Get all period ends


            //for each preiod end

            //for each account id

            //Get payments from payment team
            //payments = await _paymentService.GetAccountPayments(message.PeriodEnd, message.AccountId);

            //Get payments from database

            //Compare and see if any are missing

            //If payment is missing then report.
        }
    }
}
