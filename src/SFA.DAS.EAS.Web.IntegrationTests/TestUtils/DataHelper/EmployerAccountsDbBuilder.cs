using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
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

        public DbBuilderDependentRepositories DependentRepositories { get; }

        public async Task<EmployerAccountOutput> EnsureAccountExistsAsync(EmployerAccountInput input)
        {
            var createResult = await DependentRepositories.AccountRepository.CreateAccount(
                input.UserId(),
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

            var output = new EmployerAccountOutput
            {
                AccountId = createResult.AccountId,
                HashedAccountId = _hashingService.HashValue(createResult.AccountId),
                PublicHashedAccountId = _publicHashingService.HashValue(createResult.AccountId)
            };

            await DependentRepositories.AccountRepository.UpdateAccountHashedIds(output.AccountId, output.HashedAccountId, output.PublicHashedAccountId);

            return output;
        }

        public async Task<LegalEnityWithAgreementOutput> WithLegalEntityAsync(LegalEntityWithAgreementInput input)
        {
            var output = new LegalEnityWithAgreementOutput();

            var view = await DependentRepositories.AccountRepository.CreateLegalEntityWithAgreement(
               new LegalEntityWithAgreementInputAdapter(input));

            output.EmployerAgreementId = view.Id;
            output.LegalEntityId = view.LegalEntityId;
            output.HashedAgreementId = view.HashedAgreementId;

            return output;
        }

        public async Task<EmployerAccountsDbBuilder> SetupDataAsync(TestModelBuilder builder)
        {
            foreach (var userSetup in builder.Users)
            {
                userSetup.UserOutput = await EnsureUserExistsAsync(userSetup.UserInput);

                await SetupAccountsForUserAsync(userSetup);
            }

            return this;
        }

        private async Task SetupAccountsForUserAsync(UserSetup userSetup)
        {
            foreach (var employerAccountSetup in userSetup.Accounts)
            {
                employerAccountSetup.AccountOutput = await EnsureAccountExistsAsync(employerAccountSetup.AccountInput);

                await SetupLegalEntitiesForAccountsAsync(employerAccountSetup);
            }
        }

        private async Task SetupLegalEntitiesForAccountsAsync(EmployerAccountSetup accountSetup)
        {
            foreach (var legalEntitySetup in accountSetup.LegalEntities)
            {
                legalEntitySetup.LegalEntityWithAgreementInputOutput = await WithLegalEntityAsync(legalEntitySetup.LegalEntityWithAgreementInputInput);
            }
        }

        public async Task<UserOutput> EnsureUserExistsAsync(UserInput input)
        {
            try
            {
                var output = new UserOutput();

                await DependentRepositories.UserRepository.Upsert(new UserInputToUserAdapter(input));
                var user = await DependentRepositories.UserRepository.GetUserByRef(input.UserRef);
                output.UserRef = input.UserRef;
                output.UserId = user.Id;
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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