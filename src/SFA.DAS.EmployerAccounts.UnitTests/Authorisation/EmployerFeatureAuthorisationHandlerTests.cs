using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Results;
using SFA.DAS.EmployerAccounts.AuthorisationExtensions;
using SFA.DAS.Authorization.EmployerFeatures.Context;
using SFA.DAS.Testing;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.Authorization.Features.UnitTests.Handlers
{
    [TestFixture]
    [Parallelizable]
    public class EmployerFeatureAuthorisationHandlerTests : FluentTest<EmployerFeatureAuthorisationHandlerTestsFixture>
    {
        [Test]
        public Task GetAuthorizationResult_WhenOptionsAreNotAvailable_ThenShouldReturnValidAuthorizationResult()
        {
            return RunAsync(f => f.GetAuthorizationResult(), (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
        }

        [Test]
        public Task GetAuthorizationResult_WhenAndedOptionsAreAvailable_ThenShouldThrowNotImplementedException()
        {
            return RunAsync(f => f.SetAndedOptions(), f => f.GetAuthorizationResult(), (f, r) => r.ShouldThrow<NotImplementedException>());
        }

        [Test]
        public Task GetAuthorizationResult_WhenOredOptionIsAvailable_ThenShouldThrowNotImplementedException()
        {
            return RunAsync(f => f.SetOredOption(), f => f.GetAuthorizationResult(), (f, r) => r.ShouldThrow<NotImplementedException>());
        }

        [Test]
        public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAgreementVersionNotSet_ThenShouldReturnAuthorized()
        {
            return RunAsync(
                f => f.SetOption().SetAgreementVersion(),
                f => f.GetAuthorizationResult(),
                (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
        }

        [Test]
        public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAgreementVersionSetAndAgreementNotSigned_ThenShouldReturnUnauthorized()
        {
            return RunAsync(
                f => f.SetOption().SetAgreementVersion(1).SetAuthorizationContextValues().SetMediatorResponse(),
                f => f.GetAuthorizationResult(),
                (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => !r2.IsAuthorized));
        }

        [Test]
        public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAgreementVersionSetAndAgreementSignedButNotHighEnoughVersion_ThenShouldReturnUnauthorized()
        {
            return RunAsync(
                f => f.SetOption().SetAgreementVersion(2).SetAuthorizationContextValues().SetMediatorResponse(1, EmployerAgreementStatus.Signed),
                f => f.GetAuthorizationResult(),
                (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => !r2.IsAuthorized));
        }

        [Test]
        public Task GetAuthorizationResult_WhenOptionsAreAvailableAndAgreementVersionSetAndAgreementSignedWithHighEnoughVersion_ThenShouldReturnAuthorized()
        {
            return RunAsync(
                f => f.SetOption().SetAgreementVersion(2).SetAuthorizationContextValues().SetMediatorResponse(2, EmployerAgreementStatus.Signed),
                f => f.GetAuthorizationResult(),
                (f, r) => r.Should().NotBeNull().And.Match<AuthorizationResult>(r2 => r2.IsAuthorized));
        }

    }

    public class EmployerFeatureAuthorisationHandlerTestsFixture
    {
        public const string UserEmail = "MyTestEmail@example.com";
        public const long AccountId = 12;
        public const int AgreementVersion = 1;

        public List<string> Options { get; set; }
        public IAuthorizationContext AuthorizationContext { get; set; }
        public IAuthorizationHandler Handler { get; set; }
        public Mock<IFeatureTogglesService<EmployerFeatureToggle>> FeatureTogglesService { get; set; }
        public Mock<IMediator> Mediator { get; set; }

        public EmployerFeatureAuthorisationHandlerTestsFixture()
        {
            Options = new List<string>();
            AuthorizationContext = new AuthorizationContext();
            FeatureTogglesService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();
            Mediator = new Mock<IMediator>();
            Handler = new EmployerFeatureAuthorizationHandler(FeatureTogglesService.Object, Mediator.Object);
        }

        public Task<AuthorizationResult> GetAuthorizationResult()
        {
            return Handler.GetAuthorizationResult(Options, AuthorizationContext);
        }


        public EmployerFeatureAuthorisationHandlerTestsFixture SetAndedOptions()
        {
            Options.AddRange(new[] { "ProviderRelationships", "Tickles" });

            return this;
        }

        public EmployerFeatureAuthorisationHandlerTestsFixture SetOredOption()
        {
            Options.Add($"ProviderRelationships,ProviderRelationships");

            return this;
        }

        public EmployerFeatureAuthorisationHandlerTestsFixture SetOption()
        {
            Options.AddRange(new[] { "ProviderRelationships" });

            return this;
        }

        public EmployerFeatureAuthorisationHandlerTestsFixture SetAuthorizationContextValues(long? accountId = AccountId, string userEmail = UserEmail)
        {
            AuthorizationContext.AddEmployerFeatureValues(accountId, userEmail);

            return this;
        }

        public EmployerFeatureAuthorisationHandlerTestsFixture SetMediatorResponse(int agreementVersion = AgreementVersion, EmployerAgreementStatus agreementStatus = EmployerAgreementStatus.Pending)
        {
            var response = new GetEmployerAgreementsByAccountIdResponse
            {
                EmployerAgreements = new List<EmployerAgreement>
                {
                    new EmployerAgreement
                    {
                        StatusId = agreementStatus,
                        AccountLegalEntity = new AccountLegalEntity
                        {
                            SignedAgreementVersion = agreementStatus == EmployerAgreementStatus.Signed ? (int?)agreementVersion : null,
                            PendingAgreementVersion = agreementStatus == EmployerAgreementStatus.Pending ? (int?)agreementVersion : null
                        }
                    }
                }
            };

            Mediator.Setup(s => s.SendAsync(It.Is<GetEmployerAgreementsByAccountIdRequest>(q => q.AccountId == AccountId))).ReturnsAsync(response);

            return this;
        }

        public EmployerFeatureAuthorisationHandlerTestsFixture SetAgreementVersion(long? agreementVersion = null)
        {
            var option = Options.Single();

            FeatureTogglesService.Setup(s => s.GetFeatureToggle(option)).Returns(new EmployerFeatureToggle { Feature = "ProviderRelationships", EnabledByAgreementVersion = agreementVersion });

            return this;
        }
    }
}