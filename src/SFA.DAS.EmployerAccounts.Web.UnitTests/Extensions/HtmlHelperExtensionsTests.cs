using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MediatR;
using Microsoft.Owin;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using Claim = System.Security.Claims.Claim;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    class HtmlHelperExtensionsTests
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
        private Mock<IAuthorizationService> mockAuthorizationService;
        private Mock<IMultiVariantTestingService> mockMultiVariantTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> mockCookieStorageService;
        private Mock<EmployerTeamOrchestrator> mockEmployerTeamOrchestrator;
        private Mock<ControllerContext> mockControllerContext;
        private Mock<HttpContextBase> mockHttpContext;
        private Mock<IPrincipal> mockPrincipal;
        private Mock<ClaimsIdentity> mockClaimsIdentity;
        private bool _isAuthenticated = true;
        private List<Claim> _claims;
        private string _userId;
        private HtmlHelper htmlHelper;
        private Mock<IViewDataContainer> _viewDataContainerMock;
        private ViewContext _viewContext;
        private Mock<IMediator> _mockMediator;

        [SetUp]
        public void SetUp()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockAuthorizationService = new Mock<IAuthorizationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            mockControllerContext = new Mock<ControllerContext>();
            mockHttpContext = new Mock<HttpContextBase>();
            mockPrincipal = new Mock<IPrincipal>();
            mockClaimsIdentity = new Mock<ClaimsIdentity>();

            _userId = "TestUser";

            _claims = new List<Claim>();
            _claims.Add(new Claim(ControllerConstants.UserRefClaimKeyName, _userId));

            mockPrincipal.Setup(m => m.Identity).Returns(mockClaimsIdentity.Object);
            mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(_isAuthenticated);
            mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
            mockHttpContext.Setup(m => m.User).Returns(mockPrincipal.Object);
            mockControllerContext.Setup(m => m.HttpContext).Returns(mockHttpContext.Object);

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object,
                mockAuthorizationService.Object);

            _controller.ControllerContext = mockControllerContext.Object;
            //_controller.Request.SetOwinContext(new OwinContext());
            _viewDataContainerMock = new Mock<IViewDataContainer>();
            _viewContext = new ViewContext();
            _viewContext.Controller = _controller;
            

            _mockMediator = new Mock<IMediator>();

            htmlHelper = new HtmlHelper(_viewContext, _viewDataContainerMock.Object);
        }

        [Test]
        public void WhenAuthenticatedSupportUser_ShouldReturnTrue()
        {
            // Arrange
            _isAuthenticated = true;
            _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));

            // Act
            var result = htmlHelper.IsSupportUser();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void WhenUnauthenticatedSupportUser_ShouldReturnFalse()
        {
            // Arrange
            mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(false); // re-apply the mock return
            _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));
            // Act
            var result = htmlHelper.IsSupportUser();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void WhenUnauthenticatedNonSupportUser_ShouldReturnFalse()
        {
            // Arrange
            _isAuthenticated = false;

            // Act
            var result = htmlHelper.IsSupportUser();

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void WhenAuthenticatedNonSupportUser_ShouldReturnFalse()
        {
            // Arrange
            _isAuthenticated = true;

            // Act
            var result = htmlHelper.IsSupportUser();

            // Assert
            Assert.IsFalse(result);
        }

        [TestCaseSource(nameof(LabelCases))]
        public void WhenICallSetZenDeskLabelsWithLabels_ThenTheKeywordsAreCorrect(string[] labels, string keywords)
        {
            // Arrange
            var expected = $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [{keywords}] }});</script>";

            // Act
            var actual = Web.Extensions.HtmlHelperExtensions.SetZenDeskLabels(null, labels).ToString();

            // Assert
            Assert.AreEqual(expected, actual);
        }


        private static readonly object[] LabelCases =
        {
            new object[] { new string[] { "a string with multiple words", "the title of another page" }, "'a string with multiple words','the title of another page'"},
            new object[] { new string[] { "eas-estimate-apprenticeships-you-could-fund" }, "'eas-estimate-apprenticeships-you-could-fund'"},
            new object[] { new string[] { "eas-apostrophe's" }, @"'eas-apostrophe\'s'"},
            new object[] { new string[] { null }, "''" }
        };

        [Test]
        public void CheckIfV3AgreementIsNotSignedByAllLegalEntities_ReturnTrue()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "USER1";
            var dependancyResolver = new Mock<IDependencyResolver>();
            dependancyResolver.Setup(r => r.GetService(typeof(IMediator))).Returns(_mockMediator.Object);
            DependencyResolver.SetResolver(dependancyResolver.Object);
            _mockMediator.Setup(m => m.SendAsync(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == hashedAccountId)))
                    .Returns(Task.FromResult(new GetAccountEmployerAgreementsResponse
                    {
                        EmployerAgreements = new List<EmployerAgreementStatusDto>
                        {
                            new EmployerAgreementStatusDto
                            {
                                Pending = new PendingEmployerAgreementDetailsDto { Id = 123 },
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 1}
                            },
                            new EmployerAgreementStatusDto
                            {
                                Pending = new PendingEmployerAgreementDetailsDto { Id = 124 },
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 1}
                            },
                            new EmployerAgreementStatusDto
                            {
                                Pending = new PendingEmployerAgreementDetailsDto { Id = 125 },
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 1}
                            },
                            new EmployerAgreementStatusDto
                            {
                                Pending = new PendingEmployerAgreementDetailsDto { Id = 126 },
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 1}
                            },
                            new EmployerAgreementStatusDto
                            {
                                Signed = new SignedEmployerAgreementDetailsDto { Id = 111 , VersionNumber = 2},
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 11}
                                },
                            new EmployerAgreementStatusDto
                            {
                                Signed = new SignedEmployerAgreementDetailsDto { Id = 112, VersionNumber = 1 },
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 11}
                            },
                            new EmployerAgreementStatusDto
                            {
                                Signed = new SignedEmployerAgreementDetailsDto { Id = 113 , VersionNumber = 2},
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 12}

                            },
                            new EmployerAgreementStatusDto
                            {
                                Signed = null,
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 12}
                            }
                        }
                    }));
            //Act
            var actual = htmlHelper.HasSignedV3AgreementAsync(userId,hashedAccountId);
            //Assert
            Assert.IsTrue(actual);
        }

        public void CheckIfV3AgreementIsNotSignedByAllLegalEntities_ReturnFalse()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "USER1";
            var dependancyResolver = new Mock<IDependencyResolver>();
            dependancyResolver.Setup(r => r.GetService(typeof(IMediator))).Returns(_mockMediator.Object);
            DependencyResolver.SetResolver(dependancyResolver.Object);
            _mockMediator.Setup(m => m.SendAsync(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == hashedAccountId)))
                    .Returns(Task.FromResult(new GetAccountEmployerAgreementsResponse
                    {
                        EmployerAgreements = new List<EmployerAgreementStatusDto>
                        {
                            new EmployerAgreementStatusDto
                            {
                                Signed = new SignedEmployerAgreementDetailsDto { Id = 113 , VersionNumber = 3},
                                LegalEntity = new AccountSpecificLegalEntityDto{AccountLegalEntityId = 12}
                            }
                        }
                    }));
            //Act
            var actual = htmlHelper.HasSignedV3AgreementAsync(userId, hashedAccountId);
            //Assert
            Assert.IsFalse(actual);
        }
    }
}
