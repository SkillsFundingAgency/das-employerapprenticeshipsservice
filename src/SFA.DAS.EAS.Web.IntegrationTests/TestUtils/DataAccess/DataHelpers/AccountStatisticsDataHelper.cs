using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoFixture;
using Dapper;
using Moq;
using SFA.DAS.Configuration;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.Hashing;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers
{
    internal class AccountStatisticsDataHelper
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        
        public AccountStatisticsDataHelper()
        {
            _configuration = ConfigurationHelper.GetConfiguration<EmployerApprenticeshipsServiceConfiguration>(ServiceName);
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_configuration.DatabaseConnectionString))
            {   
                return await connection.QuerySingleAsync<StatisticsViewModel>(GetStatisticsSql);
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

            var accountDbContext = new EmployerAccountsDbContext(_configuration.DatabaseConnectionString);
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
                await accountRepo.CreateAccount(
                    createdUser.Id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    DateTime.Today,
                    $"ref-{DateTime.Now.Ticks.ToString().Substring(4, 10)}",
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    2,
                    1,
                    fixture.Create<string>());

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