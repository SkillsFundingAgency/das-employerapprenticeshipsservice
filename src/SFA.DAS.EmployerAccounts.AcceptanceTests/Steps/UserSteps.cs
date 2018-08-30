using System;
using System.Threading.Tasks;
using BoDi;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerAccounts.AcceptanceTests.Steps
{
    [Binding]
    public class UserSteps
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        public UserSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Given(@"user ([^ ]*) registered")]
        public async Task<User> GivenUserRegistered(string username)
        {
            var db = _objectContainer.Resolve<EmployerAccountsDbContext>();

            var user = db.Users.Add(new User
            {
                Email = $"{username}@test.com",
                Ref = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe"
            });

            await db.SaveChangesAsync();

            _objectContext.Set(username, user);

            return user;
        }
    }
}