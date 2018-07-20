using System;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.EAS.TestCommon.Steps
{
    [Binding]
    public class MembershipSteps
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        public MembershipSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Given(@"user ([^ ]*) has role ([^ ]*) for account ([^ ]*)")]
        public async Task GivenUserUHasRoleRForAccountA(string username, string roleName, string accountName)
        {
            var user = _objectContext.Users[username];
            var account = _objectContext.Accounts[accountName];


            var db = _objectContainer.Resolve<EmployerAccountsDbContext>();

            if (!Enum.TryParse(roleName, out Role role))
                throw new ArgumentException("roleName");

            var membership = new Membership
            {
                User = user,
                Account = account,
                Role = role
            };

            db.Memberships.Add(membership);

            await db.SaveChangesAsync();

            _objectContext.Memberships.Add(membership);
        }

        [Given(@"user ([^ ]*) adds user ([^ ]*) to account ([^ ]*) with role ([^ ]*)")]
        public void GivenUserAAddsUserBToAccountA(string creatorUserName, string userName, string accountName, string roleName)
        {
            ScenarioContext.Current.Pending();
        }

    }
}