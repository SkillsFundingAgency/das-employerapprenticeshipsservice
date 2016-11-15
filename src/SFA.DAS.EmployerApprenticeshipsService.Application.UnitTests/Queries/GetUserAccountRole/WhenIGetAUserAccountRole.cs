using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserAccountRole
{
    class WhenIGetAUserAccountRole: QueryBaseTest<GetUserAccountRoleHandler, GetUserAccountRoleQuery, GetUserAccountRoleResponse>
    {
        private const string HashedAccountId = "123ABC";
        private const string ExternalUserId = "4";

        private Mock<IMembershipRepository> _membershipRepository;
        private MembershipView _membershipView;

        public override GetUserAccountRoleQuery Query { get; set; }
        public override GetUserAccountRoleHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserAccountRoleQuery>> RequestValidator { get; set; }
        
        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _membershipView = new MembershipView
            {
                AccountId = 2,
                UserId = long.Parse(ExternalUserId),
                RoleName = Role.Owner.ToString()
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                                 .ReturnsAsync(_membershipView);

            Query = new GetUserAccountRoleQuery {HashedAccountId = HashedAccountId, ExternalUserId = ExternalUserId};
            RequestHandler = new GetUserAccountRoleHandler(RequestValidator.Object, _membershipRepository.Object);
        }
        
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _membershipRepository.Verify(x => x.GetCaller(HashedAccountId, ExternalUserId), Times.Once());
        }

        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);
            
            //Assert
            Assert.AreEqual(Role.Owner, result.UserRole);
        }

        [Test]
        public async Task ThenIfTheUserIsNotInTheTeamTheRoleWillBeNone()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                                 .ReturnsAsync(null);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(Role.None, result.UserRole);
        }
    }
}
