using Moq;
using NUnit.Framework;
using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Types;
using SFA.DAS.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Extensions;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.AuditApiClientForSupportUser
{
    public class WhenIAuditTheAuditMessage
    {
        private EmployerAccounts.Services.AuditApiClientForSupportUser _sut;
        private AuditMessage _auditMessage;
        private string SupportConsoleUsers = "Tier1User,Tier2User";
        private Mock<IAuditApiClient> _mockInnerClient;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private EmployerAccountsConfiguration _config;
        private IUserContext _userContext;

        private string _impersonatedUser;
        private string _impersonatedUserEmail;
        private string _supportUserUpn;
        private string _supportUserEmail;
        

        [SetUp]
        public void Arrange()
        {
            _config = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = SupportConsoleUsers
            };
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _userContext = new UserContext(_httpContextAccessorMock.Object, _config);
            _impersonatedUser = Guid.NewGuid().ToString();
            _impersonatedUserEmail = $"{Guid.NewGuid().ToString()}@test.co.uk";
            _supportUserUpn = Guid.NewGuid().ToString();
            _supportUserEmail = $"{Guid.NewGuid().ToString()}@test.co.uk";

            _auditMessage = new AuditMessage();
            _auditMessage.ChangedBy = new Actor { Id = _impersonatedUser, EmailAddress = _impersonatedUserEmail };

            _mockInnerClient = new Mock<IAuditApiClient>();
            
            
            _httpContextAccessorMock
                .Setup(m => m.HttpContext.User.FindFirstValue(DasClaimTypes.Id))
                .Returns(_impersonatedUser);

            _httpContextAccessorMock
                .Setup(m => m.HttpContext.User.FindFirstValue(DasClaimTypes.Email))
                .Returns(_impersonatedUserEmail);

            _httpContextAccessorMock
                .Setup(m => m.HttpContext.User.FindFirstValue(ClaimTypes.Upn))
                .Returns(_supportUserUpn);

            _httpContextAccessorMock
                .Setup(m => m.HttpContext.User.FindFirstValue(ClaimTypes.Email))
                .Returns(_supportUserEmail);

            _sut = new EmployerAccounts.Services.AuditApiClientForSupportUser(_mockInnerClient.Object, _userContext);
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public async Task ThenTheAuthenticationServiceIsCalledToIdentifyASupportUser(string role)
        {
            // arrange

            // act
            await _sut.Audit(_auditMessage);

            // assert
            _httpContextAccessorMock.Verify(m => m.HttpContext.User.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role), Times.Once);
        }

        [Test]
        public async Task ThenTheInnerClientIsCalled()
        {
            // arrange

            // act
            await _sut.Audit(_auditMessage);

            // assert
            _mockInnerClient.Verify(m =>
            m.Audit(It.Is<AuditMessage>(a =>
            a.ChangedBy.Id.Equals(_impersonatedUser) &&
            a.ChangedBy.EmailAddress.Equals(_impersonatedUserEmail) &&
            a.RelatedEntities == null
            )), Times.Once);
        }

        [Test]
        [TestCase("Tier1User")]
        [TestCase("Tier2User")]
        public async Task ThenTheInnerClientIsCalledWithModifiedFieldsForASupportUser(string role)
        {
            // arrange
            _httpContextAccessorMock
                .Setup(m => m.HttpContext.User.HasClaim(ClaimsIdentity.DefaultRoleClaimType, role))
                .Returns(true);

            // act
            await _sut.Audit(_auditMessage);

            // assert
            _mockInnerClient.Verify(m => 
            m.Audit(It.Is<AuditMessage>(a => 
            a.ChangedBy.Id.Equals(_supportUserUpn) &&
            a.ChangedBy.EmailAddress.Equals(_supportUserEmail) &&
            a.RelatedEntities.Count(e => e.Type.Equals("UserImpersonatedId") && e.Id.Equals(_impersonatedUser)).Equals(1) &&
            a.RelatedEntities.Count(e => e.Type.Equals("UserImpersonatedEmail") && e.Id.Equals(_impersonatedUserEmail)).Equals(1)
            )), Times.Once);
        }
    }
}
