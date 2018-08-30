using System;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EmployerAccounts.AcceptanceTests.Extensions;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Steps
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
            var user = _objectContext.Get<User>(username);
            var account = _objectContext.Get<Account>(accountName);


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

            _objectContext.Set(membership.GetMembershipKey(), membership);
        }
    }
}