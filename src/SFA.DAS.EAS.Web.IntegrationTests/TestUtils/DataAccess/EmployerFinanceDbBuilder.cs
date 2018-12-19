using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess
{
    internal class EmployerFinanceDbBuilder : IDbBuilder
    {
        private readonly EmployerFinanceDbContext _dbContext;

        public EmployerFinanceDbBuilder(
            DbBuilderDependentRepositories dependentRepositories,
            EmployerFinanceDbContext dbContext)
        {
            DependentRepositories = dependentRepositories;
            _dbContext = dbContext;
        }

        private DbBuilderDependentRepositories DependentRepositories { get; }
        private bool HasTransaction => _dbContext.Database.CurrentTransaction != null;

        public async Task<EmployerFinanceDbBuilder> SetupDataAsync(TestModelBuilder builder)
        {
            foreach (var payment in builder.Payments)
            {
                payment.PaymentOutput = await CreatePaymentAsync(payment.PaymentInput);
            }

            return this;
        }

        private async Task<Payment> CreatePaymentAsync(PaymentDetails paymentDetails)
        {
            await DependentRepositories.DasLevyRepository.CreatePayments(new List<PaymentDetails>
            {
                paymentDetails
            });
            return await DependentRepositories.DasLevyRepository.GetPaymentData(paymentDetails.Id);
        }

        public void BeginTransaction()
        {
            if (HasTransaction)
            {
                throw new InvalidOperationException("Cannot begin a transaction because a transaction has already been started");
            }

            _dbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (!HasTransaction)
            {
                throw new InvalidOperationException("Cannot commit a transaction because a transaction has not been started");
            }

            _dbContext.Database.CurrentTransaction.Commit();
        }

        public void RollbackTransaction()
        {
            if (!HasTransaction)
            {
                throw new InvalidOperationException("Cannot rollback a transaction because a transaction has not been started");
            }

            _dbContext.Database.CurrentTransaction.Rollback();
        }
    }
}