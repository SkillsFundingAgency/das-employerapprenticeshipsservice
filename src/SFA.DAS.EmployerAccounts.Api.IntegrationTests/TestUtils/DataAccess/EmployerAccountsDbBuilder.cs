﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.ModelBuilders;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Adapters;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.HashingService;
using SFA.DAS.Testing.Helpers;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class EmployerAccountsDbBuilder : IDbBuilder
    {
        private const string ServiceName = "SFA.DAS.EmployerAccounts";

        private readonly IServiceProvider _serviceProvider;
        private readonly IHashingService _hashingService;
        private readonly IPublicHashingService _publicHashingService;
        private readonly EmployerAccountsDbContext _dbContext;
        private readonly Lazy<IAccountRepository> _lazyAccountRepository;
        private readonly Lazy<IUserRepository> _lazyUserRepository;
        private readonly EmployerAccountsConfiguration _configuration;

        public EmployerAccountsDbBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _configuration = ConfigurationTestHelper.GetConfiguration<EmployerAccountsConfiguration>(ServiceName);

            _hashingService = _serviceProvider.GetService<IHashingService>();
            _publicHashingService = _serviceProvider.GetService<IPublicHashingService>();
            var optionsBuilder = new DbContextOptionsBuilder<EmployerAccountsDbContext>();
            optionsBuilder.UseSqlServer(_configuration.DatabaseConnectionString);
            _dbContext = new EmployerAccountsDbContext(optionsBuilder.Options);

            _lazyAccountRepository = new  Lazy<IAccountRepository>(BuildAccountRepository);
            _lazyUserRepository = new Lazy<IUserRepository>(BuildUserRepository);



        }

        private IUserRepository BuildUserRepository()
        {
            return 
                new UserRepository(
                    _configuration,
                    _serviceProvider.GetService<ILogger<UserRepository>>(),
                    new Lazy<EmployerAccountsDbContext>(() => _dbContext)
                    );
        }

        private IAccountRepository BuildAccountRepository()
        {
            return
                new AccountRepository(
                    _configuration,
                    _serviceProvider.GetService<ILogger<AccountRepository>>(),
                    new Lazy<EmployerAccountsDbContext>(() => _dbContext),
                    _serviceProvider.GetService<IAccountLegalEntityPublicHashingService>());
        }

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
            await _lazyUserRepository.Value.Upsert(new UserInputToUserAdapter(input));
            var user = await _lazyUserRepository.Value.GetUserByRef(input.Ref);

            var output = new UserOutput
            {
                UserRef = input.Ref,
                UserId = user.Id
            };

            return output;
        }

        public async Task<EmployerAccountOutput> CreateAccountAsync(EmployerAccountInput input)
        {
            var createResult = await _lazyAccountRepository.Value.CreateAccount(
                new CreateAccountParams { 
                UserId = input.UserId(),
                EmployerNumber = input.OrganisationReferenceNumber,
                EmployerName = input.OrganisationName,
                EmployerRegisteredAddress = input.OrganisationRegisteredAddress,
                EmployerDateOfIncorporation = input.OrganisationDateOfInception,
                EmployerRef = input.PayeReference,
                AccessToken = input.AccessToken,
                RefreshToken = input.RefreshToken,
                CompanyStatus = input.OrganisationStatus,
                EmployerRefName = input.EmployerRefName,
                Source = input.Source,
                PublicSectorDataSource = input.PublicSectorDataSource,
                Sector = input.Sector,
                Aorn = input.Aorn,
                AgreementType = input.AgreementType});

            var output = new EmployerAccountOutput
            {
                AccountId = createResult.AccountId,
                HashedAccountId = _hashingService.HashValue(createResult.AccountId),
                PublicHashedAccountId = _publicHashingService.HashValue(createResult.AccountId),
                LegalEntityId =  createResult.LegalEntityId
            };

            await _lazyAccountRepository.Value.UpdateAccountHashedIds(output.AccountId, output.HashedAccountId, output.PublicHashedAccountId);

            return output;
        }

        public async Task<LegalEnityWithAgreementOutput> CreateLegalEntityAsync(LegalEntityWithAgreementInput input)
        {
            var view = await _lazyAccountRepository.Value.CreateLegalEntityWithAgreement(
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