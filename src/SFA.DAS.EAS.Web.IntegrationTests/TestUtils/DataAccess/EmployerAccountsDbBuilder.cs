using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Adapters;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess
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
        public bool HasTransaction => _dbContext.Database.CurrentTransaction != null;

        /// <summary>
        ///     Persists the data defined in <see cref="TestModelBuilder"/> to the database. Keys 
        ///     will be propagated automatically. e.g. User Ids will be add to any accounts created
        ///     for that user;
        /// </summary>
        public async Task<EmployerAccountsDbBuilder> SetupDataAsync(TestModelBuilder builder)
        {
            foreach (var userSetup in builder.Users)
            {
                userSetup.UserOutput = await CreateUserAsync(userSetup.UserInput);

                await CreateAccountsForUserAsync(userSetup);
            }

            return this;
        }

        public async Task<UserOutput> CreateUserAsync(UserInput input)
        {
            await DependentRepositories.UserRepository.Upsert(new UserInputToUserAdapter(input));
            var user = await DependentRepositories.UserRepository.GetUserByRef(input.Ref);

            var output = new UserOutput
            {
                UserRef = input.Ref,
                UserId = user.Id
            };

            return output;
        }

        public async Task<EmployerAccountOutput> CreateAccountAsync(EmployerAccountInput input)
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
                PublicHashedAccountId = _publicHashingService.HashValue(createResult.AccountId),
                LegalEntityId =  createResult.LegalEntityId
            };

            await DependentRepositories.AccountRepository.UpdateAccountHashedIds(output.AccountId, output.HashedAccountId, output.PublicHashedAccountId);

            return output;
        }

        public async Task<LegalEnityWithAgreementOutput> CreateLegalEntityAsync(LegalEntityWithAgreementInput input)
        {
            var view = await DependentRepositories.AccountRepository.CreateLegalEntityWithAgreement(
               new LegalEntityWithAgreementInputAdapter(input));

            var output = new LegalEnityWithAgreementOutput
            {
                EmployerAgreementId = view.Id,
                LegalEntityId = view.LegalEntityId,
                HashedAgreementId = view.HashedAgreementId
            };

            return output;
        }

        private async Task CreateAccountsForUserAsync(UserSetup userSetup)
        {
            foreach (var employerAccountSetup in userSetup.Accounts)
            {
                employerAccountSetup.AccountOutput = await CreateAccountAsync(employerAccountSetup.AccountInput);

                await CreateLegalEntitiesForAccountsAsync(employerAccountSetup);
            }
        }

        private async Task CreateLegalEntitiesForAccountsAsync(EmployerAccountSetup accountSetup)
        {
            foreach (var legalEntitySetup in accountSetup.LegalEntities)
            {
                legalEntitySetup.LegalEntityWithAgreementInputOutput = await CreateLegalEntityAsync(legalEntitySetup.LegalEntityWithAgreementInput);
            }
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

        public void Dispose()
        {
            if (!HasTransaction) return;

            _dbContext.Database.CurrentTransaction.Dispose();
        }
    }
}