using System;
using System.Linq;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EmployerAccounts.AcceptanceTests.Steps;
using SFA.DAS.Hashing;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions
{
    public static class ObjectContextExtensions
    {
        public static async Task<Account> CreateAccount(this ObjectContext objectContext, string accountName, IObjectContainer objectContainer)
        {
            var db = objectContainer.Resolve<EmployerAccountsDbContext>();
            var hashingService = objectContainer.Resolve<IHashingService>();
            var publicHashingService = objectContainer.Resolve<IPublicHashingService>();

            var account = db.Accounts.Add(new Account
            {
                Name = accountName,
                CreatedDate = DateTime.Now,
            });

            await db.SaveChangesAsync();

            account.HashedId = hashingService.HashValue(account.Id);
            account.PublicHashedId = publicHashingService.HashValue(account.Id);


            if (objectContext.Get<LegalEntity>(account.GetDefaultLegalEntityCode()) == null)
            {
                var legalEntity = db.LegalEntities.Add(new LegalEntity
                {
                    Code = account.GetDefaultLegalEntityCode(),
                    DateOfIncorporation = DateTime.Today,
                    Status = "",
                    Source = OrganisationType.CompaniesHouse,
                    PublicSectorDataSource = 1,
                    Sector = ""
                });

                await db.SaveChangesAsync();

                objectContext.Set(legalEntity.Code, legalEntity);
            }


            objectContext.Set(accountName, account);
            return account;
        }

    }
}