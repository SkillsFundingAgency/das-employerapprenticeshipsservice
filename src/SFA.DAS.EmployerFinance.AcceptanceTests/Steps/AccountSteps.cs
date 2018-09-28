using System;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class AccountSteps : TechTalk.SpecFlow.Steps
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;
        private readonly UnitOfWorkManagerTestHelper _unitOfWorkManagerTestHelper;

        public AccountSteps(IObjectContainer objectContainer, ObjectContext objectContext, UnitOfWorkManagerTestHelper unitOfWorkManagerTestHelper)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
            _unitOfWorkManagerTestHelper = unitOfWorkManagerTestHelper;
        }

        [Given(@"We have an account")]
        public async Task GivenWeHaveAnAccount()
        {
            await Run(() => _objectContext.CreateAccount(_objectContainer));
        }

        private Task Run(Func<Task> operation)
        {
            return _unitOfWorkManagerTestHelper.RunInIsolatedTransactionAsync(operation);
        }

    }
}