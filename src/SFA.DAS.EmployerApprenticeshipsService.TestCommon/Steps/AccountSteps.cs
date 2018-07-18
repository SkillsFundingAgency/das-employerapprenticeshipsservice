using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.HashingService;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.TestCommon.Steps
{
    [Binding]
    public class AccountSteps
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        public AccountSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Given(@"user ([^ ]*) created account ([^ ]*)")]
        public async Task<Account> GivenUserCreatedAccount(string username, string accountName)
        {
            var user = _objectContext.Users[username];

            var db = _objectContainer.Resolve<EmployerAccountsDbContext>();
            var hashingService = _objectContainer.Resolve<IHashingService>();
            var publicHashingService = _objectContainer.Resolve<IPublicHashingService>();
            var agreementTemplate = await db.AgreementTemplates.OrderByDescending(t => t.VersionNumber).FirstAsync();

            var account = db.Accounts.Add(new Account
            {
                Name = accountName,
                CreatedDate = DateTime.Now,
            });

            await db.SaveChangesAsync();

            account.HashedId = hashingService.HashValue(account.Id);
            account.PublicHashedId = publicHashingService.HashValue(account.Id);


            LegalEntity legalEntity;

            var legalEntityCode = $"{accountName}-LE";
            if (_objectContext.LegalEntities.Any(l => l.Key == legalEntityCode))
            {
                legalEntity = _objectContext.LegalEntities[legalEntityCode];
            }
            else
            {
                legalEntity = db.LegalEntities.Add(new LegalEntity
                {
                    Name = "SomeName",
                    Code = legalEntityCode,
                    RegisteredAddress = "SomeAddress",
                    DateOfIncorporation = DateTime.Today,
                    Status = "",
                    Source = 1,
                    PublicSectorDataSource = 1,
                    Sector = ""
                });

                await db.SaveChangesAsync();

                _objectContext.LegalEntities.Add(legalEntity.Code, legalEntity);
            }


            var agreement = db.Agreements.Add(new EmployerAgreement
            {
                LegalEntity = legalEntity,
                Account = account,
                Template = agreementTemplate,
                StatusId = EmployerAgreementStatus.Signed
            });

            await db.SaveChangesAsync();


            //var account = user.CreateAccount(
            //    accountName,
            //    legalEntityCode,
            //    legalEntityName,
            //    legalEntityDateOfIncorporation,
            //    legalEntityRegisteredAddress,
            //    legalEntityStatus,
            //    legalEntityAgreementTemplate,
            //    payeReference);


            _objectContext.Accounts.Add(accountName, account);

            return account;
        }
    }
}