using System;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using Moq;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Testing.Helpers;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers
{
    [ExcludeFromCodeCoverage]
    internal class AccountStatisticsDataHelper
    {
        private const string ServiceName = "SFA.DAS.EmployerAccounts";
        private readonly EmployerAccountsConfiguration _configuration;
        
        public AccountStatisticsDataHelper()
        {
            _configuration = ConfigurationTestHelper.GetConfiguration<EmployerAccountsConfiguration>(ServiceName);
        }

        public async Task<Statistics> GetStatistics()
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {
                return await connection.QuerySingleAsync<Statistics>(GetStatisticsSql);
            }
        }

        private const string GetStatisticsSql = @"
select (
  select count(0)
  from employer_account.Account
) as TotalAccounts, (
  select count(0)
  from employer_account.LegalEntity
) as TotalLegalEntities, (
  select count(0)
  from employer_account.Paye
) as TotalPayeSchemes, (
  select count(0)
  from employer_account.EmployerAgreement a
  where a.StatusId = 2 -- signed
) as TotalAgreements;";

        public async Task CreateAccountStatistics()
        {
            var fixture = new Fixture();

            var optionsBuilder = new DbContextOptionsBuilder<EmployerAccountsDbContext>();
            optionsBuilder.UseSqlServer(_configuration.DatabaseConnectionString);
            var accountDbContext = new EmployerAccountsDbContext(optionsBuilder.Options);
            var lazyDb = new Lazy<EmployerAccountsDbContext>(() => accountDbContext);
            var userRepo = new UserRepository(_configuration, Mock.Of<ILog>(), lazyDb);
            var userToCreate = fixture
                .Build<User>()
                .Without(user => user.Id)
                .Without(user => user.UserRef)
                .Create();
            var accountRepo = new AccountRepository(_configuration,
                Mock.Of<ILog>(), lazyDb, Mock.Of<IAccountLegalEntityPublicHashingService>());

            accountDbContext.Database.BeginTransaction();

            try
            {
                await userRepo.Create(userToCreate);
                var createdUser = await userRepo.GetUserByRef(userToCreate.UserRef);
                await accountRepo.CreateAccount(new CreateAccountParams
                {
                    UserId = createdUser.Id,
                    EmployerDateOfIncorporation = DateTime.Today,
                    EmployerRef = $"ref-{DateTime.Now.Ticks.ToString().Substring(4, 10)}",
                    Source = 2,
                    PublicSectorDataSource = 1,
                    EmployerNumber = fixture.Create<string>(),
                    EmployerName = fixture.Create<string>(),
                    EmployerRegisteredAddress = fixture.Create<string>(),
                    AccessToken = fixture.Create<string>(),
                    RefreshToken = fixture.Create<string>(),
                    CompanyStatus = fixture.Create<string>(),
                    EmployerRefName = fixture.Create<string>(),
                    Sector = fixture.Create<string>(),
                    Aorn = fixture.Create<string>(),
                    AgreementType = fixture.Create<AgreementType>()
                });

                accountDbContext.Database.CurrentTransaction.Commit();
            }
            catch (Exception e)
            {
                accountDbContext.Database.CurrentTransaction.Rollback();
                Console.WriteLine(e);
                throw;
            }
        }
    }
}