using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using Claim = System.Security.Claims.Claim;
using ControllerContext = Microsoft.AspNetCore.Mvc.ControllerContext;
using ViewContext = Microsoft.AspNetCore.Mvc.Rendering.ViewContext;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Extensions;

class HtmlHelperExtensionsTests
{
    private Mock<ControllerContext> _mockControllerContext;
    private Mock<HttpContext> _mockHttpContext;
    private Mock<ClaimsPrincipal> _mockPrincipal;
    private Mock<ClaimsIdentity> _mockClaimsIdentity;
    private bool _isAuthenticated = true;
    private List<Claim> _claims;
    private string _userId;
    private IHtmlHelpers _htmlHelper;
    private ViewContext _viewContext;
    private Mock<IMediator> _mockMediator;
    private EmployerAccountsConfiguration _employerConfirguration;
    private readonly string _supportConsoleUsers = "Tier1User,Tier2User";

    [SetUp]
    public void SetUp()
    {
        _mockControllerContext = new Mock<ControllerContext>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockPrincipal = new Mock<ClaimsPrincipal>();
        _mockClaimsIdentity = new Mock<ClaimsIdentity>();
        _employerConfirguration = new EmployerAccountsConfiguration()
        {
            SupportConsoleUsers = _supportConsoleUsers

        };
        _userId = "TestUser";

        _claims = new List<Claim>
        {
            new Claim(ControllerConstants.UserRefClaimKeyName, _userId)
        };

        _mockPrincipal.Setup(m => m.Identity).Returns(_mockClaimsIdentity.Object);
        _mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(_isAuthenticated);
        _mockClaimsIdentity.Setup(m => m.Claims).Returns(_claims);
        _mockHttpContext.Setup(m => m.User).Returns(_mockPrincipal.Object);
        _mockControllerContext.Setup(m => m.HttpContext).Returns(_mockHttpContext.Object);

        _mockMediator = new Mock<IMediator>();

        _htmlHelper = new HtmlHelpers(_employerConfirguration, 
            Mock.Of<IMediator>(), 
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<ILogger<HtmlHelpers>>(),
            Mock.Of<DAS.Authorization.Services.IAuthorizationService>(),
            Mock.Of<ICompositeViewEngine>()
            );
        
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(provider => provider.GetService(typeof(EmployerAccountsConfiguration)))
            .Returns(_employerConfirguration);

    }

    [Test]
    public void WhenAuthenticatedSupportUser_ShouldReturnTrue()
    {
        // Arrange
        _isAuthenticated = true;
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));

        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void WhenUnauthenticatedSupportUser_ShouldReturnFalse()
    {
        // Arrange
        _mockClaimsIdentity.Setup(m => m.IsAuthenticated).Returns(false); // re-apply the mock return
        _claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, ControllerConstants.Tier2UserClaim));
        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void WhenUnauthenticatedNonSupportUser_ShouldReturnFalse()
    {
        // Arrange
        _isAuthenticated = false;

        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void WhenAuthenticatedNonSupportUser_ShouldReturnFalse()
    {
        // Arrange
        _isAuthenticated = true;

        // Act
        var result = _htmlHelper.IsSupportUser();

        // Assert
        Assert.IsFalse(result);
    }

    [TestCaseSource(nameof(LabelCases))]
    public void WhenICallSetZenDeskLabelsWithLabels_ThenTheKeywordsAreCorrect(string[] labels, string keywords)
    {
        // Arrange
        var expected = $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [{keywords}] }});</script>";

        // Act
        var actual = HtmlHelpers.SetZenDeskLabels(labels).ToString();

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
        var actual = _htmlHelper.ShowExpiringAgreementBanner(userId, hashedAccountId);

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
        var actual = _htmlHelper.ShowExpiringAgreementBanner(userId, hashedAccountId);

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
        var actual = _htmlHelper.ShowExpiringAgreementBanner(userId, hashedAccountId);

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