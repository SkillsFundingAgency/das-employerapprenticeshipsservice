using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Moq;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using StructureMap;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.TestCommon.ScenarioCommonSteps
{
    public class AccountSteps
    {
        public static void SetAccountIdForUser(IMediator mediator, ScenarioContext scenarioContext)
        {
            var accountOwnerId = scenarioContext["AccountOwnerUserId"].ToString();
            var getUserAccountsQueryResponse = mediator.SendAsync(new GetUserAccountsQuery { UserId = accountOwnerId }).Result;

            var account = getUserAccountsQueryResponse.Accounts.AccountList.FirstOrDefault();
            scenarioContext["AccountId"] = account.Id;
            scenarioContext["HashedAccountId"] = account.HashedId;
        }

        public static Guid CreateAccountWithOwner(EmployerAccountOrchestrator orchestrator, IMediator mediator, Mock<IOwinWrapper> owinWrapper, HomeOrchestrator homeOrchestrator)
        {
            var accountOwnerUserId = Guid.NewGuid();
            //ScenarioContext.Current["AccountOwnerUserId"] = accountOwnerUserId;

            var signInUserModel = new SignInUserModel
            {
                UserId = accountOwnerUserId.ToString(),
                Email = "accountowner@test.com" + Guid.NewGuid().ToString().Substring(0, 6),
                FirstName = "Test",
                LastName = "Tester"
            };

            UserSteps.UpsertUser(mediator,signInUserModel);

            var user = UserSteps.GetExistingUserAccount(accountOwnerUserId.ToString(),owinWrapper,homeOrchestrator);

            CreateDasAccount(user, orchestrator);

            return accountOwnerUserId;
        }

        public static void CreateDasAccount(SignInUserModel user, EmployerAccountOrchestrator orchestrator)
        {

            orchestrator.CreateAccount(new CreateAccountModel
            {
                UserId = user.UserId,
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                CompanyDateOfIncorporation = new DateTime(2016, 01, 01),
                EmployerRef = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}",
                CompanyName = "Test Company",
                CompanyNumber = "123456TGB" + Guid.NewGuid().ToString().Substring(0, 6),
                CompanyRegisteredAddress = "Address Line 1"
            }, new Mock<HttpContextBase>().Object).Wait();


        }
    }
}
