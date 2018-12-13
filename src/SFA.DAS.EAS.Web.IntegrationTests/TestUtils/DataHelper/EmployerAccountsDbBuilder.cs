using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Adapters;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper.Dtos;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper
{
    class EmployerAccountsDbBuilder : IDbBuilder
    {
        private readonly IHashingService _hashingService;
        private readonly IPublicHashingService _publicHashingService;
        private readonly EmployerAccountsDbContext _dbContext;

        public EmployerAccountsDbBuilder(
            DbBuilderDependentRepositories dependentRepositories,
            IHashingService hashingService,
            IPublicHashingService publicHashingService,
            EmployerAccountsDbContext dbContext)
        {
            DependentRepositories = dependentRepositories;
            _hashingService = hashingService;
            _publicHashingService = publicHashingService;
            _dbContext = dbContext;
        }

        public DbBuilderContext Context { get;  } = new DbBuilderContext();

        public DbBuilderDependentRepositories DependentRepositories { get; }

        public EmployerAccountsDbBuilder EnsureAccountExists(EmployerAccountInput input)
        {
            return EnsureAccountExists(input, CancellationToken.None);
        }

        public EmployerAccountsDbBuilder EnsureAccountExists(EmployerAccountInput input, CancellationToken cancellationToken)
        {
            return WaitDbAction(EnsureAccountExistsAsync(input), cancellationToken);
        }

        public async Task<EmployerAccountsDbBuilder> EnsureAccountExistsAsync(EmployerAccountInput input)
        {
            var output = await DependentRepositories.AccountRepository.GetAccountDetailsAsync(input.OrganisationName);

            if (output == null)
            {
                await DependentRepositories.AccountRepository.CreateAccount(
                    input.UserId,
                    input.OrganisationReferenceNumber,
                    input.OrganisationName,
                    input.OrganisationRegisteredAddress,
                    input.OrganisationDateOfInception,
                    input.PayeReference,
                    input.AccessToken,
                    input.RefreshToken,
                    input.OrganisationStatus,
                    input.EmployerRefName,
                    input.Source,
                    input.PublicSectorDataSource,
                    input.Sector);

                output = await DependentRepositories.AccountRepository.GetAccountDetailsAsync(input.OrganisationName);
            }

            if (string.IsNullOrWhiteSpace(output.HashedAccountId))
            {
                output.HashedAccountId = _hashingService.HashValue(output.AccountId);
                output.PublicHashedAccountId = _publicHashingService.HashValue(output.AccountId);
                await DependentRepositories.AccountRepository.UpdateAccountHashedIds(output.AccountId, output.HashedAccountId, output.PublicHashedAccountId);
            }

            Context.ActiveEmployerAccount = output;

            return this;
        }

        public EmployerAccountsDbBuilder WithLegalEntity(LegalEntityWithAgreementInput input)
        {
            return WithLegalEntity(input, CancellationToken.None);
        }

        public EmployerAccountsDbBuilder WithLegalEntity(LegalEntityWithAgreementInput input, CancellationToken cancellationToken)
        {
            return WaitDbAction(WithLegalEntityAsync(input), cancellationToken);
        }

        public async Task<EmployerAccountsDbBuilder> WithLegalEntityAsync(LegalEntityWithAgreementInput input)
        {
            var output = new LegalEnityWithAgreementOutput();

            var view = await DependentRepositories.AccountRepository.CreateLegalEntityWithAgreement(
               new LegalEntityWithAgreementInputAdapter(input));

            output.EmployerAgreementId = view.Id;
            output.LegalEntityId = view.LegalEntityId;
            output.HashedAgreementId = view.HashedAgreementId;

            return this;
        }

        public EmployerAccountsDbBuilder EnsureUserExists(UserInput input)
        {
            return EnsureUserExists(input, CancellationToken.None);
        }

        public EmployerAccountsDbBuilder EnsureUserExists(UserInput input, CancellationToken cancellationToken)
        {
            return WaitDbAction(EnsureUserExistsAsync(input), cancellationToken);
        }

        public async Task<EmployerAccountsDbBuilder> EnsureUserExistsAsync(UserInput input)
        {
            try
            {
                var output = new UserOutput();

                await DependentRepositories.UserRepository.Upsert(new UserInputToUserAdapter(input));
                var user = await DependentRepositories.UserRepository.GetUserByRef(input.UserRef);
                output.UserRef = input.UserRef;
                output.UserId = user.Id;
                Context.ActiveUser = output;
                return this;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private TResult WaitDbAction<TResult>(Task<TResult> action, CancellationToken cancellationToken)
        {
            try
            {
                action.Wait((int) TestConstants.DbTimeout.TotalMilliseconds, cancellationToken);
                CheckTaskResult(action);

                return action.Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private void CheckTaskResult(Task action)
        {
            var failMessage = new StringBuilder();

            CheckIfTaskCanceled(action, failMessage);

            CheckIfTaskFaulted(action, failMessage);

            if (failMessage.Length > 0)
            {
                Assert.Fail(failMessage.ToString());
            }
        }

        private static void CheckIfTaskFaulted(Task action, StringBuilder failMessage)
        {
            if (action.IsFaulted)
            {
                failMessage.Append("A DB task has faulted. ");
                if (action.Exception != null)
                {
                    foreach (var exception in action.Exception.Flatten().InnerExceptions)
                    {
                        failMessage.AppendLine($">>:e => {exception.GetType().Name} : {exception.Message}");
                    }
                }
            }
        }

        private static void CheckIfTaskCanceled(Task action, StringBuilder failMessage)
        {
            if (action.IsCanceled)
            {
                failMessage.AppendLine(
                    $"A DB task has been cancelled, possibly because it has timed out. Timeout value is: {TestConstants.DbTimeout}");
            }
        }

        public void Dispose()
        {
            if (!HasTransaction) return;

            _dbContext.Database.CurrentTransaction.Dispose();
        }

        public bool HasTransaction => _dbContext.Database.CurrentTransaction != null;

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