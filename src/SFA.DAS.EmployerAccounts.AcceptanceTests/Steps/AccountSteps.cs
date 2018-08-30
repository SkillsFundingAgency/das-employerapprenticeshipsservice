using System.Threading.Tasks;
using BoDi;
using SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Steps
{
    [Binding]
    public class AccountSteps : TechTalk.SpecFlow.Steps
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        public AccountSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Given(@"user ([^ ]*) created account ([^ ]*)")]
        public async Task<EAS.Domain.Models.Account.Account> GivenUserCreatedAccount(string username, string accountName)
        {
            var account = await _objectContext.CreateAccount(accountName, _objectContainer );

            return account;
        }
    }
}