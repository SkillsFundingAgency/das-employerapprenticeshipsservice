﻿using System.Net;
using System.Security.Claims;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Infrastructure;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.Invitations;

public class WhenTestingResendGet
{
    [Test, MoqAutoData]
    public async Task ItShouldReturnViewAndModelOnSuccess(
        string hashedAccountId,
        string email,
        string externalUserId,
        Mock<IEmployerAccountsApiService> accountsApiService
    )
    {
        var sut = new InvitationsController(Mock.Of<ILogger<InvitationsController>>(), accountsApiService.Object);

        var actual = await sut.Resend(hashedAccountId, email);

        using (new AssertionScope())
        {
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            ((ViewResult)actual).ViewName.Should().Be("Confirm");

            var model = ((ViewResult)actual).Model as SendInvitationCompletedModel;

            model?.Should().BeOfType<SendInvitationCompletedModel>();
            model?.Success.Should().BeTrue();
            model?.MemberEmail.Should().Be(email);
            model?.ReturnToTeamUrl.Should().Be(string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", hashedAccountId));

            accountsApiService.Verify(x => x.ResendInvitation(hashedAccountId, email, CancellationToken.None), Times.Once);
        }
    }

    [Test, MoqAutoData]
    public async Task ItShouldCorrectlyHandleEmailWithPlusSymbol(
        string hashedAccountId,
        string externalUserId,
        Mock<IEmployerAccountsApiService> accountsApiService
    )
    {
        const string email = "test+email@address.test";

        var sut = new InvitationsController(Mock.Of<ILogger<InvitationsController>>(), accountsApiService.Object);

        var actual = await sut.Resend(hashedAccountId, email);

        using (new AssertionScope())
        {
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            ((ViewResult)actual).ViewName.Should().Be("Confirm");

            var model = ((ViewResult)actual).Model as SendInvitationCompletedModel;

            model?.Should().BeOfType<SendInvitationCompletedModel>();
            model?.Success.Should().BeTrue();
            model?.MemberEmail.Should().Be(email);
            model?.ReturnToTeamUrl.Should().Be(string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", hashedAccountId));

            accountsApiService.Verify(x => x.ResendInvitation(hashedAccountId, email, CancellationToken.None), Times.Once);
        }
    }
}