using System;
using System.Reflection;
using System.Threading;
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
        private Mock<IMembershipRepository> _membershipRepository;
        public override GetUserAccountRoleQuery Query { get; set; }
        public override GetUserAccountRoleHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserAccountRoleQuery>> RequestValidator { get; set; }

        private MembershipView _membershipView;

        private const long AccountId = 2;
        private const string ExternalUserId = "4";
        
        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _membershipView = new MembershipView
            {
                AccountId = AccountId,
                UserId = long.Parse(ExternalUserId),
                RoleName = Role.Owner.ToString()
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                                 .ReturnsAsync(_membershipView);
          

            Query = new GetUserAccountRoleQuery {AccountId = AccountId, ExternalUserId = ExternalUserId};
            RequestHandler = new GetUserAccountRoleHandler(_membershipRepository.Object, RequestValidator.Object);
        }
        
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _membershipRepository.Verify(x => x.GetCaller(AccountId, ExternalUserId), Times.Once());
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
