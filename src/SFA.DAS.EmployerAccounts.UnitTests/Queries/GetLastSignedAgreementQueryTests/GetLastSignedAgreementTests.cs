using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetLastSignedAgreement;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetLastSignedAgreementQueryTests;

[TestFixture]
class GetLastSignedAgreementTests : Testing.FluentTest<GetLastSignedAgreementTestFixtures>
{
    private const long AccountLegalEntityId = 3;

    [Test]
    public Task GetLastSignedAgreement_IfRequestIsNotValid_DoNotGetAgreement()
    {
        return TestExceptionAsync(
            arrange:fixtures => fixtures.WithInvalidRequest(),  
            act:fixtures => fixtures.Handle(AccountLegalEntityId),
            assert:(f, r) => r.Should().ThrowAsync<InvalidRequestException>());
    }

    [Test]
    public Task GetLastSignedAgreement_ShouldReturnLatestSignedAgreement()
    {
        EmployerAgreement latestSignedAgreement = null;

        return TestAsync(
            arrange: fixtures => fixtures.WithSignedAgreement(123354, 2, AccountLegalEntityId, 1, DateTime.Now.AddDays(-30), out latestSignedAgreement),
            act: fixtures => fixtures.Handle(AccountLegalEntityId),
            assert: fixtures =>
            {
                Assert.AreEqual(latestSignedAgreement.Id, fixtures.Response.LastSignedAgreement.Id);
            });
    }

    [Test]
    public Task GetLastSignedAgreement_IfNoSignedAgreementsExists_ShouldReturnNoSignedAgreement()
    {
        return TestAsync(
            act: fixtures => fixtures.Handle(AccountLegalEntityId),
            assert: fixtures =>
                Assert.IsNull(fixtures.Response.LastSignedAgreement));
    }
}

internal class GetLastSignedAgreementTestFixtures : FluentTestFixture
{
    public Mock<IValidator<GetLastSignedAgreementRequest>> Validator { get; }
    public IConfigurationProvider ConfigurationProvider { get; }

    public GetLastSignedAgreementResponse Response { get; private set; }

    public GetLastSignedAgreementTestFixtures()
    {
        EmployerAgreementBuilder = new EmployerAgreementBuilder();

        Validator = new Mock<IValidator<GetLastSignedAgreementRequest>>();

        ConfigurationProvider = new MapperConfiguration(c =>
        {
            c.AddProfile<AccountMappings>();
            c.AddProfile<AgreementMappings>();
            c.AddProfile<EmploymentAgreementStatusMappings>();
            c.AddProfile<LegalEntityMappings>();

        });

        Validator.Setup(x => x.ValidateAsync(It.IsAny<GetLastSignedAgreementRequest>()))
            .ReturnsAsync(new ValidationResult());
    }

    public async Task Handle(long accountLegalEntityId)
    {
        EmployerAgreementBuilder.EvaluateSignedAndPendingAgreementIdsForAllAccountLegalEntities();
        EmployerAgreementBuilder.SetupMockDbContext();
        var request = BuildRequest(accountLegalEntityId);

        var handler = new GetLastSignedAgreementQueryHandler(
            new Lazy<EmployerAccountsDbContext>(() => EmployerAgreementBuilder.EmployerAccountDbContext),
            Mock.Of<IEncodingService>(),
            Validator.Object,
            ConfigurationProvider);

        Response = await handler.Handle(request, CancellationToken.None);
    }

    public static GetLastSignedAgreementRequest BuildRequest(long accountLegalEntityId)
    {
        var request = new GetLastSignedAgreementRequest
        {
            AccountLegalEntityId = accountLegalEntityId
        };

        return request;
    }

    public GetLastSignedAgreementTestFixtures WithSignedAgreement(long accountId, long legalEntityId, long accountLegalEntityId, int agreementVersion,DateTime signedTime, out EmployerAgreement employerAgreement)
    {
        EmployerAgreementBuilder.WithAccount(accountId, "ABC1234");
        EmployerAgreementBuilder.WithSignedAgreement(accountId, legalEntityId, accountLegalEntityId, agreementVersion, signedTime, out employerAgreement);
        return this;
    }

    public GetLastSignedAgreementTestFixtures WithInvalidRequest()
    {
        Validator.Setup(x => x.ValidateAsync(It.IsAny<GetLastSignedAgreementRequest>()))
            .ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string>
                {
                    {"AccountLegalEntityId", "Account Legal Entity Id is not a valid Id"}
                },
                IsUnauthorized = false
            });

        return this;
    }

    private EmployerAgreementBuilder EmployerAgreementBuilder { get; }
}