using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Web.Controllers;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using Claim = System.Security.Claims.Claim;
using Microsoft.AspNetCore.Mvc.Rendering;
using HtmlHelper = Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelper;
using ViewContext = Microsoft.AspNetCore.Mvc.Rendering.ViewContext;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions
{
    class HtmlHelperExtensionsTests
    {
        private EmployerTeamController _controller;

        private Mock<IAuthenticationService> mockAuthenticationService;
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
        private IHtmlHelper _sut;
        private IHtmlHelper htmlHelper;
        private Mock<IViewDataContainer> _viewDataContainerMock;
        private ViewContext _viewContext;
        private Mock<IMediator> _mockMediator;
        private EmployerAccountsConfiguration _employerConfirguration;
        private readonly string _supportConsoleUsers = "Tier1User,Tier2User";

        [SetUp]
        public void SetUp()
        {
            mockAuthenticationService = new Mock<IAuthenticationService>();
            mockMultiVariantTestingService = new Mock<IMultiVariantTestingService>();
            mockCookieStorageService = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            mockEmployerTeamOrchestrator = new Mock<EmployerTeamOrchestrator>();
            mockControllerContext = new Mock<ControllerContext>();
            mockHttpContext = new Mock<HttpContextBase>();
            mockPrincipal = new Mock<IPrincipal>();
            mockClaimsIdentity = new Mock<ClaimsIdentity>();
            _employerConfirguration = new EmployerAccountsConfiguration()
            {
                SupportConsoleUsers = _supportConsoleUsers

            };
            _userId = "TestUser";

            _claims = new List<Claim>
            {
                new Claim(ControllerConstants.UserRefClaimKeyName, _userId)
            };

            mockPrincipal.Setup(m => m.Identity).Returns(mockClaimsIdentity.Object);
            mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(_isAuthenticated);
            mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
            mockHttpContext.Setup(m => m.User).Returns(mockPrincipal.Object);
            mockControllerContext.Setup(m => m.HttpContext).Returns(mockHttpContext.Object);

            _controller = new EmployerTeamController(
                mockAuthenticationService.Object,
                mockMultiVariantTestingService.Object,
                mockCookieStorageService.Object,
                mockEmployerTeamOrchestrator.Object)
            {
                ControllerContext = mockControllerContext.Object
            };
            _viewDataContainerMock = new Mock<IViewDataContainer>();
            _viewContext = new ViewContext
            {
                Controller = _controller
            };


            _mockMediator = new Mock<IMediator>();

            _sut = new HtmlHelper(_viewContext, _viewDataContainerMock.Object);

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(provider => provider.GetService(typeof(EmployerAccountsConfiguration)))
                .Returns(_employerConfirguration);

            var dependencyResolver = new Mock<IDependencyResolver>();
            dependencyResolver
                .Setup(mock => mock.GetService(typeof(EmployerAccountsConfiguration)))
                .Returns(_employerConfirguration);

            DependencyResolver.SetResolver(dependencyResolver.Object);

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
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void SingleOrg_SignedV3Agreement_ShouldNotShowExpiringAgreementBanner(bool hasSignedV1, bool hasSignedV2, bool shouldShowBanner)
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "USER1";
            var dependancyResolver = new Mock<IDependencyResolver>();
            dependancyResolver.Setup(r => r.GetService(typeof(IMediator))).Returns(_mockMediator.Object);
            DependencyResolver.SetResolver(dependancyResolver.Object);
            _mockMediator.Setup(m => m.Send(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(new GetAccountEmployerAgreementsResponse
                    {
                        EmployerAgreements = new List<EmployerAgreementStatusDto>
                        {
                            new EmployerAgreementStatusDto
                            {
                                Signed = hasSignedV1 ? new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 } : null,
                                Pending = hasSignedV1 ? null :  new PendingEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 },
                                LegalEntity = new AccountSpecificLegalEntityDto{ AccountLegalEntityId = 1 }
                            },
                            new EmployerAgreementStatusDto
                            {
                                Signed = hasSignedV2 ? new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 2 } : null,
                                Pending = hasSignedV2 ? null :  new PendingEmployerAgreementDetailsDto { Id = 123, VersionNumber = 2 },
                                LegalEntity = new AccountSpecificLegalEntityDto{ AccountLegalEntityId = 1 }
                            },
                            new EmployerAgreementStatusDto
                            {
                                Signed = new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 3 },
                                LegalEntity = new AccountSpecificLegalEntityDto{ AccountLegalEntityId = 1 }
                            }
                        }
                    }));
            
            //Act
            var actual = htmlHelper.ShowExpiringAgreementBanner(userId, hashedAccountId);
            
            //Assert
            Assert.AreEqual(shouldShowBanner, actual);
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        public void SingleOrg_PreviousAgreementSigned_V3NotSigned_ShouldShowExpiringAgreementBanner(bool hasSignedV1, bool hasSignedV2, bool shouldShowBanner)
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "USER1";
            var dependancyResolver = new Mock<IDependencyResolver>();
            dependancyResolver.Setup(r => r.GetService(typeof(IMediator))).Returns(_mockMediator.Object);
            DependencyResolver.SetResolver(dependancyResolver.Object);
            _mockMediator.Setup(m => m.Send(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(new GetAccountEmployerAgreementsResponse
                    {
                        EmployerAgreements = new List<EmployerAgreementStatusDto>
                        {
                            new EmployerAgreementStatusDto
                            {
                                Signed = hasSignedV1 ? new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 } : null,
                                Pending = hasSignedV1 ? null :  new PendingEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 },
                                LegalEntity = new AccountSpecificLegalEntityDto{ AccountLegalEntityId = 1 }
                            },
                            new EmployerAgreementStatusDto
                            {
                                Signed = hasSignedV2 ? new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 } : null,
                                Pending = hasSignedV2 ? null :  new PendingEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 },
                                LegalEntity = new AccountSpecificLegalEntityDto{ AccountLegalEntityId = 1 }
                            }
                        }
                    }));
            
            //Act
            var actual = htmlHelper.ShowExpiringAgreementBanner(userId, hashedAccountId);
            
            //Assert
            Assert.AreEqual(shouldShowBanner, actual);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        public void MultipleOrg_OneWithSignedPrevious_AndV3NotSigned_ShouldShowExpiringAgreementBanner(int numOfOrgsWithSignedV3)
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var userId = "USER1";
            var dependancyResolver = new Mock<IDependencyResolver>();
            dependancyResolver.Setup(r => r.GetService(typeof(IMediator))).Returns(_mockMediator.Object);
            DependencyResolver.SetResolver(dependancyResolver.Object);

            var employerAgreements = GetAgreementTestData(numOfOrgsWithSignedV3);
            employerAgreements.Add(new EmployerAgreementStatusDto
            {
                Signed = new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 },
                LegalEntity = new AccountSpecificLegalEntityDto { AccountLegalEntityId = 3 }
            });

            employerAgreements.Add(new EmployerAgreementStatusDto
            {
                Signed = new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 2 },
                LegalEntity = new AccountSpecificLegalEntityDto { AccountLegalEntityId = 3 }
            });

            _mockMediator.Setup(m => m.Send(It.Is<GetAccountEmployerAgreementsRequest>(q => q.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(new GetAccountEmployerAgreementsResponse
                    {
                        EmployerAgreements = employerAgreements
                    }));

            //Act
            var actual = htmlHelper.ShowExpiringAgreementBanner(userId, hashedAccountId);

            //Assert
            Assert.IsTrue(actual);
        }

        private List<EmployerAgreementStatusDto> GetAgreementTestData(int numOfOrgsWithSignedV3)
        {
            var employerAgreementStatusDtos = new List<EmployerAgreementStatusDto>();

            for (int i = 1; i <= numOfOrgsWithSignedV3; i++)
            {
                employerAgreementStatusDtos.Add(new EmployerAgreementStatusDto
                {
                    Signed = new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 1 },
                    LegalEntity = new AccountSpecificLegalEntityDto { AccountLegalEntityId = 1 }
                });

                employerAgreementStatusDtos.Add(new EmployerAgreementStatusDto
                {
                    Signed = new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 2 },
                    LegalEntity = new AccountSpecificLegalEntityDto { AccountLegalEntityId = 1 }
                });

                employerAgreementStatusDtos.Add(new EmployerAgreementStatusDto
                {
                    Signed = new SignedEmployerAgreementDetailsDto { Id = 123, VersionNumber = 3 },
                    LegalEntity = new AccountSpecificLegalEntityDto { AccountLegalEntityId = 1 }
                });
            }

            return employerAgreementStatusDtos;
        }
    }
}
