using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.EmployerFinance.AcceptanceTests.Models;
using SFA.DAS.EmployerFinance.AcceptanceTests.Steps;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.HashingService;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class ObjectContextExtensions
    {
        public static IObjectContainer AddEmployerFinanceImplementations(this IObjectContainer objectContainer, IContainer container)
        {
            objectContainer.RegisterInstanceAs(container.GetInstance<EmployerAccountsDbContext>());
            objectContainer.RegisterInstanceAs(container.GetInstance<EmployerFinanceDbContext>());

            objectContainer.RegisterInstanceAs(container.GetInstance<ITransactionRepository>());
            objectContainer.RegisterInstanceAs(container.GetInstance<ITestTransactionRepository>());

            return objectContainer;
        }

        public static IObjectContainer DeleteExistingTransactions(this IObjectContainer objectContainer)
        {
            objectContainer.Resolve<ITestTransactionRepository>().InitialiseDatabaseData();

            return objectContainer;
        }

        public static async Task<Account> CreateAccount(this ObjectContext objectContext, IObjectContainer objectContainer)
        {
            var hashingService = objectContainer.Resolve<IHashingService>();
            var accountId = await objectContainer.Resolve<ITestTransactionRepository>().GetNextAccountId();

            var account = new Account
            {
                Id = accountId,
                HashedId = hashingService.HashValue(accountId)
            };

            objectContext.Set(accountId, account).SetupAuthorizedUser(objectContainer);
            objectContainer.SetupAuthorizedUser(account);

            return account;
        }

        public static IEnumerable<long> ProcessingSubmissionIds(this ObjectContext objectContext)
        {
            return objectContext.GetAll<ProcessingSubmissionId>().Select(s => s.Value.SubmissionId);
        }

        
        public static IDictionary<long, DateTime?> ProcessingSubmissionIdsDictionary(this ObjectContext objectContext)
        {
            return objectContext.GetAll<ProcessingSubmissionId>().ToDictionary(item => item.Value.SubmissionId, item => item.Value.SubmissionDate);
        }

        public static void ImportCurrentlyProcessingSubmissionIds(this ObjectContext objectContext, Table table)
        {
            foreach (var tableRow in table.Rows)
            {
                var submissionId = long.Parse(tableRow["Id"]);

                DateTime? createdDate = null;
                if (tableRow.ContainsKey("CreatedDate") && tableRow["CreatedDate"] != null)
                {
                    createdDate = DateTime.ParseExact(tableRow["CreatedDate"], "yyyy-MM-dd",
                        CultureInfo.InvariantCulture);
                }

                objectContext.Set(submissionId.ToString(),
                    new ProcessingSubmissionId {SubmissionId = submissionId, SubmissionDate = createdDate});
            }
        }
    }
}